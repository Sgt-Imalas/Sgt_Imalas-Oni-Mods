using Database;
using FMOD;
using HarmonyLib;
using Klei.AI;
using Klei.CustomSettings;
using Newtonsoft.Json;
using ProcGen;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UtilLibs;
using static ClusterTraitGenerationManager.Patches;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static Klei.ClusterLayoutSave;
using static ProcGen.WorldPlacement;

namespace ClusterTraitGenerationManager
{
    public class CGSMClusterManager
    {
        public static GameObject Screen = null;

        public static ColonyDestinationSelectScreen selectScreen;

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
                ///Change to check for moonlet/vanilla start
                if (CustomCluster == null)
                {
                    var defaultCluster = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                    CGSMClusterManager.CreateCustomClusterFrom(defaultCluster);
                }

                Screen = Util.KInstantiateUI(ModAssets.CGM_MainMenu, FrontEndManager.Instance.gameObject, true);
                selectScreen = parent;

                UIUtils.ListAllChildren(Screen.transform);
                UIUtils.ListAllChildrenPath(Screen.transform);
                Screen.name = "ClusterSelectionView";
                CGM_Screen = Screen.AddComponent<CGM_MainScreen_UnityScreen>();
            }
            if (CustomCluster == null)
            {
                var defaultCluster = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                CGSMClusterManager.CreateCustomClusterFrom(defaultCluster);
            }
            LoadCustomCluster = false;

            Screen.transform.SetAsLastSibling();
            Screen.gameObject.SetActive(true);
            CGM_Screen = Screen.GetComponent<CGM_MainScreen_UnityScreen>();
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
            Starter,
            Warp,
            Outer,
            POI
        }

        public const int ringMax = 25, ringMin = 6;
        public class CustomClusterData
        {
            int GetAdjustedOuterExpansion()
            {
                int planetDiff = (CustomCluster.OuterPlanets.Count - CustomCluster.defaultOuterPlanets);


                return planetDiff;
            }
            [JsonIgnore] public int AdjustedOuterExpansion => GetAdjustedOuterExpansion();

            public int defaultRings = 12;
            public int defaultOuterPlanets = 6;
            public int Rings { get; private set; }
            public StarmapItem StarterPlanet { get; set; }
            public StarmapItem WarpPlanet { get; set; }
            public Dictionary<string, StarmapItem> OuterPlanets = new Dictionary<string, StarmapItem>();
            public Dictionary<string, StarmapItem> POIs = new Dictionary<string, StarmapItem>();

            public bool HasStarmapItem(string id, out StarmapItem item1)
            {
                if (id == null || id.Length == 0)
                {
                    item1 = null;
                    return false;
                }

                if (StarterPlanet != null && StarterPlanet.id == id)
                {
                    item1 = StarterPlanet;
                    return true;
                }
                else if (WarpPlanet != null && WarpPlanet.id == id)
                {
                    item1 = WarpPlanet;
                    return true;
                }
                else if (OuterPlanets.ContainsKey(id))
                {
                    item1 = OuterPlanets[id];
                    return true;
                }
                else if (POIs.ContainsKey(id))
                {
                    item1 = POIs[id];
                    return true;
                }

                if (PlanetoidDict().TryGetValue(id, out item1))
                {
                    return false;
                }
                return false;
            }

            public List<StarmapItem> GetAllPlanets()
            {
                var list = new List<StarmapItem>();
                if (StarterPlanet != null)
                    list.Add(StarterPlanet);
                if (WarpPlanet != null) list.Add(WarpPlanet);
                list.AddRange(OuterPlanets.Values);
                return list;
            }

            public List<string> GiveWorldTraitsForWorldGen(ProcGen.World world)
            {

                List<string> list = new List<string>();

                if (HasStarmapItem(world.filePath, out var starmapItem))
                {
                    list = starmapItem.GetWorldTraits();
                }
                ///RNG planet
                else
                {
                    int count = 0;
                    int random = UnityEngine.Random.Range(1, 101);
                    if (random >= 85)
                        count = 2;
                    else if (random >= 45)
                        count = 1;
                    List<string> randomSelectedTraits = new List<string>();
                    return AddRandomTraitsForWorld(randomSelectedTraits, world, count);
                }

                if (list.Any(id => id.Contains("CGMRandomTraits")))
                {
                    int count = 1;

                    int random = UnityEngine.Random.Range(1, 101);

                    if (random >= 85)
                        count = 3;
                    else if (random >= 50)
                        count = 2;

                    List<string> randomSelectedTraits = new List<string>();
                    return AddRandomTraitsForWorld(randomSelectedTraits, world, count);
                }


                return list;
            }
            public static List<string> AddRandomTraitsForWorld(List<string> existing, ProcGen.World world, int count)
            {
                for (int i = 0; i < count; ++i)
                {
                    var possibleTraits = StarmapItem.AllowedWorldTraitsFor(existing, world);
                    if (possibleTraits.Count == 0)
                        break;
                    else
                    {
                        possibleTraits.Shuffle();
                        string randTrait = possibleTraits.First().filePath.Contains("CGMRandomTraits") ? possibleTraits.Last().filePath : possibleTraits.First().filePath;
                        existing.Add(randTrait);
                    }
                }
                return existing;
            }

            public void SetRings(int rings, bool defaultRing = false)
            {
                rings = Math.Min(rings, ringMax);
                rings = Math.Max(rings, ringMin);

                Rings = rings;
                if (defaultRing)
                    defaultRings = rings;

                if (StarterPlanet != null && StarterPlanet.placement != null)
                {
                    if (StarterPlanet.placement.allowedRings.max >= rings)
                    {
                        StarterPlanet.SetOuterRing(rings);
                    }
                    if (StarterPlanet.placement.allowedRings.min >= rings)
                    {
                        StarterPlanet.SetInnerRing(rings);
                    }
                }
                if (WarpPlanet != null && WarpPlanet.placement != null)
                {
                    if (WarpPlanet.placement.allowedRings.max >= rings)
                    {
                        WarpPlanet.SetOuterRing(rings);
                    }
                    if (WarpPlanet.placement.allowedRings.min >= rings)
                    {
                        WarpPlanet.SetInnerRing(rings);
                    }
                }

                foreach (var planet in OuterPlanets.Values)
                {
                    if (planet.placement != null)
                    {
                        if (planet.placement.allowedRings.max >= rings)
                        {
                            planet.SetOuterRing(rings);
                        }
                        if (planet.placement.allowedRings.min >= rings)
                        {
                            planet.SetInnerRing(rings);
                        }
                    }
                }
                foreach (var planet in POIs.Values)
                {
                    if (planet.placementPOI != null)
                    {
                        if (planet.placementPOI.allowedRings.max >= rings)
                        {
                            planet.SetOuterRing(rings);
                        }
                        if (planet.placementPOI.allowedRings.min >= rings)
                        {
                            planet.SetInnerRing(rings);
                        }
                    }
                }
            }
        }

        public enum WorldSizePresets
        {
           // Tiny = 25,
           // Smaller = 40,
           // Small = 55,
            //SlightlySmaller = 75,

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


        public class StarmapItem
        {
            public string id;
            public StarmapItemCategory category;
            public bool DisablesStoryTraits = false;

            [JsonIgnore] public Sprite planetSprite;

            [JsonIgnore] public ProcGen.World world;

            public WorldPlacement placement;

            public SpaceMapPOIPlacement placementPOI;

            public bool IsPOI => category == StarmapItemCategory.POI;
            public bool IsRandom => id.Contains(RandomKey);


            public string DisplayName
            {
                get
                {
                    if (id.Contains(RandomKey))
                    {
                        switch (category)
                        {
                            case StarmapItemCategory.Starter:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_STARTER.NAME;
                            case StarmapItemCategory.Warp:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_WARP.NAME;
                            case StarmapItemCategory.Outer:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_OUTER.NAME;
                            case StarmapItemCategory.POI:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.NAME;
                        }
                    }

                    if (world != null)
                    {
                        Strings.TryGet(world.name, out var name);
                        return name;
                    }
                    else if (_poiID != null)
                    {
                        return _poiName;
                    }
                    return id;
                }
            }
            public string DisplayDescription
            {
                get
                {

                    if (id.Contains(RandomKey))
                    {
                        switch (category)
                        {
                            case StarmapItemCategory.Starter:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_STARTER.DESCRIPTION;
                            case StarmapItemCategory.Warp:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_WARP.DESCRIPTION;
                            case StarmapItemCategory.Outer:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_OUTER.DESCRIPTION;
                            case StarmapItemCategory.POI:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.DESCRIPTION;
                        }
                    }

                    if (world != null)
                    {
                        Strings.TryGet(world.description, out var description);
                        return description.String;
                    }
                    else if (_poiID != null)
                    {
                        return _poiDesc;
                    }
                    return id;
                }
            }
            public string _poiID { get; private set; }

            public string _poiName { get; private set; }
            public string _poiDesc { get; private set; }

            //private float XYratio = -1f;
            public float ApplySizeMultiplierToValue(float inputValue)
            {
                float sizePercentage = (float)SizePreset / 100f;
                return Mathf.RoundToInt(sizePercentage * inputValue);
            }

            [JsonIgnore] public Vector2I CustomPlanetDimensions
            {
                get
                {
                    if (UsingCustomDimensions)
                    {
                        return new(CustomX, CustomY);
                    }
                    var dim = new Vector2I(0, 0);

                    if (world != null)
                    {
                        float sizePercentage = (float)SizePreset / 100f;
                        float ratioModifier = 0;

                        bool? ChooseHeight = null;

                        int intRatio = (int)RatioPreset;
                        if (intRatio != 100)
                        {
                            if (intRatio < 100)
                            {
                                ratioModifier = 1f - ((float)RatioPreset / 100f);
                                ChooseHeight = false;
                            }
                            else if (intRatio > 100)
                            {
                                ratioModifier = ((float)RatioPreset / 100f) - 1;
                                ChooseHeight = true;
                            }
                        }
                        float sizeIncreaseMultiplier = Mathf.Sqrt(sizePercentage);

                        dim.X = Mathf.RoundToInt(world.worldsize.X * sizeIncreaseMultiplier);
                        dim.Y = Mathf.RoundToInt(world.worldsize.Y * sizeIncreaseMultiplier);

                        if (ChooseHeight == true)
                        {
                            dim.Y = dim.Y + Mathf.RoundToInt(dim.Y * ratioModifier);
                        }
                        if (ChooseHeight == false)
                        {
                            dim.X = dim.X + Mathf.RoundToInt(dim.X * ratioModifier);
                        }
                    }
                    return dim;
                }
            }

            //int CustomWorldSizeX = -1, CustomWorldSizeY = -1;
            WorldSizePresets SizePreset = WorldSizePresets.Normal;
            WorldRatioPresets RatioPreset = WorldRatioPresets.Normal;

            [JsonIgnore] public WorldSizePresets CurrentSizePreset => SizePreset;
            [JsonIgnore] public WorldRatioPresets CurrentRatioPreset => RatioPreset;

            [JsonIgnore] public float CurrentSizeMultiplier => UsingCustomDimensions ? CustomSizeIncrease : (float)SizePreset / 100f;

            public void SetPlanetSizeToPreset(WorldSizePresets preset)
            {
                CustomX = -1;
                CustomY = -1;
                UsingCustomDimensions = false;
                SizePreset = preset;
            }
            public void SetPlanetRatioToPreset(WorldRatioPresets preset)
            {
                CustomX = -1;
                CustomY = -1;
                UsingCustomDimensions = false;
                RatioPreset = preset;
            }
            public int CustomX = -1, CustomY = -1;
            private bool UsingCustomDimensions = false;
            public void ApplyCustomDimension(int value, bool heightTrueWidthFalse)
            {

                var currentDims = CustomPlanetDimensions;
                UsingCustomDimensions = true;
                if (CustomX == -1)
                {
                    CustomX = currentDims.X;
                }
                if (CustomY == -1)
                {
                    CustomY = currentDims.Y;
                }

                if (world != null)
                {
                    if (heightTrueWidthFalse)
                    {
                        var rounded = Mathf.RoundToInt(Mathf.Max(Mathf.Min(value, world.worldsize.Y * 2.6f), world.worldsize.Y));
                        if (rounded != CustomY)
                            CustomY = rounded;
                    }
                    else
                    {
                        var rounded = Mathf.RoundToInt(Mathf.Max(Mathf.Min(value, world.worldsize.X * 2.6f), world.worldsize.X));

                        if (rounded != CustomX)
                            CustomX = rounded;
                    }
                    CustomSizeIncrease = (float)(CustomX * CustomY) / (float)(world.worldsize.X * world.worldsize.Y);
                }
            }
            float CustomSizeIncrease = -1f;

            public float InstancesToSpawn = 1;
            public float MaxNumberOfInstances = 1;
            [JsonIgnore] public int minRing => placement != null ? placement.allowedRings.min : placementPOI != null ? placementPOI.allowedRings.min : -1;
            [JsonIgnore] public int maxRing => placement != null ? placement.allowedRings.max : placementPOI != null ? placementPOI.allowedRings.max : -1;
            [JsonIgnore] public int buffer => placement != null ? placement.buffer : -1;


            #region SetterMethods
            public void SetSpawnNumber(float newNumber)
            {
                if (newNumber <= MaxNumberOfInstances)
                {
                    InstancesToSpawn = newNumber;
                }
            }
            public void IncreaseSpawnNumber(float newNumber)
            {
                InstancesToSpawn += newNumber;
            }
            public void SubtractOneSpawn()
            {
                InstancesToSpawn -= 1;
            }

            public void ResetPOI()
            {
                if (this.category == StarmapItemCategory.POI)
                {
                    if (originalMaxPOI != null)
                    {
                        SetOuterRing((int)originalMaxPOI);
                    }
                    if (originalMinPOI != null)
                    {
                        SetInnerRing((int)originalMinPOI);
                    }
                }
            }

            [JsonIgnore] public int? originalMinPOI = null, originalMaxPOI = null;
            public void SetInnerRing(int newRing, bool original = false)
            {
                if (original)
                    originalMinPOI = newRing;

                newRing = Math.Min(newRing, CustomCluster.Rings);
                if (newRing >= 0)
                {
                    if (placement != null)
                    {
                        var rings = placement.allowedRings;
                        rings.min = newRing;
                        if (newRing > rings.max)
                        {
                            rings.max = newRing;
                        }
                        placement.allowedRings = rings;
                    }
                    else if (placementPOI != null)
                    {
                        var rings = placementPOI.allowedRings;
                        rings.min = newRing;
                        if (newRing > rings.max)
                        {
                            rings.max = newRing;
                        }
                        placementPOI.allowedRings = rings;
                    }
                    else
                        SgtLogger.warning(this.id + ": no placement component found!");
                }
            }
            public void SetOuterRing(int newRing, bool original = false)
            {
                newRing = Math.Min(newRing, CustomCluster.Rings);
                if (original)
                    originalMaxPOI = newRing;
                if (newRing >= 0)
                {
                    if (placement != null)
                    {
                        var rings = placement.allowedRings;
                        rings.max = newRing;
                        if (newRing < rings.min)
                        {
                            rings.min = newRing;
                        }
                        placement.allowedRings = rings;
                    }
                    else if (placementPOI != null)
                    {
                        var rings = placementPOI.allowedRings;
                        rings.max = newRing;
                        if (newRing < rings.min)
                        {
                            rings.min = newRing;
                        }
                        placementPOI.allowedRings = rings;
                    }
                    else
                        SgtLogger.warning(this.id + ": no placement component found!");
                }
            }
            
            public void SetBuffer(int newBuffer)
            {
                newBuffer = Math.Min(newBuffer, CustomCluster.Rings);
                if (newBuffer >= 0)
                {
                    if (placement != null)
                    {
                        //var buffer = placement.buffer;
                        //rings.max = newRing;
                        //if (newRing < rings.min)
                        //{
                        //    rings.min = newRing;
                        //}
                        placement.buffer = newBuffer;
                    }
                    else
                        SgtLogger.warning(this.id + ": no placement component found!");
                }
            }

            #endregion

            public StarmapItem(string id, StarmapItemCategory category, Sprite sprite)
            {
                this.id = id;
                this.category = category;
                this.planetSprite = sprite;
            }
            public StarmapItem MakeItemPlanet(ProcGen.World world)
            {
                this.world = world;
                //XYratio = (float)world.worldsize.X / (float)world.worldsize.Y;
                return this;
            }
            public StarmapItem AddItemWorldPlacement(WorldPlacement placement2, float morethanone = 1)
            {
                this.MaxNumberOfInstances = morethanone;

                this.placement = new WorldPlacement();
                placement.startWorld = placement2.startWorld;
                placement.world = placement2.world;
                placement.y = placement2.y;
                placement.x = placement2.x;
                placement.height = placement2.height;
                placement.width = placement2.width;
                placement.allowedRings = new(placement2.allowedRings.min, placement2.allowedRings.max);
                placement.buffer = placement2.buffer;
                placement.locationType = placement2.locationType;
                return this;
            }
            public StarmapItem MakeItemPOI(string _poiID, SpaceMapPOIPlacement placement2, float MaxNumberOfInstances, string _poiName, string _poiDesc)
            {
                this._poiID = _poiID;
                this._poiName = _poiName;
                this._poiDesc = _poiDesc;
                this.placementPOI = new SpaceMapPOIPlacement();
                placementPOI.pois = placement2.pois;
                placementPOI.canSpawnDuplicates = placement2.canSpawnDuplicates;
                placementPOI.avoidClumping = placement2.avoidClumping;
                placementPOI.numToSpawn = placement2.numToSpawn;
                placementPOI.allowedRings = new(placement2.allowedRings.min, placement2.allowedRings.max);

                this.MaxNumberOfInstances = MaxNumberOfInstances;
                return this;
            }

            #region PlanetMeteors

            public List<MeteorShowerSeason> CurrentMeteorSeasons
            {
                get
                {
                    var seasons = new List<MeteorShowerSeason>();
                    if (world != null)
                    {
                        var db = Db.Get();
                        foreach (var season in world.seasons)
                        {
                            var seasonData = (db.GameplaySeasons.TryGet(season) as MeteorShowerSeason);
                            if (seasonData != null)
                                seasons.Add(seasonData);
                        }
                    }
                    return seasons;
                }
            }

            public List<MeteorShowerEvent> CurrentMeteorShowerTypes
            {
                get
                {
                    var showers = new List<MeteorShowerEvent>();
                    if (world != null)
                    {
                        var db = Db.Get();
                        foreach (var season in world.seasons)
                        {
                            var seasonData = (db.GameplaySeasons.TryGet(season) as MeteorShowerSeason);
                            if (seasonData != null)
                            {
                                foreach (var shower in seasonData.events)
                                {
                                    var showerEvent = shower as MeteorShowerEvent;
                                    if (!showers.Contains(showerEvent))
                                        showers.Add(showerEvent);

                                }

                            }
                        }
                    }
                    return showers;
                }
            }

            private void BackupOriginalWorldTraits()
            {
                if (world != null)
                {
                    if (!ModAssets.ChangedMeteorSeasons.ContainsKey(world))
                    {
                        ModAssets.ChangedMeteorSeasons[world] = new List<string>(world.seasons);
                    }
                }
            }

            public void AddMeteorSeason(string id)
            {
                BackupOriginalWorldTraits();
                if (world != null)
                {
                    world.seasons.Add(id);
                }
            }

            public void RemoveMeteorSeason(string id)
            {
                BackupOriginalWorldTraits();
                if (world != null)
                {
                    if (world.seasons.Contains(id))
                    {
                        world.seasons.Remove(id);
                    }
                }
            }




            #endregion
            #region PlanetTraits

            private List<string> currentPlanetTraits = new List<string>();
            [JsonIgnore] public List<string> CurrentTraits => currentPlanetTraits;
            public void SetWorldTraits (List<string> NEWs) => currentPlanetTraits = NEWs;


            public static List<WorldTrait> AllowedWorldTraitsFor(List<string> currentTraits, ProcGen.World world)
            {
                List<WorldTrait> AllTraits = ModAssets.AllTraitsWithRandomValuesOnly;

                if (world == null)
                {
                    return new List<WorldTrait>();
                }
                List<string> ExclusiveWithTags
                    = new List<string>();

                if (currentTraits.Count > 0)
                {
                    AllTraits.RemoveAll((WorldTrait trait) => trait.filePath.Contains("CGMRandomTraits"));
                }


                foreach (var trait in currentTraits)
                {
                    if (SettingsCache.worldTraits.ContainsKey(trait))
                    {
                        ExclusiveWithTags.AddRange(SettingsCache.worldTraits[trait].exclusiveWithTags);
                    }
                    if (trait.Contains("CGMRandomTraits"))
                        return new List<WorldTrait>();
                }

                foreach (ProcGen.World.TraitRule rule in world.worldTraitRules)
                {

                    TagSet requiredTags = ((rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null);
                    TagSet forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);

                    AllTraits.RemoveAll((WorldTrait trait) =>
                        (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags))
                        || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags))
                        || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.filePath))
                        || !trait.IsValid(world, logErrors: true));

                }
                AllTraits.RemoveAll((WorldTrait trait) =>
                     !trait.IsValid(world, logErrors: true)
                    || trait.exclusiveWithTags.Any(x => ExclusiveWithTags.Any(y => y == x))
                    || currentTraits.Contains(trait.filePath)
                    || trait.exclusiveWith.Any(x => currentTraits.Any(y => y == x))
                    );
                return AllTraits;

            }
            [JsonIgnore] public List<WorldTrait> AllowedPlanetTraits => AllowedWorldTraitsFor(currentPlanetTraits, world);

            public bool RemoveWorldTrait(WorldTrait trait)
            {
                //SgtLogger.l(trait.filePath, "TryingToRemove");

                string traitID = trait.filePath;
                bool allowed = currentPlanetTraits.Contains(traitID);
                if (allowed)
                    currentPlanetTraits.Remove(traitID);
                return allowed;
            }

            public bool AddWorldTrait(WorldTrait trait)
            {
                string traitID = trait.filePath;
                bool allowed = !currentPlanetTraits.Contains(traitID);
                if (allowed)
                    currentPlanetTraits.Add(traitID);
                return allowed;
            }
            public void ClearWorldTraits()
            {
                currentPlanetTraits.Clear();
            }
            public List<string> GetWorldTraits()
            {
                return currentPlanetTraits;
            }
            #endregion
        }

        public const string CustomClusterIDCoordinate= "CGM";
        public const string CustomClusterID = "expansion1::clusters/CGMCluster";
        public static ClusterLayout GeneratedLayout => GenerateClusterLayoutFromCustomData();
        public static CustomClusterData CustomCluster;


        public static void AddCustomClusterAndInitializeClusterGen()
        {
            LoadCustomCluster = true;
            var GeneratedCustomCluster = GenerateClusterLayoutFromCustomData();
            SettingsCache.clusterLayouts.clusterCache[CustomClusterID] = GeneratedCustomCluster;
            CustomGameSettings.Instance.LoadClusters();// levels.clusterCache[CustomClusterID] = GeneratedCustomCluster;
            //selectScreen.newGameSettings.SetSetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout, CustomClusterID);
            SgtLogger.l(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id.ToString());
            CustomGameSettings.Instance.SetQualitySetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout, GeneratedCustomCluster.filePath);
            SgtLogger.l(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id.ToString());
            CGSMClusterManager.selectScreen.LaunchClicked();
        }

        static void ApplySizeMultiplier(WorldPlacement placement, float multiplier)
        {
            float min = placement.allowedRings.min, max = placement.allowedRings.max;
            //min*= multiplier;

            if (max < 3)
                max = 3;

            max *= multiplier;
            max = Math.Min(max, CustomCluster.Rings);

            int max2 = Math.Min(placement.allowedRings.max + CustomCluster.AdjustedOuterExpansion, CustomCluster.Rings);
            int newMax = Math.Max((int)Math.Round(max), max2);
            placement.allowedRings = new MinMaxI((int)min, newMax);

            SgtLogger.l("Set inner and outer limits to " + placement.allowedRings.ToString(), placement.world);
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



        public static ClusterLayout GenerateClusterLayoutFromCustomData()
        {
            SgtLogger.l("Started generating custom cluster");
            var layout = new ClusterLayout();


            //var Reference = SettingsCache.clusterLayouts.GetClusterData(ClusterID);
            //SgtLogger.log(Reference.ToString());
            layout.filePath = CustomClusterID;
            layout.name = CustomClusterID;
            layout.description = CustomClusterID;
            layout.worldPlacements = new List<WorldPlacement>();
            layout.coordinatePrefix = CustomClusterIDCoordinate;
            float multiplier = 1f;
            if (CustomCluster.Rings > CustomCluster.defaultRings)
            {
                multiplier = (float)CustomCluster.Rings / (float)CustomCluster.defaultRings;
            }
            SgtLogger.l("Cluster Size: " + CustomCluster.Rings);
            SgtLogger.l("Placement Multiplier: " + multiplier);

            if (CustomCluster.StarterPlanet != null)
            {
                ///Disabling Story Traits if MiniBase worlds are active
                if (CustomCluster.StarterPlanet.DisablesStoryTraits)
                    layout.disableStoryTraits = true;


                if (CustomCluster.StarterPlanet.id.Contains(RandomKey))
                {
                    var randomItem = GetRandomItemOfType(StarmapItemCategory.Starter);
                    var placement = GivePrefilledItem(randomItem).placement;

                    placement.allowedRings = CustomCluster.StarterPlanet.placement.allowedRings;
                    placement.buffer = CustomCluster.StarterPlanet.placement.buffer;

                    placement.startWorld = true;

                    layout.worldPlacements.Add(placement);
                    SgtLogger.l(randomItem.id, "Random Start Planet");

                }
                else
                {
                    CustomCluster.StarterPlanet.placement.startWorld = true;
                    layout.worldPlacements.Add(CustomCluster.StarterPlanet.placement);
                    SgtLogger.l(CustomCluster.StarterPlanet.id, "Start Planet");
                }

            }
            else
                SgtLogger.warning("No start planet selected");

            if (CustomCluster.WarpPlanet != null)
            {
                ///Disabling Story Traits if MiniBase worlds are active
                if (CustomCluster.WarpPlanet.DisablesStoryTraits)
                    layout.disableStoryTraits = true;

                if (CustomCluster.StarterPlanet.placement.allowedRings.max == 0
                    && CustomCluster.WarpPlanet.placement.allowedRings.max < CustomCluster.StarterPlanet.placement.buffer)
                {
                    var vector = CustomCluster.WarpPlanet.placement.allowedRings;
                    CustomCluster.WarpPlanet.placement.allowedRings = new MinMaxI(vector.min, CustomCluster.StarterPlanet.placement.buffer + 1);
                }
                if (CustomCluster.WarpPlanet.id.Contains(RandomKey))
                {
                    var randomItem = GetRandomItemOfType(StarmapItemCategory.Warp);
                    var placement = GivePrefilledItem(randomItem).placement;

                    placement.allowedRings = CustomCluster.WarpPlanet.placement.allowedRings;
                    placement.buffer = CustomCluster.WarpPlanet.placement.buffer;

                    layout.worldPlacements.Add(placement);
                    SgtLogger.l(randomItem.id, "Random Warp Planet");
                }
                else
                {
                    layout.worldPlacements.Add(CustomCluster.WarpPlanet.placement);
                    SgtLogger.l(CustomCluster.WarpPlanet.id, "Warp Planet");
                }
            }
            else
                SgtLogger.log("No warp planet selected");
            ///STRINGS.NAMEGEN.WORLD.ROOTS

            RandomOuterPlanets.Clear();
            List<StarmapItem> OuterPlanets = CustomCluster.OuterPlanets.Values.ToList();
            if(OuterPlanets.Count > 0)
            {
                SgtLogger.l(OuterPlanets.Count + " outer planets selected");
                OuterPlanets = OuterPlanets.OrderBy(item => item.placement.allowedRings.max).ThenBy(item => item.placement.allowedRings.min).ToList();
                foreach (var world in OuterPlanets)
            {
                ///Disabling Story Traits if MiniBase worlds are active
                if (world.DisablesStoryTraits)
                    layout.disableStoryTraits = true;

                if (world.id.Contains(RandomKey))
                {
                    SgtLogger.l(world.InstancesToSpawn.ToString(), "Random Planets to select");
                    for (int i = 1; i <= (int)world.InstancesToSpawn; i++)
                    {
                        var randomItem = GetRandomItemOfType(StarmapItemCategory.Outer);
                        if (randomItem == null)
                        {

                            SgtLogger.l("Failed to get unused", "Random Outer Planet");
                            break;
                        }

                        var placement = GivePrefilledItem(randomItem).placement;
                        placement.allowedRings = world.placement.allowedRings;
                        placement.buffer = world.placement.buffer;

                        ApplySizeMultiplier(placement, multiplier);
                        layout.worldPlacements.Add(placement);

                        SgtLogger.l(randomItem.id, "selected random outer Planet");
                    }
                }
                else
                {
                    var placement = world.placement;
                    ApplySizeMultiplier(placement, multiplier);
                    layout.worldPlacements.Add(placement);
                    SgtLogger.l(world.id, "Outer Planet");
                }
            }

            }

            //layout.worldPlacements = layout.worldPlacements.OrderBy(item => item.allowedRings.max).ThenBy(item => item.allowedRings.min).ToList();
            //layout.startWorldIndex = layout.worldPlacements.FindIndex(placement => placement.startWorld == true);

            SgtLogger.l("Planet Placements done");
            layout.poiPlacements = new List<SpaceMapPOIPlacement>();

            foreach (var poi in CustomCluster.POIs)
            {
                ///random handler
                if (poi.Key.Contains(RandomKey))
                {
                    float randomInstancesToSpawn = poi.Value.InstancesToSpawn;

                    for (int i = 0; i < randomInstancesToSpawn; i++)
                    {
                        var randomItem = GetRandomItemOfType(StarmapItemCategory.POI);
                        if (randomItem == null)
                            break;
                        randomItem.SetInnerRing(poi.Value.minRing);
                        randomItem.SetOuterRing(poi.Value.maxRing);
                        ApplySizeMultiplier(randomItem.placementPOI, multiplier);

                        layout.poiPlacements.Add(randomItem.placementPOI);

                        SgtLogger.l(randomItem.id, "Random POI");

                    }
                    continue;
                }


                float instancesToSpawn = poi.Value.InstancesToSpawn;
                while (instancesToSpawn >= 1)
                {

                    var poiPlacement = poi.Value.placementPOI;
                    ApplySizeMultiplier(poiPlacement, multiplier);
                    layout.poiPlacements.Add(poiPlacement);

                    SgtLogger.l(poi.Value.id, "POI");
                    --instancesToSpawn;
                }
                if (instancesToSpawn < 1 && instancesToSpawn > 0.01)
                {
                    float chance = UnityEngine.Random.Range(0.01f, 1.01f);
                    if (chance <= instancesToSpawn)
                    {
                        var poiPlacement = poi.Value.placementPOI;
                        ApplySizeMultiplier(poiPlacement, multiplier);
                        layout.poiPlacements.Add(poiPlacement);
                        SgtLogger.l(poi.Value.id + ", succeeded: " + chance * 100f, "POI Chance: " + instancesToSpawn.ToString("P"));
                        // pity = 0;
                    }
                    else
                    {
                        SgtLogger.l(poi.Value.id + ", failed: " + chance * 100f, "POI Chance: " + instancesToSpawn.ToString("P"));
                        //pity += 1-instancesToSpawn;
                    }
                }
            }

            SgtLogger.l("POI Placements done");
            layout.numRings = CustomCluster.Rings + 1;
            //layout.difficulty = Reference.difficulty;
            //layout.requiredDlcId = Reference.requiredDlcId;
            //layout.forbiddenDlcId = Reference.forbiddenDlcId;
            layout.startWorldIndex = 0;// Reference.startWorldIndex;
                                       //CustomLayout.clusterCategory = Reference.clusterCategory;

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


        public static void CreateCustomClusterFrom(string clusterID, string singleItemId = "", bool ForceRegen = false)
        {
            if (lastWorldGenFailed && !ForceRegen)
                return;
            ClusterLayout Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);

            if (Reference == null || selectScreen == null || selectScreen.newGameSettings == null)
                return;
            string setting = selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);

            if (setting == null || setting.Length == 0)
                return;

            int seed = int.Parse(setting);

            if (singleItemId == string.Empty)
            {
                CustomCluster = new CustomClusterData();
                CustomCluster.SetRings(Reference.numRings - 1, true);
            }
            else
            {
                ///when planet not normally in cluster, but selected rn and to reset - reload from data
                if (!Reference.worldPlacements.Any(placement => placement.world == singleItemId))
                {
                    if (PlanetoidDict().TryGetValue(singleItemId, out var FoundPlanet))
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

                if (PlanetoidDict().TryGetValue(planetpath, out var FoundPlanet))
                {
                    if (singleItemId != string.Empty && FoundPlanet.id != singleItemId)
                    {
                        continue;
                    }

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

            if (singleItemId == string.Empty)
            {
                CustomCluster.defaultOuterPlanets = Reference.worldPlacements.Count;
                CustomCluster.POIs.Clear();
            }
            else
            {
                if (CustomCluster.POIs.ContainsKey(singleItemId))
                {
                    CustomCluster.POIs.Remove(singleItemId);
                }
            }

            if (Reference.poiPlacements == null)
                return;

            foreach (SpaceMapPOIPlacement pOIPlacement in Reference.poiPlacements)
            {
                if (pOIPlacement.numToSpawn < 1 || pOIPlacement.pois == null)
                    continue;

                float percentagePerItem = (float)pOIPlacement.numToSpawn / (float)pOIPlacement.pois.Count;

                foreach (var lonePOI in pOIPlacement.pois)
                {
                    if (PlanetoidDict().TryGetValue(lonePOI, out var ClusterPOI))
                    {
                        if (singleItemId != string.Empty && ClusterPOI.id != singleItemId)
                        {
                            continue;
                        }

                        ClusterPOI.ResetPOI();
                        if (CustomCluster.POIs.ContainsKey(ClusterPOI.id))
                        {
                            ClusterPOI.SetInnerRing(Math.Min(ClusterPOI.minRing, pOIPlacement.allowedRings.min));
                            ClusterPOI.SetOuterRing(Math.Max(ClusterPOI.maxRing, pOIPlacement.allowedRings.max));
                            ClusterPOI.IncreaseSpawnNumber(percentagePerItem);
                            CustomCluster.POIs[ClusterPOI.id] = ClusterPOI;
                        }
                        else
                        {
                            ClusterPOI.SetSpawnNumber(percentagePerItem);
                            CustomCluster.POIs[ClusterPOI.id] = (ClusterPOI);
                        }
                    }
                }
            }
            LastPresetGenerated = clusterID;
        }

        public static bool RerollTraitsWithSeedChange = true;
        public static void RerollTraits()
        {
            if (CustomCluster == null || (!RerollTraitsWithSeedChange && CustomSettingsController.Instance.IsCurrentlyActive))
                return;

            int seed = int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id);

            var planets = CustomCluster.GetAllPlanets();
            for (int i = 0; i < planets.Count; i++)
            {
                var FoundPlanet = planets[i];
                if(FoundPlanet.world==null) continue;

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

            if(ToAdd.id.Contains(CGSMClusterManager.RandomKey))
                return ToAdd;

            if (ToAdd.id == null)
                return ToAdd;

            if (PredefinedPlacementData.ContainsKey(ToAdd.id))
                ToAdd.AddItemWorldPlacement(PredefinedPlacementData[ToAdd.id]);
            else
            {
                if (!ToAdd.IsPOI)
                {
                    var item = new WorldPlacement();
                    item.world = ToAdd.id;
                    item.allowedRings = new MinMaxI(0, CustomCluster.Rings);
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
                    CustomCluster.OuterPlanets[item.id] = (item);
                else
                    CustomCluster.OuterPlanets.Remove(item.id);
            }
            else
            {
                if (!CustomCluster.POIs.ContainsKey(item.id))
                    CustomCluster.POIs[item.id] = (item);
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
            foreach (var planet in CustomCluster.POIs)
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
            foreach (var planet in CustomCluster.POIs)
            {
                planetPaths.Add(planet.Key);
            }
            return planetPaths;
        }

        public static void PopulatePredefinedClusterPlacements()
        {
            if (PredefinedPlacementData != null) { return; }

            PredefinedPlacementData = new Dictionary<string, WorldPlacement>();
            //PredefinedPlacementDataPOI = new Dictionary<string, SpaceMapPOIPlacement>();

            foreach (var ClusterLayout in SettingsCache.clusterLayouts.clusterCache.ToList())
            {
                // SgtLogger.debuglog(ClusterLayout.Key,"HALP");
                if (ClusterLayout.Key.Contains("clusters/SandstoneDefault"))
                {
                    continue;
                }

                if(ClusterLayout.Value.disableStoryTraits)
                {
                    foreach(var planet in ClusterLayout.Value.worldPlacements)
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

                foreach (var poi_tab in ClusterLayout.Value.poiPlacements)
                {
                    foreach (var lonePOI in poi_tab.pois)
                    {
                        //var animFile = lonePOI.Contains("ArtifactSpacePOI") ? Assets.GetAnim((HashedString)"gravitas_space_poi_kanim") : Assets.GetAnim((HashedString)"harvestable_space_poi_kanim");

                        KAnimFile animFile = null;
                        string animName = "ui";
                        string name = string.Empty;
                        string desc = string.Empty;
                        string id = string.Empty;

                        //SgtLogger.l(lonePOI, "LonePOI");
                        GameObject gameObject = Util.KInstantiateUI(Assets.GetPrefab((Tag)lonePOI));

                        ClusterGridEntity component1 = gameObject.GetComponent<ClusterGridEntity>();
                        if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                        {
                            animName = component1.AnimConfigs.First().initialAnim;
                            animFile = component1.AnimConfigs.First().animFile;
                            if (gameObject.TryGetComponent<InfoDescription>(out var descHolder))
                            {
                                desc = descHolder.description;
                                name = component1.Name;
                            }
                            if (component1 is ArtifactPOIClusterGridEntity)
                            {
                                string artifact_ID = component1.PrefabID().ToString().Replace("ArtifactSpacePOI_", string.Empty);
                                name = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".NAME"));
                                desc = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".DESC"));
                            }
                            if (component1 is TemporalTear)
                            {
                                name = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.NAME"));
                                desc = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.DESCRIPTION"));
                            }
                            id = component1.PrefabID().ToString();
                        }

                        //id = id.Replace("ArtifactSpacePOI_", string.Empty);
                        //id = id.Replace("HarvestableSpacePOI_", string.Empty);

                        if (lonePOI.Contains(HarvestablePOIConfig.CarbonAsteroidField)) ///carbon field fix
                            animName = "carbon_asteroid_field";

                        if (animName == "closed_loop")///Temporal tear
                            animName = "ui";


                        float moreThanOne = lonePOI != "ArtifactSpacePOI_RussellsTeapot" && lonePOI != "TemporalTear" ? MaxAmountPOI : 1;

                        Sprite POIsprite = Def.GetUISpriteFromMultiObjectAnim(animFile, animName, true);

                        UnityEngine.Object.Destroy(gameObject);

                        var poi = new StarmapItem(lonePOI, StarmapItemCategory.POI, POIsprite);

                        SpaceMapPOIPlacement placement = new SpaceMapPOIPlacement();
                        placement.pois = new List<string> { lonePOI };
                        placement.numToSpawn = 1;
                        placement.allowedRings = poi_tab.allowedRings;
                        placement.avoidClumping = poi_tab.avoidClumping;
                        poi = poi.MakeItemPOI(id, placement, moreThanOne, name, desc);

                        if (!PlanetsAndPOIs.ContainsKey(lonePOI))
                        {
                            poi.SetInnerRing(placement.allowedRings.min, true);
                            poi.SetOuterRing(placement.allowedRings.max, true);
                            PlanetsAndPOIs.Add(lonePOI, poi);
                        }
                        else
                        {
                            var toEdit = PlanetsAndPOIs[lonePOI];
                            toEdit.SetInnerRing(Math.Min(toEdit.minRing, placement.allowedRings.min), true);
                            toEdit.SetOuterRing(Math.Max(toEdit.maxRing, placement.allowedRings.max), true);
                        }
                    }

                    ///Requires different handling
                    //PredefinedPlacementDataPOI[poi.]
                    //SgtLogger.l("", "POI:");
                    //foreach (var poi2 in poi_tab.pois)
                    //{
                    //    //SgtLogger.l(poi2, "Poi in list:");
                    //}
                    //SgtLogger.l(poi.avoidClumping.ToString(), "avoid clumping");
                    //SgtLogger.l(poi.canSpawnDuplicates.ToString(), "Allow Duplicates");
                    //SgtLogger.l(poi.allowedRings.ToString(), "Allowed Rings");
                    //SgtLogger.l(poi.numToSpawn.ToString(), "Number to spawn");
                }
            }

        }

        public const float MaxAmountPOI = 5f;
        public const float MaxAmountRandomPOI = 32f;
        public const float MaxAmountRandomPlanet = 6f;
        public const string RandomKey = "CGM_RANDOM_";
        static Dictionary<string, StarmapItem> PlanetsAndPOIs = null;


        static List<string> RandomOuterPlanets = new List<string>();

        public static StarmapItem GetRandomItemOfType(StarmapItemCategory starmapItemCategory)
        {
            List<StarmapItem> items = PlanetoidDict().Values.ToList().FindAll(item => item.category == starmapItemCategory);
            items.Shuffle();

            StarmapItem item = null;
            int i;
            for (i = 0; i < items.Count; ++i)
            {
                item = items[i];
                //SgtLogger.l(item.id, "Random");
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
                RandomOuterPlanets.Add(item.id);

            //while (item.category != starmapItemCategory || item.id.Contains("TemporalTear") || item.id == null || item.id == string.Empty)
            //{
            //    item = PlanetoidDict().Values.GetRandom();
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

        public static Dictionary<string, StarmapItem> PlanetoidDict()
        {
            if (PlanetsAndPOIs == null)
            {
                PlanetsAndPOIs = new Dictionary<string, StarmapItem>();

                foreach (StarmapItemCategory category in (StarmapItemCategory[])Enum.GetValues(typeof(StarmapItemCategory)))
                {
                    var key = RandomKey + category.ToString();
                    var randomItem = new StarmapItem
                        (
                        key,
                        category,
                        GetRandomSprite(category)
                        );

                    randomItem.SetSpawnNumber(1);

                    if (category == StarmapItemCategory.POI)
                    {
                        var placement = new SpaceMapPOIPlacement();
                        placement.allowedRings = new MinMaxI(0, CustomCluster.Rings);
                        placement.canSpawnDuplicates = true;
                        placement.numToSpawn = 1;
                        placement.avoidClumping = false;
                        randomItem = randomItem.MakeItemPOI(key, placement, MaxAmountRandomPOI, SPACEDESTINATIONS.CGM_RANDOM_POI.NAME, SPACEDESTINATIONS.CGM_RANDOM_POI.DESCRIPTION);
                        PlanetsAndPOIs[key] = randomItem;
                    }
                    else
                    {
                        var placement = new WorldPlacement();
                        placement.allowedRings = category == StarmapItemCategory.Starter ? new MinMaxI(0, 0) : new MinMaxI(0, CustomCluster.Rings);
                        placement.startWorld = category == StarmapItemCategory.Starter;
                        placement.locationType = category == StarmapItemCategory.Starter ? LocationType.Startworld : LocationType.Cluster;

                        randomItem = randomItem.AddItemWorldPlacement(placement, category == StarmapItemCategory.Outer ? MaxAmountRandomPlanet : 1);
                        PlanetsAndPOIs[key] = randomItem;
                    }
                }

                foreach (var WorldFromCache in SettingsCache.worlds.worldCache)
                {
                    StarmapItemCategory category = StarmapItemCategory.Outer;
                    //SgtLogger.l(World.Key + "; " + World.Value.ToString());
                    ProcGen.World world = WorldFromCache.Value;

                   // SgtLogger.l(WorldFromCache.Key.ToUpperInvariant(), "PLANETKEY");
                    if ((int)world.skip >= 99)
                        continue;


                    ///Hardcoded checks due to others not having the correct folder structure
                    string KeyUpper = WorldFromCache.Key.ToUpperInvariant();
                    bool SkipModdedWorld =
                        KeyUpper.Contains("EMPTERA") && !KeyUpper.Contains("DLC")
                        || KeyUpper.Contains("ISLANDS") && !KeyUpper.Contains("DLC")
                        || KeyUpper.Contains("FULERIA") && !KeyUpper.Contains("DLC")
                        ;

                   


                    if (!WorldFromCache.Key.Contains("worlds/SandstoneDefault") && !SkipModdedWorld)
                    {
                        category = DeterminePlanetType(world);

                        Sprite sprite = ColonyDestinationAsteroidBeltData.GetUISprite(WorldFromCache.Value.asteroidIcon);

                        PlanetsAndPOIs[WorldFromCache.Key] = (new StarmapItem
                        (
                        WorldFromCache.Key,
                        category,
                        sprite
                        ).MakeItemPlanet(world));

                        if (KeyUpper.Contains("BABY"))
                        {
                            SgtLogger.l(WorldFromCache.Key + " will disable story traits due to Baby size");
                            PlanetsAndPOIs[WorldFromCache.Key].DisablesStoryTraits = true;
                        }
                    }

                }

                PopulatePredefinedClusterPlacements();
            }
            return PlanetsAndPOIs;
        }

        static int AdjustedClusterSize => CustomCluster.defaultRings + Mathf.Max(0, (CustomCluster.AdjustedOuterExpansion / 4));
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
               , (DebugHandler.enabled) ? UIUtils.ColorText("[Debug only] Try generating anyway", Color.red) : null
               , (DebugHandler.enabled) ? aaanyways : null
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
    }
}
