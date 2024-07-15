using Klei.AI;
using Newtonsoft.Json;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using UnityEngine;
using UtilLibs;

namespace ClusterTraitGenerationManager.ClusterData
{
    public class StarmapItem
    {
        public string id;
        public StarmapItemCategory category;
        public bool DisablesStoryTraits = false;
        [JsonIgnore] public bool originalPOIGroup => POIGroupUID == string.Empty;

        public string POIGroupUID = string.Empty;

        [JsonIgnore] public Sprite planetSprite;

        [JsonIgnore] public ProcGen.World world;
        [JsonIgnore] public Vector2I originalWorldDimensions;
        [JsonIgnore] public string ModName = string.Empty;


        public WorldPlacement placement;

        public SpaceMapPOIPlacement placementPOI;

        public bool IsPOI => category == StarmapItemCategory.POI;
        public bool IsRandom => id.Contains(RandomKey);

        public int PredefinedPlacementOrder = -1;

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

                if (world != null && world.name != null)
                {
                    if (Strings.TryGet(world.name, out var nameEntry))
                    {
                        var name = nameEntry.ToString();
                        if (ModName != string.Empty)
                            name += " " + UIUtils.ColorText(STRINGS.UI.SPACEDESTINATIONS.MODDEDPLANET, UIUtils.rgb(212, 244, 199));

                        return name;
                    }
                }
                else if (category == StarmapItemCategory.POI)
                {
                    return id.Substring(0, 8);
                }

                //else if (_poiID != null)
                //{
                //    return _poiName;
                //}
                return id;
            }
        }
        public string DisplayDescription
        {
            get
            {
                string desc = string.Empty;
                if (ModName != string.Empty)
                {
                    desc += UIUtils.ColorText(string.Format(STRINGS.UI.SPACEDESTINATIONS.MODDEDPLANETDESC, ModName), UIUtils.rgb(212, 244, 199));
                    desc += "\n";
                    desc += "\n";
                }

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

                if (world != null && world.description != null)
                {
                    if (Strings.TryGet(world.description, out var description))
                    {
                        desc += description.String;
                        return desc;
                    }
                }
                //else if (_poiID != null)
                //{
                //    return _poiDesc;
                //}
                desc += id;
                return desc;
            }
        }

        //private float XYratio = -1f;
        public float ApplySizeMultiplierToValue(float inputValue)
        {
            return (CurrentSizeMultiplier * inputValue);
        }

        [JsonIgnore]
        public Vector2I CustomPlanetDimensions
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

                    dim.X = Mathf.RoundToInt(originalWorldDimensions.X * sizeIncreaseMultiplier);
                    dim.Y = Mathf.RoundToInt(originalWorldDimensions.Y * sizeIncreaseMultiplier);

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
                    var rounded = Mathf.RoundToInt(Mathf.Max(Mathf.Min(value, originalWorldDimensions.Y * 2.6f), originalWorldDimensions.Y * 0.55f));
                    if (rounded != CustomY)
                        CustomY = rounded;
                }
                else
                {
                    var rounded = Mathf.RoundToInt(Mathf.Max(Mathf.Min(value, originalWorldDimensions.X * 2.6f), originalWorldDimensions.X * 0.55f));

                    if (rounded != CustomX)
                        CustomX = rounded;
                }
                CustomSizeIncrease = (float)(CustomX * CustomY) / (float)(originalWorldDimensions.X * originalWorldDimensions.Y);
            }
        }
        public float SizeMultiplierX()
        {
            if (UsingCustomDimensions && world != null)
            {
                return CustomX / originalWorldDimensions.X;
            }
            else
            {
                float sizePercentage = (float)SizePreset / 100f;
                return Mathf.Sqrt(sizePercentage);
            }

        }
        public float SizeMultiplierY()
        {
            if (UsingCustomDimensions && world != null)
            {
                return CustomY / originalWorldDimensions.Y;
            }
            else
            {
                float sizePercentage = (float)SizePreset / 100f;
                return Mathf.Sqrt(sizePercentage);
            }
        }


        float CustomSizeIncrease = -1f;

        public float InstancesToSpawn = 1;
        public bool MoreThanOnePossible = false;
        //public float MaxNumberOfInstances = 1;
        [JsonIgnore] public int minRing => placement != null ? placement.allowedRings.min : placementPOI != null ? placementPOI.allowedRings.min : -1;
        [JsonIgnore] public int maxRing => placement != null ? placement.allowedRings.max : placementPOI != null ? placementPOI.allowedRings.max : -1;
        [JsonIgnore] public int buffer => placement != null ? placement.buffer : -1;

        [JsonIgnore] public bool SupportsGeyserOverride
        {
            get
            {
                if(_geyserOverrideCount == -1)
                {
                    if(world != null)
                    {
                        _geyserOverrideCount = 0;
                        foreach (var poiRule in world.worldTemplateRules)
                        {
                            if(poiRule.names!=null && poiRule.names.Count == 1 && poiRule.names.First()== "geysers/generic")
                            {
                                _geyserOverrideCount += poiRule.times;
                            }
                        }
                    }
                }
                return _geyserOverrideCount > 0;
            }
        }

        private int _geyserOverrideCount = -1;

        #region SetterMethods

        public void RefresDuplicateState()
        {
            if (placementPOI != null && InstancesToSpawn < placementPOI.pois.Count)
            {
                placementPOI.canSpawnDuplicates = true;
            }
        }

        public void SetSpawnNumber(float newNumber, bool force = false)
        {
            if (newNumber < 0)
                newNumber = 0;
            if (this.IsPOI)
            {
                if (newNumber <= MaxPOICount || force)
                {
                    InstancesToSpawn = newNumber;
                }
                else
                    InstancesToSpawn = MaxPOICount;

                RefresDuplicateState();
            }
            else
            {
                if (newNumber <= MaxAmountRandomPlanet || force)
                {
                    InstancesToSpawn = newNumber;
                }
                else
                    InstancesToSpawn = MaxAmountRandomPlanet;
            }
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
            this.originalWorldDimensions = world.worldsize;
            //this.InitGeyserInfo();

            //XYratio = (float)world.worldsize.X / (float)world.worldsize.Y;


            string filepath = world.filePath;
            ///Dynamically created planets, check for original instead
            if (ModAssets.ModPlanetOriginPaths.ContainsKey(filepath))
                filepath = ModAssets.ModPlanetOriginPaths[filepath];

            string path = SettingsCache.RewriteWorldgenPathYaml(filepath);

            if (ModAssets.IsModdedAsteroid(path, out var sourceMod))
            {
                this.ModName = sourceMod.title;
            }

            return this;
        }

        //private void InitGeyserInfo()
        //{
        //    var Templates = this.world.worldTemplateRules;
        //    foreach (var Template in Templates)
        //    {

        //    }
        //}
        //[JsonIgnore]public List<GeyserInfo> GeyserInfos;
        //public class GeyserInfo
        //{
        //    public Sprite PreviewImage;
        //    public string Name, Description;
        //    public int minCount, maxCount;
        //}


        public StarmapItem AddItemWorldPlacement(WorldPlacement placement2, bool morethanone = false)
        {
            //this.MaxNumberOfInstances = morethanone;
            this.MoreThanOnePossible = morethanone;
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
        public StarmapItem MakeItemPOI(SpaceMapPOIPlacement placement2)
        {
            MoreThanOnePossible = true;
            placementPOI = new SpaceMapPOIPlacement();
            placementPOI.pois = new(placement2.pois);
            placementPOI.canSpawnDuplicates = placement2.canSpawnDuplicates;
            placementPOI.avoidClumping = placement2.avoidClumping;
            placementPOI.numToSpawn = placement2.numToSpawn;
            placementPOI.allowedRings = new(placement2.allowedRings.min, placement2.allowedRings.max);
            originalMaxPOI = placement2.allowedRings.max;
            originalMinPOI = placement2.allowedRings.min;
            //MaxNumberOfInstances = placement2.numToSpawn * 5f; 
            InstancesToSpawn = placement2.numToSpawn;
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
        public void SetWorldTraits(List<string> NEWs) => currentPlanetTraits = NEWs;


        public static List<WorldTrait> AllowedWorldTraitsFor(List<string> currentTraits, ProcGen.World world)
        {
            List<WorldTrait> AllTraits = ModAssets.AllTraitsWithRandomValuesOnly;

            if (world == null)
            {
                return new List<WorldTrait>();
            }

            List<WorldTrait> AlwaysAvailableTraits = AllTraits.FindAll((WorldTrait trait) => trait.traitTags.Contains(ModAPI.CGM_TraitTags.OverrideWorldRules_AlwaysAllow));

            List<string> ExclusiveWithTags = new List<string>();

            if (currentTraits.Count > 0 || (world != null && world.disableWorldTraits))
            {
                AllTraits.RemoveAll((WorldTrait trait) => trait.filePath == ModAssets.CGM_RandomTrait);
            }


            foreach (var trait in currentTraits)
            {
                if (SettingsCache.worldTraits.ContainsKey(trait))
                {
                    ExclusiveWithTags.AddRange(SettingsCache.worldTraits[trait].exclusiveWithTags);
                }
                if (trait == ModAssets.CGM_RandomTrait) //random trait is mutually exclusive with everything else
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
            AllTraits = AllTraits.Union(AlwaysAvailableTraits).ToList();
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
}
