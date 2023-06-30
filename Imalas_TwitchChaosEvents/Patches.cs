using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
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

        public static class Rainbow_Liquid_Patch
        {

            // Patch Game.StepTheSim (yikes!) to edit the brightness of the native the liquid texture returned by the sim.
            // Editing the native memory this soon is necessary for compatibility with FastTrack by Peter Han. (Which changes how PropertyTextures works)
            [HarmonyPatch(typeof(Game), "StepTheSim")]
            [HarmonyPriority(Priority.High)]
            public static class Game_Update_Patch
            {
                static ushort CreeperIDx=0;
                static float time = 0;
                const float looptime = 180f;

                [HarmonyPriority(Priority.High)]
                public static void Prefix()
                {
                    if(CreeperIDx == 0)
                    {
                        var element = ElementLoader.FindElementByHash(ModElements.Creeper.SimHash);
                        if(element != null)
                        {
                            CreeperIDx = element.idx;                            
                        }
                    }
                }

                public static Color CurrentColor => Color.HSVToRGB(time / looptime, 1, 1);

                [HarmonyPriority(Priority.High)]
                public static void Postfix()
                {
                    time++;
                    IntPtr pixelsPtr = PropertyTextures.externalLiquidTex;
                    var currentcolor = CurrentColor;
                    byte rByte = (byte)(CurrentColor.r * 255f), gByte = (byte)(currentcolor.g * 255f), bByte = (byte)(currentcolor.b * 255f);

                    Parallel.For(0, Grid.CellCount, (i) => ProcessPixel(pixelsPtr, i, rByte, gByte,bByte));
                    time %= looptime;
                }

                private static unsafe void ProcessPixel(IntPtr pixelsPtr, int i, byte r, byte g, byte b)
                {
                    if (!Grid.IsActiveWorld(i) || Grid.elementIdx[i]!= CreeperIDx || CreeperIDx == 0) return;

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
