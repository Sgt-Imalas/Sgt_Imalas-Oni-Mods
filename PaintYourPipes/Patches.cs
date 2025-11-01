using HarmonyLib;
using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static OverlayModes;
using static OverlayModes.Logic;

namespace PaintYourPipes
{
	internal class Patches
	{


		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				OverrideBlobColor.ExecutePatch();
			}
		}

		private static ObjectLayer _activeLayer = (ObjectLayer)(-1);
		public static ObjectLayer ActiveOverlay
		{
			get
			{
				return _activeLayer;
			}
			set
			{
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
			static Tag MaterialColor_ExcludedTag = new Tag("NoPaint");
			public static void ExecutePatch(Harmony harmony)
			{
				var DoPostConfigureUnderConstruction_Postfix = AccessTools.Method(typeof(AddColorComponentToFinishedBuildings), "DoPostConfigureUnderConstruction_Postfix");
				var DoPostConfigureComplete_Postfix = AccessTools.Method(typeof(AddColorComponentToFinishedBuildings), "DoPostConfigureComplete_Postfix");
				foreach (var m_TargetType in TargetBuildingTypes())
				{
					if (m_TargetType != null)
					{
						var m_TargetMethod_DoPostConfigureComplete = AccessTools.DeclaredMethod(m_TargetType, "DoPostConfigureComplete");
						if (m_TargetMethod_DoPostConfigureComplete == null)
						{
							SgtLogger.warning("target method DoPostConfigureComplete not found on " + m_TargetType.Name);
						}
						else
						{
							harmony.Patch(m_TargetMethod_DoPostConfigureComplete, postfix: new HarmonyMethod(DoPostConfigureComplete_Postfix));
						}

						var m_TargetMethod_DoPostConfigureUnderConstruction = AccessTools.DeclaredMethod(m_TargetType, "DoPostConfigureUnderConstruction");
						if (m_TargetMethod_DoPostConfigureUnderConstruction == null)
						{
							SgtLogger.warning("target method DoPostConfigureUnderConstruction not found on " + m_TargetType.Name);
						}
						else
						{
							harmony.Patch(m_TargetMethod_DoPostConfigureUnderConstruction, postfix: new HarmonyMethod(DoPostConfigureUnderConstruction_Postfix));
						}
					}

				}
			}
			public static void DoPostConfigureComplete_Postfix(GameObject go)
			{
				go.AddOrGet<ColorableConduit>();
				if(!Config.Instance.OverlayOnly)
					go.AddOrGet<KPrefabID>().AddTag(MaterialColor_ExcludedTag);

			}
			public static void DoPostConfigureUnderConstruction_Postfix(GameObject go)
			{
				go.AddComponent<ColorableConduit_UnderConstruction>();
				if (!Config.Instance.OverlayOnly)
					go.AddOrGet<KPrefabID>().AddTag(MaterialColor_ExcludedTag);
			}

			static List<Type> TargetBuildingTypes()
			{
				var values = new List<Type>
				{
					typeof(SolidConduitBridgeConfig),
					typeof(LiquidConduitBridgeConfig),
					typeof(GasConduitBridgeConfig),

					typeof(LiquidConduitConfig),
					typeof(LiquidConduitRadiantConfig),
					typeof(InsulatedLiquidConduitConfig),

					typeof(GasConduitConfig),
					typeof(GasConduitRadiantConfig),
					typeof(InsulatedGasConduitConfig),

					typeof(SolidConduitConfig),

					typeof(WireConfig),
					typeof(WireHighWattageConfig),
					typeof(WireRefinedConfig),
					typeof(WireRefinedHighWattageConfig),

					typeof(WireBridgeConfig),
					typeof(WireBridgeHighWattageConfig),
					typeof(WireRefinedBridgeConfig),
					typeof(WireRefinedBridgeHighWattageConfig),

					typeof(LogicWireConfig),
					typeof(LogicRibbonConfig),
					typeof(LogicWireBridgeConfig),
					typeof(LogicRibbonBridgeConfig),
					typeof(LogicGateBaseConfig),
					typeof(LogicGateBufferConfig),
					typeof(LogicGateFilterConfig),
					typeof(LogicMemoryConfig),
					typeof(LogicRibbonReaderConfig),
					typeof(LogicRibbonWriterConfig),
				};

				//Insulated Wire Briges:
				var InsulatedWireBridgeHighWattageConfig = AccessTools.TypeByName("InsulatedWireBridgeHighWattageConfig");
				if (InsulatedWireBridgeHighWattageConfig != null)
					values.Add(InsulatedWireBridgeHighWattageConfig);

				var InsulatedWireRefinedBridgeHighWattageConfig = AccessTools.TypeByName("InsulatedWireRefinedBridgeHighWattageConfig");
				if (InsulatedWireRefinedBridgeHighWattageConfig != null)
					values.Add(InsulatedWireRefinedBridgeHighWattageConfig);

				var LongInsulatedRefinedWireBridgeHighWattageConfig = AccessTools.TypeByName("LongInsulatedRefinedWireBridgeHighWattageConfig");
				if (LongInsulatedRefinedWireBridgeHighWattageConfig != null)
					values.Add(LongInsulatedRefinedWireBridgeHighWattageConfig);

				var LongInsulatedWireBridgeHighWattageConfig = AccessTools.TypeByName("LongInsulatedWireBridgeHighWattageConfig");
				if (LongInsulatedWireBridgeHighWattageConfig != null)
					values.Add(LongInsulatedWireBridgeHighWattageConfig);

				//GigawattWire
				var GigawattWireBridgeConfig = AccessTools.TypeByName("GigawattWireBridgeConfig");
				if (GigawattWireBridgeConfig != null)
					values.Add(GigawattWireBridgeConfig);

				var GigawattWireConfig = AccessTools.TypeByName("GigawattWireConfig");
				if (GigawattWireConfig != null)
					values.Add(GigawattWireConfig);

				var JacketedWireBridgeConfig = AccessTools.TypeByName("JacketedWireBridgeConfig");
				if (JacketedWireBridgeConfig != null)
					values.Add(JacketedWireBridgeConfig);

				var JacketedWireConfig = AccessTools.TypeByName("JacketedWireConfig");
				if (JacketedWireConfig != null)
					values.Add(JacketedWireConfig);

				var MegawattWireBridgeConfig = AccessTools.TypeByName("MegawattWireBridgeConfig");
				if (MegawattWireBridgeConfig != null)
					values.Add(MegawattWireBridgeConfig);

				var MegawattWireConfig = AccessTools.TypeByName("MegawattWireConfig");
				if (MegawattWireConfig != null)
					values.Add(MegawattWireConfig);

				//HighPressureApplications
				var HighPressureGasConduitBridgeConfig = AccessTools.TypeByName("HighPressureGasConduitBridgeConfig");
				if (HighPressureGasConduitBridgeConfig != null)
					values.Add(HighPressureGasConduitBridgeConfig);

				var HighPressureGasConduitConfig = AccessTools.TypeByName("HighPressureGasConduitConfig");
				if (HighPressureGasConduitConfig != null)
					values.Add(HighPressureGasConduitConfig);

				var HighPressureLiquidConduitBridgeConfig = AccessTools.TypeByName("HighPressureLiquidConduitBridgeConfig");
				if (HighPressureLiquidConduitBridgeConfig != null)
					values.Add(HighPressureLiquidConduitBridgeConfig);

				var HighPressureLiquidConduitConfig = AccessTools.TypeByName("HighPressureLiquidConduitConfig");
				if (HighPressureLiquidConduitConfig != null)
					values.Add(HighPressureLiquidConduitConfig);
			
				//PlasticUtilities
				var PlasticGasConduitConfig = AccessTools.TypeByName("PlasticGasConduitConfig");
				if (PlasticGasConduitConfig != null)
					values.Add(PlasticGasConduitConfig);
				var PlasticLiquidConduitConfig = AccessTools.TypeByName("PlasticLiquidConduitConfig");
				if (PlasticLiquidConduitConfig != null)
					values.Add(PlasticLiquidConduitConfig);

				return values;
			}
		}

		[HarmonyPatch(typeof(CopyBuildingSettings), nameof(CopyBuildingSettings.ApplyCopy))]
		public static class ApplyCopy_Color
		{
			public static void Postfix(ref bool __result, int targetCell, GameObject sourceGameObject)
			{
				if (sourceGameObject.TryGetComponent<ColorableConduit>(out var sourcebuilding))
				{
					if (ColorableConduit.TryGetColorable(targetCell, sourcebuilding, out ColorableConduit targetConduit))
					{
						targetConduit.Trigger((int)GameHashes.CopySettings, sourceGameObject);
						__result = true;
					}
					if (ColorableConduit.TryGetColorableBridge(targetCell, sourcebuilding, out ColorableConduit targetBridge))
					{
						targetBridge.Trigger((int)GameHashes.CopySettings, sourceGameObject);
						__result = true;
					}
				}
			}
		}
		[HarmonyPatch(typeof(Game), nameof(Game.DestroyInstances))]
		public static class Game_DestroyInstance_Patch
		{
			public static void Prefix()
			{
				ColorableConduit.FlushDictionary();
			}
		}

		[HarmonyPatch(typeof(PlanScreen), "OnClickCopyBuilding")]
		public static class PlanScreen_OnClickCopyBuilding_Patch
		{
			public static void Prefix()
			{
				if (SelectTool.Instance.selected == null)
					return;

				if (SelectTool.Instance.selected.TryGetComponent(out ColorableConduit colorbuilding))
				{
					ColorableConduit_UnderConstruction.BuildFromColor = colorbuilding.ColorHex;
					ColorableConduit_UnderConstruction.HasColorOverride = true;
				}
			}

		}
		[HarmonyPatch(typeof(BuildTool), "OnDeactivateTool")]
		public class BuildTool_OnDeactivateTool_Patch
		{
			public static void Postfix() => ColorableConduit_UnderConstruction.HasColorOverride = false;
		}

		static bool HighlightIfApplicable(ColorableConduit target, ICollection<UtilityNetwork> networks, Color targetColor, out Color highlighted)
		{
			highlighted = targetColor;
			if (networks.Count == 0) return false;

			SgtLogger.l(networks.Count() + " " + target.NetworkItem);
			if (target.NetworkItem != null && target.NetworkItem.IsConnectedToNetworks(networks)
			 || target.solidConduit != null && networks.Contains(target.solidConduit.GetNetwork()))
			{
				float highlightMultiplier = OverlayModes.ModeUtil.GetHighlightScale();

				highlighted = new Color(targetColor.r * highlightMultiplier, (targetColor.g * highlightMultiplier), (targetColor.b * highlightMultiplier), 0);

				//SgtLogger.l(targetColor + " <> "+ highlighted);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Tint the blobs on pipes in the same color as the pipe
		/// </summary>
		//[HarmonyPatch(typeof(ConduitFlowVisualizer), nameof(ConduitFlowVisualizer.GetCellTintColour))]
		public static class OverrideBlobColor
		{
			public static void ExecutePatch()
			{
				var m_TargetMethod = AccessTools.Method("ConduitFlowVisualizer, Assembly-CSharp:GetCellTintColour");
				var m_Postfix = AccessTools.Method(typeof(OverrideBlobColor), "Postfix");
				Mod.Harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix));
			}
			public static void Postfix(ConduitFlowVisualizer __instance, int cell, ref Color32 __result)
			{
				if (__instance == Game.Instance.liquidFlowVisualizer && ColorableConduit.ConduitsByLayer[(int)ObjectLayer.LiquidConduit].ContainsKey(cell))
				{

					var colorOverrider = ColorableConduit.ConduitsByLayer[(int)ObjectLayer.LiquidConduit][cell];
					if (ActiveOverlay != ObjectLayer.LiquidConduit)
					{
						if (!Config.Instance.OverlayOnly)
							__result = __result.Multiply(colorOverrider.TintColor);
					}
					else if (ColorableConduit.ShowOverlayTint)
						__result = colorOverrider.TintColor;
				}
				else if (__instance == Game.Instance.gasFlowVisualizer && ColorableConduit.ConduitsByLayer[(int)ObjectLayer.GasConduit].ContainsKey(cell))
				{
					var colorOverrider = ColorableConduit.ConduitsByLayer[(int)ObjectLayer.GasConduit][cell];
					if (ActiveOverlay != ObjectLayer.GasConduit)
					{
						if (!Config.Instance.OverlayOnly)
							__result = __result.Multiply(colorOverrider.TintColor);
					}
					else if (ColorableConduit.ShowOverlayTint)
						__result = colorOverrider.TintColor;
				}
			}
		}

		#region logicOverlay
		[HarmonyPatch(typeof(Logic), nameof(Logic.Enable))]
		public static class ColorsInOverlay_Logic_OnEnable
		{
			public static void Postfix(Logic __instance)
			{
				ActiveOverlay = ObjectLayer.LogicWire;

				if (!ColorableConduit.ShowOverlayTint)
					return;
				ColorableConduit.RefreshOfConduitType(ActiveOverlay);
			}
		}

		[HarmonyPatch(typeof(Logic), nameof(Logic.Update))]
		public static class ColorsInOverlayy_Logic_Update
		{
			public static void Postfix(Logic __instance)
			{
				Color targetColor;
				ColorableConduit.ToggleNormalTint((int)ObjectLayer.LogicGate);
				foreach (KBatchedAnimController wireController in __instance.wireControllers)
				{
					if (!ColorableConduit.ShowOverlayTint || wireController == null || !wireController.TryGetComponent<ColorableConduit>(out var building))
					{
						continue;
					}
					HighlightIfApplicable(building, __instance.connectedNetworks, building.GetColor(), out targetColor);
					//wireController.TintColour = targetColor;
					building.RefreshColor(targetColor);
				}

				foreach (KBatchedAnimController ribbonController in __instance.ribbonControllers)
				{
					if (ribbonController == null || !ribbonController.TryGetComponent<ColorableConduit>(out var building))
					{
						continue;
					}
					if (!ColorableConduit.ShowOverlayTint)
					{
						ribbonController.TintColour = Color.white;
					}
					else
					{
						HighlightIfApplicable(building, __instance.connectedNetworks, building.GetColor(), out targetColor);
						ribbonController.SetSymbolTint(RIBBON_WIRE_1_SYMBOL_NAME, targetColor);
						ribbonController.SetSymbolTint(RIBBON_WIRE_2_SYMBOL_NAME, targetColor);
						ribbonController.SetSymbolTint(RIBBON_WIRE_3_SYMBOL_NAME, targetColor);
						ribbonController.SetSymbolTint(RIBBON_WIRE_4_SYMBOL_NAME, targetColor);
					}
				}

				foreach (BridgeInfo bridgeController in __instance.bridgeControllers)
				{
					if (!ColorableConduit.ShowOverlayTint || bridgeController.controller == null || !bridgeController.controller.TryGetComponent<ColorableConduit>(out var building))
					{
						continue;
					}
					HighlightIfApplicable(building, __instance.connectedNetworks, building.GetColor(), out targetColor);
					//bridgeController.controller.TintColour = targetColor;
					building.RefreshColor(targetColor);
				}

				foreach (BridgeInfo ribbonBridgeController in __instance.ribbonBridgeControllers)
				{
					if (ribbonBridgeController.controller == null || !ribbonBridgeController.controller.TryGetComponent<ColorableConduit>(out var building))
					{
						continue;
					}
					if (!ColorableConduit.ShowOverlayTint)
					{
						ribbonBridgeController.controller.TintColour = Color.white;
					}
					else
					{
						HighlightIfApplicable(building, __instance.connectedNetworks, building.GetColor(), out targetColor);
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_1_SYMBOL_NAME, targetColor);
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_2_SYMBOL_NAME, targetColor);
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_3_SYMBOL_NAME, targetColor);
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_4_SYMBOL_NAME, targetColor);
					}
				}
			}
		}

		[HarmonyPatch(typeof(Logic), nameof(Logic.Disable))]
		public static class ColorsInOverlayy_Logic_OnDisable
		{
			public static void Postfix()
			{
				ActiveOverlay = (ObjectLayer)(-1);
				ColorableConduit.RefreshAll();
			}
		}
		#endregion

		#region SolidOverlay


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

		[HarmonyPatch(typeof(SolidConveyor), nameof(SolidConveyor.Update))]
		public static class ColorsInOverlayy_Solid_Update
		{
			public static void Postfix(SolidConveyor __instance)
			{

				ColorableConduit.ToggleNormalTint((int)ObjectLayer.SolidConduitConnection);

				if (!ColorableConduit.ShowOverlayTint)
					return;

				foreach (SaveLoadRoot layerTarget in __instance.layerTargets)
				{
					if (layerTarget != null && layerTarget.TryGetComponent<ColorableConduit>(out var building))
					{
						if (HighlightIfApplicable(building, __instance.connectedNetworks, building.GetColor(), out var highlighted))
							building.RefreshColor(highlighted);
						else
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
		#endregion

		#region PowerOverlay
		[HarmonyPatch(typeof(BlockTileRenderer), nameof(BlockTileRenderer.GetCellColour))]
		public static class BlockTileRenderer_GetCellColour
		{
			public static void Postfix(int cell, SimHashes element, BlockTileRenderer __instance, ref Color __result)
			{
				if (!ColorableConduit.ShowOverlayTint || ActiveOverlay != ObjectLayer.Wire)
					return;

				if (ColorableConduit.ConduitsByLayer[(int)ObjectLayer.Building].ContainsKey(cell))
				{
					__result = ColorableConduit.ConduitsByLayer[(int)ObjectLayer.Building][cell].GetColor();
				}
			}
		}

		[HarmonyPatch(typeof(Power), nameof(Power.Enable))]
		public static class ColorsInOverlay_Power_OnEnable
		{
			public static void Postfix()
			{
				ActiveOverlay = ObjectLayer.Wire;

				if (!ColorableConduit.ShowOverlayTint)
					return;
				ColorableConduit.RefreshOfConduitType(ActiveOverlay);
			}
		}

		[HarmonyPatch(typeof(Power), nameof(Power.Update))]
		public static class ColorsInOverlayy_Power_Update
		{
			public static void Postfix(Power __instance)
			{
				if (!ColorableConduit.ShowOverlayTint)
					return;

				foreach (SaveLoadRoot layerTarget in __instance.layerTargets)
				{
					if (layerTarget != null && layerTarget.TryGetComponent<ColorableConduit>(out var building))
					{
						if (HighlightIfApplicable(building, __instance.connectedNetworks, building.GetColor(), out var highlighted))
							building.RefreshColor(highlighted);
						else
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

		[HarmonyPatch(typeof(Power), nameof(Power.Disable))]
		public static class ColorsInOverlayy_Power_OnDisable
		{
			public static void Postfix(Power __instance)
			{
				ActiveOverlay = (ObjectLayer)(-1);
				ColorableConduit.RefreshAll();
			}
		}
		#endregion

		#region ConduitOverlays
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
						if (HighlightIfApplicable(building, __instance.connectedNetworks, building.GetColor(), out var highlighted))
							building.RefreshColor(highlighted);
						else
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
				ActiveOverlay = (ObjectLayer)(-1);
				ColorableConduit.RefreshAll();
			}
		}

		#endregion

		#region Input_UI

		[HarmonyPatch(typeof(TopLeftControlScreen))]
		[HarmonyPatch(nameof(TopLeftControlScreen.OnActivate))]
		public static class Add_Colorable_Button
		{
			static MultiToggle ToggleColorOverlayButton = null;
			static ToolTip ToggleColorOverlayButtonTooltip = null;
			static Image image = null;
			public static void ToggleColorsInOverlay()
			{
				ColorableConduit.ToggleOverlayTint();
				UpdateDebugToggleState();
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
			}
			public static void UpdateDebugToggleState()
			{
				image.sprite = Assets.GetSprite("brush");
				ToggleColorOverlayButton.ChangeState(ColorableConduit.ShowOverlayTint ? 2 : 1);
				ToggleColorOverlayButtonTooltip.SetSimpleTooltip(GameUtil.ReplaceHotkeyString(STRINGS.PAINTABLEBUILDING.TOGGLE_TOOLTIP, ModAssets.HotKeys.ToggleOverlayColors.GetKAction()));
			}

			public static void Postfix(TopLeftControlScreen __instance)
			{

				var debugTimeButton = Util.KInstantiateUI(__instance.sandboxToggle.gameObject, __instance.sandboxToggle.transform.parent.gameObject, true).transform;
				//UIUtils.ListAllChildrenWithComponents(debugButton);
				debugTimeButton.Find("FG").TryGetComponent<Image>(out image);
				debugTimeButton.Find("Label").GetComponent<LocText>().text = STRINGS.PAINTABLEBUILDING.TOGGLE_TEXT;
				image.sprite = Assets.GetSprite("brush");
				debugTimeButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
				debugTimeButton.TryGetComponent<MultiToggle>(out ToggleColorOverlayButton);
				debugTimeButton.TryGetComponent<ToolTip>(out ToggleColorOverlayButtonTooltip);
				debugTimeButton.SetSiblingIndex(__instance.kleiItemDropButton.transform.GetSiblingIndex());
				ToggleColorOverlayButton.onClick = (System.Action)Delegate.Combine(ToggleColorOverlayButton.onClick, new System.Action(ToggleColorsInOverlay));
				UpdateDebugToggleState();
			}
		}

		[HarmonyPatch(typeof(PlayerController), "OnKeyDown")]
		public class PlayerController_OnKeyDown_Patch
		{
			public static void Prefix(KButtonEvent e)
			{
				if (e.TryConsume(ModAssets.HotKeys.ToggleOverlayColors.GetKAction()))
				{
					ColorableConduit.ToggleOverlayTint();
					Add_Colorable_Button.UpdateDebugToggleState();
				}
			}
		}
		#endregion

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

				if (__instance.sideScreen != null
					&& __instance.sideScreen.gameObject != null)
				{
					if (__instance.target.TryGetComponent<ColorableConduit>(out var colorable))
						ColorableConduit_SideScreen.Target = colorable;
					else
						ColorableConduit_SideScreen.Target = null;

					ColorableConduit_SideScreen.RefreshUIState(__instance.sideScreen.transform);
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
