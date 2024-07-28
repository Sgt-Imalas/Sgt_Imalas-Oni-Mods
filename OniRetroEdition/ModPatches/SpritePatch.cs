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


        public static string Gas_Input_Connected = "RetroOni_Gas_Input_Connected";
        public static string Gas_Input_Disconnected = "RetroOni_Gas_Input_Disconnected";
        public static string Liquid_Input_Connected = "RetroOni_Liquid_Input_Connected";
        public static string Liquid_Input_Disconnected = "RetroOni_Liquid_Input_Disconnected";


        public static string DigHex = "iconHex01";
        public static string DigHardness = "inspectorUI_hardness_icon";
        public static string DigMass = "inspectorUI_mass_icon_orange";


        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            [HarmonyPriority(Priority.LowerThanNormal)] 
            public static void Prefix(Assets __instance)
            {
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.SlurpIcon);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.SlurpActionIcon);

                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.Gas_Input_Connected);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.Gas_Input_Disconnected);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.Liquid_Input_Connected);
                InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.Liquid_Input_Disconnected);

                //InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.DigHex);
                //InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.DigHardness);
                //InjectionMethods.AddSpriteToAssets(__instance, SpritePatch.DigMass);

            }
        }
    }
}
