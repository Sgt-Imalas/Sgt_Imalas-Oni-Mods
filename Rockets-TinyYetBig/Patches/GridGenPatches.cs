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

        /// <summary>
        /// fixes a vanilla crash that can happen when this has eventID==null
        /// </summary>
        [HarmonyPatch(typeof(ClusterMapMeteorShower.Def))]
        [HarmonyPatch(nameof(ClusterMapMeteorShower.Def.GetDescriptors))]
        public static class FixesVanillaCrashOnPlanetSelection
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

        /// <summary>
        /// Fixes a bug with the cleanup method that would cause invisible solid tiles in the next world at that location
        /// </summary>
        [HarmonyPatch(typeof(Grid))]
        [HarmonyPatch(nameof(Grid.FreeGridSpace))]
        public static class CleanupOfWorldsFix
        {
            internal static void Prefix(Vector2I size, Vector2I offset)
            {
                int cell = Grid.XYToCell(offset.x, offset.y), width = size.x, stride =
                    Grid.WidthInCells - width;
                for (int y = size.y; y > 0; y--)
                {
                    for (int x = width; x > 0; x--)
                    {
                        if (Grid.IsValidCell(cell))
                            SimMessages.ReplaceElement(cell, SimHashes.Vacuum, null, 0.0f);
                        cell++;
                    }
                    cell += stride;
                }
            }
        }
    }
}
