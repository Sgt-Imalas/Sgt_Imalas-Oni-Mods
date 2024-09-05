using ClusterTraitGenerationManager.UI.Screens;
using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static ProcGen.ClusterLayout;
using static ProcGen.WorldPlacement;
using static STRINGS.UI.SPACEDESTINATIONS;

namespace ClusterTraitGenerationManager.ClusterData
{
    public class CGSMClusterManager
    {

        public const string CustomClusterIDCoordinate = "CGM";
        public const string CustomClusterID = "expansion1::clusters/CGMCluster";
        public const string CustomClusterClusterTag = "CGM_CustomCluster";


        public const float MaxAmountPOI = 5f;
        public const float MaxAmountRandomPOI = 32f; //TODO: dynamic scale with map size
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

        public static int MaxClassicOuterPlanets = 3, CurrentClassicOuterPlanets = 0;

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
        public static bool RandomGeyserInBlacklist(string traitId) => BlacklistedGeysers.Contains(traitId);
        public static bool ToggleRandomGeyserBlacklist(string traitID)
        {
            if (BlacklistedGeysers.Contains(traitID))
            {
                BlacklistedGeysers.Remove(traitID);
                return false;
            }
            else
            {
                BlacklistedGeysers.Add(traitID);
                return true;
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

        public static async void InstantiateClusterSelectionView(ColonyDestinationSelectScreen parent, System.Action onClose = null)
        {
            if (Screen == null)
            {
                ///Change to check for moonlet/classic start
                if (CustomCluster == null)
                {
                    string defaultCluster;
                    if (DlcManager.IsExpansion1Active())
                        defaultCluster = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                    else
                        defaultCluster = "SandstoneDefault";

                    CreateCustomClusterFrom(defaultCluster);
                }

                Screen = Util.KInstantiateUI(ModAssets.CGM_MainMenu, FrontEndManager.Instance.gameObject, true);
                selectScreen = parent;

                Screen.name = "ClusterSelectionView";
                CGM_Screen = Screen.AddComponent<CGM_MainScreen_UnityScreen>();
            }
            if (CustomCluster == null)
            {
                string defaultCluster;

                if (DlcManager.IsExpansion1Active())
                    defaultCluster = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                else
                    defaultCluster = "SandstoneDefault";

                CreateCustomClusterFrom(defaultCluster);
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

        //static void ApplySizeMultiplier(WorldPlacement placement, float multiplier)
        //{
        //    float min = placement.allowedRings.min, max = placement.allowedRings.max;
        //    //min*= multiplier;

        //    //if (max < 3)
        //        //max = 3;

        //    max *= multiplier;
        //    max = Math.Min(max, CustomCluster.Rings);

        //    int max2 = Math.Min(placement.allowedRings.max + CustomCluster.AdjustedOuterExpansion, CustomCluster.Rings);
        //    int newMax = Math.Max((int)Math.Round(max), max2);
        //    placement.allowedRings = new MinMaxI((int)min, newMax);

        //    SgtLogger.l("Set inner and outer limits to " + placement.allowedRings.ToString(), placement.world);
        //}

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

        public static ClusterLayout GenerateDummyCluster(bool spacedOut, bool ceres)
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
                clusterLayout.clusterAudio = new ClusterAudioSettings()
                {
                    musicWelcome = "Music_WattsonMessage_DLC2",
                    musicFirst = "Ice_Planet",
                    stingerDay = "Stinger_Day_DLC2",
                    stingerNight = "Stinger_Loop_Night_DLC2"
                };
            }
            return clusterLayout;
        }

        public static ClusterLayout GenerateClusterLayoutFromCustomData(bool log)
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

            layout.clusterTags = new();


            if (DlcManager.IsExpansion1Active())
                layout.requiredDlcIds = new string[] { DlcManager.EXPANSION1_ID };
            else
                layout.forbiddenDlcIds = new string[] { DlcManager.EXPANSION1_ID };

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
                SgtLogger.warning("No start planetData selected");

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
                        if (log)
                            SgtLogger.l(poi.Value.id + ", succeeded: " + rolledChance * 100f, "POI Chance: " + percentageAdditional.ToString("P"));
                        poi.Value.placementPOI.numToSpawn += 1;
                    }
                    else
                    {
                        if (log)
                            SgtLogger.l(poi.Value.id + ", failed: " + rolledChance * 100f, "POI Chance: " + percentageAdditional.ToString("P"));
                    }
                    seed++;
                }
                if (radomns)
                {
                    poi.Value.placementPOI.canSpawnDuplicates = true;
                }
                if (log)
                    poi.Value.placementPOI.pois.ForEach(poi => SgtLogger.l(poi, "poi in group"));

                if (poi.Value.placementPOI.pois.Count == 0)
                {
                    continue;
                }


                if (poi.Value.placementPOI.pois.Count < poi.Value.placementPOI.numToSpawn)
                    poi.Value.placementPOI.canSpawnDuplicates = true;



                if (log)
                    SgtLogger.l($"\navoidClumping: {poi.Value.placementPOI.avoidClumping},\nguarantee: {poi.Value.placementPOI.guarantee},\nallowDuplicates: {poi.Value.placementPOI.canSpawnDuplicates},\nRings: {poi.Value.placementPOI.allowedRings}\nNumberToSpawn: {poi.Value.placementPOI.numToSpawn}", "POIGroup " + poi.Key.Substring(0, 8));

                if (poi.Value.placementPOI.numToSpawn >= 1)
                {
                    layout.poiPlacements.Add(poi.Value.placementPOI);
                }
            }

            if (log)
                SgtLogger.l("POI Placements done");
            layout.numRings = CustomCluster.Rings + 1;

            if (log)
                SgtLogger.l("Ordering Asteroids");

            List<StarmapItem> allPlanets = CustomCluster.GetAllPlanets();
            if (log)
            {
                foreach (var item in allPlanets)
                {
                    SgtLogger.l(item.PredefinedPlacementOrder.ToString(), item.id);
                }
            }
            foreach (var item in allPlanets)
            {
                var world = item.world;
                if (item.DlcID == DlcManager.DLC2_ID || world != null && world.worldTags.Contains("Ceres"))
                {
                    layout.clusterTags.Add("CeresCluster");
                    layout.clusterTags.Add("GeothermalImperative");
                    layout.clusterAudio = new ClusterAudioSettings()
                    {
                        musicWelcome = "Music_WattsonMessage_DLC2",
                        musicFirst = "Ice_Planet",
                        stingerDay = "Stinger_Day_DLC2",
                        stingerNight = "Stinger_Loop_Night_DLC2"
                    };
                }
            }

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
        public static int CurrentSeed = -1;
        public static int GlobalWorldSeed => CurrentSeed - 1 + CustomCluster.GetAllPlanets().Count;


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
                placements.AddRange(dlcMixingSettings.spacePois);
                SgtLogger.l(dlcmixing + " is enabled, adding space pois");
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
            if (singleItemId != string.Empty)
            {
                SgtLogger.l("Regenerating stats for " + singleItemId + " in " + clusterID);
            }
            else
                SgtLogger.l("Generating custom cluster from " + clusterID);

            if (lastWorldGenFailed && !ForceRegen)
                return;
            ClusterLayout ReferenceLayout = SettingsCache.clusterLayouts.GetClusterData(clusterID);

            if (ReferenceLayout == null || selectScreen == null || selectScreen.newGameSettingsPanel == null)
                return;
            string setting = selectScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.WorldgenSeed);

            if (setting == null || setting.Length == 0)
                return;

            int seed = int.Parse(setting);
            var mutated = new MutatedClusterLayout(ReferenceLayout);
            WorldgenMixing.RefreshWorldMixing(mutated, seed, true, true);
            CurrentSeed = seed;
            if (singleItemId == string.Empty)
            {
                SgtLogger.l("Rebuilding Cluster Data");
                CustomCluster = new CustomClusterData();
                CustomCluster.SetRings(mutated.layout.numRings - 1, true);
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
                    SgtLogger.l(FoundPlanet.category.ToString());
                    if (singleItemId != string.Empty && FoundPlanet.id != singleItemId)
                    {
                        continue;
                    }
                    FoundPlanet.PredefinedPlacementOrder = i;

                    FoundPlanet.AddItemWorldPlacement(planetPlacement);

                    if (isWorldMixed && PlanetoidDict.TryGetValue(mixingAsteroid, out StarmapItem _mixingItem))
                    {
                        SetMixingWorld(FoundPlanet, _mixingItem);
                        FoundPlanet.SetWorldMixing(_mixingItem);
                    }
                    else
                        SetMixingWorld(FoundPlanet, null);

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
                    int seedTrait = FoundPlanet.IsMixed ? seed - 1 : seed + i; //mixing target is not in cluster -> position will be -1 in original code (potentially adjust in the future)
                    var traits = SettingsCache.GetRandomTraits(seedTrait, FoundPlanet.world);
                    foreach (var planetTrait in traits)
                    {
                        WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
                        FoundPlanet.AddWorldTrait(cachedWorldTrait);
                        SgtLogger.l(planetTrait, FoundPlanet.DisplayName);
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
            target.SetWorldMixing(mixingSource);
            if (mixingSource != null)
            {
                if (CustomCluster.MixingWorldsWithTarget.TryGetValue(mixingSource, out var OldTarget))
                {
                    OldTarget.SetWorldMixing(null);
                }
                CustomCluster.MixingWorldsWithTarget[mixingSource] = target;
            }
        }

        public static void RegenerateAllPOIData() => RegeneratePOIDataFrom(LastPresetGenerated);

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
                    if (resettingSingle && CustomCluster.HasStarmapItem(singleItemId, out var item))
                    {
                        CGM_Screen.SelectItem(item);
                    }
                    else
                    {
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
        public static void RerollMixings()
        {
            if (CustomCluster == null || !RerollMixingsWithSeedChange && CGM_Screen.IsCurrentlyActive)
                return;

            foreach(var planet in CustomCluster.GetAllPlanets())
            {
                if(planet== null||planet.world==null||planet.placement == null ) continue;
                planet.SetWorldMixing(null);
                planet.placement.UndoWorldMixing();
            }

            int seed = int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id);

            var layout = GeneratedLayout;

            var mutated = WorldgenMixing.DoWorldMixing(layout, seed, true, true);

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
                    SgtLogger.l(planetpath, "MIXING");
                }
                if (CustomCluster.HasStarmapItem(planetpath, out var FoundPlanet))
                {
                    SgtLogger.l(FoundPlanet.category.ToString());
                    if (isWorldMixed && PlanetoidDict.TryGetValue(mixingAsteroid, out StarmapItem _mixingItem))
                    {
                        SetMixingWorld(FoundPlanet, _mixingItem);
                        FoundPlanet.SetWorldMixing(_mixingItem);
                    }
                    else
                        SetMixingWorld(FoundPlanet, null);
                }
            }
        }
        public static void RerollTraits()
        {
            if (CustomCluster == null || !RerollTraitsWithSeedChange && CGM_Screen.IsCurrentlyActive)
                return;


            int seed = int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id);

            CurrentSeed = seed;

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
            PopulatePredefinedClusterPlacements();

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
        private static void SetDLCMixingSettings(SettingConfig ConfigToSet, object valueId)
        {
            string valueToSet = valueId.ToString();
            if (valueId is bool val)
            {
                var toggle = ConfigToSet as ToggleSettingConfig;
                valueToSet = val ? toggle.on_level.id : toggle.off_level.id;
            }
            //SgtLogger.l("changing " + ConfigToSet.id.ToString() + " from " + CustomGameSettings.Instance.GetCurrentMixingSettingLevel(ConfigToSet).id + " to " + valueToSet.ToString());
            CustomGameSettings.Instance.SetMixingSetting(ConfigToSet, valueToSet);
        }
        public static void ToggleDlc2(bool enabled)
        {
            SetDLCMixingSettings(CustomMixingSettingsConfigs.DLC2Mixing, enabled);
            RegenerateAllPOIData();
            CGM_Screen?.RebuildStarmap(true);
        }

        public static void TogglePlanetoid(StarmapItem ToAdd)
        {
            var item = GivePrefilledItem(ToAdd); ///Prefilled
                                                 ///only one starter at a time
                                                 ///
            if (item.DlcID == DlcManager.DLC2_ID)
            {
                if (!CustomCluster.HasStarmapItem(ToAdd.id, out _)) //enable dlc2 for when a ceres asteroid was added
                {
                    ToggleDlc2(true);
                }
            }

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


        static Dictionary<string, WorldPlacement> PredefinedPlacementData = null;
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
            if (PredefinedPlacementData != null) { return; }

            SgtLogger.l("Populating cluster placements");
            PredefinedPlacementData = new Dictionary<string, WorldPlacement>();
            //PredefinedPlacementDataPOI = new Dictionary<string, SpaceMapPOIPlacement>();

            foreach (var ClusterLayout in SettingsCache.clusterLayouts.clusterCache.ToList())
            {
                if (DlcManager.IsExpansion1Active())
                {

                    if (ClusterLayout.Key.Contains("clusters/SandstoneDefault") // Uncomment when klei removes the default cluster
                        || ClusterLayout.Value.forbiddenDlcIds != null && ClusterLayout.Value.forbiddenDlcIds.Contains(DlcManager.EXPANSION1_ID))
                    {
                        continue;
                    }
                }
                bool disablesStoryTraits = ClusterLayout.Value.disableStoryTraits;
                var tags = ClusterLayout.Value.clusterTags;

                foreach (var planet in ClusterLayout.Value.worldPlacements)
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

                if (ClusterLayout.Value.poiPlacements == null)
                    continue;
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

        public static StarmapItemCategory DeterminePlanetType(ProcGen.World world)
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

                    PopulatePredefinedClusterPlacements();
                }
                return PlanetsAndPOIs;
            }
        }

        static int AdjustedClusterSize => CustomCluster.defaultRings + Mathf.Max(0, CustomCluster.AdjustedOuterExpansion / 4);
        public static void InitializeGeneration()
        {

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


                KMod.Manager.Dialog(Global.Instance.globalCanvas,
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
        public static void LastWorldGenDidFail(bool fail = true)
        {
            lastWorldGenFailed = fail;
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

            string sourcePath = SettingsCache.RewriteWorldgenPathYaml(world);
            if (ModAssets.ModPlanetOriginPaths.TryGetValue(world, out var moddedPath))
            {
                world = moddedPath;
                sourcePath = SettingsCache.RewriteWorldgenPathYaml(moddedPath);
            }

            if (ModAssets.IsModdedAsteroid(sourcePath, out var mod) || worldItem.isModded)
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
            else if (dlcId == DlcManager.DLC2_ID)
            {
                SgtLogger.l(world + " is FP content", "contentChecker");
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
