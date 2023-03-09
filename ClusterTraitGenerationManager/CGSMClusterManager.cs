using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.CLUSTERMAP;

namespace ClusterTraitGenerationManager
{
    internal class CGSMClusterManager
    {
        static GameObject Screen = null;
        public static void InstantiateClusterSelectionView(System.Action onClose = null)
        {
            if (Screen == null)
            {
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
                LockerNavigator.Instance.PushScreen(newScreen, onClose);
            }
            else
            {
                LockerNavigator.Instance.PushScreen(Screen, onClose);
            }

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


        static List<PlanetoidGridItem> PlanetsAndPOIs = null;
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

                foreach(var ClusterLayout in SettingsCache.clusterLayouts.clusterCache)
                {
                    SgtLogger.l(ClusterLayout.Key + ":");
                    foreach(var planet in ClusterLayout.Value.worldPlacements)
                    {
                        SgtLogger.l("", "PLANET:");
                        SgtLogger.l(planet.world,"FilePath"); //Path , aka id
                        //SgtLogger.l(planet.x.ToString()); //muda
                        //SgtLogger.l(planet.y.ToString());//muda
                        //SgtLogger.l(planet.width.ToString());//muda
                        //SgtLogger.l(planet.height.ToString());//muda
                        SgtLogger.l(planet.locationType.ToString(),"LocationType"); //startWorld / inner cluster / cluster
                        SgtLogger.l(planet.startWorld.ToString(),"IsStartWorld"); //isStartWorld?
                        SgtLogger.l(planet.buffer.ToString(),"min distance to others"); //min distance to other planets
                        SgtLogger.l(planet.allowedRings.ToString(),"allowed rings to spawn");//Allowed spawn ring (center is ring 0)

                    }
                    if (ClusterLayout.Value.poiPlacements == null)
                        continue;

                    foreach (var poi in ClusterLayout.Value.poiPlacements)
                    {
                        SgtLogger.l("", "POI:");
                        foreach (var poi2 in poi.pois)
                        {
                            SgtLogger.l(poi2, "Poi in list:");
                        }
                        SgtLogger.l(poi.avoidClumping.ToString(),"avoid clumping");
                        SgtLogger.l(poi.canSpawnDuplicates.ToString(),"Allow Duplicates");
                        SgtLogger.l(poi.allowedRings.ToString(),"Allowed Rings");
                        SgtLogger.l(poi.numToSpawn.ToString(),"Number to spawn");

                    }
                }
            }

            return PlanetsAndPOIs;
        }
    }
}
