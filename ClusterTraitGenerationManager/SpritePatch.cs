using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ClusterTraitGenerationManager.CGSMClusterManager;

namespace ClusterTraitGenerationManager
{
    internal class SpritePatch
    {
        public static string randomStarter = "CGM_random_starter";
        public static string randomWarp = "CGM_random_warp";
        public static string randomOuter = "CGM_random_outer";
        public static string randomPOI = "CGM_random_poi";
        public static string noneSelected = "CGM_none_selected";
        public static string missingHoleTexture = "SpaceHole";
        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
            {
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.randomStarter);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.randomWarp);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.randomOuter);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.randomPOI);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.noneSelected);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingHoleTexture);
            }
        }
    }
}
