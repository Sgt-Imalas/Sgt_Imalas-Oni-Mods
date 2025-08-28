using ClusterTraitGenerationManager.UI.Screens;
using Klei.CustomSettings;
using ObjectCloner;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static ProcGen.ClusterLayout;
using static ProcGen.WorldPlacement;
using static ResearchTypes;

namespace ClusterTraitGenerationManager.ClusterData
{
	public class CGSMClusterManager
	{

		public const string CustomClusterIDCoordinate = "CGM";
		public const string CustomClusterID = "expansion1::clusters/CGMCluster";
		public const string CustomClusterClusterTag = "CGM_CustomCluster";


		public const float MaxAmountRandomPlanet = 6f;
		public const string RandomKey = "CGM_RANDOM_";
		static Dictionary<string, StarmapItem> PlanetsAndPOIs = null;
		static List<string> RandomOuterPlanets = new List<string>();

		public const int ringMax = 25, ringMin = 6;
		public static int MaxPOICount = 1;
		public static GameObject Screen = null;

		public static ColonyDestinationSelectScreen selectScreen;
		// public static StarmapItem RandomPOIStarmapItem = null;
		public static StarmapItem RandomOuterPlanetsStarmapItem = null;

		public static HashSet<string> BlacklistedTraits = new HashSet<string>();
		public static HashSet<string> BlacklistedGeysers = new HashSet<string>();
		public static bool BlacklistAffectsNonGenerics = true;

		public static int MaxClassicOuterPlanets = 3, CurrentClassicOuterPlanets = 0;

		static bool PlacementDataInitialized => _predefinedPlacementData != null;
		static Dictionary<string, WorldPlacement> PredefinedPlacementData
		{
			get
			{
				if (_predefinedPlacementData == null)
					PopulatePredefinedClusterPlacements();
				return _predefinedPlacementData;
			}
		}
		static Dictionary<string, WorldPlacement> _predefinedPlacementData = null;

		public static Dictionary<string, CustomClusterAudio> DlcAudioSettings
		{
			get
			{
				if (_dlcAudioSettings == null)
				{
					PopulatePredefinedClusterPlacements();
				}
				return _dlcAudioSettings;
			}
		}

		static Dictionary<string, CustomClusterAudio> _dlcAudioSettings = null;

		public static bool LoadCustomCluster
		{
			get
			{
				return _loadCustomCluster;
			}
			set
			{
				_loadCustomCluster = value;
				if (value == false)
				{
					ResetMeteorSeasons();
					ResetSkyFixedTraits();
				}
			}
		}
		private static bool _loadCustomCluster = false;


		public static bool RandomTraitInBlacklist(string traitId) => BlacklistedTraits.Contains(traitId);
		public static bool ToggleRandomTraitBlacklist(string traitID)
		{
			if (BlacklistedTraits.Contains(traitID))
			{
				BlacklistedTraits.Remove(traitID);
				return false;
			}
			else
			{
				BlacklistedTraits.Add(traitID);
				return true;
			}
		}

		public static void ResetSharedGeyserOverrides()
		{
			BlacklistedGeysers.Clear();
			BlacklistAffectsNonGenerics = true;
		}
		public static void SetSharedGeyserBlacklistState(string geyserId, bool blacklisted)
		{
			if (blacklisted)
				BlacklistedGeysers.Add(geyserId);
			else
				BlacklistedGeysers.Remove(geyserId);
		}
		public static HashSet<string> GetBlacklistedGeyserIdsFor(StarmapItem asteroid)
		{
			if (asteroid.GeyserBlacklistShared)
			{
				return BlacklistedGeysers;
			}
			else
			{
				return asteroid.BlacklistedGeyserIds;
			}
		}

		public static void ResetSkyFixedTraits(ProcGen.World singleItem = null)
		{
			if (singleItem == null)
			{
				foreach (var planetData in ModAssets.ChangedSkyFixedTraits)
				{
					planetData.Key.fixedTraits = new List<string>(planetData.Value);
				}
				ModAssets.ChangedSkyFixedTraits.Clear();
			}
			else
			{
				if (singleItem != null && ModAssets.ChangedSkyFixedTraits.ContainsKey(singleItem))
				{
					singleItem.fixedTraits = new List<string>(ModAssets.ChangedSkyFixedTraits[singleItem]);
					ModAssets.ChangedSkyFixedTraits.Remove(singleItem);
				}
			}
		}
		public static void ResetMeteorSeasons(ProcGen.World singleItem = null)
		{
			if (singleItem == null)
			{
				foreach (var planet in ModAssets.ChangedMeteorSeasons)
				{
					planet.Key.seasons = new List<string>(planet.Value);
				}
				ModAssets.ChangedMeteorSeasons.Clear();
			}
			else
			{
				if (singleItem != null && ModAssets.ChangedMeteorSeasons.ContainsKey(singleItem))
				{
					singleItem.seasons = new List<string>(ModAssets.ChangedMeteorSeasons[singleItem]);
					ModAssets.ChangedMeteorSeasons.Remove(singleItem);
				}
			}
		}
		public static CGM_MainScreen_UnityScreen CGM_Screen = null;

		public static void GenerateDefaultCluster()
		{
			string defaultCluster = "SandstoneDefault";
			SgtLogger.l("DestinationSelectPanel.ChosenClusterCategorySetting: " + DestinationSelectPanel.ChosenClusterCategorySetting);

			switch (DestinationSelectPanel.ChosenClusterCategorySetting)
			{
				case 0:
					defaultCluster = "SandstoneDefault"; break;
				case 1:
					defaultCluster = "expansion1::clusters/VanillaSandstoneCluster"; break;
				case 2:
					defaultCluster = "expansion1::clusters/SandstoneStartCluster"; break;
				case 3:
					defaultCluster = DlcManager.IsExpansion1Active() ? "expansion1::clusters/KleiFest2023Cluster" : "clusters/KleiFest2023"; break;

			}

			SgtLogger.l("Creating from Default Cluster: " + defaultCluster);
			CreateCustomClusterFrom(defaultCluster);
		}

		public static async void InstantiateClusterSelectionView(ColonyDestinationSelectScreen parent, System.Action onClose = null)
		{
			if (Screen == null)
			{
				SgtLogger.l("initializing CGM View");
				///Change to check for moonlet/classic start
				if (CustomCluster == null)
				{
					GenerateDefaultCluster();
				}

				Screen = Util.KInstantiateUI(ModAssets.CGM_MainMenu, FrontEndManager.Instance.gameObject, true);
				selectScreen = parent;

				Screen.name = "ClusterSelectionView";
				CGM_Screen = Screen.AddComponent<CGM_MainScreen_UnityScreen>();
			}
			if (CustomCluster == null)
			{
				SgtLogger.l("initializing CGM default cluster");
				GenerateDefaultCluster();
			}
			//LoadCustomCluster = false;

			Screen.transform.SetAsLastSibling();
			Screen.gameObject.SetActive(true);
			CGM_Screen = Screen.GetComponent<CGM_MainScreen_UnityScreen>();
			CGM_Screen.Show(true);
			CGM_Screen.OnActivateWindow();
			CGM_Screen.SelectCategory(StarmapItemCategory.Starter);
			CGM_Screen.RefreshView();
		}

		public static void SetAndStretchToParentSize(RectTransform _mRect, RectTransform _parent)
		{
			_mRect.anchoredPosition = _parent.position;
			_mRect.anchorMin = new Vector2(1, 0);
			_mRect.anchorMax = new Vector2(0, 1);
			_mRect.pivot = new Vector2(0.5f, 0.5f);
			_mRect.sizeDelta = _parent.rect.size;
			_mRect.transform.SetParent(_parent);
		}


		public enum StarmapItemCategory
		{
			Starter = 0,
			Warp = 1,
			Outer = 2,
			POI = 3,

			///These only exist for the UI handlers
			None = -1,
			StoryTraits = -2,
			GameSettings = -3,
			VanillaStarmap = -4,
			SpacedOutStarmap = -5,
			MixingSettings = -6,
		}



		public enum WorldSizePresets
		{
			Tiny = 30,
			Smaller = 45,
			Small = 60,
			SlightlySmaller = 80,

			Custom = -1,
			Normal = 100,
			SlightlyLarger = 125,
			Large = 150,
			Huge = 200,
			Massive = 300,
			Enormous = 400
		}
		public enum WorldRatioPresets
		{
			Custom = -1,
			LotWider = 70,
			Wider = 80,
			SlightlyWider = 90,
			Normal = 100,
			SlightlyTaller = 110,
			Taller = 120,
			LotTaller = 130
		}


		public static ClusterLayout GeneratedLayout => GenerateClusterLayoutFromCustomData(false);
		public static CustomClusterData CustomCluster;


		public static void AddCustomClusterAndInitializeClusterGen()
		{
			LoadCustomCluster = true;
			var GeneratedCustomCluster = GenerateClusterLayoutFromCustomData(true);
			SettingsCache.clusterLayouts.clusterCache[CustomClusterID] = GeneratedCustomCluster;

			CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.ClusterLayout, GeneratedCustomCluster.filePath);
			//SgtLogger.l(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id.ToString());
			selectScreen.LaunchClicked();
		}

		static void ApplySizeMultiplier(SpaceMapPOIPlacement placement, float multiplier)
		{
			float min = placement.allowedRings.min, max = placement.allowedRings.max;
			//min*= multiplier;x
			max *= multiplier;

			int max2 = placement.allowedRings.max + CustomCluster.AdjustedOuterExpansion;
			int newMax = Math.Max((int)Math.Round(max), max2);
			placement.allowedRings = new MinMaxI((int)min, newMax);
		}

		public static bool PlanetIsClassic(StarmapItem item)
		{
			if (item.IsPOI)
				return false;

			if (item.id.Contains("Vanilla") || item.world != null && item.originalWorldDimensions.x * item.originalWorldDimensions.y > 90000)
				return true;

			return false;
		}
		public static bool PlanetIsMiniBase(StarmapItem item)
		{
			if (item.IsPOI)
				return false;

			string KeyUpper = item.id.ToUpperInvariant();

			return PlanetByIdIsMiniBase(KeyUpper);
		}
		public static bool PlanetByIdIsMiniBase(string KeyUpper)
		{
			KeyUpper = KeyUpper.ToUpperInvariant();
			return KeyUpper.Contains("BABY") || KeyUpper.Contains("MINIBASE");
		}

		///Random Asteroids stay random in preview

		public static ClusterLayout GenerateDummyCluster(bool spacedOut, bool ceres, bool prehistoric)
		{
			ClusterLayout clusterLayout = new ClusterLayout();
			clusterLayout.filePath = CustomClusterID;
			clusterLayout.name = "STRINGS.CLUSTER_NAMES.CGM.NAME";
			clusterLayout.description = "STRINGS.CLUSTER_NAMES.CGM.DESCRIPTION";
			clusterLayout.worldPlacements = new List<WorldPlacement>();
			clusterLayout.coordinatePrefix = CustomClusterIDCoordinate;
			clusterLayout.clusterTags.Add(CustomClusterClusterTag);
			if (ceres)
			{
				clusterLayout.clusterTags.Add("CeresCluster");
				clusterLayout.clusterTags.Add("GeothermalImperative");
				clusterLayout.clusterAudio = DlcAudioSettings[DlcManager.DLC2_ID].ToAudioSetting();
			}
			if (prehistoric)
			{
				clusterLayout.clusterTags.Add("PrehistoricCluster");
				clusterLayout.clusterTags.Add("DemoliorImperative");
				clusterLayout.clusterAudio = DlcAudioSettings[DlcManager.DLC4_ID].ToAudioSetting();
			}
			return clusterLayout;
		}

		public static ClusterLayout GenerateClusterLayoutFromCustomData(bool log, bool logPois = false)
		{
			if (log)
				SgtLogger.l("Started generating custom cluster layout");
			var layout = new ClusterLayout();
			CurrentClassicOuterPlanets = 0;


			string setting = selectScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
			int seed = int.Parse(setting);
			if (log)
				SgtLogger.l(setting, "CurrentSeed");
			if (log)
				SgtLogger.l(MaxClassicOuterPlanets.ToString(), "Max. allowed classic sized");

			//var Reference = SettingsCache.clusterLayouts.GetClusterData(ClusterID);
			//SgtLogger.log(Reference.ToString());
			layout.filePath = CustomClusterID;
			layout.name = "STRINGS.CLUSTER_NAMES.CGM.NAME";
			layout.description = "STRINGS.CLUSTER_NAMES.CGM.DESCRIPTION";
			layout.worldPlacements = new List<WorldPlacement>();
			layout.coordinatePrefix = CustomClusterIDCoordinate;
			layout.clusterTags = [CustomClusterClusterTag];

			if (DlcManager.IsExpansion1Active())
				layout.requiredDlcIds = [DlcManager.EXPANSION1_ID];
			else
				layout.forbiddenDlcIds = [DlcManager.EXPANSION1_ID];

			float multiplier = 1f;
			if (CustomCluster.Rings > CustomCluster.defaultRings)
			{
				multiplier = CustomCluster.Rings / (float)CustomCluster.defaultRings;
			}
			if (log)
				SgtLogger.l("Cluster Size: " + (CustomCluster.Rings + 1));
			if (log)
				SgtLogger.l("Placement Multiplier: " + multiplier);

			if (CustomCluster.StarterPlanet != null)
			{

				if (PlanetIsClassic(CustomCluster.StarterPlanet))
					CurrentClassicOuterPlanets++;

				///Disabling Story Traits if MiniBase worlds are active
				if (CustomCluster.StarterPlanet.DisablesStoryTraits)
					layout.disableStoryTraits = true;

				if (CustomCluster.StarterPlanet.id.Contains(RandomKey))
				{
					var randomItem = GetRandomItemOfType(StarmapItemCategory.Starter, seed);
					var placement = GivePrefilledItem(randomItem).placement;

					placement.allowedRings = CustomCluster.StarterPlanet.placement.allowedRings;
					placement.buffer = CustomCluster.StarterPlanet.placement.buffer;

					placement.startWorld = true;
					placement.locationType = LocationType.Startworld;

					layout.worldPlacements.Add(placement);
					if (log)
						SgtLogger.l(randomItem.id, "Random Start Planet");

				}
				else
				{
					CustomCluster.StarterPlanet.placement.startWorld = true;
					layout.worldPlacements.Add(CustomCluster.StarterPlanet.placement);
					if (log)
						SgtLogger.l(CustomCluster.StarterPlanet.id, "Start Planet");
				}
				seed++;
			}
			else
				SgtLogger.error("No start planetData selected");

			if (CustomCluster.WarpPlanet != null)
			{
				if (PlanetIsClassic(CustomCluster.WarpPlanet))
					CurrentClassicOuterPlanets++;
				///Disabling Story Traits if MiniBase worlds are active
				if (CustomCluster.WarpPlanet.DisablesStoryTraits)
					layout.disableStoryTraits = true;

				//if (CustomCluster.StarterPlanet.placement.allowedRings.max == 0
				//    && CustomCluster.WarpPlanet.placement.allowedRings.max < CustomCluster.StarterPlanet.placement.buffer)
				//{
				//    var vector = CustomCluster.WarpPlanet.placement.allowedRings;
				//    CustomCluster.WarpPlanet.placement.allowedRings = new MinMaxI(vector.min, CustomCluster.StarterPlanet.placement.buffer + 1);
				//}
				if (CustomCluster.WarpPlanet.id.Contains(RandomKey))
				{
					var randomItem = GetRandomItemOfType(StarmapItemCategory.Warp, seed);
					var placement = GivePrefilledItem(randomItem).placement;

					placement.allowedRings = CustomCluster.WarpPlanet.placement.allowedRings;
					placement.buffer = CustomCluster.WarpPlanet.placement.buffer;

					layout.worldPlacements.Add(placement);
					if (log)
						SgtLogger.l(randomItem.id, "Random Warp Planet");
				}
				else
				{
					layout.worldPlacements.Add(CustomCluster.WarpPlanet.placement);
					if (log)
						SgtLogger.l(CustomCluster.WarpPlanet.id, "Warp Planet");
				}
				seed++;
			}
			else
			{
				if (DlcManager.IsExpansion1Active())
				{
					if (log)
						SgtLogger.log("No warp planetData selected");
					CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.Teleporters, (CustomGameSettingConfigs.Teleporters as ToggleSettingConfig).off_level.id);
				}
			}
			///STRINGS.NAMEGEN.WORLD.ROOTS

			RandomOuterPlanets.Clear();
			List<StarmapItem> OuterPlanets = CustomCluster.OuterPlanets.Values.ToList();
			if (OuterPlanets.Count > 0)
			{
				if (log)
					SgtLogger.l(OuterPlanets.Count + " outer planets selected");
				OuterPlanets =
					OuterPlanets
					.OrderBy(item => item.placement.allowedRings.max)
					.ThenBy(item => item.placement.allowedRings.max - item.placement.allowedRings.min)
					.ThenBy(item => item.placement.world)
					.ToList();

				foreach (var world in OuterPlanets)
				{
					///Disabling Story Traits if MiniBase worlds are active
					if (world.DisablesStoryTraits)
						layout.disableStoryTraits = true;

					if (world.id.Contains(RandomKey))
					{
						if (log)
							SgtLogger.l(world.InstancesToSpawn.ToString(), "Random Planets to select");
						for (int i = 0; i < (int)world.InstancesToSpawn; i++)
						{
							var randomItem = GetRandomItemOfType(StarmapItemCategory.Outer, seed);
							seed++;

							if (randomItem == null)
							{

								if (log)
									SgtLogger.l("Failed to get unused", "Random Outer Planet");
								break;
							}

							var placement = GivePrefilledItem(randomItem).placement;
							placement.allowedRings = world.placement.allowedRings;
							placement.buffer = world.placement.buffer;

							//ApplySizeMultiplier(placement, multiplier);

							layout.worldPlacements.Add(placement);

							if (log)
								SgtLogger.l(randomItem.id, "selected random outer Planet");
						}
					}
					else
					{
						var placement = world.placement;

						if (placement.worldMixing.mixingWasApplied)
						{
							SgtLogger.l("mixing placement: " + placement.world);
						}

						//ApplySizeMultiplier(placement, multiplier);
						layout.worldPlacements.Add(placement);
						if (log)
							SgtLogger.l(world.id, "Outer Planet");
						seed++;
					}
				}
			}

			//layout.worldPlacements = layout.worldPlacements.OrderBy(item => item.allowedRings.max).ThenBy(item => item.allowedRings.min).ToList();
			//layout.startWorldIndex = layout.worldPlacements.FindIndex(placement => placement.startWorld == true);

			if (log)
				SgtLogger.l("Planet Placements done");
			layout.poiPlacements = new List<SpaceMapPOIPlacement>();

			seed = int.Parse(setting) + 100;
			foreach (var poi in CustomCluster.POIs)
			{
				var radomns = poi.Value.placementPOI.pois.Any(i => i.Contains(RandomKey));

				//poi.Value.placementPOI.pois.RemoveAll(i => i.Contains(RandomKey));

				poi.Value.placementPOI.numToSpawn = Mathf.FloorToInt(poi.Value.InstancesToSpawn);
				float percentageAdditional = poi.Value.InstancesToSpawn % 1f;
				if (percentageAdditional > 0)
				{
					float rolledChance = new System.Random(seed).Next(1, 101) / 100f;
					if (rolledChance < percentageAdditional)
					{
						if (log && logPois)
							SgtLogger.l(poi.Value.id + ", succeeded: " + rolledChance * 100f, "POI Chance: " + percentageAdditional.ToString("P"));
						poi.Value.placementPOI.numToSpawn += 1;
					}
					else
					{
						if (log && logPois)
							SgtLogger.l(poi.Value.id + ", failed: " + rolledChance * 100f, "POI Chance: " + percentageAdditional.ToString("P"));
					}
					seed++;
				}
				if (radomns)
				{
					poi.Value.placementPOI.canSpawnDuplicates = true;
				}
				if (log && logPois)
					poi.Value.placementPOI.pois.ForEach(poi => SgtLogger.l(poi, "poi in group"));

				if (poi.Value.placementPOI.pois.Count == 0)
				{
					continue;
				}


				if (poi.Value.placementPOI.pois.Count < poi.Value.placementPOI.numToSpawn)
					poi.Value.placementPOI.canSpawnDuplicates = true;



				if (log && logPois)
					SgtLogger.l($"\navoidClumping: {poi.Value.placementPOI.avoidClumping},\nguarantee: {poi.Value.placementPOI.guarantee},\nallowDuplicates: {poi.Value.placementPOI.canSpawnDuplicates},\nRings: {poi.Value.placementPOI.allowedRings}\nNumberToSpawn: {poi.Value.placementPOI.numToSpawn}", "POIGroup " + poi.Key.Substring(0, 8));

				if (poi.Value.placementPOI.numToSpawn >= 1)
				{
					layout.poiPlacements.Add(poi.Value.placementPOI);
				}
			}

			if (log && logPois)
				SgtLogger.l("POI Placements done");
			layout.numRings = CustomCluster.Rings + 1;

			if (log && logPois)
				SgtLogger.l("Ordering Asteroids");

			List<StarmapItem> allPlanets = CustomCluster.GetAllPlanets();
			if (log)
			{
				foreach (var item in allPlanets)
				{
					SgtLogger.l(item.PredefinedPlacementOrder.ToString(), item.id);
				}
			}
			PostProcessCluster(layout, allPlanets, CustomCluster.StarterPlanet);


			if (CustomCluster.GetAllPlanets().All(item => item.PredefinedPlacementOrder != -1))
			{
				if (log)
					SgtLogger.l("vanilla cluster loadout, using predefined order");
				layout.worldPlacements =
					layout.worldPlacements.OrderBy(placement =>
					{
						CustomCluster.HasStarmapItem(placement.world, out var item);
						return item.PredefinedPlacementOrder;
					})
					.ThenBy(placement => placement.world).ToList();
			}
			else
			{
				if (log)
					SgtLogger.l("not all planets had predefined world order, using fallback method");
				///Not perfect, hence using the original order if nothing had changed:
				layout.worldPlacements = layout.worldPlacements
					.OrderBy(placement => placement.allowedRings.max)
					.ThenBy(placement => placement.allowedRings.max - placement.allowedRings.min)
					.ThenBy(placement => placement.world).ToList();
			}
			foreach (var world in layout.worldPlacements)
			{
				if (log)
					SgtLogger.l(world.world + ": " + world.allowedRings.ToString());
			}


			if (CustomCluster.StarterPlanet == null)
				layout.startWorldIndex = 0;
			else
			{
				layout.startWorldIndex = layout.worldPlacements.FindIndex(item => item.startWorld == true);
			}
			if (log)
				SgtLogger.l("StartIndex: " + layout.startWorldIndex);

			if (log)
				SgtLogger.l("Finished generating custom cluster");

			lastWorldGenFailed = false;
			return layout;
		}

		private static void PostProcessCluster(ClusterLayout layout, List<StarmapItem> planets, StarmapItem starterPlanet)
		{
			SgtLogger.l("PostProcessing custom cluster");
			if (starterPlanet.world?.requiredDlcIds != null)
			{
				foreach (var reqDlc in starterPlanet.world.requiredDlcIds)
				{
					if (DlcAudioSettings.TryGetValue(reqDlc, out var audioSettings))
					{
						SgtLogger.l("found custom audio setting");
						layout.clusterAudio = audioSettings.ToAudioSetting();
						break;
					}
				}
			}
			foreach (var item in planets)
			{
				var world = item.world;
				if (world == null)
				{
					SgtLogger.warning("World for item " + item.id + " is null, skipping post processing");
					continue;
				}
				if (CGMWorldGenUtils.HasGeothermalPump(world) && !layout.clusterTags.Contains("CeresCluster"))
				{
					layout.clusterTags.Add("CeresCluster");
					layout.clusterTags.Add("GeothermalImperative");
				}
				if (CGMWorldGenUtils.HasImpactorShower(world) && !layout.clusterTags.Contains("PrehistoricCluster"))
				{
					layout.clusterTags.Add("PrehistoricCluster");
					if (world.filePath.ToLowerInvariant().Contains("shattered")) //lab relica with 10 cycles of impact
					{
						layout.clusterTags.Add("DemoliorImminentImpact");
						layout.clusterTags.Add("DemoliorSurivedAchievement");
					}
					else
						layout.clusterTags.Add("DemoliorImperative");
				}
			}
			SgtLogger.l("PostProcessing finished");
		}

		static string LastPresetGenerated = string.Empty;
		public static void ResetToLastPreset()
		{
			if (LastPresetGenerated != string.Empty)
			{
				CreateCustomClusterFrom(LastPresetGenerated);
			}
		}
		public static void ResetPlanetFromPreset(string PlanetID)
		{
			if (LastPresetGenerated != string.Empty)
			{
				CreateCustomClusterFrom(LastPresetGenerated, PlanetID);
			}
		}
		/// <summary>
		/// Global world seed will always be the seed of the last asteroid in the cluster
		/// </summary>
		public static int GlobalWorldSeed => CurrentSeed - 1 + CustomCluster.GetAllPlanets().Count;
		public static int CurrentSeed => int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id);


		public static string GetPOIGroupId(SpaceMapPOIPlacement _placement, bool includeTimeForUId = false)
		{
			string data = string.Empty;
			_placement.pois.ForEach(id => data += id);
			data += _placement.numToSpawn;
			data += _placement.avoidClumping;
			data += _placement.canSpawnDuplicates;
			data += _placement.allowedRings.ToString();
			if (includeTimeForUId)
				data += System.DateTime.Now.ToString();
			return GenerateHash(data);

		}
		public static string GenerateHash(string str)
		{
			using var md5Hasher = MD5.Create();
			var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
			return BitConverter.ToString(data).Replace("-", "");
		}

		public static List<StarmapItem> GetPOIGroups(ClusterLayout cluster)
		{
			var values = new List<StarmapItem>();

			List<SpaceMapPOIPlacement> placements = new(cluster.poiPlacements);
			foreach (var dlcmixing in CustomGameSettings.Instance.GetCurrentDlcMixingIds())
			{
				DlcMixingSettings dlcMixingSettings = SettingsCache.GetCachedDlcMixingSettings(dlcmixing);
				if (dlcMixingSettings != null && dlcMixingSettings.spacePois != null && dlcMixingSettings.spacePois.Any())
				{
					//no ceres mixing pois on ceres cluster / dlc4 mixing on dlc4is enabled, 
					if (cluster.requiredDlcIds != null && cluster.requiredDlcIds.Contains(dlcmixing))
					{
						SgtLogger.l("skipping " + dlcmixing + " mixing space pois because the cluster is from that dlc");
						continue;
					}
					placements.AddRange(dlcMixingSettings.spacePois);
					SgtLogger.l(dlcmixing + " is enabled, adding mixing space pois");
				}

			}

			foreach (SpaceMapPOIPlacement pOIPlacement in placements)
			{
				if (pOIPlacement.pois == null)
					continue;

				string id = GetPOIGroupId(pOIPlacement);
				var item = new StarmapItem(id, StarmapItemCategory.POI, null);
				item.MakeItemPOI(pOIPlacement);
				values.Add(item);
			}


			return values;
		}

		public static void CreateCustomClusterFrom(string clusterID, string singleItemId = "", bool ForceRegen = false)
		{
			bool ResetSingleItem = singleItemId != string.Empty;


			if (ResetSingleItem)
				SgtLogger.l("Regenerating stats for " + singleItemId + " in " + clusterID);
			else
				SgtLogger.l("Generating custom cluster from " + clusterID);

			if (lastWorldGenFailed && !ForceRegen)
				return;
			ClusterLayout ReferenceLayout = SettingsCache.clusterLayouts.GetClusterData(clusterID);

			if (ReferenceLayout == null || selectScreen == null || selectScreen.newGameSettingsPanel == null)
				return;

			string seedSetting = selectScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.WorldgenSeed);

			if (seedSetting == null || seedSetting.Length == 0)
				return;

			int seed = int.Parse(seedSetting);

			var mutated = new MutatedClusterLayout(ReferenceLayout);
			WorldgenMixing.RefreshWorldMixing(mutated, seed, true, true);

			if (!ResetSingleItem)
			{
				SgtLogger.l("Rebuilding Cluster Data");
				CustomCluster = new CustomClusterData();
				CustomCluster.SetRings(mutated.layout.numRings - 1, true);

				foreach (var planet in PlanetoidDict.Values)
				{
					planet.ClearWorldTraits();
					planet.ClearGeyserOverrides();
					planet.SetPlanetSizeToPreset(WorldSizePresets.Normal);
					planet.SetPlanetRatioToPreset(WorldRatioPresets.Normal);

					if (planet.world == null) continue;
					ResetMeteorSeasons(planet.world);
					ResetSkyFixedTraits(planet.world);
				}
				ResetSharedGeyserOverrides();
				ResetStarmap();
			}
			else
			{
				///when planet not normally in cluster, but selected rn and to reset - reload from data
				if (!mutated.layout.worldPlacements.Any(placement => placement.world == singleItemId))
				{
					if (PlanetoidDict.TryGetValue(singleItemId, out var FoundPlanet))
					{
						FoundPlanet = GivePrefilledItem(FoundPlanet);
						switch (FoundPlanet.category)
						{
							case StarmapItemCategory.Starter:
								CustomCluster.StarterPlanet = FoundPlanet;
								break;
							case StarmapItemCategory.Warp:
								CustomCluster.WarpPlanet = FoundPlanet;
								break;
							case StarmapItemCategory.Outer:
								CustomCluster.OuterPlanets[FoundPlanet.id] = FoundPlanet;
								break;
						}
						ResetMeteorSeasons(FoundPlanet.world);
						ResetSkyFixedTraits(FoundPlanet.world);
						FoundPlanet.ClearWorldTraits();
						FoundPlanet.ClearGeyserOverrides();
						//SgtLogger.l("Grabbing Traits");
						//foreach (var planetTrait in SettingsCache.GetRandomTraits(seed, FoundPlanet.world))
						//{
						//    WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
						//    FoundPlanet.AddWorldTrait(cachedWorldTrait);
						//    SgtLogger.l(planetTrait, FoundPlanet.DisplayName);
						//}
						FoundPlanet.SetPlanetSizeToPreset(WorldSizePresets.Normal);
						FoundPlanet.SetPlanetRatioToPreset(WorldRatioPresets.Normal);
					}
				}
			}

			for (int i = 0; i < mutated.layout.worldPlacements.Count; i++)
			{
				WorldPlacement planetPlacement = mutated.layout.worldPlacements[i];
				string planetpath = planetPlacement.world;
				//SgtLogger.l(planetpath, "planet in cluster");
				string mixingAsteroid = string.Empty;
				bool isWorldMixed = false;

				if (planetPlacement.worldMixing != null && planetPlacement.worldMixing.mixingWasApplied)
				{
					mixingAsteroid = planetpath;
					planetpath = planetPlacement.worldMixing.previousWorld;
					isWorldMixed = true;
					SgtLogger.l(planetpath, "MIXING");
				}

				if (PlanetoidDict.TryGetValue(planetpath, out StarmapItem FoundPlanet))
				{
					//SgtLogger.l(FoundPlanet.category.ToString());
					if (singleItemId != string.Empty && FoundPlanet.id != singleItemId)
					{
						continue;
					}
					FoundPlanet.PredefinedPlacementOrder = i;

					FoundPlanet.AddItemWorldPlacement(planetPlacement);
					SetMixingWorld(FoundPlanet, null);

					if (isWorldMixed && PlanetoidDict.TryGetValue(mixingAsteroid, out StarmapItem _mixingItem))
					{
						SetMixingWorld(FoundPlanet, _mixingItem);
					}

					switch (FoundPlanet.category)
					{
						case StarmapItemCategory.Starter:
							CustomCluster.StarterPlanet = FoundPlanet;
							break;
						case StarmapItemCategory.Warp:
							CustomCluster.WarpPlanet = FoundPlanet;
							break;
						case StarmapItemCategory.Outer:
							CustomCluster.OuterPlanets[FoundPlanet.id] = FoundPlanet;
							break;
					}

					ResetMeteorSeasons(FoundPlanet.world);
					ResetSkyFixedTraits(FoundPlanet.world);
					FoundPlanet.ClearWorldTraits();
					FoundPlanet.ClearGeyserOverrides();

					//SgtLogger.l("Grabbing Traits");
					//int seedTrait = FoundPlanet.IsMixed ? seed - 1 : seed + i; //mixing target is not in cluster -> position will be -1 in original code (potentially adjust in the future)
					
					///The statement above is what the start screen uses - this is a bug.
					///during worldgen it uses the index of the replaced asteroid, yielding different traits than shown
					///using the coordinate everywhere yields the proper mirrored result
					int seedTrait = seed + i;
					var traits = SettingsCache.GetRandomTraits(seedTrait, FoundPlanet.world);
					foreach (var planetTrait in traits)
					{
						WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
						FoundPlanet.AddWorldTrait(cachedWorldTrait);
						//SgtLogger.l(planetTrait, FoundPlanet.id);
					}
					FoundPlanet.SetPlanetSizeToPreset(WorldSizePresets.Normal);
					FoundPlanet.SetPlanetRatioToPreset(WorldRatioPresets.Normal);
				}
				else
					SgtLogger.warning("could not find asteroid in dict: " + planetpath);
			}
			LastPresetGenerated = clusterID;
			RegeneratePOIDataFrom(clusterID, singleItemId);
		}
		public static void SetMixingWorld(StarmapItem target, StarmapItem mixingSource)
		{
			SgtLogger.l(target.id + " is getting remixed with " + (mixingSource?.id ?? "nothing"));
			if (mixingSource != null)
			{
				if (CustomCluster.MixingWorldsWithTarget.TryGetValue(mixingSource, out var OldTarget)
					&& OldTarget != target
					&& OldTarget.MixingAsteroidSource == mixingSource
					)
				{
					OldTarget.SetWorldMixing(null);
				}
				CustomCluster.MixingWorldsWithTarget[mixingSource] = target;
			}
			else
			{
				SgtLogger.l("clearing mixing on " + target.DisplayName);
			}
			target.SetWorldMixing(mixingSource);
		}

		public static void RegenerateAllPOIData()
		{
			SgtLogger.l("Regenerating all POI Data");
			RegeneratePOIDataFrom(LastPresetGenerated);
		}

		public static void RegeneratePOIDataFrom(string clusterID, string singleItemId = "")
		{
			ClusterLayout Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);
			if (LastPresetGenerated == null || LastPresetGenerated.Length == 0)
				return;


			if (singleItemId == string.Empty)
			{
				//CustomCluster.defaultOuterPlanets = Reference.worldPlacements.Count;
				CustomCluster.POIs.Clear();
				CustomCluster.SetRings(Reference.numRings - 1, true);
				SgtLogger.l("Resetting all POIs");
			}
			else
			{
				if (CustomCluster.POIs.ContainsKey(singleItemId))
				{
					CustomCluster.POIs.Remove(singleItemId);
				}
				SgtLogger.l("Resetting POI: " + singleItemId);
			}
			SgtLogger.l("building new POIs");
			if (Reference.poiPlacements != null)
			{
				var items = GetPOIGroups(Reference);
				bool resettingSingle = singleItemId != string.Empty;
				foreach (var item in items)
				{
					if (resettingSingle && item.id != singleItemId)
					{
						continue;
					}
					CustomCluster.POIs[item.id] = item;
				}
				if (CGM_Screen != null)
				{
					if (resettingSingle)
					{
						if (CustomCluster.HasStarmapItem(singleItemId, out var item))
							CGM_Screen.SelectItem(item);
						else
							CGM_Screen.DeselectCurrentItem();
					}
				}
			}
			else if (DlcManager.IsExpansion1Active())
				SgtLogger.l("poi placements were null");

			CustomCluster.SO_Starmap = null;

			if (CGM_Screen != null)
			{
				CGM_Screen.PresetApplied = false;
				if (!DlcManager.IsExpansion1Active())
					CGM_Screen.RebuildVanillaStarmap(true);
			}
		}

		public static bool RerollStarmapWithSeedChange = true;
		public static bool RerollTraitsWithSeedChange = true;
		public static bool RerollMixingsWithSeedChange = true;

		public static void RemoveActiveMixingAsteroids()
		{
			SgtLogger.l("Clearing all asteroid remixes");
			//undo all ongoing worldmixings
			foreach (var planet in CustomCluster.GetAllPlanets())
			{
				if (planet == null || planet.world == null || planet.placement == null) continue;
				SetMixingWorld(planet, null);
			}
		}
		public static void RerollMixings()
		{
			if (CustomCluster == null || !RerollMixingsWithSeedChange && CGM_Screen.IsCurrentlyActive)
				return;

			///!!ALWAYS clear mixings before applying them en masse
			RemoveActiveMixingAsteroids();

			int seed = int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id);

			//grab the unmixed layout
			var layout = GeneratedLayout;
			//temporarily remove the cluster tag so it can roll the mixings
			layout.clusterTags.Remove(CustomClusterClusterTag);
			var mutated = new MutatedClusterLayout(layout);
			//throw it throught the mixer to apply all the mixings , todo: grab biome mixings somewhere
			WorldgenMixing.RefreshWorldMixing(mutated, seed, true, true);
			//readd cluster tags
			mutated.layout.clusterTags.Add(CustomClusterClusterTag);

			Dictionary<string, string> mixingTargets = new Dictionary<string, string>();
			foreach (var planetPlacement in mutated.layout.worldPlacements)
			{
				bool isWorldMixed = false;
				string planetpath = planetPlacement.world;
				string mixingAsteroid = string.Empty;
				if (planetPlacement.worldMixing != null && planetPlacement.worldMixing.mixingWasApplied)
				{
					mixingAsteroid = planetpath;
					planetpath = planetPlacement.worldMixing.previousWorld;
					isWorldMixed = true;
					SgtLogger.l(planetpath, "Mixed original");
					mixingTargets[planetpath] = mixingAsteroid;
				}
				//apply mixing relations for UI
				if (CustomCluster.HasStarmapItem(planetpath, out var FoundPlanet))
				{
					SetMixingWorld(FoundPlanet, null);
				}
				else
					SgtLogger.warning(planetpath + " not found for mixing relation!");
			}
			foreach (var target in mixingTargets)
			{
				if (CustomCluster.HasStarmapItem(target.Key, out var mixTarget)
					&& PlanetoidDict.TryGetValue(target.Value, out var mixing))
				{
					SgtLogger.l("Mixing " + mixing.DisplayName + " into " + mixTarget.DisplayName);
					SetMixingWorld(mixTarget, mixing);
				}
				else
					SgtLogger.error("could not mix " + target.Value + " into " + target.Key);
			}

		}
		public static void RerollTraits()
		{
			if (CustomCluster == null || !RerollTraitsWithSeedChange && CGM_Screen.IsCurrentlyActive)
				return;


			int seed = int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id);

			var planets = CustomCluster.GetAllPlanets();
			for (int i = 0; i < planets.Count; i++)
			{
				var FoundPlanet = planets[i];
				if (FoundPlanet.world == null) continue;

				FoundPlanet.ClearWorldTraits();
				foreach (var planetTrait in SettingsCache.GetRandomTraits(seed + i, FoundPlanet.world))
				{
					WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
					FoundPlanet.AddWorldTrait(cachedWorldTrait);
				}
			}
		}

		public static StarmapItem GivePrefilledItem(StarmapItem ToAdd)
		{
			if (ToAdd.id.Contains(RandomKey))
				return ToAdd;

			if (ToAdd.id == null)
				return ToAdd;

			if (PredefinedPlacementData.ContainsKey(ToAdd.id))
				ToAdd.AddItemWorldPlacement(PredefinedPlacementData[ToAdd.id]);
			else
			{
				if (!ToAdd.IsPOI)
				{

					MinMaxI startCoords = new(0, CustomCluster.Rings);
					if (ToAdd.category == StarmapItemCategory.Starter)
						startCoords = new MinMaxI(0, 0);
					else if (ToAdd.category == StarmapItemCategory.Warp)
						startCoords = new MinMaxI(3, 5);

					if (ModAssets.Moonlets.Any(moonlet => ToAdd.id.Contains(moonlet)))
					{
						if (PredefinedPlacementData.TryGetValue(ToAdd.id.Replace("Start", ""), out var OuterVariant))
						{
							startCoords = OuterVariant.allowedRings;
						}
						else if (PredefinedPlacementData.TryGetValue(ToAdd.id.Replace("Warp", ""), out var OuterVariant2))
						{
							startCoords = OuterVariant2.allowedRings;
						}
						else if (PredefinedPlacementData.TryGetValue(ToAdd.id + "Start", out var StartVariant))
						{
							startCoords = StartVariant.allowedRings;
						}
						else if (PredefinedPlacementData.TryGetValue(ToAdd.id + "Warp", out var WarpVariant))
						{
							startCoords = WarpVariant.allowedRings;
						}
					}

					var item = new WorldPlacement();
					item.world = ToAdd.id;
					item.allowedRings = startCoords;
					item.startWorld = ToAdd.category == StarmapItemCategory.Starter;
					ToAdd.AddItemWorldPlacement(item);
				}
			}

			return ToAdd;
		}
		private static void SetMixingSetting(SettingConfig ConfigToSet, object valueId)
		{
			if (!DlcManager.IsAllContentSubscribed(ConfigToSet.required_content))
				return;

			string valueToSet = valueId.ToString();
			if (valueId is bool val)
			{
				var toggle = ConfigToSet as ToggleSettingConfig;
				valueToSet = val ? toggle.on_level.id : toggle.off_level.id;
			}
			//SgtLogger.l("changing " + ConfigToSet.id.ToString() + " from " + CustomGameSettings.Instance.GetCurrentMixingSettingLevel(ConfigToSet).id + " to " + valueToSet.ToString());
			CustomGameSettings.Instance.SetMixingSetting(ConfigToSet, valueToSet);
		}

		public static void PreProcessAsteroidAdding(StarmapItem adding)
		{
			foreach (var dlcID in DlcManager.DLC_PACKS.Keys)
			{
				if (adding.IsDlcRequired(dlcID))
				{
					ToggleWorldgenAffectingDlc(true, dlcID);
				}
			}
			if (CGMWorldGenUtils.HasGeothermalPump(adding.world))//geothermal pump story trait from mod
			{
				DisableModdedGeopumpStoryTrait();
			}
			if (CGMWorldGenUtils.HasImpactorShower(adding.world)) //impactor shower from potential mod in the future
			{
				DisableModdedImpactorShowerStoryTrait();
			}
		}
		public static void DisableModdedImpactorShowerStoryTrait()
		{
			if (!CustomGameSettings.Instance.StorySettings.TryGetValue(CGMWorldGenUtils.CGM_Impactor_StoryTrait, out var storyTrait))
				return;
			bool isCurrentlyEnabled = CustomGameSettings.Instance.GetCurrentStoryTraitSetting(storyTrait).id == StoryContentPanel.StoryState.Guaranteed.ToString();

			if (!isCurrentlyEnabled)
				return;
			CustomGameSettings.Instance.SetStorySetting(storyTrait, false);
		}
		public static void DisableModdedGeopumpStoryTrait()
		{
			if (!CustomGameSettings.Instance.StorySettings.TryGetValue(CGMWorldGenUtils.CGM_Heatpump_StoryTrait, out var storyTrait))
				return;
			bool isCurrentlyEnabled = CustomGameSettings.Instance.GetCurrentStoryTraitSetting(storyTrait).id == StoryContentPanel.StoryState.Guaranteed.ToString();

			if (!isCurrentlyEnabled)
				return;
			CustomGameSettings.Instance.SetStorySetting(storyTrait, false);
		}

		public static void ToggleWorldgenAffectingDlc(bool enabled, string dlcId)
		{
			var settingsInstance = CustomGameSettings.Instance;
			if (!settingsInstance.MixingSettings.ContainsKey(dlcId))
			{
				SgtLogger.error("Tried to toggle a non-existing DLC mixing setting: " + dlcId);
				return;
			}

			var dlcSetting = CustomGameSettings.Instance.MixingSettings[dlcId] as ToggleSettingConfig;
			var currentLevel = CustomGameSettings.Instance.GetCurrentMixingSettingLevel(dlcId);
			if ((currentLevel == dlcSetting.on_level && enabled) || (currentLevel == dlcSetting.off_level && !enabled))
				return;

			SetMixingSetting(dlcSetting, enabled);
			RegenerateAllPOIData();
			CGM_Screen?.RebuildStarmap(true);
		}
		public static void ToggleNonWorldGenDlc(bool enabled, SettingConfig dlc)
		{
			SetMixingSetting(dlc, enabled);
		}

		public static void TogglePlanetoid(StarmapItem ToAdd)
		{
			var item = GivePrefilledItem(ToAdd); ///Prefilled
												 ///only one starter at a time

			if (!CustomCluster.HasStarmapItem(ToAdd.id, out _))
				PreProcessAsteroidAdding(item);


			CustomCluster.SO_Starmap = null;
			if (item.category == StarmapItemCategory.Starter)
			{
				if (item.Equals(CustomCluster.StarterPlanet))
					return;
				else
				{
					CustomCluster.StarterPlanet = item;
					return;
				}
			}
			///only one teleport asteroid at a time 
			else if (item.category == StarmapItemCategory.Warp)
			{
				if (item.Equals(CustomCluster.WarpPlanet))
				{
					CustomCluster.WarpPlanet = null;
					return;
				}
				else
				{
					CustomCluster.WarpPlanet = item;
					return;
				}
			}
			if (ToAdd.category != StarmapItemCategory.POI)
			{
				if (!CustomCluster.OuterPlanets.ContainsKey(item.id))
					CustomCluster.OuterPlanets[item.id] = item;
				else
					CustomCluster.OuterPlanets.Remove(item.id);
			}
			else
			{
				if (!CustomCluster.POIs.ContainsKey(item.id))
					CustomCluster.POIs[item.id] = item;
				else
					CustomCluster.POIs.Remove(item.id);
			}
			return;
		}

		public static List<StarmapItem> GetActivePlanetsStarmapitems()
		{
			var planets = new List<StarmapItem>();

			if (CustomCluster.StarterPlanet != null)
				planets.Add(CustomCluster.StarterPlanet);

			if (CustomCluster.WarpPlanet != null)
				planets.Add(CustomCluster.WarpPlanet);

			foreach (var planet in CustomCluster.OuterPlanets)
			{
				planets.Add(planet.Value);
			}
			return planets;
		}
		public static List<string> GetActivePlanetsCluster()
		{
			var planetPaths = new List<string>();

			if (CustomCluster.StarterPlanet != null)
				planetPaths.Add(CustomCluster.StarterPlanet.id);

			if (CustomCluster.WarpPlanet != null)
				planetPaths.Add(CustomCluster.WarpPlanet.id);

			foreach (var planet in CustomCluster.OuterPlanets)
			{
				planetPaths.Add(planet.Key);
			}
			return planetPaths;
		}

		public static void PopulatePredefinedClusterPlacements()
		{
			if (PlacementDataInitialized) { return; }

			SgtLogger.l("Populating cluster placements");
			_predefinedPlacementData = new Dictionary<string, WorldPlacement>();
			//PredefinedPlacementDataPOI = new Dictionary<string, SpaceMapPOIPlacement>();
			_dlcAudioSettings = new();

			foreach (var ClusterLayout in SettingsCache.clusterLayouts.clusterCache.ToList())
			{
				var clusterId = ClusterLayout.Key;
				var clusterData = ClusterLayout.Value;
				if (DlcManager.IsExpansion1Active())
				{
					if (clusterId.Contains("clusters/SandstoneDefault") // Comment out when klei removes the default cluster
						|| clusterData.forbiddenDlcIds != null && clusterData.forbiddenDlcIds.Contains(DlcManager.EXPANSION1_ID)
						|| !DlcManager.IsCorrectDlcSubscribed(clusterData))
					{
						continue;
					}
				}
				bool disablesStoryTraits = clusterData.disableStoryTraits;
				var tags = clusterData.clusterTags;

				foreach (var planet in clusterData.worldPlacements)
				{
					if (PlanetsAndPOIs.TryGetValue(planet.world, out StarmapItem starmapItem))
					{
						if (disablesStoryTraits)
						{
							SgtLogger.l(planet.world + " will disable story traits");
							starmapItem.DisablesStoryTraits = true;
						}
					}
					else
						SgtLogger.warning("Tried to add disabling story traits to a planetData that does not exist: " + planet.world);
				}


				foreach (var planetPlacement in ClusterLayout.Value.worldPlacements)
				{
					PredefinedPlacementData[planetPlacement.world] = planetPlacement;
				}

				if (clusterData.clusterAudio != null
					&& !clusterData.dlcIdFrom.IsNullOrWhiteSpace()
					&& !clusterData.dlcIdFrom.Contains("DLC") //no basegame/spacedout 
					&& _dlcAudioSettings.ContainsKey(clusterData.dlcIdFrom))
				{
					SgtLogger.l("Caching audio data for dlc: " + clusterData.dlcIdFrom);
					var sourceAudio = clusterData.clusterAudio;
					_dlcAudioSettings[clusterData.dlcIdFrom] = new()
					{
						musicWelcome = sourceAudio.musicWelcome,
						musicFirst = sourceAudio.musicFirst,
						stingerDay = sourceAudio.stingerDay,
						stingerNight = sourceAudio.stingerNight
					};
				}
			}
		}

		public static string GetRandomPOI(int seed)
		{
			var ItemList = ModAssets.NonUniquePOI_Ids.Shuffle(new System.Random(seed)).ToList();
			return ItemList[0];
		}


		public static StarmapItem GetRandomItemOfType(StarmapItemCategory starmapItemCategory, int seed = -1)
		{
			List<StarmapItem> items = PlanetoidDict.Values.ToList().FindAll(item => item.category == starmapItemCategory);

			SgtLogger.l(seed + ", count: " + items.Count, "Getting Random item, Seed");

			if (seed != -1)
				items = items.Shuffle(new System.Random(seed)).ToList();
			else
				items.Shuffle();
			bool isClassic = false;
			StarmapItem item = null;
			int i;
			bool isOuter = starmapItemCategory == StarmapItemCategory.Outer;

			for (i = 0; i < items.Count; ++i)
			{
				isClassic = false;
				item = items[i];

				isClassic = PlanetIsClassic(item);


				if (isClassic)
				{
					if (CurrentClassicOuterPlanets >= MaxClassicOuterPlanets)
					{
						continue;
						//item.SetPlanetSizeToPreset(WorldSizePresets.Smaller); ///Reduce size to terrania level size for classic size planet
						//SgtLogger.l("Classic size limit reached, generating " + item.id + " at smaller size");
					}
				}

				//SgtLogger.l("CCCCC");

				//Debug.Log(CustomCluster.OuterPlanets);
				//SgtLogger.l("DDDD");
				//Debug.Log(RandomOuterPlanets);

				//Debug.Log(item);
				//Debug.Log(item.id);

				if (item.id == null || item.id == string.Empty || item.id.Contains("TemporalTear"))
					continue;

				if (!(item.id == null
					|| CustomCluster.OuterPlanets.ContainsKey(item.id)
					|| (isOuter && RandomOuterPlanets.Contains(item.id))
					|| item.id.Contains(RandomKey))
					)
				{
					break;
				}

				if (i == items.Count - 1 && isOuter)
				{
					item = null;
				}
			}
			//Debug.Log("out of loop");
			//Debug.Log(item);
			if (item != null && isOuter)
			{
				RandomOuterPlanets.Add(item.id);
				if (isClassic)
					CurrentClassicOuterPlanets++;
			}


			//while (item.category != starmapItemCategory || item.id.Contains("TemporalTear") || item.id == null || item.id == string.Empty)
			//{
			//    item = PlanetoidDict.Values.GetRandom();
			//}
			return item;
		}



		static Sprite GetRandomSprite(StarmapItemCategory category)
		{
			switch (category)
			{
				case StarmapItemCategory.Starter:
					return Assets.GetSprite(SpritePatch.randomStarter);
				case StarmapItemCategory.Warp:
					return Assets.GetSprite(SpritePatch.randomWarp);
				case StarmapItemCategory.Outer:
					return Assets.GetSprite(SpritePatch.randomOuter);
				case StarmapItemCategory.POI:
					return Assets.GetSprite(SpritePatch.randomPOI);
				default:
					return Assets.GetSprite("unknown");
			}
		}

		public static StarmapItemCategory DeterminePlanetType(ProcGen.World world, bool log = false)
		{

			StarmapItemCategory category = StarmapItemCategory.Outer;

			if (world.startingBaseTemplate != null)
			{
				string stripped = world.startingBaseTemplate.Replace("bases/", string.Empty);

				// SgtLogger.l(stripped.ToUpperInvariant(),"KEY");

				if (stripped.ToUpperInvariant().Contains("WARPWORLD")
					//|| KeyUpper.ToUpperInvariant().Contains("CGSM")&&
					//stripped.ToUpperInvariant().Contains("WARPBASE")
					)
				{
					category = StarmapItemCategory.Warp;
				}
				else if (stripped.ToUpperInvariant().Contains("BASE")
					|| stripped.ToUpperInvariant().Contains("ALLIN1") ///Vortex base name.
					//||KeyUpper.ToUpperInvariant().Contains("CGSM") && 
					//stripped.ToUpperInvariant().Contains("BASE")
					)
				{
					category = StarmapItemCategory.Starter;
				}
			}
			else
			{
				category = StarmapItemCategory.Outer;
			}
			if (log)
				SgtLogger.l(world.filePath + " is of category: " + category.ToString());
			return category;
		}

		public static List<StarmapItemCategory> AvailableStarmapItemCategories
		{
			get
			{
				var items = new List<StarmapItemCategory>();
				items.Add(StarmapItemCategory.Starter);
				if (DlcManager.IsExpansion1Active())
				{
					items.Add(StarmapItemCategory.Warp);
					items.Add(StarmapItemCategory.Outer);
					//items.Add(StarmapItemCategory.POI);
				}
				return items;
			}
		}
		public static List<StarmapItemCategory> AvailableStarmapItemCategoriesWithoutMixing
		{
			get
			{
				var items = new List<StarmapItemCategory>();
				items.Add(StarmapItemCategory.Starter);
				if (DlcManager.IsExpansion1Active())
				{
					items.Add(StarmapItemCategory.Warp);
					items.Add(StarmapItemCategory.Outer);
					//items.Add(StarmapItemCategory.POI);
				}
				return items;
			}
		}

		public static Dictionary<string, StarmapItem> PlanetoidDict
		{
			get
			{
				if (PlanetsAndPOIs == null)
				{
					PlanetsAndPOIs = new Dictionary<string, StarmapItem>();
					foreach (StarmapItemCategory category in AvailableStarmapItemCategoriesWithoutMixing)
					{
						if (category < 0)
							continue;


						var key = RandomKey + category.ToString();
						var randomItem = new StarmapItem
							(
							key,
							category,
							GetRandomSprite(category)
							);

						randomItem.SetSpawnNumber(1);

						var placement = new WorldPlacement();
						MinMaxI startCoords = new MinMaxI(0, CustomCluster.Rings);

						if (category == StarmapItemCategory.Starter)
							startCoords = new MinMaxI(0, 0);
						else if (category == StarmapItemCategory.Warp)
							startCoords = new MinMaxI(3, 5);



						placement.allowedRings = startCoords;
						placement.startWorld = category == StarmapItemCategory.Starter;
						placement.locationType = category == StarmapItemCategory.Starter ? LocationType.Startworld : LocationType.Cluster;

						randomItem = randomItem.AddItemWorldPlacement(placement, category == StarmapItemCategory.Outer);
						PlanetsAndPOIs[key] = randomItem;

						if (category == StarmapItemCategory.Outer)
							RandomOuterPlanetsStarmapItem = randomItem;
						//}
					}

					foreach (var WorldFromCache in SettingsCache.worlds.worldCache)
					{
						StarmapItemCategory category = StarmapItemCategory.Outer;
						//SgtLogger.l(World.Key + "; " + World.Value.ToString());
						ProcGen.World world = WorldFromCache.Value;

						//SgtLogger.l(world.skip.ToString(), "skip?1 "+WorldFromCache.Key);
						if ((int)world.skip >= 99)
							continue;

						//if (!WorldAllowedByDlcSelection(WorldFromCache.Key))
						//    continue;


						string KeyUpper = WorldFromCache.Key.ToUpperInvariant();
						bool SkipWorld = SkipWorldForDlcReasons(WorldFromCache.Key, WorldFromCache.Value);
						bool isMixingWorld = IsWorldMixingAsteroid(WorldFromCache.Key);

						if (!SkipWorld)
						{
							Sprite sprite = ColonyDestinationAsteroidBeltData.GetUISprite(WorldFromCache.Value.asteroidIcon);

							if (isMixingWorld)
							{
								category = StarmapItemCategory.None;
								PlanetsAndPOIs[WorldFromCache.Key] = new StarmapItem(
									WorldFromCache.Key,
									category,
									sprite).MakeItemPlanet(world);
								continue;
							}

							category = DeterminePlanetType(world);

							PlanetsAndPOIs[WorldFromCache.Key] = new StarmapItem
							(
							WorldFromCache.Key,
							category,
							sprite
							)
							.MakeItemPlanet(world)
							;

							if (PlanetIsMiniBase(PlanetsAndPOIs[WorldFromCache.Key]))
							{
								SgtLogger.l("making " + KeyUpper);
								SgtLogger.l(WorldFromCache.Key + " will disable story traits due to Baby size");
								PlanetsAndPOIs[WorldFromCache.Key].DisablesStoryTraits = true;
							}
							//SgtLogger.l("isClassic: " + PlanetIsClassic(PlanetsAndPOIs[WorldFromCache.Key]), WorldFromCache.Key);
						}
						else
							SgtLogger.l("skipping worlditemCreation: " + KeyUpper);
					}
				}
				return PlanetsAndPOIs;
			}
		}

		static int AdjustedClusterSize => CustomCluster.defaultRings + Mathf.Max(0, CustomCluster.AdjustedOuterExpansion / 4);
		public static void InitializeGeneration()
		{
			AddCustomClusterAndInitializeClusterGen();
			return;

			/// placement failures are detected by the cluster map automatically
			int randoPlanets = 0;
			if (CustomCluster.HasStarmapItem(RandomKey + StarmapItemCategory.Outer.ToString(), out var item))
			{
				randoPlanets = -1;
				randoPlanets += (int)item.InstancesToSpawn;
			}

			if (CustomCluster.OuterPlanets.Count + randoPlanets > 6 && CustomCluster.Rings < AdjustedClusterSize)
			{
				System.Action AjustSize = () =>
				{
					int ringAmount = AdjustedClusterSize;
					SgtLogger.l("Adjusted Ring Amount to " + ringAmount);
					CustomCluster.SetRings(ringAmount);
					InitializeGeneration();
				};
				System.Action nothing = () =>
				{
					//int ringAmount = AdjustedClusterSize;
					//CustomCluster.SetRings(ringAmount);
					//InitializeGeneration();
				};
				System.Action aaanyways = () =>
				{
					AddCustomClusterAndInitializeClusterGen();
				};


				DialogUtil.CreateConfirmDialogFrontend(
			   GENERATIONWARNING.WINDOWNAME,
			   GENERATIONWARNING.DESCRIPTION,
			   GENERATIONWARNING.YES,
			   AjustSize,
			   GENERATIONWARNING.NOMANUAL
			   , nothing
			   , DebugHandler.enabled ? UIUtils.ColorText("[Debug only] Try generating anyway", Color.red) : null
			   , DebugHandler.enabled ? aaanyways : null
			   );
			}
			else
			{
				AddCustomClusterAndInitializeClusterGen();
			}
		}

		public static bool LastGenFailed => lastWorldGenFailed;

		static bool lastWorldGenFailed = false;
		static string CurrentDifficultySettings;
		public static void LastWorldGenDidFail(bool fail = true)
		{
			lastWorldGenFailed = fail;

			///Store difficulty settings as these get overridden otherwise
			if (fail)
				CurrentDifficultySettings = CustomGameSettings.Instance.GetOtherSettingsCode();
			else if (!CurrentDifficultySettings.IsNullOrWhiteSpace())
			{
				CustomGameSettings.Instance.ParseAndApplySettingsCode(CurrentDifficultySettings);
				CurrentDifficultySettings = null;
			}
		}

		public static void OpenPresetWindow(System.Action onclose = null)
		{
			UnityPresetScreen.ShowWindow(CustomCluster, onclose);
		}

		internal static void ResetStarmap()
		{
			if (CustomCluster != null)
			{
				if (!DlcManager.IsExpansion1Active())
					CustomCluster.ResetVanillaStarmap();
			}
		}

		static HashSet<string> DevWorlds = new HashSet<string>()
		{
            //SO:
            "Moon_Barren",
			"SulfurMoonlet",
			"SpaceshipInterior",
			"TinyEmpty",
			"TinyIce",
			"TinyMagma",
			"TinyStart",
			"TwinMoonlet",
			"OilyMoonlet",
            //basegame:
            "TinySurface",
			"BigEmpty"
		};

		internal static bool SkipModdedWorld(string world, KMod.Mod mod)
		{
			if (mod == null)
				return false;

			string rewrittenPath = world;// SettingsCache.RewriteWorldgenPathYaml(world);

			string staticModID = mod.staticID;
			//RollerSnakes Check
			if (staticModID == "test447.RollerSnake")
			{
				bool isBaseGameWorld = rewrittenPath == "worlds/Tetrament";

				bool exclude = DlcManager.IsExpansion1Active() ? isBaseGameWorld : !isBaseGameWorld;
				return exclude;
			}
			//Empty Worlds, Fuleria, ILoveSlicksters Check
			if (staticModID == "EmptyWorlds" || staticModID == "AllBiomesWorld" || staticModID == "ILoveSlicksters")
			{
				SgtLogger.l("found " + staticModID + " world: " + rewrittenPath);
				bool isBaseGameWorld = !rewrittenPath.Contains("DLC");

				bool exclude = DlcManager.IsExpansion1Active() ? isBaseGameWorld : !isBaseGameWorld;
				return exclude;
			}
			return false;
		}

		internal static bool SkipWorldForDlcReasons(string world, ProcGen.World worldItem)
		{
			if ((int)worldItem.skip >= 99 || worldItem.moduleInterior)
				return true;
			string fileName = System.IO.Path.GetFileNameWithoutExtension(world);
			//hardcoded list due to skip being broken on current dev build:
			if (DevWorlds.Contains(fileName))
			{
				SgtLogger.l("skipping dev rewrittenPath manually: " + world);
				return true;
			}

			if (ModAssets.ModPlanetOriginPaths.TryGetValue(world, out var moddedPath))
			{
				world = moddedPath;
			}

			if (ModAssets.IsModdedAsteroid(world, out var mod) || worldItem.isModded)
			{
				//SgtLogger.l(world + " is modded");
				return SkipModdedWorld(world, mod);
			}

			SettingsCache.GetDlcIdAndPath(world, out var dlcId, out _);
			//basegame
			bool skip = false;
			if (dlcId == "")
			{
				SgtLogger.l(world + " is basegame content", "contentChecker");
				skip = DlcManager.IsExpansion1Active();
			}
			else if (dlcId == DlcManager.EXPANSION1_ID)
			{
				SgtLogger.l(world + " is SO content", "contentChecker");
				skip = !DlcManager.IsExpansion1Active();
			}
			else if (DlcManager.DLC_PACKS.TryGetValue(dlcId, out var dlcinfo))
			{
				SgtLogger.l(world + " is " + dlcinfo.id + " content", "contentChecker");
				skip = (DlcManager.IsExpansion1Active() ? world.ToUpperInvariant().Contains("BASEGAME") : !world.ToUpperInvariant().Contains("BASEGAME"));
			}
			if (!skip)
			{
				//skip = SkipForWorldMixingReasons(world);
				//if(skip)
				//    SgtLogger.l("skipping asteroid " + world + " for world mixing reasons");
			}
			else
			{
				SgtLogger.l("skipping asteroid " + world + " for dlc reasons");
			}


			return skip;
		}
		internal static bool IsWorldMixingAsteroid(string world)
		{
			if (CachedWorldMixingSettingsWorlds == null)
			{
				CachedWorldMixingSettingsWorlds = new();
				foreach (var worldMixing in SettingsCache.worldMixingSettings.Values)
				{
					CachedWorldMixingSettingsWorlds.Add(worldMixing.world);
				}
			}

			return CachedWorldMixingSettingsWorlds.Contains(world);
		}
		static HashSet<string> CachedWorldMixingSettingsWorlds = null;

		public static void RemoveFromCache()
		{
			if (SettingsCache.clusterLayouts.clusterCache.ContainsKey(CustomClusterID))
			{
				SettingsCache.clusterLayouts.clusterCache.Remove(CustomClusterID);
			}
		}

		static Dictionary<string, string> StarterPlanetsByCluster = null;

		internal static bool TryGetClusterForStarter(StarmapItem starterPlanet, out string clusterID)
		{
			if (StarterPlanetsByCluster == null)
			{
				StarterPlanetsByCluster = new Dictionary<string, string>();
				foreach (var ClusterFromCache in SettingsCache.clusterLayouts.clusterCache)
				{
					var starter = ClusterFromCache.Value.GetStartWorld();
					if (!StarterPlanetsByCluster.ContainsKey(starter))
					{
						StarterPlanetsByCluster.Add(starter, ClusterFromCache.Key);
					}
				}
			}
			SgtLogger.l("trying to fetch cluster for starter " + starterPlanet.id);
			return StarterPlanetsByCluster.TryGetValue(starterPlanet.id, out clusterID);
		}
	}
}
