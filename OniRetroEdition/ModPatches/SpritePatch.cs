using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
    internal class SpritePatch
    {
        public static string SlurpIcon = "RetroOni_SlurpIcon";
        public static string SlurpActionIcon = "RetroOni_icon_action_slurp";


        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            [HarmonyPriority(Priority.LowerThanNormal)] 
            public static void Prefix(Assets __instance)
            {
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.SlurpIcon);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.SlurpActionIcon);
            }
        }
    }
}
