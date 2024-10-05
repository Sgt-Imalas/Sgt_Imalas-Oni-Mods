using _WorldGenStateCapture.WorldStateData;
using _WorldGenStateCapture.WorldStateData.Starmap.SpacemapItems;
using _WorldGenStateCapture.WorldStateData.WorldPOIs;
using Klei.CustomSettings;
using ProcGen;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Networking;
using static ProcGen.ClusterLayout;

namespace _WorldGenStateCapture
{
	internal class ModAssets
	{

        //if any other mods are installed
        public static bool ModDilution = false;
        public static string ModPath => System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public static Dictionary<WorldContainer, List<MapGeyser>> currentGeysers = new();
		public static Dictionary<WorldContainer, List<MapPOI>> currentPOIs = new();
		public static List<HexMap_Entry> dlcStarmapItems = new List<HexMap_Entry>();
		public static List<VanillaMap_Entry> baseStarmapItems = new List<VanillaMap_Entry>();

		static string BaseGameFolder = "BasegameSeeds";
		static string DlcClassicFolder = "DlcClassicSeeds";
		static string DlcSpacedOutFolder = "DlcSOSeeds";

		internal static void AccumulateSeedData()
		{

            if (ModAssets.ModDilution)
            {
                Debug.LogError("Other active mods detected, aborting world parsing.");
                return;
            }

            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(ModPath, BaseGameFolder));
			System.IO.Directory.CreateDirectory(System.IO.Path.Combine(ModPath, DlcClassicFolder));
			System.IO.Directory.CreateDirectory(System.IO.Path.Combine(ModPath, DlcSpacedOutFolder));


			bool dlcActive = DlcManager.IsExpansion1Active();

			WorldDataInstance DataItem = new WorldDataInstance();

			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout);
			if (currentQualitySetting == null)
			{
				Debug.LogError("Clusterlayout was null");
				return;
			}

			ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting.id);
			SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
			//string otherSettingsCode = CustomGameSettings.Instance.GetOtherSettingsCode();
			string storyTraitSettingsCode = CustomGameSettings.Instance.GetStoryTraitSettingsCode();

			int.TryParse(currentQualitySetting2.id, out int seed);
			
			// DataItem.seed = seed;
			
			DataItem.cluster = clusterData.GetCoordinatePrefix();
			DataItem.coordinate = CustomGameSettings.Instance.GetSettingsCoordinate();

			var activeDlcIds = DlcManager.GetActiveDLCIds();

			var cleanDlcIds = new List<string>();

            foreach (var dlcId in activeDlcIds)
            {
                switch (dlcId)
                {
                    case "DLC2_ID":
                        cleanDlcIds.Add("FrostyPlanet");
                        break;
                    case "EXPANSION1_ID":
                        cleanDlcIds.Add("SpacedOut");
                        break;
                    default:
                        cleanDlcIds.Add(dlcId); // If it's not a known ID, keep it as is
                        break;
                }
            }

			DataItem.dlcs = cleanDlcIds;

            Debug.Log("accumulating pois...");
			foreach (var asteroid in ClusterManager.Instance.WorldContainers)
			{
				Debug.Log("collecting " + asteroid.GetProperName());

                // Clean worldTraits by removing parts before "/"
                var cleanWorldTraits = asteroid.WorldTraitIds.Select(trait => trait.Contains("/")
                    ? trait.Split('/').Last()
                    : trait).ToList();

                var asteroidData = new AsteroidData()
				{
					id = System.IO.Path.GetFileName(asteroid.worldName),
					offsetX = asteroid.WorldOffset.X,
					offsetY = asteroid.WorldOffset.Y,
					sizeX = asteroid.WorldSize.X,
					sizeY = asteroid.WorldSize.Y,
					worldTraits = cleanWorldTraits
				};

				if (currentPOIs.ContainsKey(asteroid))
					asteroidData.pointsOfInterest = new(currentPOIs[asteroid]);

				if (currentGeysers.ContainsKey(asteroid))
					asteroidData.geysers = new(currentGeysers[asteroid]);

				DataItem.asteroids.Add(asteroidData);
			}

			if (dlcActive)
			{
				DataItem.starMapEntriesSpacedOut = new(dlcStarmapItems);
			}
			else
			{
				DataItem.starMapEntriesVanilla = new(baseStarmapItems);
			}

			string parentPath = string.Empty;

			switch (clusterData.clusterCategory)
			{
				case ClusterCategory.Vanilla:
					parentPath = System.IO.Path.Combine(ModPath, BaseGameFolder);
					break;
				case ClusterCategory.SpacedOutVanillaStyle:
					parentPath = System.IO.Path.Combine(ModPath, DlcClassicFolder);
					break;
				case ClusterCategory.SpacedOutStyle:
					parentPath = System.IO.Path.Combine(ModPath, DlcSpacedOutFolder);
					break;

			}

			if (parentPath != string.Empty)
			{

                Debug.Log("Serialize data...");

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(DataItem);


                Debug.Log("Send data to webservice...");

                //attach the coroutine to the main game object
                Game.Instance.StartCoroutine(TryPostRequest(json));
				
				//^ if that still crashes, use this:
				//Task.Run(() => TryPostRequest(json));
			}
			else
			{

				Debug.LogWarning("Parent path not found!");

                ClearAndRestart();
            }
				
		}

		static IEnumerator TryPostRequest(string data)
        {

            // Convert JSON string to bytes
            byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

			using (UnityWebRequest request = new UnityWebRequest("https://api.mapsnotincluded.org/ingest", "POST"))
			{

				// Set the request body to the JSON byte array
				request.uploadHandler = new UploadHandlerRaw(bodyRaw);
				request.downloadHandler = new DownloadHandlerBuffer();

				// Set the content type to JSON
				request.SetRequestHeader("Content-Type", "application/json");

				// Send the API key
				request.SetRequestHeader("MNI_API_KEY", "KAEofp47Zu8JRUi");

                Debug.Log("request.SendWebRequest() ...");

                yield return request.SendWebRequest();

				if (request.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError(request.error);
					ClearAndRestart();
				}
				else
				{
					Debug.Log("Form upload complete!");
					ClearAndRestart();
				}
			}
		}

		public static void ClearAndRestart()
		{
			currentGeysers.Clear();
			currentPOIs.Clear();
			dlcStarmapItems.Clear();
			baseStarmapItems.Clear();
			App.instance.Restart();
		}
	}
}
