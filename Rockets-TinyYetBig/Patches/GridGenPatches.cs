using HarmonyLib;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Debug.Log("RocketryExpanded: Increased free grid space allocation");
                }
            }
        }
    }
}
