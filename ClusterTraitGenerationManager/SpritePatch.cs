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
        public static string randomTraitsTraitIcon = "CGMRandomTraits";
        //Empty Worlds
        public static string missingHoleTexture = "SpaceHole";
        //SolarSystemWorlds
        public static string missingMoltenCoreTexture = "IronCore";
        //Geoapocalypse
        public static string missingGeoApocalypseTexture = "GeoApocalypse";
        //Improved World Traits
        public static string missingTexture_GeoHyperActive = "GeoHyperActive";
        public static string missingTexture_GeoInactive = "GeoInactive";
        public static string missingTexture_GasPoor = "GasPoor";
        public static string missingTexture_GasRich = "GasRich";
        public static string missingTexture_LiquidPoor = "LiquidPoor";
        public static string missingTexture_LiquidRich = "LiquidRich";
        public static string missingTexture_MineralPoor = "MineralPoor";
        public static string missingTexture_MineralRich = "MineralRich";
        public static string missingTexture_PowerProducers = "PowerProducers";
        public static string missingTexture_ResourcePoor = "ResourcePoor";
        public static string missingTexture_ResourceRich = "ResourceRich";
        public static string missingTexture_WaterProducers = "WaterProducers";
        public static string missingTexture_ResourceProducers = "ResourceProducers";
        public static string missingTexture_CryoVolcanoes = "CryoVolcanoes";
        public static string missingTexture_ = "toReplace";


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
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingMoltenCoreTexture);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.randomTraitsTraitIcon);

                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingGeoApocalypseTexture);

                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_GeoHyperActive);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_GeoInactive);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_GasPoor);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_GasRich);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_LiquidPoor);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_LiquidRich);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_MineralPoor);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_MineralRich);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_PowerProducers);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_ResourcePoor);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_ResourceRich);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_WaterProducers);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_ResourceProducers);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.missingTexture_CryoVolcanoes);
            }
        }
    }
}
