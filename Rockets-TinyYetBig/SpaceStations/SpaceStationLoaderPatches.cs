using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.SpaceStations
{
    public class SpaceStationLoaderPatches
    {
        [HarmonyPatch(typeof(LaunchPadMaterialDistributor.Instance), "GetLandedRocketFromPad")]
        public static class CustomSideScreenPatch_SatelliteCarrier
        {
            public static bool Prefix(LaunchPadMaterialDistributor.Instance __instance,ref RocketModuleCluster __result)
            {
               if(__instance.gameObject.TryGetComponent<DockingDoor>(out var door))
                {
                    __result = door.GetDockedCraftModuleInterface().ClusterModules.First().Get();
                    return false;
                }
               return true;
            }
        }
    }
}
