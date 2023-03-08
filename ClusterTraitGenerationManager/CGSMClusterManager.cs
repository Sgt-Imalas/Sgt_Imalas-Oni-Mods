using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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
                    SgtLogger.l(World.Key + "; " + World.Value.ToString());
                    ProcGen.World world = World.Value;

                    if ((int)world.skip >= 99)
                        continue;

                    SgtLogger.l(
                    world.startingBaseTemplate, "START TEMPLATE");
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
