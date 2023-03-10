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
                if(CustomCluster == null)
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

        public enum SpawnChance
        {
            None = 0,
            Perhaps = 50,
            Guaranteed = 100,
        }
        public enum StarmapItemCategory
        {
            Starter,
            Warp,
            Outer,
            POI
        }

        public struct StarmapItem
        {
            public string id;
            public StarmapItemCategory category;

            public Sprite planetSprite;
            public ProcGen.World world;
            public WorldPlacement placement;
            public SpaceMapPOIPlacement placementPOI;


            public int maxAllowed = 1;
            public int minRing = 0;
            public int maxRing = 0;
            public SpawnChance SpawnChance = SpawnChance.Guaranteed;

            public StarmapItem(string id, StarmapItemCategory category, Sprite sprite, int allowed, SpawnChance _SpawnChance)
            {
                this.id = id;
                this.category = category;
                this.planetSprite = sprite;
                this.maxAllowed = allowed;
                SpawnChance = _SpawnChance;
            }
            public StarmapItem MakeItemPlanet(ProcGen.World world, WorldPlacement placement = null)
            {
                this.placement = placement;
                this.world = world;
                return this;
            }
            public StarmapItem AddItemWorldPlacement(WorldPlacement placement)
            {
                this.placement = placement;
                return this;
            }
            public StarmapItem MakeItemPOI(SpaceMapPOIPlacement placement2)
            {
                this.placementPOI = placement2;
                return this;
            }

            public StarmapItem(string id, StarmapItemCategory category, Sprite sprite = null)
            {
                this.id = id;
                this.category = category;
                this.planetSprite = sprite;
            }
        }


        public const string CustomClusterID = "CMGM";
        public static ClusterLayout GeneratedLayout => GenerateClusterLayoutFromCustomData(CustomCluster);
        public static CustomClusterData CustomCluster;

        public static void AddCustomCluster()
        {
            SettingsCache.clusterLayouts.clusterCache[CustomClusterID] = GeneratedLayout;
            foreach (var key in SettingsCache.clusterLayouts.clusterCache.Keys)
            {
                SgtLogger.l(key);
            }

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

        public static ClusterLayout GenerateClusterLayoutFromCustomData(CustomClusterData data)
        {
            var layout = new ClusterLayout();
            GetPredefinedClusters();

            //var Reference = SettingsCache.clusterLayouts.GetClusterData(ClusterID);
            //SgtLogger.log(Reference.ToString());
            GeneratedLayout.filePath = CustomClusterID;
            GeneratedLayout.name = CustomClusterID;
            GeneratedLayout.description = "Custom";
            GeneratedLayout.worldPlacements = new List<WorldPlacement>();


            layout.worldPlacements.Add(data.StarterPlanet.placement);

            layout.worldPlacements.Add(data.WarpPlanet.placement);



            foreach (var world in data.OuterPlanets)
            {
                layout.worldPlacements.Add(world.placement);
            }

            layout.poiPlacements = new List<SpaceMapPOIPlacement>();

            foreach (var poi in data.POIs)
            {
                layout.poiPlacements.Add(poi.placementPOI);
            }

            layout.numRings = data.Rings;
            //layout.difficulty = Reference.difficulty;
            //layout.requiredDlcId = Reference.requiredDlcId;
            //layout.forbiddenDlcId = Reference.forbiddenDlcId;
            layout.startWorldIndex = 0;// Reference.startWorldIndex;
            //CustomLayout.clusterCategory = Reference.clusterCategory;
            return layout;
        }

        public static void CreateCustomClusterFrom(string clusterID)
        {
            GetPredefinedClusters();

           // clusterID = clusterID.Trim();
            CustomCluster = new CustomClusterData();

            SgtLogger.l(clusterID, "ClusterID");
            SgtLogger.l("Contains key: "+SettingsCache.clusterLayouts.clusterCache.ContainsKey(clusterID), "ClusterID");

            foreach (var key in SettingsCache.clusterLayouts.clusterCache)
            {
               SgtLogger.l($"{key.Key}: {key.Value}, isEqual=>{key.Key ==clusterID}","Item in dict");
                    
            };
            ClusterLayout Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);
            if (true)
            {
                foreach (WorldPlacement planetPlacement in Reference.worldPlacements)
                {
                    string planetpath = planetPlacement.world;
                    SgtLogger.l(planetpath, "PlanetPath");
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
            }
            SgtLogger.l(Reference == null ? "REF NULL" : Reference.ToString(), "CLUSTERLAYOUT");
        }

        public static void TogglePlanetoid(StarmapItem item)
        {
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

            if (!CustomCluster.OuterPlanets.Contains(item))
                CustomCluster.OuterPlanets.Add(item);
            else
                CustomCluster.OuterPlanets.Remove(item);

            return;

            //var existing = CustomLayout.worldPlacements.Find(planet => planet.world == item.world.filePath);

            var newItem = new WorldPlacement();
            newItem.world = item.world.filePath;
            newItem.startWorld = item.category == StarmapItemCategory.Starter ? true : false;
            newItem.locationType = item.category == StarmapItemCategory.Starter ? LocationType.Startworld : LocationType.Cluster;
            newItem.allowedRings = new MinMaxI(2, 4);
        }

        public class CustomClusterData
        {
            public int Rings { get; set; }
            public StarmapItem StarterPlanet { get; set; }
            public StarmapItem WarpPlanet { get; set; }
            public List<StarmapItem> OuterPlanets = new List<StarmapItem>();
            public List<StarmapItem> POIs = new List<StarmapItem>();
        }

        static Dictionary<string, StarmapItem> PlanetsAndPOIs = null;

        static Dictionary<string, List<string>> PredefinedClusters = null;


        public static List<string> GetActivePlanetsCluster()
        {
            GetPredefinedClusters();
            var planetPaths = new List<string>();
            planetPaths.Add(CustomCluster.StarterPlanet.id);
            planetPaths.Add(CustomCluster.WarpPlanet.id);

            foreach (var planet in CustomCluster.OuterPlanets)
            {
                planetPaths.Add(planet.id);
            }
            return planetPaths;
        }

        public static Dictionary<string, List<string>> GetPredefinedClusters()
        {
            if (PredefinedClusters == null)
            {
                PlanetoidDict();
                PredefinedClusters = new Dictionary<string, List<string>>();

                foreach (var ClusterLayout in SettingsCache.clusterLayouts.clusterCache)
                {
                    if (!ClusterLayout.Key.Contains("expansion1"))
                    {
                        continue;
                    }
                    SgtLogger.l(ClusterLayout.Key,"PREDEFINEDCLUSTER");
                    var planetList = new List<string>();

                    foreach (var planetPlacement in ClusterLayout.Value.worldPlacements)
                    {
                        //SgtLogger.l("", "PLANET:");
                        //SgtLogger.l(planetPlacement.world, "FilePath");

                        //Path , aka id
                        //SgtLogger.l(planet.x.ToString()); //muda
                        //SgtLogger.l(planet.y.ToString());//muda
                        //SgtLogger.l(planet.width.ToString());//muda
                        //SgtLogger.l(planet.height.ToString());//muda
                        if (PlanetoidDict().ContainsKey(planetPlacement.world))
                        {
                            PlanetoidDict()[planetPlacement.world].AddItemWorldPlacement(planetPlacement);
                        }

                        //SgtLogger.l(planetPlacement.locationType.ToString(), "LocationType"); //startWorld / inner cluster / cluster
                        //SgtLogger.l(planetPlacement.startWorld.ToString(), "IsStartWorld"); //isStartWorld?
                        //SgtLogger.l(planetPlacement.buffer.ToString(), "min distance to others"); //min distance to other planets
                        //SgtLogger.l(planetPlacement.allowedRings.ToString(), "allowed rings to spawn");//Allowed spawn ring (center is ring 0)

                        planetList.Add(planetPlacement.world);
                    }
                    PredefinedClusters[ClusterLayout.Key] = planetList;

                    if (ClusterLayout.Value.poiPlacements == null)
                        continue;

                    foreach (var poi in ClusterLayout.Value.poiPlacements)
                    {
                        //SgtLogger.l("", "POI:");
                        foreach (var poi2 in poi.pois)
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
            return PredefinedClusters;
        }

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
                            if (world.startingBaseTemplate.Contains("warpworld")
                                && world.startingBaseTemplate.Contains("Base")
                                || world.startingBaseTemplate.Contains("onewayteleport")) //baator naming
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
            }

            return PlanetsAndPOIs;
        }
    }
}
