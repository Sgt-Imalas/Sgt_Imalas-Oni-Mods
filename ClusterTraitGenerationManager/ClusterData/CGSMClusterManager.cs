using ClusterTraitGenerationManager.UI.Screens;
using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static ProcGen.WorldPlacement;

namespace ClusterTraitGenerationManager.ClusterData
{
    public class CGSMClusterManager
    {
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



        public const string CustomClusterIDCoordinate = "CGM";
        public const string CustomClusterID = "expansion1::clusters/CGMCluster";
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

            if (DlcManager.IsExpansion1Active())
                layout.requiredDlcId = DlcManager.EXPANSION1_ID;
            else
                layout.forbiddenDlcId = DlcManager.EXPANSION1_ID;

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
                SgtLogger.warning("No start planet selected");


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
                        SgtLogger.log("No warp planet selected");
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
                    OuterPlanets.OrderBy(item => item.placement.allowedRings.max)
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
                    //for (int i = 0; i < poi.Value.placementPOI.numToSpawn; i++)
                    //{
                    //    string randomId = GetRandomPOI(seed);
                    //    if (randomId.Length > 0)
                    //        poi.Value.placementPOI.pois.Add(randomId);
                    //    seed++;
                    //}
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
                    SgtLogger.l($"\navoidClumping: {poi.Value.placementPOI.avoidClumping},\nallowDuplicates: {poi.Value.placementPOI.canSpawnDuplicates},\nRings: {poi.Value.placementPOI.allowedRings}\nNumberToSpawn: {poi.Value.placementPOI.numToSpawn}", "POIGroup " + poi.Key.Substring(0, 8));

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

            if (log)
            {
                foreach (var item in CustomCluster.GetAllPlanets())
                {
                    SgtLogger.l(item.PredefinedPlacementOrder.ToString(), item.id);
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
            else
                SgtLogger.l("custom cluster data regenerated");

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
            using (var md5Hasher = MD5.Create())
            {
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
                return BitConverter.ToString(data).Replace("-", "");
            }
        }

        public static List<StarmapItem> GetPOIGroups(ClusterLayout cluster)
        {
            var values = new List<StarmapItem>();

            foreach (SpaceMapPOIPlacement pOIPlacement in cluster.poiPlacements)
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
            ClusterLayout Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);

            if (Reference == null || selectScreen == null || selectScreen.newGameSettingsPanel == null)
                return;
            string setting = selectScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.WorldgenSeed);

            if (setting == null || setting.Length == 0)
                return;

            int seed = int.Parse(setting);

            CurrentSeed = seed;
            if (singleItemId == string.Empty)
            {
                SgtLogger.l("Rebuilding Cluster Data");
                CustomCluster = new CustomClusterData();
                CustomCluster.SetRings(Reference.numRings - 1, true);
                ResetStarmap();
            }
            else
            {
                ///when planet not normally in cluster, but selected rn and to reset - reload from data
                if (!Reference.worldPlacements.Any(placement => placement.world == singleItemId))
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
                        FoundPlanet.ClearWorldTraits();
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
            for (int i = 0; i < Reference.worldPlacements.Count; i++)
            {
                WorldPlacement planetPlacement = Reference.worldPlacements[i];

                string planetpath = planetPlacement.world;

                if (PlanetoidDict.TryGetValue(planetpath, out StarmapItem FoundPlanet))
                {
                    if (singleItemId != string.Empty && FoundPlanet.id != singleItemId)
                    {
                        continue;
                    }
                    FoundPlanet.PredefinedPlacementOrder = i;

                    FoundPlanet.AddItemWorldPlacement(planetPlacement);
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
                    FoundPlanet.ClearWorldTraits();
                    //SgtLogger.l("Grabbing Traits");
                    foreach (var planetTrait in SettingsCache.GetRandomTraits(seed + i, FoundPlanet.world))
                    {
                        WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
                        FoundPlanet.AddWorldTrait(cachedWorldTrait);
                        //SgtLogger.l(planetTrait, FoundPlanet.DisplayName);
                    }
                    FoundPlanet.SetPlanetSizeToPreset(WorldSizePresets.Normal);
                    FoundPlanet.SetPlanetRatioToPreset(WorldRatioPresets.Normal);
                }
            }
            LastPresetGenerated = clusterID;
            RegeneratePOIDataFrom(clusterID, singleItemId);
        }
        public static void RegenerateAllPOIData() => RegeneratePOIDataFrom(LastPresetGenerated);

        public static void RegeneratePOIDataFrom(string clusterID, string singleItemId = "")
        {
            ClusterLayout Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);
            if (LastPresetGenerated == null || LastPresetGenerated.Length == 0)
                return;


            if (singleItemId == string.Empty)
            {
                CustomCluster.defaultOuterPlanets = Reference.worldPlacements.Count;
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
            else
                SgtLogger.l("poi placements were null");

            CustomCluster.SO_Starmap = null;

            if (CGM_Screen != null)
            {
                CGM_Screen.PresetApplied = false;
                if (!DlcManager.IsExpansion1Active())
                    CGM_Screen.RebuildVanillaStarmap(true);
            }
        }



        public static bool RerollVanillaStarmapWithSeedChange = true;
        public static bool RerollTraitsWithSeedChange = true;
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

                    MinMaxI startCoords = new MinMaxI(0, CustomCluster.Rings);
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

        public static void TogglePlanetoid(StarmapItem ToAdd)
        {
            var item = GivePrefilledItem(ToAdd); ///Prefilled
                                                 ///only one starter at a time
                                                 ///
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
            //foreach (var planet in CustomCluster.POIs)
            //{
            //    planets.Add(planet.Value);
            //}
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
            //foreach (var planet in CustomCluster.POIs)
            //{
            //    planetPaths.Add(planet.Key);
            //}
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
                    if (ClusterLayout.Key.Contains("clusters/SandstoneDefault") || ClusterLayout.Value.forbiddenDlcId == DlcManager.EXPANSION1_ID)
                    {
                        continue;
                    }
                }

                if (ClusterLayout.Value.disableStoryTraits)
                {
                    foreach (var planet in ClusterLayout.Value.worldPlacements)
                    {
                        if (PlanetsAndPOIs.ContainsKey(planet.world))
                        {
                            SgtLogger.l(planet.world + " will disable story traits");
                            PlanetsAndPOIs[planet.world].DisablesStoryTraits = true;
                        }
                        else
                            SgtLogger.warning("Tried to add disabling story traits to a planet that does not exist: " + planet.world);
                    }
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

            SgtLogger.l(seed + "", "Getting Random item, Seed");

            if (seed != -1)
                items = items.Shuffle(new System.Random(seed)).ToList();
            else
                items.Shuffle();

            bool isClassic = false;
            StarmapItem item = null;
            int i;
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
                        item.SetPlanetSizeToPreset(WorldSizePresets.Smaller); ///Reduce size to terrania level size for classic size planet
                        SgtLogger.l("Classic size limit reached, generating " + item.id + " at smaller size");
                    }
                }

                if (!(item.id.Contains("TemporalTear")
                    || item.id == null
                    || item.id == string.Empty
                    || CustomCluster.OuterPlanets.ContainsKey(item.id)
                    || RandomOuterPlanets.Contains(item.id)
                    || item.id.Contains(RandomKey))
                    )
                {
                    break;
                }

                if (i == items.Count - 1 && starmapItemCategory == StarmapItemCategory.Outer)
                {
                    item = null;
                }
            }

            if (item != null && starmapItemCategory == StarmapItemCategory.Outer)
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

        public static Dictionary<string, StarmapItem> PlanetoidDict
        {
            get
            {
                if (PlanetsAndPOIs == null)
                {
                    PlanetsAndPOIs = new Dictionary<string, StarmapItem>();

                    foreach (StarmapItemCategory category in AvailableStarmapItemCategories)
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

                        //if (category == StarmapItemCategory.POI)
                        //{
                        //    var placement = new SpaceMapPOIPlacement();
                        //    placement.allowedRings = new MinMaxI(0, CustomCluster.Rings);
                        //    placement.canSpawnDuplicates = true;
                        //    placement.numToSpawn = 1;
                        //    placement.avoidClumping = false;
                        //    randomItem = randomItem.MakeItemPOI(key, placement, MaxAmountRandomPOI, SPACEDESTINATIONS.CGM_RANDOM_POI.NAME, SPACEDESTINATIONS.CGM_RANDOM_POI.DESCRIPTION);
                        //    PlanetsAndPOIs[key] = randomItem;
                        //    RandomPOIStarmapItem = randomItem;
                        //}
                        //else
                        //{
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

                        //SgtLogger.l(world.skip.ToString(), "skip?");
                        if ((int)world.skip >= 99
                            //&& !DebugHandler.enabled
                            )
                            continue;

                        ///Hardcoded checks due to other mods not having the correct folder structure
                        string KeyUpper = WorldFromCache.Key.ToUpperInvariant();
                        bool SkipWorld =
                               KeyUpper.Contains("EMPTERA") && DlcManager.IsExpansion1Active() ? !KeyUpper.Contains("DLC") : KeyUpper.Contains("DLC")
                            || KeyUpper.Contains("ISLANDS") && DlcManager.IsExpansion1Active() ? !KeyUpper.Contains("DLC") : KeyUpper.Contains("DLC")
                            || KeyUpper.Contains("FULERIA") && DlcManager.IsExpansion1Active() ? !KeyUpper.Contains("DLC") : KeyUpper.Contains("DLC")
                            || DlcManager.IsExpansion1Active() && KeyUpper.Contains("WORLDS/SANDSTONEDEFAULT");





                        if (!SkipWorld)
                        {
                            category = DeterminePlanetType(world);
                            Sprite sprite = ColonyDestinationAsteroidBeltData.GetUISprite(WorldFromCache.Value.asteroidIcon);

                            PlanetsAndPOIs[WorldFromCache.Key] = new StarmapItem
                            (
                            WorldFromCache.Key,
                            category,
                            sprite
                            ).MakeItemPlanet(world);

                            if (PlanetIsMiniBase(PlanetsAndPOIs[WorldFromCache.Key]))
                            {
                                SgtLogger.l(WorldFromCache.Key + " will disable story traits due to Baby size");
                                PlanetsAndPOIs[WorldFromCache.Key].DisablesStoryTraits = true;
                            }
                            SgtLogger.l("isClassic: " + PlanetIsClassic(PlanetsAndPOIs[WorldFromCache.Key]), WorldFromCache.Key);
                        }

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
    }
}
