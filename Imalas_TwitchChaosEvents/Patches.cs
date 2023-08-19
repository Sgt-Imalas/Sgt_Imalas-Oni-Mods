using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
using Imalas_TwitchChaosEvents.Fire;
using Imalas_TwitchChaosEvents.Meteors;
using Imalas_TwitchChaosEvents.OmegaSawblade;
using Klei.AI;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;
using static Imalas_TwitchChaosEvents.ModAssets;
using static ResearchTypes;

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

        //public class Moped
        //{

        //    [HarmonyPatch(typeof(StringTable), nameof(StringTable.Get))]
        //    [HarmonyPatch(new Type[] { typeof(StringKey) })]
        //    public class Moped2
        //    {
        //        const string moped = "Moped ";
        //        static int mopedCount = moped.Length;
        //        public static void Postfix(StringEntry __result)
        //        {
        //            if (__result !=null && __result.String.Length > 0 && !__result.String.Contains("MISSING"))
        //            {
        //                int count = __result.String.Length;
        //                int fit = Math.Max(1, count / mopedCount);
        //                __result.String = string.Concat(Enumerable.Repeat(moped, fit));
        //            }

        //        }
        //    }
        //}
        public class MoveFogGO
        {

            [HarmonyPatch(typeof(CameraController), nameof(CameraController.ActiveWorldStarWipe), new Type[] { typeof(int), typeof(bool ), typeof(Vector3), typeof(float), typeof(System.Action) })]
            public class MoveFogToNewWorlds
            {
                public static void Postfix(bool useForcePosition, Vector3 forcePosition)
                {
                    if(ModAssets.CurrentFogGO != null)
                    {
                        var pos = CameraController.Instance.baseCamera.transform.GetPosition();
                        pos.z = 40;
                        ModAssets.CurrentFogGO.transform.SetPosition(pos);
                        SgtLogger.l($"Moved Fog GO to new world pos; {pos} -> {ModAssets.CurrentFogGO.transform.position}");
                    }
                }
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
                    //FireUpdater.instance = __instance.gameObject.AddOrGet<FireUpdater>();
                    FireManager.Instance = __instance.gameObject.AddOrGet<FireManager>();
                }
            }

            //[HarmonyPatch(typeof(VirtualInputModule), "SetCursor")]
            //public class CursorHp
            //{
            //    public static void Postfix(VirtualInputModule __instance)
            //    {
            //        if (__instance.m_VirtualCursor == null)
            //            return;
            //        var hp = Util.KInstantiateUI(ModAssets.CursorHP, __instance.m_VirtualCursor.gameObject, true);
            //        hp.AddComponent<CursorHP>();
            //        hp.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 10, 10);
            //    }
            //}
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
                static Color32[] ColourValues;
                static ushort CreeperIDx = 0;
                static int time = 0;
                static int colourStep = 0;
                const int looptime = 100;

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
                        ColourValues = new Color32[looptime];

                        for (int i = 0; i < looptime; ++i)
                            AddColourEntry(i);
                    }
                }

                static void AddColourEntry(int i)
                {
                    var stepColor = Color.HSVToRGB((float)i / (float)looptime, 1, 1);
                    byte rByte = (byte)(stepColor.r * 255f), gByte = (byte)(stepColor.g * 255f), bByte = (byte)(stepColor.b * 255f);
                    ColourValues[i] = new Color32(rByte, gByte, bByte, (byte)255);
                }

                public static Color CurrentColor => Color.HSVToRGB(time / looptime, 1, 1);

                static int internalTimer = 0;
                static int CameraOffset = 0;
                static int TimeAndCameraOffset = 0;

                [HarmonyPriority(Priority.High)]
                public static void Postfix()
                {
                    if (SpeedControlScreen.Instance == null ||
                        !ModAssets.RainbowLiquids
                        )
                        return;
                    if (!SpeedControlScreen.Instance.IsPaused)
                        internalTimer += 1 + SpeedControlScreen.Instance.GetSpeed();

                    if (internalTimer >= 2)
                    {
                        time += internalTimer / 2;
                        internalTimer %= 2;
                    }
                    var cameraVector = CameraController.Instance.transform.position;
                    if (Grid.IsWorldValidCell(Grid.PosToCell(cameraVector)))
                        CameraOffset = (Mathf.RoundToInt(cameraVector.x + cameraVector.y + cameraVector.z) / 2);

                    TimeAndCameraOffset = time + CameraOffset;

                    IntPtr pixelsPtr = PropertyTextures.externalLiquidTex;

                    //Parallel.For(0, Grid.CellCount, (i) => ProcessPixel(pixelsPtr, i, rByte, gByte,bByte));
                    Parallel.For(0, Grid.CellCount, (i) => ProcessPixelbyTime(pixelsPtr, i));
                    time %= looptime;
                }
                private static unsafe void ProcessPixelbyTime(IntPtr pixelsPtr, int i)
                {
                    if (!Grid.IsActiveWorld(i) || !Grid.IsLiquid(i)) return;


                    int current = GetCurrentColourIndex(i);
                    ref var colour = ref ColourValues[current];

                    byte* pixel = (byte*)pixelsPtr.ToPointer() + (i * 4);
                    pixel[0] = colour.r;
                    pixel[1] = colour.g;
                    pixel[2] = colour.b;
                }
                static int GetCurrentColourIndex(int cell)
                {
                    int Y = Grid.CellRow(cell)
                        , X = Grid.CellColumn(cell)
                        ;
                    //this:
                    return ((Y + X) / 2 + TimeAndCameraOffset) % looptime;
                    ////or this:
                    //var val = ((Y +X)/2) + TimeAndCameraOffset;
                    //while (val >= looptime)
                    //    val -= looptime;
                    //return val;
                }


                private static unsafe void ProcessPixel(IntPtr pixelsPtr, int i, byte r, byte g, byte b)
                {
                    if (!Grid.IsActiveWorld(i) || Grid.elementIdx[i] != CreeperIDx || CreeperIDx == 0)
                        return;

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
