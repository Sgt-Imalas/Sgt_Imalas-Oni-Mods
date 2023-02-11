using HarmonyLib;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    internal class HabitatInteriorRadiationPatches
    {

        [HarmonyPatch(typeof(RocketInteriorLiquidInputPortConfig))]
        [HarmonyPatch(nameof(RocketInteriorLiquidInputPortConfig.CreateBuildingDef))]
        public static class InputPortIsFoundation
        {
            static void Postfix(ref BuildingDef __result)
            {
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    BuildingTemplates.CreateFoundationTileDef(__result);
                }
            }
        }
        [HarmonyPatch(typeof(RocketInteriorLiquidOutputPortConfig))]
        [HarmonyPatch(nameof(RocketInteriorLiquidOutputPortConfig.CreateBuildingDef))]
        public static class OutputPortIsFoundation
        {
            static void Postfix(ref BuildingDef __result)
            {
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    BuildingTemplates.CreateFoundationTileDef(__result);
                }
            }
        }

        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.Sim4000ms))]
        public static class AdjustHabitats
        {
            public static void Postfix(Clustercraft __instance)
            {
                if (Config.Instance.HabitatInteriorRadiation && !(__instance is SpaceStation))
                {
                    KPrefabID prefab = null;
                    ClustercraftExteriorDoor door = null;
                    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)__instance.m_moduleInterface.ClusterModules)
                    {
                        if (clusterModule.Get().TryGetComponent(out door))
                        {
                            door.TryGetComponent(out prefab);
                            break;
                        }
                    }
                    if (door == null)
                        return;

                    var world = door.GetMyWorld();
                    var target = door.targetDoor;
                    if (target == null)
                        return;
                    var interiorWorld = door.targetDoor.GetMyWorld();
                    //if(prefab!= null)
                    //{
                    //    if(prefab.PrefabID() == HabitatModulePlatedNoseconeLargeConfig.ID) 
                    //    {
                    //        interiorWorld.cosmicRadiation = 0;
                    //        return;
                    //    }
                    //}

                    if (__instance.status != Clustercraft.CraftStatus.InFlight)
                    {

                        int cell = Grid.PosToCell(door);
                        if (Grid.ExposedToSunlight[cell] > 0)
                        {
                            interiorWorld.sunlight = world.sunlight;
                        }
                        else
                        {
                            interiorWorld.sunlight = 0;
                        }

                        if (Grid.Radiation[cell] > 0)
                        {
                            interiorWorld.cosmicRadiation = (int)Grid.Radiation[cell];
                        }
                        else
                        {
                            interiorWorld.cosmicRadiation = 0;
                        }
                    }
                    else
                    {
                        interiorWorld.sunlight = FIXEDTRAITS.SUNLIGHT.DEFAULT_VALUE;
                        interiorWorld.cosmicRadiation = SpaceStationManager.SpaceRadiationRocket(__instance.Location);
                    }

                }
            }
        }
    }
}
