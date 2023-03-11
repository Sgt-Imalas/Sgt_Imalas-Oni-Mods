using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static KAnim;
using static Klei.ClusterLayoutSave;
using static LogicGate.LogicGateDescriptions;
using static ProcGen.Mob;
using static ProcGen.WorldPlacement;
using static STRINGS.CLUSTER_NAMES;
using static STRINGS.UI.CLUSTERMAP;
using static STRINGS.UI.FRONTEND;

namespace ClusterTraitGenerationManager
{
    internal class CGSMClusterManager
    {
        public static GameObject Screen = null;

        public static ColonyDestinationSelectScreen selectScreen;


        public static void InstantiateClusterSelectionView(ColonyDestinationSelectScreen parent, System.Action onClose = null)
        {
            if (true)//Screen == null)
            {
                if (CustomCluster == null)
                {
                    ///Change to check for moonlet/vanilla start
                    var defaultCluster = CustomCluster == null ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                    CreateCustomClusterFrom(defaultCluster);
                }

                LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen);
                LockerNavigator.Instance.PopScreen();

                var window = Util.KInstantiateUI(LockerNavigator.Instance.kleiInventoryScreen.gameObject);
                window.SetActive(false);
                var copy = window.transform;
                UnityEngine.Object.Destroy(window);
                var newScreen = Util.KInstantiateUI(copy.gameObject, parent.transform.parent.gameObject, true);
                selectScreen = parent;
                newScreen.name = "ClusterSelectionView";
                var cmp = newScreen.AddComponent(typeof(FeatureSelectionScreen));

                Screen = newScreen;
                //onClose += ()=>AddCustomCluster();

                //UIUtils.ListAllChildren(Screen.transform);

                //LockerNavigator.Instance.PushScreen(newScreen, onClose);
            }
            else
            {
                SgtLogger.l("not new", "SCREEN");
                //LockerNavigator.Instance.PushScreen(Screen, onClose);
            }

            Screen.gameObject.SetActive(true);
            Screen.GetComponent<FeatureSelectionScreen>().RefreshView();

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
            public int Rings { get; private set; }
            public StarmapItem StarterPlanet { get; set; }
            public StarmapItem WarpPlanet { get; set; }
            public List<StarmapItem> OuterPlanets = new List<StarmapItem>();
            public List<StarmapItem> POIs = new List<StarmapItem>();

            public bool HasPlanet(StarmapItem item, out StarmapItem item1)
            {
                

                item1 = GivePrefilledItem(item);
                if (StarterPlanet == item)
                {
                    item1 = StarterPlanet;
                    return true;
                }
                else if (WarpPlanet == item)
                {
                    item1 = WarpPlanet;
                    return true;
                }
                else if (OuterPlanets.Contains(item))
                {
                    item1 = OuterPlanets.Find((i) => i == item);
                    return true;
                }
                else if (POIs.Contains(item))
                {
                    item1 = POIs.Find((i) => i == item);
                    return true;
                }
                return false;
            }


            public bool HasPOI(StarmapItem item, out StarmapItem item1)
            {
                item1 = item;
                if (POIs.Contains(item))
                {
                    item1 = POIs.Find((i) => i == item);
                    return true;
                }
                return false;
            }


            public StarmapItem DataSetterFor(StarmapItem item)
            {
                HasPlanet(item, out StarmapItem item1);
                return item1;
            }

            public void SetRings(int rings)
            {
                Rings = rings;

                if (StarterPlanet.placement != null)
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
                if (WarpPlanet.placement != null)
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

                foreach (var planet in OuterPlanets)
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
                foreach (var planet in POIs)
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


        public struct StarmapItem
        {
            public string id;
            public StarmapItemCategory category;

            public Sprite planetSprite;
            public ProcGen.World world;
            public WorldPlacement placement;
            public SpaceMapPOIPlacement placementPOI;
            public string DisplayName
            {
                get
                {
                    if (world != null)
                    {
                        Strings.TryGet(world.name, out var name);
                        return name;
                    }
                    else if (_poiID != null)
                    {
                        if(Strings.TryGet(new StringKey("STRINGS.UI.SPACEDESTINATIONS.HARVESTABLE_POI." + _poiID.ToUpper() + ".NAME"), out var name))
                            return name;
                        else if (Strings.TryGet(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + _poiID.ToUpper() + ".NAME"), out var name2))
                            return name2;
                        else if (_poiID.ToUpper() == "TEMPORALTEAR")
                        {
                            if(Strings.TryGet(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.NAME"), out var name3))
                                return name3;
                        }
                    }
                    return id;
                }
            }
            public string DisplayDescription {
                get
                {
                    if (world != null)
                    {
                        Strings.TryGet(world.description, out var description);
                        return description.String;
                    }
                    else if (_poiID != null)
                    {
                        if(Strings.TryGet(new StringKey("STRINGS.UI.SPACEDESTINATIONS.HARVESTABLE_POI." + _poiID.ToUpper() + ".DESC"), out var name))
                            return name;
                        else if (Strings.TryGet(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + _poiID.ToUpper() + ".DESC"), out var name2))
                            return name2;
                        else if (_poiID.ToUpper() == "TEMPORALTEAR")
                        {
                            if(Strings.TryGet(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.DESCRIPTION"), out var name3))
                                return name3;
                        }
                    }
                    return id;
                } 
            }
            public string _poiID { get; private set; }

            public float InstancesToSpawn = 1;
            public bool moreThanOneAllowed = false;
            public int minRing => placement != null ? placement.allowedRings.min : placementPOI != null ? placementPOI.allowedRings.min : -1;
            public int maxRing => placement != null ? placement.allowedRings.max : placementPOI != null ? placementPOI.allowedRings.max : -1;
            public int buffer => placement != null ? placement.buffer : -1;


            #region SetterMethods

            public void SetInnerRing(int newRing)
            {
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
                }
            }
            public void SetOuterRing(int newRing)
            {
                newRing = Math.Min(newRing, CustomCluster.Rings);
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
                }
            }

            #endregion


            public StarmapItem(string id, StarmapItemCategory category, Sprite sprite, int allowed)
            {
                this.id = id;
                this.category = category;
                this.planetSprite = sprite;
                this.InstancesToSpawn = allowed;
            }
            public StarmapItem MakeItemPlanet(ProcGen.World world)
            {
                this.world = world;
                return this;
            }
            public StarmapItem AddItemWorldPlacement(WorldPlacement placement)
            {
                this.placement = placement;
                return this;
            }
            public StarmapItem MakeItemPOI(string _displayName, SpaceMapPOIPlacement placement2, bool moreThanOneAllowed)
            {
                this._poiID = _displayName;
                this.placementPOI = placement2;
                this.moreThanOneAllowed = moreThanOneAllowed;
                return this;
            }
            #region EqualityComparer

            public StarmapItem(string id, StarmapItemCategory category, Sprite sprite = null)
            {
                this.id = id;
                this.category = category;
                this.planetSprite = sprite;
            }
            public override bool Equals(System.Object obj)
            {
                return obj is StarmapItem c && this == c;
            }
            public override int GetHashCode()
            {
                return id.GetHashCode() ^ category.GetHashCode();
            }
            public static bool operator ==(StarmapItem x, StarmapItem y)
            {
                return x.id == y.id && x.category == y.category;
            }
            public static bool operator !=(StarmapItem x, StarmapItem y)
            {
                return !(x == y);
            }
            #endregion

        }


        public const string CustomClusterID = "CMGM";
        public static ClusterLayout GeneratedLayout => GenerateClusterLayoutFromCustomData();
        public static CustomClusterData CustomCluster;

        public static void AddCustomCluster()
        {
            SettingsCache.clusterLayouts.clusterCache[CustomClusterID] = GenerateClusterLayoutFromCustomData();

            // selectScreen.destinationMapPanel.UpdateDisplayedClusters();

            selectScreen.newGameSettings.SetSetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout, CustomClusterID);
            selectScreen.newGameSettings.Refresh();

            foreach (var key in selectScreen.newGameSettings.settings.CurrentQualityLevelsBySetting)
            {
                SgtLogger.l(key.Key + "; " + key.Value);
            }
            int seed = int.Parse(selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed));

            //selectScreen.destinationMapPanel.UpdateDisplayedClusters();
            //selectScreen.destinationMapPanel.clusterKeys.Add(ClusterID);
            //selectScreen.destinationMapPanel.SelectCluster(ClusterID, seed);
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


            layout.worldPlacements.Add(CustomCluster.StarterPlanet.placement);

            layout.worldPlacements.Add(CustomCluster.WarpPlanet.placement);

            foreach (var world in CustomCluster.OuterPlanets)
            {
                layout.worldPlacements.Add(world.placement);
            }

            layout.poiPlacements = new List<SpaceMapPOIPlacement>();

            foreach (var poi in CustomCluster.POIs)
            {
                layout.poiPlacements.Add(poi.placementPOI);
            }

            layout.numRings = CustomCluster.Rings;
            //layout.difficulty = Reference.difficulty;
            //layout.requiredDlcId = Reference.requiredDlcId;
            //layout.forbiddenDlcId = Reference.forbiddenDlcId;
            layout.startWorldIndex = 0;// Reference.startWorldIndex;
                                       //CustomLayout.clusterCategory = Reference.clusterCategory;

            SgtLogger.l("Finished generating custom cluster");
            return layout;
        }

        public static void CreateCustomClusterFrom(string clusterID)
        {

            // clusterID = clusterID.Trim();
            CustomCluster = new CustomClusterData();

            ClusterLayout Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);
            if (true)
            {
                CustomCluster.SetRings(Reference.numRings);

                foreach (WorldPlacement planetPlacement in Reference.worldPlacements)
                {
                    string planetpath = planetPlacement.world;
                    //SgtLogger.l(planetpath, "PlanetPath");
                    //SgtLogger.l(planetPlacement.buffer + "<-buffer", "otherData");
                    //SgtLogger.l(planetPlacement.allowedRings + "<-rings", "otherData");
                    if (PlanetoidDict().TryGetValue(planetpath, out var FoundPlanet))
                    {
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
                                CustomCluster.OuterPlanets.Add(FoundPlanet);
                                break;
                        }
                    }
                }

                foreach (SpaceMapPOIPlacement pOIPlacement in Reference.poiPlacements)
                {
                    float percentagePerItem = (float)pOIPlacement.numToSpawn/(float)pOIPlacement.pois.Count;
                    SgtLogger.l(percentagePerItem.ToString(), "Amount of POI");

                    foreach (var lonePOI in pOIPlacement.pois)
                    {
                        if (PlanetoidDict().TryGetValue(lonePOI, out var ClusterPOI))
                        {
                            SgtLogger.l(lonePOI, "POI");
                            SgtLogger.l(ClusterPOI.DisplayName, "POI2");
                            SgtLogger.l(ClusterPOI.id, "POI2");
                            if (CustomCluster.HasPlanet(ClusterPOI, out var ToEdit))
                            {
                                ToEdit.SetInnerRing(Math.Min(ToEdit.minRing, pOIPlacement.allowedRings.min));
                                ToEdit.SetOuterRing(Math.Max(ToEdit.maxRing, pOIPlacement.allowedRings.max));
                                ToEdit.InstancesToSpawn += percentagePerItem;
                                SgtLogger.l(ToEdit.InstancesToSpawn.ToString(), "New MaxPOI");
                            }
                            else
                            {
                                ToEdit.InstancesToSpawn = percentagePerItem;
                                CustomCluster.POIs.Add(ToEdit);
                            }
                        }
                    }
                }
            }
            //SgtLogger.l(Reference == null ? "REF NULL" : Reference.ToString(), "CLUSTERLAYOUT");
        }

        public static StarmapItem GivePrefilledItem(StarmapItem ToAdd)
        {
            PopulatePredefinedClusterPlacements();

            if (ToAdd.id == null)
                return ToAdd;

            //SgtLogger.l(ToAdd + ", " + ToAdd.id);

            if (PredefinedPlacementData.ContainsKey(ToAdd.id))
                ToAdd.AddItemWorldPlacement(PredefinedPlacementData[ToAdd.id]);

            //SgtLogger.l(ToAdd.placement + ", " + ToAdd.placement.allowedRings);

            return ToAdd;

        }

        public static void TogglePlanetoid(StarmapItem ToAdd)
        {
            var item = GivePrefilledItem(ToAdd); ///Prefilled
            //only one starter at a time
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
            ///only one teleport asteroid at a time (TODO; change that maybe)
            else if (item.category == StarmapItemCategory.Warp)
            {
                if (item.Equals(CustomCluster.WarpPlanet))
                    return;
                else
                {
                    CustomCluster.WarpPlanet = item;
                    return;
                }
            }
            //SgtLogger.l(item.id + "; has it: " + CustomCluster.OuterPlanets.Contains(item), "THIS");

            //foreach (var outerPlanet in CustomCluster.OuterPlanets)
            //{
            //    SgtLogger.l(outerPlanet.id, "OTHERS");
            //}
            if (ToAdd.category != StarmapItemCategory.POI)
            {
                if (!CustomCluster.OuterPlanets.Contains(item))
                    CustomCluster.OuterPlanets.Add(item);
                else
                    CustomCluster.OuterPlanets.Remove(item);
            }
            else
            {
                if (!CustomCluster.POIs.Contains(item))
                    CustomCluster.POIs.Add(item);
                else
                    CustomCluster.POIs.Remove(item);
            }

            return;

            //var existing = CustomLayout.worldPlacements.Find(planet => planet.world == item.world.filePath);

            var newItem = new WorldPlacement();
            newItem.world = item.world.filePath;
            newItem.startWorld = item.category == StarmapItemCategory.Starter ? true : false;
            newItem.locationType = item.category == StarmapItemCategory.Starter ? LocationType.Startworld : LocationType.Cluster;
            newItem.allowedRings = new MinMaxI(2, 4);
        }



        static Dictionary<string, WorldPlacement> PredefinedPlacementData = null;

        ///Requires different handling
        static Dictionary<string, SpaceMapPOIPlacement> PredefinedPlacementDataPOI = null;


        public static List<string> GetActivePlanetsCluster()
        {
            var planetPaths = new List<string>();
            planetPaths.Add(CustomCluster.StarterPlanet.id);
            planetPaths.Add(CustomCluster.WarpPlanet.id);

            foreach (var planet in CustomCluster.OuterPlanets)
            {
                planetPaths.Add(planet.id);
            }

            foreach (var planet in CustomCluster.POIs)
            {
                planetPaths.Add(planet.id);
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
                if (!ClusterLayout.Key.Contains("expansion1"))
                {
                    continue;
                }

                foreach (var planetPlacement in ClusterLayout.Value.worldPlacements)
                {
                    PredefinedPlacementData[planetPlacement.world] = planetPlacement;

                    //SgtLogger.l("", "PLANET:");
                    //SgtLogger.l(planetPlacement.world, "FilePath");

                    //Path , aka id
                    //SgtLogger.l(planet.x.ToString()); //muda
                    //SgtLogger.l(planet.y.ToString());//muda
                    //SgtLogger.l(planet.width.ToString());//muda
                    //SgtLogger.l(planet.height.ToString());//muda
                    //SgtLogger.l(planetPlacement.locationType.ToString(), "LocationType"); //startWorld / inner cluster / cluster
                    //SgtLogger.l(planetPlacement.startWorld.ToString(), "IsStartWorld"); //isStartWorld?
                    //SgtLogger.l(planetPlacement.buffer.ToString(), "min distance to others"); //min distance to other planets
                    //SgtLogger.l(planetPlacement.allowedRings.ToString(), "allowed rings to spawn");//Allowed spawn ring (center is ring 0)
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
                        string id = string.Empty;

                        SgtLogger.l(lonePOI, "LonePOI");
                        GameObject gameObject = Util.KInstantiate(Assets.GetPrefab((Tag)lonePOI));

                        ClusterGridEntity component1 = gameObject.GetComponent<ClusterGridEntity>();
                        if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                        {
                            animName = component1.AnimConfigs.First().initialAnim;
                            animFile = component1.AnimConfigs.First().animFile;
                            name = component1.Name;
                            id = component1.PrefabID().ToString();
                        }

                        id = id.Replace("ArtifactSpacePOI_", string.Empty);
                        id = id.Replace("HarvestableSpacePOI_", string.Empty);

                        if (animName == "cloud")
                            animName = "carbon_asteroid_field";

                        if (animName == "closed_loop")
                            animName = "ui";


                        bool moreThanOne = lonePOI != "ArtifactSpacePOI_RussellsTeapot" && lonePOI != "TemporalTear";

                        Sprite POIsprite = Def.GetUISpriteFromMultiObjectAnim(animFile, animName, true);

                        UnityEngine.Object.Destroy(gameObject);

                        var poi = new StarmapItem(lonePOI, StarmapItemCategory.POI, POIsprite);

                        SpaceMapPOIPlacement placement = new SpaceMapPOIPlacement();
                        placement.pois = new List<string> { lonePOI };
                        placement.numToSpawn = 1;
                        placement.allowedRings = poi_tab.allowedRings;
                        placement.avoidClumping = poi_tab.avoidClumping;
                        poi = poi.MakeItemPOI(id, placement, moreThanOne);

                        if (!PlanetsAndPOIs.ContainsKey(lonePOI))
                        {
                            PlanetsAndPOIs.Add(lonePOI, poi);
                        }
                        //else
                        //{
                        //    var toEdit = PlanetsAndPOIs[lonePOI];
                        //    toEdit.SetInnerRing(Math.Min(toEdit.minRing, placement.allowedRings.min));
                        //    toEdit.SetOuterRing(Math.Min(toEdit.maxRing, placement.allowedRings.max));
                        //    toEdit.InstancesToSpawn++;
                        //}
                    }

                    ///Requires different handling
                    //PredefinedPlacementDataPOI[poi.]
                    //SgtLogger.l("", "POI:");
                    foreach (var poi2 in poi_tab.pois)
                    {
                        //SgtLogger.l(poi2, "Poi in list:");
                    }
                    //SgtLogger.l(poi.avoidClumping.ToString(), "avoid clumping");
                    //SgtLogger.l(poi.canSpawnDuplicates.ToString(), "Allow Duplicates");
                    //SgtLogger.l(poi.allowedRings.ToString(), "Allowed Rings");
                    //SgtLogger.l(poi.numToSpawn.ToString(), "Number to spawn");
                }
            }

        }


        static Dictionary<string, StarmapItem> PlanetsAndPOIs = null;
        public static Dictionary<string, StarmapItem> PlanetoidDict()
        {
            if (PlanetsAndPOIs == null)
            {
                PlanetsAndPOIs = new Dictionary<string, StarmapItem>();

                foreach (var World in SettingsCache.worlds.worldCache)
                {
                    StarmapItemCategory category = StarmapItemCategory.Outer;
                    //SgtLogger.l(World.Key + "; " + World.Value.ToString());
                    ProcGen.World world = World.Value;

                    if ((int)world.skip >= 99)
                        continue;

                    //SgtLogger.l(                   world.startingBaseTemplate, "START TEMPLATE");
                    if (World.Key.Contains("expansion1"))
                    {
                        if (world.startingBaseTemplate != null)
                        {
                            if (world.startingBaseTemplate.Contains("warpworld"))
                            {
                                category = StarmapItemCategory.Warp;
                            }
                            else if (world.startingBaseTemplate.Contains("Base"))
                            {
                                category = StarmapItemCategory.Starter;
                            }


                        }

                        Sprite sprite = ColonyDestinationAsteroidBeltData.GetUISprite(World.Value.asteroidIcon);

                        PlanetsAndPOIs[World.Key] = (new StarmapItem
                        (
                        World.Key,
                        category,
                        sprite
                        ).MakeItemPlanet(world));

                    }

                }

                PopulatePredefinedClusterPlacements();
            }
            return PlanetsAndPOIs;
        }
    }
}
