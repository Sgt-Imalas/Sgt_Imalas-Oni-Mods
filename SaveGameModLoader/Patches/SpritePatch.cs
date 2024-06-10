using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SaveGameModLoader.Patches
{
    internal class SpritePatch
    {
        public static string pinSymbol = "MPM_Pin";
        public static string delSymbol = "MPM_DEL";
        public static string copySymbol = "MPM_CLIPBOARD";
        public static string flagSymbol = "MPM_Flag";
        public static string tagSymbol = "MPM_TAG";


        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
            {
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.pinSymbol);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.delSymbol);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.copySymbol);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.flagSymbol);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.tagSymbol);
            }
        }
    }
}
