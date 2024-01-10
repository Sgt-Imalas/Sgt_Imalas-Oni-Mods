using Database;
using HarmonyLib;
using Klei.AI;
using Mono.CompilerServices.SymbolWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Game;
using static OverlayModes;
using static PaintYourPipes.ModAssets;

namespace PaintYourPipes
{
    internal class Patches
    {
        public static bool OverlayActive = false;

        /// <summary>
        /// Add Colourable-Component to bridges and pipes
        /// 
        /// </summary>
        /// 
        [HarmonyPatch]
        public static class AddPortsToBuildMenu
        {
            static Tag SolidColor = TagManager.Create("ColorConduit_solid");
            static Tag LiquidColor = TagManager.Create("ColorConduit_liquid");
            static Tag GasColor = TagManager.Create("ColorConduit_gas");

            [HarmonyPostfix]
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<ColourableBuilding>();
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
                yield return typeof(SolidConduitBridgeConfig).GetMethod(name);
                yield return typeof(LiquidConduitBridgeConfig).GetMethod(name);
                yield return typeof(GasConduitBridgeConfig).GetMethod(name);

                yield return typeof(LiquidConduitConfig).GetMethod(nameof(LiquidConduitConfig.CommonConduitPostConfigureComplete));
            }
        }

        [HarmonyPatch(typeof(CopyBuildingSettings), nameof(CopyBuildingSettings.ApplyCopy))]
        public static class ApplyCopy_Color
        {
            public static void Postfix(ref bool __result, int targetCell, GameObject sourceGameObject)
            {
                if(sourceGameObject.TryGetComponent<ColourableBuilding>(out var sourcebuilding) && !__result)
                {
                    if (ColourableBuilding.TryGetColorable(targetCell, sourcebuilding, out ColourableBuilding targetConduit))
                    {
                        targetConduit.Trigger(-905833192, (object)sourceGameObject);
                        __result = true;
                    }
                    if (ColourableBuilding.TryGetColorableBridge(targetCell, sourcebuilding, out ColourableBuilding targetBridge))
                    {
                        targetBridge.Trigger(-905833192, (object)sourceGameObject);
                        __result = true;
                    }


                }
            }
        }



        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Enable))]
        public static class ColorsInOverlay_OnEnable
        {
            public static void Postfix(OverlayModes.ConduitMode __instance, ref HashSet<SaveLoadRoot> __state)
            {
                OverlayActive = true;
                ColourableBuilding.RefreshAll();
            }
        }
        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Update))]
        public static class ColorsInOverlay_Update
        {
            public static void Postfix(OverlayModes.ConduitMode __instance)
            {
                foreach (SaveLoadRoot layerTarget in __instance.layerTargets)
                {
                    if (layerTarget != null && layerTarget.TryGetComponent<ColourableBuilding>(out var building) && building.ShowInOverlay)
                    {
                        building.RefreshColor();
                        if (building.AnimController.enabled)
                        {
                            building.AnimController.enabled = false;
                            building.AnimController.enabled = true;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(ConduitFlowVisualizer), nameof(ConduitFlowVisualizer.GetCellTintColour))]
        public static class OverrideBlobColor
        {
            public static void Postfix(ConduitFlowVisualizer __instance, int cell, ref Color32 __result)
            {

                if (__instance == Game.Instance.liquidFlowVisualizer && ColourableBuilding.LiquidConduits.ContainsKey(cell))
                {
                    var colorOverrider = ColourableBuilding.LiquidConduits[cell];
                    if (colorOverrider.ShowInOverlay)
                    {
                        __result = colorOverrider.TintColor;
                    }
                }
                else if (__instance == Game.Instance.gasFlowVisualizer && ColourableBuilding.GasConduits.ContainsKey(cell))
                {
                    var colorOverrider = ColourableBuilding.GasConduits[cell];
                    if (colorOverrider.ShowInOverlay)
                    {
                        __result = colorOverrider.TintColor;
                    }
                }
            }
        }



        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Disable))]
        public static class ColorsInOverlay_OnDisable
        {
            public static void Postfix(OverlayModes.ConduitMode __instance, ref HashSet<SaveLoadRoot> __state)
            {
                OverlayActive = false;
                ColourableBuilding.RefreshAll();
            }
        }

        /// <summary>
        /// Grab ColorPicker gameobject from Pixelpack sidescreen
        /// </summary>
        [HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.OnPrefabInit))]
        public static class CustomSideScreenPatch_OnPrefabInit
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                var pixelPackScreenRef = ___sideScreens.Find(screen => screen.screenPrefab.GetType() == typeof(PixelPackSideScreen));
                if (pixelPackScreenRef != null)
                {
                    var pixelPackScreen = pixelPackScreenRef.screenPrefab as PixelPackSideScreen;
                    ColourableBuildingSideScreen.colorPickerContainerPrefab = pixelPackScreen.colorSwatchContainer;
                    ColourableBuildingSideScreen.colorPickerSwatchEntryPrefab = pixelPackScreen.swatchEntry;
                    ColourableBuildingSideScreen.SwatchColors = new(pixelPackScreen.colorSwatch);
                }
                else
                    SgtLogger.error("Pixelpack sidescreen not found, mod cannot function!");

            }
        }
        /// <summary>
        /// Show Color picker on colorable buldings
        /// </summary>
        [HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.Refresh))]
        public static class CustomSideScreenPatch_Refresh
        {
            public static void Postfix(DetailsScreen __instance)
            {

                if (__instance.currentSideScreen != null
                    && __instance.currentSideScreen.gameObject != null)
                {
                    if(__instance.target.TryGetComponent<ColourableBuilding>(out var colorable))
                        ColourableBuildingSideScreen.Target = colorable;
                    else
                        ColourableBuildingSideScreen.Target = null;

                    ColourableBuildingSideScreen.RefreshUIState(__instance.currentSideScreen.transform);
                }
            }
        }

        /// <summary>
        /// Delete Color picker screen
        /// </summary>
        [HarmonyPatch(typeof(Game), nameof(Game.OnDestroy))]
        public static class GameOnDestroy
        {
            public static void Postfix() => ColourableBuildingSideScreen.Destroy();
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
        
        
    }
}
