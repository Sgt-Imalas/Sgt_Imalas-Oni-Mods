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
    internal class HabitatInteriorRadiation
    {

        //[HarmonyPatch(typeof(GameUtil))]
        //[HarmonyPatch(nameof(GameUtil.GetRadiationAbsorptionPercentage))]
        //[HarmonyPatch(new Type[] { typeof(int)})]
        //public static class LeadTilesInRocketNoRads
        //{
        //    static bool Prefix(int cell, ref float __result)
        //    {
        //        if (Grid.IsValidCell(cell)
        //            && SpaceStationManager.WorldIsRocketInterior(Grid.WorldIdx[cell]) 
        //            && Grid.Element[cell].radiationAbsorptionFactor>0.7f)
        //        {
        //            __result = 1f;
        //            return false;
        //        }
        //        return true;
        //    }
        //}


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
                    if (door == null) return;

                    var world = door.GetMyWorld();
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
                        interiorWorld.cosmicRadiation = FIXEDTRAITS.COSMICRADIATION.BASELINE;
                    }

                }
            }
        }
    }
}
