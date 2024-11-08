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
using static STRINGS.COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.STATUSITEMS;
using static STRINGS.UI.CLUSTERMAP;

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
					biomePaths = ConvertBiomePathData(AccumulateBiomePathData(asteroid)), // AccumulateBiomeData(asteroid),
																						  //biomes = AccumulateBiomePathData(asteroid)
				};


				if (currentPOIs.ContainsKey(asteroid))
					asteroidData.pointsOfInterest = new(currentPOIs[asteroid]);

				if (currentGeysers.ContainsKey(asteroid))
					asteroidData.geysers = new(currentGeysers[asteroid]);

				CleanPOICoordinates(asteroidData);


				if (asteroid.IsStartWorld)
					DataItem.asteroids.Insert(0, asteroidData);
				else
					DataItem.asteroids.Add(asteroidData);


				//Debug.Log("SVG:");
				//Console.WriteLine(asteroidData.biomesSVG);

				//Debug.Log("json:");
				//Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(asteroidData.biomes));

				//Debug.Log("CustomRepresentation:");
				//Console.WriteLine(ConvertBiomePathData(asteroidData.biomes));
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


			Console.WriteLine(json);
			//attach the coroutine to the main game object
			//Game.Instance.StartCoroutine(TryPostRequest(json));
		}

		/// <summary>
		/// The game uses bottom left origin grid coordinates.
		/// SVG uses top left, so we have to invert them.
		/// We also have to subtract the world offsets from each poi coordinate
		/// </summary>
		/// <param name="asteroid"></param>
		internal static void CleanPOICoordinates(AsteroidData asteroid)
		{
			foreach (var poi in asteroid.pointsOfInterest)
			{
				poi.y = asteroid.sizeY - (poi.y - asteroid.offsetY);
				poi.x -= asteroid.offsetX;
			}
			foreach (var geyser in asteroid.geysers)
			{
				geyser.y = asteroid.sizeY - (geyser.y - asteroid.offsetY);
				geyser.x -= asteroid.offsetX;
			}
		}

		internal static string ConvertBiomePathData(Dictionary<ProcGen.SubWorld.ZoneType, List<biomePolygon>> data)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var biome in data)
			{
				sb.Append($"{biome.Key}:");
				foreach (var polygon in biome.Value)
				{
					foreach (var point in polygon.points)
					{
						sb.AppendFormat("{0:0},{1:0} ", point.x, point.y);
					}
					sb.Remove(sb.Length - 1, 1); // remove last space
					sb.Append(";");
				}
				sb.Remove(sb.Length - 1, 1);  //remove last semicolon
				sb.Append('\n'); //newline as separator
			}
			sb.Remove(sb.Length - 1, 1);  //remove last newline
			return sb.ToString();
		}


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

				if (currentZoneType == ZoneType.Space)
					continue;

				foreach (var vert in biomeBlob.poly.Vertices)
				{
					//if the vertex is outside the asteroids bounding box, the polygon isnt part of the asteroid, skip it
					if (vert.x < minX || vert.x > maxX || vert.y < minY || vert.y > maxY)
					{
						partOfAsteroid = false;
						break;
					}
					PathBuilder.AppendFormat("{0:0},{1:0} ", vert.x - worldOffsetX, asteroidHeight - (vert.y - worldOffsetY));
				}
				if (!partOfAsteroid)
					continue;

				//add biomecolor to stylesheet if not added yet
				if (!addedZoneType.Contains(currentZoneType))
				{
					var biomeColor = GetBiomeColor(currentZoneType);
					string colorHex = Util.ToHexString(biomeColor).Substring(0, 6);

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
		/// <summary>
		/// generates an SVG image of all biome polygons of the given asteroid
		/// </summary>
		/// <param name="targetAsteroid"></param>
		/// <returns>svg biomes image as string</returns>
		internal static Dictionary<ProcGen.SubWorld.ZoneType, List<biomePolygon>> AccumulateBiomePathData(WorldContainer targetAsteroid)
		{
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

			var data = new Dictionary<ZoneType, List<biomePolygon>>();

			//iterate all zone tile blobs
			foreach (Klei.WorldDetailSave.OverworldCell biomeBlob in SaveLoader.Instance.clusterDetailSave.overworldCells)
			{

				bool partOfAsteroid = true;
				var currentZoneType = biomeBlob.zoneType;
				string ZoneTypeCssClass = $"zone{(int)currentZoneType}";

				if (currentZoneType == ZoneType.Space)
					continue;

				var polygon = new biomePolygon();

				foreach (var vert in biomeBlob.poly.Vertices)
				{
					//if the vertex is outside the asteroids bounding box, the polygon isnt part of the asteroid, skip it
					if (vert.x < minX || vert.x > maxX || vert.y < minY || vert.y > maxY)
					{
						partOfAsteroid = false;
						break;
					}
					polygon.points.Add(new(vert.x - worldOffsetX, asteroidHeight - (vert.y - worldOffsetY)));
				}
				if (!partOfAsteroid)
					continue;

				//add biomecolor to stylesheet if not added yet
				if (!data.ContainsKey(currentZoneType))
				{
					data.Add(currentZoneType, new());
				}
				data[currentZoneType].Add(polygon);
			}
			return data;
		}
		static SubworldZoneRenderData cachedRenderData = null;
		static void DumpBiomeColors()
		{
			Console.WriteLine("Biome Color Mapping:");
			foreach (SubWorld.ZoneType zoneType in Enum.GetValues(typeof(SubWorld.ZoneType)))
			{
				string hex = Util.ToHexString(cachedRenderData.zoneColours[(int)zoneType]).Substring(0, 6);
				Console.WriteLine(zoneType.ToString() + " (" + (int)zoneType + ", Color(0xFF" + hex + ")),");
			}
		}

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

		//static IEnumerator TryPostRequest(string data)
		//{
			//// Convert JSON string to bytes
			//byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
			//using (UnityWebRequest request = new UnityWebRequest("https://oni-seed-uploader-stefan-oltmann.koyeb.app/upload", "POST"))
			//{
			//	request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			//	request.downloadHandler = new DownloadHandlerBuffer();
			//	request.SetRequestHeader("Content-Type", "application/json");

			//	// Send the API key
			//	request.SetRequestHeader("MNI_API_KEY", "KAEofp47Zu8JRUi");

			//	Debug.Log("request.SendWebRequest() ...");
			//	yield return request.SendWebRequest();
			//	if (request.result != UnityWebRequest.Result.Success)
			//	{
			//		Debug.LogError(request.error);
			//		ClearAndRestart();
			//	}
			//	else
			//	{
			//		Debug.Log("Form upload complete!");
			//		ClearAndRestart();
			//	}
			//}
		//}

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
