using _WorldGenStateCapture.WorldStateData;
using _WorldGenStateCapture.WorldStateData.Starmap.SpacemapItems;
using _WorldGenStateCapture.WorldStateData.WorldPOIs;
using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static ProcGen.ClusterLayout;
using static ProcGen.SubWorld;

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

		/// <summary>
		/// generates an SVG image of all biome polygons of the given asteroid
		/// </summary>
		/// <param name="targetAsteroid"></param>
		/// <returns>svg biomes image as string</returns>
		internal static string AccumulateBiomeData(WorldContainer targetAsteroid)
		{
			//connects the individual vertices to a path for each biome blob for polygon generation
			StringBuilder PathBuilder = new StringBuilder();
			//collects the polygons
			StringBuilder PolyBuilder = new StringBuilder();
			//stylesheet generation dynamically based on existing zonetype colors
			StringBuilder styleSheetBuilder = new StringBuilder();

			//only add zoneType styles on demand
			HashSet<SubWorld.ZoneType> addedZoneType = new HashSet<SubWorld.ZoneType>();

			//grid size
			int height = Grid.HeightInCells;
			int width = Grid.WidthInCells;

			//asteroid size
			int asteroidHeight = targetAsteroid.Height;
			int asteroidWidth = targetAsteroid.Width;

			//bottom left corner of the asteroid
			int worldOffsetX = targetAsteroid.worldOffset.X;
			int worldOffsetY = targetAsteroid.worldOffset.Y;

			//bounding box of asteroid on the world grid
			int minX = worldOffsetX;
			int maxX = asteroidWidth + worldOffsetX;
			int minY = worldOffsetY;
			int maxY = asteroidHeight + worldOffsetY;

			//start building stylesheet
			styleSheetBuilder.Append("<style>");
			styleSheetBuilder.Append("polygon{stroke-width:0.5}");

			//iterate all zone tile blobs
			foreach (Klei.WorldDetailSave.OverworldCell biomeBlob in SaveLoader.Instance.clusterDetailSave.overworldCells)
			{
				PathBuilder.Clear();

				bool partOfAsteroid = true;
				var currentZoneType = biomeBlob.zoneType;
				string ZoneTypeCssClass = $"zone{(int)currentZoneType}";

				foreach (var vert in biomeBlob.poly.Vertices)
				{
					//if the vertex is outside the asteroids bounding box, the polygon isnt part of the asteroid, skip it
					if (vert.x < minX || vert.x > maxX || vert.y < minY || vert.y > maxY)
					{
						partOfAsteroid = false;
						break;
					}
					PathBuilder.AppendFormat("{0:0.0},{1:0.0} ", vert.x - worldOffsetX, asteroidHeight - (vert.y - worldOffsetY));
				}
				if (!partOfAsteroid)
					continue;

				//add biomecolor to stylesheet if not added yet
				if (!addedZoneType.Contains(currentZoneType))
				{
					var biomeColor = GetBiomeColor(currentZoneType);
					string colorHex = Util.ToHexString(biomeColor);

					addedZoneType.Add(currentZoneType);
					styleSheetBuilder.Append($".{ZoneTypeCssClass}{{fill:#{colorHex};stroke:#{colorHex};}}");
				}

				//add polygon for this biomeBlob to polygons-stringbuilder
				PolyBuilder.Append($"<path class=\"{ZoneTypeCssClass}\" d=\"M{PathBuilder.ToString().TrimEnd()}z\"/>");
			}
			//close stylesheet stringbuilder
			styleSheetBuilder.Append("</style>");
			string styleSheet = styleSheetBuilder.ToString();

			string poly_string = PolyBuilder.ToString();

			string svg = $"<svg height=\"{asteroidHeight * 2}\" width=\"{asteroidWidth * 2}\" viewBox=\"0 0 {asteroidWidth} {asteroidHeight}\" xmlns=\"http://www.w3.org/2000/svg\">\r\n    {styleSheet}\r\n    <g>\r\n    {poly_string}\r\n    </g>\r\n</svg>";
			//Console.WriteLine(svg);
			return svg;
		}

		internal static void AccumulateSeedData()
		{

			if (ModAssets.ModDilution)
			{
				Debug.LogError("Other active mods detected, aborting world parsing.");
				return;
			}



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

			var cleanDlcIds = new List<string>();

			foreach (var dlcId in DlcManager.GetActiveDLCIds())
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

			Debug.Log("accumulating asteroid data...");
			foreach (var asteroid in ClusterManager.Instance.WorldContainers)
			{
				Debug.Log("collecting " + asteroid.GetProperName());
				// Clean worldTraits by removing parts before "/"
				var cleanWorldTraits = asteroid.WorldTraitIds.Select(trait => System.IO.Path.GetFileNameWithoutExtension(trait)).ToList();

				var asteroidData = new AsteroidData()
				{
					id = System.IO.Path.GetFileName(asteroid.worldName),
					offsetX = asteroid.WorldOffset.X,
					offsetY = asteroid.WorldOffset.Y,
					sizeX = asteroid.WorldSize.X,
					sizeY = asteroid.WorldSize.Y,
					worldTraits = cleanWorldTraits,
					BiomesSVG = AccumulateBiomeData(asteroid)
				};

				if (currentPOIs.ContainsKey(asteroid))
					asteroidData.pointsOfInterest = new(currentPOIs[asteroid]);

				if (currentGeysers.ContainsKey(asteroid))
					asteroidData.geysers = new(currentGeysers[asteroid]);

				if (asteroid.IsStartWorld)
					DataItem.asteroids.Insert(0, asteroidData);
				else
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
			Debug.Log("Serializing data...");

			string json = Newtonsoft.Json.JsonConvert.SerializeObject(DataItem);


			Debug.Log("Send data to webservice...");

			//attach the coroutine to the main game object
			Game.Instance.StartCoroutine(TryPostRequest(json));
		}

		static SubworldZoneRenderData cachedRenderData = null;
		static Color GetBiomeColor(SubWorld.ZoneType type)
		{
			if (cachedRenderData == null)
			{
				cachedRenderData = World.Instance.GetComponent<SubworldZoneRenderData>();
			}
			// Biomes: Get the original color for the biome
			Color color = cachedRenderData.zoneColours[(int)type];

			// Reset the alpha value so it can be shown nicely both in the map and the legend
			color.a = 1f;
			return color;
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
