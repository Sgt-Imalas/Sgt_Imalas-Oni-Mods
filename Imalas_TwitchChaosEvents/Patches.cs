using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
using Imalas_TwitchChaosEvents.Meteors;
using Klei.AI;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Imalas_TwitchChaosEvents.ModAssets;

namespace Imalas_TwitchChaosEvents
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
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
        public class SaveGamePatch
        {

            [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
            public class SaveGame_OnPrefabInit_Patch
            {
                public static void Postfix(SaveGame __instance)
                {
                    __instance.gameObject.AddOrGet<ChaosTwitch_SaveGameStorage>();
                }
            }
        }
        [HarmonyPatch(typeof(ComplexFabricatorSideScreen), "AnyRecipeRequirementsDiscovered")]
        public class ComplexFabricatorSideScreen_AnyRecipeRequirementsDiscovered_Patch
        {
            public static void Postfix(ComplexRecipe recipe, ref bool __result)
            {
                if (ChaosTwitch_SaveGameStorage.Instance == null)
                    return;

                if (recipe.id == TacoConfig.ID)
                    __result = ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe;
            }
        }

        public static class Rainbow_Liquid_Patch
        {

            // Patch Game.StepTheSim (yikes!) to edit the brightness of the native the liquid texture returned by the sim.
            // Editing the native memory this soon is necessary for compatibility with FastTrack by Peter Han. (Which changes how PropertyTextures works)
            [HarmonyPatch(typeof(Game), "StepTheSim")]
            [HarmonyPriority(Priority.High)]
            public static class Game_Update_Patch
            {
                static Dictionary<int, System.Tuple<byte, byte, byte>> ColourValues;
                static ushort CreeperIDx = 0;
                static int time = 0;
                static int colourStep = 0;
                const int looptime = 120;

                [HarmonyPriority(Priority.High)]
                public static void Prefix()
                {
                    if (CreeperIDx == 0)
                    {
                        var element = ElementLoader.FindElementByHash(ModElements.Creeper.SimHash);
                        if (element != null)
                        {
                            CreeperIDx = element.idx;
                        }
                        ColourValues = new Dictionary<int, System.Tuple<byte, byte, byte>>();

                        for (int i = 0; i < looptime; ++i)
                            AddColourEntry(i);
                    }
                }

                static void AddColourEntry(int i)
                {
                    var stepColor = Color.HSVToRGB((float)i / (float)looptime, 1, 1);
                    byte rByte = (byte)(stepColor.r * 255f), gByte = (byte)(stepColor.g * 255f), bByte = (byte)(stepColor.b * 255f);
                    ColourValues[i] = System.Tuple.Create(rByte, gByte, bByte);
                }

                public static Color CurrentColor => Color.HSVToRGB(time / looptime, 1, 1);

                [HarmonyPriority(Priority.High)]
                public static void Postfix()
                {
                    if (SpeedControlScreen.Instance == null ||
                        false
                        )
                        return;
                    if (!SpeedControlScreen.Instance.IsPaused)
                        time++;
                    IntPtr pixelsPtr = PropertyTextures.externalLiquidTex;

                    //Parallel.For(0, Grid.CellCount, (i) => ProcessPixel(pixelsPtr, i, rByte, gByte,bByte));
                    Parallel.For(0, Grid.CellCount, (i) => ProcessPixelbyTime(pixelsPtr, i, time));
                    time %= looptime;
                }
                private static unsafe void ProcessPixelbyTime(IntPtr pixelsPtr, int i, int time)
                {
                    if (!Grid.IsActiveWorld(i) || !Grid.IsLiquid(i)) return;

                    var colour = ColourValues[GetCurrentColour(i, time)];

                    byte* pixel = (byte*)pixelsPtr.ToPointer() + (i * 4);
                    pixel[0] = colour.Item1;
                    pixel[1] = colour.Item2;
                    pixel[2] = colour.Item3;
                    // pixel[3] = (byte)10;
                }
                static int GetCurrentColour(int cell, int time)
                {
                    int Y = Grid.CellRow(cell)
                        //,                        X = Grid.CellColumn(cell)
                        ;

                    return (Y + time) % looptime;
                }


                private static unsafe void ProcessPixel(IntPtr pixelsPtr, int i, byte r, byte g, byte b)
                {
                    if (!Grid.IsActiveWorld(i) || Grid.elementIdx[i] != CreeperIDx || CreeperIDx == 0) return;



                    byte* pixel = (byte*)pixelsPtr.ToPointer() + (i * 4);
                    pixel[0] = r;
                    pixel[1] = g;
                    pixel[2] = b;
                    // pixel[3] = (byte)10;
                }
            }
        }
    }
}
