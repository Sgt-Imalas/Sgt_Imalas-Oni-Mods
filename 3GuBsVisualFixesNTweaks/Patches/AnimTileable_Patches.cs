using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class AnimTileable_Patches
    {

        [HarmonyPatch(typeof(AnimTileable), nameof(AnimTileable.UpdateEndCaps))]
        public class AnimTileable_UpdateEndCaps_Patch
        {
            public static void Postfix(AnimTileable __instance)
            {
                if(__instance?.gameObject?.TryGetComponent<LadderAnimTileable>(out var lat)??false)
                {
                    lat.UpdateEndCaps();
                }
                else
                {
                    //SgtLogger.l(__instance.GetProperName() + " <- no cap cmp");
                }
            }
        }
    }
}
