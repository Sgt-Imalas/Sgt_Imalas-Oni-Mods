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
        /// <summary>
        /// More free grid space at world gen
        /// </summary>
        [HarmonyPatch(typeof(BestFit))]
        [HarmonyPatch(nameof(BestFit.BestFitWorlds))]
        public static class IncreaseFreeGridSpace
        {
            public static void Postfix(ref Vector2I __result)
            {
                if (DlcManager.FeatureClusterSpaceEnabled())
                {
                    __result.x += 312;
                    __result.y = Math.Max(__result.y, 324);
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

        
    }
}
