using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace DuperyFixed.MinionImages
{
    internal class MinionImagePatches
    {
        [HarmonyPatch(typeof(Personality))]
        [HarmonyPatch(nameof(Personality.GetMiniIcon))]
        public class ReplaceMissingDreamIcons
        {
            [HarmonyPriority(Priority.HigherThanNormal)]
            public static void Postfix(Personality __instance, ref Sprite __result)
            {
                if (__result == null)
                {
                    __result = ModAssets.BuildDynamicDreamImage(__instance); 
                }
            }
        }
    }
}
