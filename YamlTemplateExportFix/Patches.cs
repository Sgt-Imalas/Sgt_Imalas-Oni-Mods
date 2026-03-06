using HarmonyLib;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TemplateClasses;
using UnityEngine;
using Color = UnityEngine.Color;

namespace YamlTemplateExportFix
{
	class Patches
	{
		[HarmonyPatch(typeof(DebugBaseTemplateButton))]
		[HarmonyPatch("GetSelectionAsAsset")]
		public static class FixPositionInYaml_patch
		{
			public static void Postfix(ref TemplateContainer __result, DebugBaseTemplateButton __instance)
			{
				bool doY, doX;
				int lXmin = 0, lXmax = 0, lYmin = 0, lYmax = 0;

				foreach (var cell in __result.cells)
				{
					lXmin = cell.location_x < lXmin ? cell.location_x : lXmin;
					lYmin = cell.location_y < lYmin ? cell.location_y : lYmin;
					lXmax = cell.location_x > lXmax ? cell.location_x : lXmax;
					lYmax = cell.location_y > lYmax ? cell.location_y : lYmax;
				}
				doY = Math.Abs(lYmax - lYmin) % 2 != 0;
				doX = Math.Abs(lXmax - lXmin) % 2 != 0;
				Debug.Log(string.Format("Patching X coordinates: {0}, Patching Y coordinates: {1}", doX, doY));

				if (__result.cells != null && __result.cells.Count > 0)
				{


					foreach (var cell in __result.cells)
					{
						if (doX)
							cell.location_x--;
						if (doY)
							cell.location_y--;
					}
					Debug.Log("Cells Done");
				}
				if (__result.buildings != null && __result.buildings.Count > 0)
				{
					foreach (Prefab building in __result.buildings)
					{
						if (doX)
							building.location_x--;
						if (doY)
							building.location_y--;
					}
					Debug.Log("Buildings Done");
				}
				if (__result.pickupables != null && __result.pickupables.Count > 0)
				{
					foreach (Prefab pickupable in __result.pickupables)
					{
						if (doX)
							pickupable.location_x--;
						if (doY)
							pickupable.location_y--;
					}
					Debug.Log("Items Done");
				}
				if (__result.elementalOres != null && __result.elementalOres.Count > 0)
				{
					foreach (Prefab elementalOre in __result.elementalOres)
					{
						if (doX)
							elementalOre.location_x--;
						if (doY)
							elementalOre.location_y--;
					}
				}
				if (__result.otherEntities != null && __result.otherEntities.Count > 0)
				{
					foreach (Prefab entity in __result.otherEntities)
					{
						if (doX)
							entity.location_x--;
						if (doY)
							entity.location_y--;
					}
					Debug.Log("Entities Done");
				}

			}
		}
		// Register in the overlay
		[HarmonyPatch(typeof(OverlayMenu), "InitializeToggles")]
		public static class OverlayMenu_InitializeToggles_Patch
		{
			public static void Postfix(List<KIconToggleMenu.ToggleInfo> ___overlayToggleInfos)
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
						"Info Overlay",
						"overlay_info",
						InfoOverlay.ID,
						"",
						Action.NumActions,
						"Displays various information about tiles",
						"Info Overlay",
					}
				);

				___overlayToggleInfos.Add((KIconToggleMenu.ToggleInfo)obj);
			}
		}

		[HarmonyPatch(typeof(OverlayScreen), "RegisterModes")]
		public static class OverlayScreen_RegisterModes_Patch
		{
			public static void Postfix()
			{
				var overlayScreen = Traverse.Create(OverlayScreen.Instance);
				overlayScreen.Method("RegisterMode", new InfoOverlay()).GetValue();
			}
		}

		// Remove log spam
		[HarmonyPatch(typeof(StatusItem), "GetStatusItemOverlayBySimViewMode")]
		public static class StatusItem_GetStatusItemOverlayBySimViewMode_Patch
		{
			public static void Prefix(Dictionary<HashedString, StatusItem.StatusItemOverlays> ___overlayBitfieldMap)
			{
				if (!___overlayBitfieldMap.ContainsKey(InfoOverlay.ID))
				{
					___overlayBitfieldMap.Add(InfoOverlay.ID, StatusItem.StatusItemOverlays.None);
				}
			}
		}

		// Cell coloring code
		[HarmonyPatch(typeof(SimDebugView), "OnPrefabInit")]
		public static class SimDebugView_OnPrefabInit_Patch
		{
			private static int selectedCell;

			public static void Postfix(Dictionary<HashedString, Func<SimDebugView, int, Color>> ___getColourFuncs)
			{
				___getColourFuncs.Add(InfoOverlay.ID, GetCellColor);
				SimDebugView.Instance.StartCoroutine(GetSelectedCell());
			}

			private static IEnumerator GetSelectedCell()
			{
				while (true)
				{
					if (SimDebugView.Instance.GetMode() == InfoOverlay.ID && Camera.main is Camera c)
					{
						selectedCell = Grid.PosToCell(c.ScreenToWorldPoint(KInputManager.GetMousePos()));
					}

					yield return new WaitForEndOfFrame();
				}
			}

			private static Color GetCellColor(SimDebugView instance, int cell)
			{
				return cell == selectedCell ? new Color(0f, 1f, 0.3f, 0.3f) : Color.clear;
			}
		}

		[HarmonyPatch(typeof(SelectToolHoverTextCard), "UpdateHoverElements")]
		public static class SelectToolHoverTextCard_UpdateHoverElements_Patch
		{
			private static readonly FieldInfo InfoId = AccessTools.Field(typeof(InfoOverlay), nameof(InfoOverlay.ID));

			private static readonly FieldInfo LogicId = AccessTools.Field(
				typeof(OverlayModes.Logic),
				nameof(OverlayModes.Logic.ID)
			);

			private static readonly MethodInfo HashEq = AccessTools.Method(
				typeof(HashedString),
				"op_Equality",
				new[] { typeof(HashedString), typeof(HashedString) }
			);

			private static readonly MethodInfo Helper = AccessTools.Method(
				typeof(SelectToolHoverTextCard_UpdateHoverElements_Patch),
				nameof(DrawerHelper)
			);

			public static IEnumerable<CodeInstruction> Transpiler(
				IEnumerable<CodeInstruction> orig,
				ILGenerator generator
			)
			{
				return orig; ///Dev Build crash
				var codes = orig.ToList();
				var logicId = codes.FindIndex(ci => ci.operand is FieldInfo info && info == LogicId);
				var thisLabel = generator.DefineLabel();
				codes[logicId + 2].operand = thisLabel;
				var idx = codes.FindIndex(logicId, ci => ci.opcode == OpCodes.Endfinally) + 1;
				var elseLabel = generator.DefineLabel();
				codes[idx].labels.Add(elseLabel);
				var i = idx;
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_2) { labels = { thisLabel } });
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldsfld, InfoId));
				codes.Insert(i++, new CodeInstruction(OpCodes.Call, HashEq));
				codes.Insert(i++, new CodeInstruction(OpCodes.Brfalse, elseLabel));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_1));
				codes.Insert(i++, new CodeInstruction(OpCodes.Call, Helper));
				codes.Insert(i++, new CodeInstruction(OpCodes.Br, elseLabel));
				return codes;
			}

			private static void DrawerHelper(SelectToolHoverTextCard inst, int cell, HoverTextDrawer drawer)
			{
				// Cell position info
				drawer.BeginShadowBar();
				var pos = Grid.CellToPos(cell);
				drawer.DrawText("POSITION", inst.Styles_Title.Standard);
				drawer.NewLine();
				drawer.DrawText($"({pos.x}, {pos.y})", inst.Styles_BodyText.Standard);
				drawer.NewLine();
				drawer.DrawText($"Cell {cell}", inst.Styles_BodyText.Standard);
				drawer.EndShadowBar();

				// Counts
				drawer.BeginShadowBar();
				drawer.DrawText("COUNT", inst.Styles_Title.Standard);
				drawer.NewLine();
				var p = Grid.Objects[cell, (int)ObjectLayer.Pickupables];
				var count = 0;
				while (p != null && p.GetComponent<Pickupable>() is Pickupable p2)
				{
					++count;
					p = p2.objectLayerListItem?.nextItem?.gameObject;
				}

				drawer.DrawText($"Pickupables: {count}", inst.Styles_BodyText.Standard);
				drawer.EndShadowBar();

				// Element info
				drawer.BeginShadowBar();
				var element = Grid.Element[cell];
				drawer.DrawText("ELEMENT", inst.Styles_Title.Standard);
				drawer.NewLine();
				drawer.DrawText($"Name: {element.name}", inst.Styles_BodyText.Standard);
				drawer.NewLine();
				var hardnessStr = GameUtil.GetHardnessString(element);
				if (hardnessStr == ELEMENTS.HARDNESS.NA)
				{
					hardnessStr = "Not Solid";
				}

				drawer.DrawText($"Hardness: {hardnessStr}", inst.Styles_BodyText.Standard);

				// element.HasTransitionDown doesn't exist :c
				if (element.lowTempTransitionTarget != 0 && element.lowTempTransitionTarget != SimHashes.Unobtanium &&
					element.lowTempTransition != null && element.lowTempTransition != element)
				{
					drawer.NewLine();
					drawer.DrawText(
						$"Transition down to {element.lowTempTransition.name} at {Math.Round(GameUtil.GetTemperatureConvertedFromKelvin(element.lowTemp, GameUtil.temperatureUnit), 2)}{GameUtil.GetTemperatureUnitSuffix()}",
						inst.Styles_BodyText.Standard
					);
				}

				if (element.HasTransitionUp)
				{
					drawer.NewLine();
					drawer.DrawText(
						$"Transition up to {element.highTempTransition.name} at {Math.Round(GameUtil.GetTemperatureConvertedFromKelvin(element.highTemp, GameUtil.temperatureUnit), 2)}{GameUtil.GetTemperatureUnitSuffix()}",
						inst.Styles_BodyText.Standard
					);
				}

				drawer.NewLine();
				drawer.DrawText(
					$"Specific Heat Capacity: {GameUtil.GetFormattedSHC(element.specificHeatCapacity)}",
					inst.Styles_BodyText.Standard
				);

				drawer.NewLine();
				drawer.DrawText(
					$"Thermal Conductivity: {GameUtil.GetThermalConductivityString(element)}",
					inst.Styles_BodyText.Standard
				);

				drawer.EndShadowBar();
			}
		}
	}
}
