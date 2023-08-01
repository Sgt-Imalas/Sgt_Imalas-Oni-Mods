
using Database;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static BiomeClimates.ModAssets;

namespace BiomeClimates
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(SubworldZoneRenderData))]
        [HarmonyPatch(nameof(SubworldZoneRenderData.OnActiveWorldChanged))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
            }
        }
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
            {
                InjectionMethods.AddSpriteToAssets(__instance, "LS_Exploration_Sat");
                InjectionMethods.AddSpriteToAssets(__instance, "LS_Solar_Sat");
            }
        }

        [HarmonyPatch(typeof(OverlayMenu), "InitializeToggles")]
        public static class OverlayMenu_InitializeToggles_Patch
        {
            /// <summary>
            /// Applied after InitializeToggles runs.
            /// </summary>
            internal static void Postfix(ICollection<KIconToggleMenu.ToggleInfo> ___overlayToggleInfos)
            {
                var constructor = AccessTools.Constructor(
                    AccessTools.Inner(typeof(OverlayMenu), "OverlayToggleInfo"),
                    new[]
                    {
                        typeof(string),
                        typeof(string),
                        typeof(HashedString),
                        typeof(string),
                        typeof(Action),
                        typeof(string),
                        typeof(string),
                    }
                );

                var obj = constructor.Invoke(
                    new object[]
                    {
                        "Biome Overlay",
                        "overlay_biomes",
                        "dux",
                        "",
                        Action.NumActions,
                        "Displays various information about biomes",
                        "Biome Overlay",
                    }
                );
                ___overlayToggleInfos.Add((KIconToggleMenu.ToggleInfo)obj);
            }
        }
    }
}
