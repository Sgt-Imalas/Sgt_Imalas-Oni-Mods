using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ProcGen.WorldPlacement;
using static STRINGS.UI.CLUSTERMAP;
using static STRINGS.UI.FRONTEND;

namespace ClusterTraitGenerationManager
{
    internal class CGSMClusterManager
    {
        static GameObject Screen = null;

        public static ColonyDestinationSelectScreen selectScreen;

        public static string PrefabTemplate = string.Empty;

        public static void InstantiateClusterSelectionView(System.Action onClose = null)
        {
            if (Screen == null)
            {
                var defaultCluster = PrefabTemplate != string.Empty ? PrefabTemplate : "expansion1::clusters/SandstoneStartCluster";
                CreateCustomClusterFrom(defaultCluster);

                LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen);
                LockerNavigator.Instance.PopScreen();

                var window = Util.KInstantiateUI(LockerNavigator.Instance.kleiInventoryScreen.gameObject);
                window.SetActive(false);
                var copy = window.transform;
                UnityEngine.Object.Destroy(window);
                var newScreen = Util.KInstantiateUI(copy.gameObject, Global.Instance.globalCanvas);
                newScreen.name = "ClusterSelectionView";
                newScreen.AddComponent(typeof(FeatureSelectionScreen));
                Screen = newScreen;
                onClose += ()=>AddCustomCluster();
                LockerNavigator.Instance.PushScreen(newScreen, onClose);
            }
            else
            {
                SgtLogger.l("not new", "SCREEN");
                LockerNavigator.Instance.PushScreen(Screen, onClose);
            }
            Screen.GetComponent<FeatureSelectionScreen>().RefreshView();

        }

        public struct PlanetoidGridItem
        {
            public string id;
            public PlanetCategory category;
            public Sprite planetSprite;
            public ProcGen.World world;
            public int maxAllowed = 1;
            public PlanetoidGridItem(string id, PlanetCategory category, Sprite sprite = null, ProcGen.World world = null, int allowed = 1)
            {
                this.id = id;
                this.category = category;
                this.world = world;
                this.planetSprite = sprite;
                this.maxAllowed = allowed;
            }
        }
        public enum PlanetCategory
        {
            Starter,
            Teleport,
            Outer,
            POI,
            Derelict
        }

        public const string ClusterID = "CMGM";
        public static ClusterLayout CustomLayout;

        public static void AddCustomCluster()
        {
            SettingsCache.clusterLayouts.clusterCache[ClusterID] = CustomLayout;
            foreach(var key in SettingsCache.clusterLayouts.clusterCache.Keys)
            {
                SgtLogger.l(key);
            }

            selectScreen.newGameSettings.SetSetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout, ClusterID);
            selectScreen.newGameSettings.Refresh();
            foreach (var key in selectScreen.newGameSettings.settings.CurrentQualityLevelsBySetting)
            {
                SgtLogger.l(key.Key +"; "+key.Value) ;
            }
        }


        public static void CreateCustomClusterFrom(string clusterID)
        {
            SgtLogger.log(clusterID);
            CustomLayout = new ClusterLayout();

            var Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);
            SgtLogger.log(Reference.ToString());
            CustomLayout.filePath = "";
            CustomLayout.name = clusterID;
            CustomLayout.description = "Custom";
            CustomLayout.worldPlacements = new List<WorldPlacement>();
            foreach (var world in Reference.worldPlacements)
            {
                CustomLayout.worldPlacements.Add(world);
            }
            CustomLayout.poiPlacements = new List<SpaceMapPOIPlacement>();
            foreach (var poi in Reference.poiPlacements)
            {
                CustomLayout.poiPlacements.Add(poi);
            }
            CustomLayout.numRings = Reference.numRings;
            CustomLayout.difficulty = Reference.difficulty;
            CustomLayout.requiredDlcId= Reference.requiredDlcId;
            CustomLayout.forbiddenDlcId= Reference.forbiddenDlcId;
            CustomLayout.startWorldIndex = Reference.startWorldIndex;
        }

        public static void TogglePlanetoid(PlanetoidGridItem item)
        {
            var existing = CustomLayout.worldPlacements.Find(planet => planet.world == item.world.filePath);
            if(existing != null)
            {
                CustomLayout.worldPlacements.Remove(existing);
            }
            else
            {
                var newItem = new WorldPlacement();
                newItem.world = item.world.filePath;
                newItem.startWorld = item.category == PlanetCategory.Starter ? true : false;
                newItem.locationType = item.category == PlanetCategory.Starter ? LocationType.Startworld : LocationType.Cluster;
                CustomLayout.worldPlacements.Add(newItem);
            }
        }


        static List<PlanetoidGridItem> PlanetsAndPOIs = null;

        static Dictionary<string,List<string>> PredefinedClusters = null;


        public static List<string> GetActivePlanetsCluster()
        {
            PopulateClusterDict();
            var planetPaths = new List<string>();
            foreach(var planet in CustomLayout.worldPlacements)
            {
                planetPaths.Add(planet.world);
            }
            return planetPaths;

            if (PrefabTemplate==null)
                return PredefinedClusters.FirstOrDefault().Value;
            else
            {
                if (PredefinedClusters.TryGetValue(PrefabTemplate, out var list))
                {
                    return list;
                }
                else return PredefinedClusters.FirstOrDefault().Value;
            }
        }

        public static void PopulateClusterDict()
        {
            if (PredefinedClusters == null)
            {
                PredefinedClusters = new Dictionary<string, List<string>>();

                foreach (var ClusterLayout in SettingsCache.clusterLayouts.clusterCache)
                {
                    if (!ClusterLayout.Key.Contains("expansion1"))
                    {
                        continue;
                    }
                        SgtLogger.l(ClusterLayout.Key + ":");
                    var planetList = new List<string>();

                    foreach (var planet in ClusterLayout.Value.worldPlacements)
                    {
                        SgtLogger.l("", "PLANET:");
                        SgtLogger.l(planet.world, "FilePath"); //Path , aka id
                        //SgtLogger.l(planet.x.ToString()); //muda
                        //SgtLogger.l(planet.y.ToString());//muda
                        //SgtLogger.l(planet.width.ToString());//muda
                        //SgtLogger.l(planet.height.ToString());//muda
                        SgtLogger.l(planet.locationType.ToString(), "LocationType"); //startWorld / inner cluster / cluster
                        SgtLogger.l(planet.startWorld.ToString(), "IsStartWorld"); //isStartWorld?
                        SgtLogger.l(planet.buffer.ToString(), "min distance to others"); //min distance to other planets
                        SgtLogger.l(planet.allowedRings.ToString(), "allowed rings to spawn");//Allowed spawn ring (center is ring 0)

                        planetList.Add(planet.world);
                    }
                    PredefinedClusters[ClusterLayout.Key] = planetList;

                    if (ClusterLayout.Value.poiPlacements == null)
                        continue;

                    foreach (var poi in ClusterLayout.Value.poiPlacements)
                    {
                        SgtLogger.l("", "POI:");
                        foreach (var poi2 in poi.pois)
                        {
                            SgtLogger.l(poi2, "Poi in list:");
                        }
                        SgtLogger.l(poi.avoidClumping.ToString(), "avoid clumping");
                        SgtLogger.l(poi.canSpawnDuplicates.ToString(), "Allow Duplicates");
                        SgtLogger.l(poi.allowedRings.ToString(), "Allowed Rings");
                        SgtLogger.l(poi.numToSpawn.ToString(), "Number to spawn");

                    }
                }
            }

        }

        public static List<PlanetoidGridItem> PopulatePlanetoidDict()
        {
            if (PlanetsAndPOIs == null)
            {
                PlanetsAndPOIs = new List<PlanetoidGridItem>();

                foreach (var World in SettingsCache.worlds.worldCache)
                {
                    PlanetCategory category = PlanetCategory.Outer;
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
                                category = PlanetCategory.Teleport;
                            }
                            else if (world.startingBaseTemplate.Contains("Base"))
                            {
                                category = PlanetCategory.Starter;
                            }



                        }

                        Sprite sprite = ColonyDestinationAsteroidBeltData.GetUISprite(World.Value.asteroidIcon);

                        PlanetsAndPOIs.Add(new PlanetoidGridItem
                        (
                        World.Key,
                        category,
                        sprite,
                        World.Value
                        ));
                    }

                }
            }

            return PlanetsAndPOIs;
        }
    }
}
