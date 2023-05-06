using HarmonyLib;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class GridGenPatches
    {
        [HarmonyPatch(typeof(BestFit))]
        [HarmonyPatch(nameof(BestFit.BestFitWorlds))]
        public static class IncreaseFreeGridSpace
        {
            public static void Postfix(ref Vector2I __result)
            {
                if (DlcManager.FeatureClusterSpaceEnabled())
                {
                    __result.x += 272;
                    __result.y = Math.Max(__result.y, 272);
                    SgtLogger.debuglog("RocketryExpanded: Increased free grid space allocation");
                }
            }
        }
        //[HarmonyPatch(typeof(Cluster))]
        //[HarmonyPatch(nameof(Cluster.Save))]
        //public static class IncreaseFreeGridSpaceOnSaving
        //{
        //    public static void Postfix()
        //    {
        //        if (DlcManager.FeatureClusterSpaceEnabled())
        //        {
        //            BestFit.GetGridOffset(ClusterManager.Instance.WorldContainers, size, out offset);
        //        }
        //    }
        //}

        /// <summary>
        /// fixes a crash that can happen here
        /// </summary>
        [HarmonyPatch(typeof(ClusterMapMeteorShower.Def))]
        [HarmonyPatch(nameof(ClusterMapMeteorShower.Def.GetDescriptors))]
        public static class IncreaseFreeGridSpaceOnSaving
        {
            public static bool Prefix(ClusterMapMeteorShower.Def __instance, ref List<Descriptor> __result)
            {
                if(__instance.eventID==string.Empty|| __instance.eventID == null) 
                {
                    __result = new List<Descriptor>();
                    return false;
                }
                return true;
            }
        }
    }
}
