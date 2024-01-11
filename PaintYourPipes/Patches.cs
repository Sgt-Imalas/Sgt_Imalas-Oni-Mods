using Database;
using HarmonyLib;
using Klei.AI;
using Mono.CompilerServices.SymbolWriter;
using rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UtilLibs;
using static AmbienceManager;
using static Game;
using static OverlayModes;
using static PaintYourPipes.ModAssets;
using static STRINGS.UI.TOOLS;

namespace PaintYourPipes
{
    internal class Patches
    {

        private static ObjectLayer _activeLayer = (ObjectLayer)(-1);
        public static ObjectLayer ActiveOverlay
        {
            get { return _activeLayer; 
            }
            set { 
                _activeLayer = value;
               // SgtLogger.l("active set: " + _activeLayer.ToString());
            }
        }

        /// <summary>
        /// Add Colourable-Component to bridges and pipes
        /// 
        /// </summary>
        /// 
        [HarmonyPatch]
        public static class AddColorComponentToFinishedBuildings
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<ColorableConduit>();
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
                yield return typeof(SolidConduitBridgeConfig).GetMethod(name);
                yield return typeof(LiquidConduitBridgeConfig).GetMethod(name);
                yield return typeof(GasConduitBridgeConfig).GetMethod(name);
                //yield return typeof(BaseWireConfig).GetMethod(name);

                yield return typeof(LiquidConduitConfig).GetMethod(nameof(LiquidConduitConfig.CommonConduitPostConfigureComplete));
            }
        }

        [HarmonyPatch(typeof(CopyBuildingSettings), nameof(CopyBuildingSettings.ApplyCopy))]
        public static class ApplyCopy_Color
        {
            public static void Postfix(ref bool __result, int targetCell, GameObject sourceGameObject)
            {
                if(sourceGameObject.TryGetComponent<ColorableConduit>(out var sourcebuilding))
                {
                    if (ColorableConduit.TryGetColorable(targetCell, sourcebuilding, out ColorableConduit targetConduit) )
                    {
                        targetConduit.Trigger(-905833192, (object)sourceGameObject);
                        __result = true;
                    }
                    if (ColorableConduit.TryGetColorableBridge(targetCell, sourcebuilding, out ColorableConduit targetBridge) )
                    {
                        targetBridge.Trigger(-905833192, (object)sourceGameObject);
                        __result = true;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(SolidConveyor), nameof(SolidConveyor.Enable))]
        public static class ColorsInOverlay_Solid_OnEnable
        {
            public static void Postfix(SolidConveyor __instance, ref HashSet<SaveLoadRoot> __state)
            {
                ActiveOverlay = ObjectLayer.SolidConduit;

                if (!ColorableConduit.ShowOverlayTint)
                    return;
                ColorableConduit.RefreshOfConduitType(ActiveOverlay);
            }
        }


        //[HarmonyPatch(typeof(PlanScreen), "OnClickCopyBuilding")]
        //public static class PlanScreen_OnClickCopyBuilding_Patch
        //{
        //    public static void Prefix() 
        //    {
        //        if(SelectTool.Instance.selected == null)
        //            return;

        //        if (SelectTool.Instance.selected.TryGetComponent(out ColorableConduit colorbuilding))
        //        {
        //            ColorableConduit.BuildFromColor = colorbuilding.ColorHex;
        //            ColorableConduit.HasColorOverride = true;
        //        }
        //    }

        //}
        //[HarmonyPatch(typeof(BuildTool), "OnDeactivateTool")]
        //public class BuildTool_OnDeactivateTool_Patch
        //{
        //    public static void Postfix() => ColorableConduit.HasColorOverride = false;
        //}


        [HarmonyPatch(typeof(SolidConveyor), nameof(SolidConveyor.Update))]
        public static class ColorsInOverlayy_Solid_Update
        {
            public static void Postfix(SolidConveyor __instance)
            {
                if (!ColorableConduit.ShowOverlayTint)
                    return;

                foreach (SaveLoadRoot layerTarget in __instance.layerTargets)
                {
                    if (layerTarget != null && layerTarget.TryGetComponent<ColorableConduit>(out var building))
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

        [HarmonyPatch(typeof(SolidConveyor), nameof(SolidConveyor.Disable))]
        public static class ColorsInOverlayy_Solid_OnDisable
        {
            public static void Postfix(SolidConveyor __instance)
            {
                ActiveOverlay = (ObjectLayer)(-1);
                ColorableConduit.RefreshAll();
            }
        }

        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Enable))]
        public static class ColorsInOverlay_OnEnable
        {
            public static void Postfix(OverlayModes.ConduitMode __instance, ref HashSet<SaveLoadRoot> __state)
            {

                if (__instance is GasConduits)
                    ActiveOverlay = ObjectLayer.GasConduit;
                else if (__instance is LiquidConduits)
                    ActiveOverlay = ObjectLayer.LiquidConduit;

                if (!ColorableConduit.ShowOverlayTint)
                    return;

                ColorableConduit.RefreshOfConduitType(ActiveOverlay);
            }
        }
        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Update))]
        public static class ColorsInOverlay_Update
        {
            public static void Postfix(OverlayModes.ConduitMode __instance)
            {
                if (!ColorableConduit.ShowOverlayTint)
                    return;

                foreach (SaveLoadRoot layerTarget in __instance.layerTargets)
                {
                    if (layerTarget != null && layerTarget.TryGetComponent<ColorableConduit>(out var building))
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

        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Disable))]
        public static class ColorsInOverlay_OnDisable
        {
            public static void Postfix(OverlayModes.ConduitMode __instance, ref HashSet<SaveLoadRoot> __state)
            {
                ActiveOverlay = (ObjectLayer) (-1);
                ColorableConduit.RefreshAll();
            }
        }

        /// <summary>
        /// Tint the blobs on pipes in the same color as the pipe
        /// </summary>
        [HarmonyPatch(typeof(ConduitFlowVisualizer), nameof(ConduitFlowVisualizer.GetCellTintColour))]
        public static class OverrideBlobColor
        {
            public static void Postfix(ConduitFlowVisualizer __instance, int cell, ref Color32 __result)
            {

                if (__instance == Game.Instance.liquidFlowVisualizer && ColorableConduit.ConduitsByLayer[(int)ObjectLayer.LiquidConduit].ContainsKey(cell))
                {
                    var colorOverrider = ColorableConduit.ConduitsByLayer[(int)ObjectLayer.LiquidConduit][cell];
                    if (ColorableConduit.ShowOverlayTint)
                    {
                        __result = __result.Multiply(colorOverrider.TintColor);
                    }
                }
                else if (__instance == Game.Instance.gasFlowVisualizer && ColorableConduit.ConduitsByLayer[(int)ObjectLayer.GasConduit].ContainsKey(cell))
                {
                    var colorOverrider = ColorableConduit.ConduitsByLayer[(int)ObjectLayer.GasConduit][cell];
                    if (ColorableConduit.ShowOverlayTint)
                    {
                        __result = __result.Multiply(colorOverrider.TintColor);
                    }
                }
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
                    ColorableConduit_SideScreen.colorPickerContainerPrefab = pixelPackScreen.colorSwatchContainer;
                    ColorableConduit_SideScreen.colorPickerSwatchEntryPrefab = pixelPackScreen.swatchEntry;
                    ColorableConduit_SideScreen.SwatchColors = new(pixelPackScreen.colorSwatch);
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
                    if(__instance.target.TryGetComponent<ColorableConduit>(out var colorable))
                        ColorableConduit_SideScreen.Target = colorable;
                    else
                        ColorableConduit_SideScreen.Target = null;

                    ColorableConduit_SideScreen.RefreshUIState(__instance.currentSideScreen.transform);
                }
            }
        }

        /// <summary>
        /// Delete Color picker screen
        /// </summary>
        [HarmonyPatch(typeof(Game), nameof(Game.OnDestroy))]
        public static class GameOnDestroy
        {
            public static void Postfix() => ColorableConduit_SideScreen.Destroy();
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
