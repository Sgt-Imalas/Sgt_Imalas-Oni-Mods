using Database;
using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static HoverTextConfiguration;
using static STRINGS.BUILDING.STATUSITEMS;

namespace UtilLibs.BuildingPortUtils
{
	public static class ConduitDisplayPortPatching
	{
		private static HashSet<string> buildings = new HashSet<string>();
		internal static bool HasBuilding(string name)
		{
			return buildings.Contains(name);
		}

		// Add a building to the cache
		internal static void AddBuilding(string ID)
		{
			buildings.Add(ID);
		}

		static readonly string PLib_Registry_StatusItems = "PLib_Registry_PipeStatusItems";

		public static void PatchAll(Harmony harmony)
		{
			var target = AccessTools.Method(typeof(EntityCellVisualizer), nameof(EntityCellVisualizer.DrawIcons));
			harmony.Patch(target, new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(PortDrawPrefix)));

			var target2 = AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.MarkArea));
			harmony.Patch(target2, null, new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(MarkAreaPostfix)));

			var target3 = AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.AreConduitPortsInValidPositions));
			harmony.Patch(target3, null, new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(AreConduitPortsInValidPositionsPostfix)));

			if (!PRegistry.GetData<bool>(PLib_Registry_StatusItems))
			{
				var createStatusItems = AccessTools.Method(typeof(BuildingStatusItems), nameof(BuildingStatusItems.CreateStatusItems));
				harmony.Patch(createStatusItems, null, new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(CreatePortStatusItemsPostfix)));

				var injectStatusItemStrings = AccessTools.Method(typeof(Localization), nameof(Localization.Initialize));
				harmony.Patch(injectStatusItemStrings, null, new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(CreateStatusItemStrings)));
				PRegistry.PutData(PLib_Registry_StatusItems, true);
			}

			PortInfoDrawing.PatchAll(harmony);
		}

		public static void PortDrawPrefix(EntityCellVisualizer __instance, HashedString mode)
		{
			if (__instance is BuildingCellVisualizer bcVis && buildings.Contains(bcVis.building.Def.PrefabID))
			{
				UnityEngine.GameObject go = bcVis.building.gameObject;
				PortDisplayController controller = go.GetComponent<PortDisplayController>();
				if (controller != null)
				{
					controller.Draw(bcVis, mode, go);
				}
			}
		}


		public static void MarkAreaPostfix(BuildingDef __instance, int cell, Orientation orientation, ObjectLayer layer, GameObject go)
		{
			foreach (PortDisplay2 portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
			{
				ConduitType secondaryConduitType2 = portDisplay.type;
				ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(secondaryConduitType2);
				CellOffset rotatedCellOffset8 = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
				int cell11 = Grid.OffsetCell(cell, rotatedCellOffset8);
				__instance.MarkOverlappingPorts(Grid.Objects[cell11, (int)objectLayerForConduitType4], go);
				Grid.Objects[cell11, (int)objectLayerForConduitType4] = go;
			}
		}
		public static void AreConduitPortsInValidPositionsPostfix(BuildingDef __instance, ref bool __result, GameObject source_go, int cell, Orientation orientation, ref string fail_reason)
		{
			if (__result)
			{
				foreach (PortDisplay2 portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
				{
					CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
					int utility_cell = Grid.OffsetCell(cell, rotatedCellOffset);
					__result = __instance.IsValidConduitConnection(source_go, portDisplay.type, utility_cell, ref fail_reason);
					if (!__result)
					{
						return;
					}
				}
			}
		}

		static class PortInfoDrawing
		{
			static bool AddAny = false;
			static string portText = "";
			static Sprite portSprite;
			static Color portColor = Color.white;
			static TextStyleSetting textStyle = null;

			public static void PatchAll(Harmony harmony)
			{
				var target = AccessTools.Method(typeof(EntityCellVisualizer), nameof(EntityCellVisualizer.DrawIcons));
				harmony.Patch(target, postfix: new HarmonyMethod(typeof(PortInfoDrawing), nameof(EntityCellVisualizer_DrawIcons_Postfix)));


				var target2 = AccessTools.Method(typeof(SelectToolHoverTextCard), nameof(SelectToolHoverTextCard.UpdateHoverElements));
				var target3 = AccessTools.Method(typeof(BuildToolHoverTextCard), nameof(SelectToolHoverTextCard.UpdateHoverElements));
				harmony.Patch(target2, new HarmonyMethod(typeof(PortInfoDrawing), nameof(HoverTextConfiguration_UpdateHoverElements_Prefix)));
				harmony.Patch(target3, new HarmonyMethod(typeof(PortInfoDrawing), nameof(HoverTextConfiguration_UpdateHoverElements_Prefix)));

				var target4 = AccessTools.Method(typeof(HoverTextDrawer), nameof(HoverTextDrawer.EndDrawing));
				harmony.Patch(target4, new HarmonyMethod(typeof(PortInfoDrawing), nameof(HoverTextDrawer_EndDrawing_Prefix)));
			}

			static void EntityCellVisualizer_DrawIcons_Postfix(EntityCellVisualizer __instance, HashedString mode)
			{
				if (__instance is BuildingCellVisualizer bcVis)
					PortDisplayController.HandleVanillaPortInfo(bcVis, mode);
			}
			static void HoverTextConfiguration_UpdateHoverElements_Prefix(SelectToolHoverTextCard __instance, List<KSelectable> hoverObjects)
			{
				if (textStyle == null)
				{
					var existingFontStyle = __instance.Styles_Title.Standard;
					textStyle = new TextStyleSetting()
					{
						sdfFont = existingFontStyle.sdfFont,
						fontSize = existingFontStyle.fontSize,
						textColor = existingFontStyle.textColor,
						style = existingFontStyle.style,
						enableWordWrapping = existingFontStyle.enableWordWrapping
					};
					textStyle.fontSize = 16;
				}

				int mouseCell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
				if (OverlayScreen.Instance == null || !Grid.IsValidCell(mouseCell))
					return;

				if (PortDisplayController.TryGetActivePortDesc(mouseCell, out portText, out portSprite, out portColor))
				{
					AddAny = true;
				}
				else
				{
					AddAny = false;
				}
			}
			static void HoverTextDrawer_EndDrawing_Prefix(HoverTextDrawer __instance)
			{
				if (!AddAny)
					return;
				AddAny = false;

				__instance.BeginShadowBar();
				__instance.DrawIcon(portSprite, portColor, 20);
				__instance.DrawText(portText, textStyle);
				__instance.EndShadowBar();
			}
		}



		public static StatusItem GetInputStatusItem(ConduitType type)
		{
			switch (type)
			{
				case ConduitType.Gas:
					if (M_NeedGasIn == null)
						M_NeedGasIn = Db.Get().BuildingStatusItems.Get(M_NeedGasIn_Key);
					return M_NeedGasIn;
				case ConduitType.Liquid:
					if (M_NeedLiquidIn == null)
						M_NeedLiquidIn = Db.Get().BuildingStatusItems.Get(M_NeedLiquidIn_Key);
					return M_NeedLiquidIn;
				case ConduitType.Solid:
					if (M_NeedSolidIn == null)
						M_NeedSolidIn = Db.Get().BuildingStatusItems.Get(M_NeedSolidIn_Key);
					return M_NeedSolidIn;
				default:
					throw new ArgumentException($"Unknown conduit type: {type}");
			}
		}
		public static StatusItem GetOutputStatusItem(ConduitType type)
		{
			switch (type)
			{
				case ConduitType.Gas:
					if (M_NeedGasOut == null)
						M_NeedGasOut = Db.Get().BuildingStatusItems.Get(M_NeedGasOut_Key);
					return M_NeedGasOut;
				case ConduitType.Liquid:
					if (M_NeedLiquidOut == null)
						M_NeedLiquidOut = Db.Get().BuildingStatusItems.Get(M_NeedLiquidOut_Key);
					return M_NeedLiquidOut;
				case ConduitType.Solid:
					if (M_NeedSolidOut == null)
						M_NeedSolidOut = Db.Get().BuildingStatusItems.Get(M_NeedSolidOut_Key);
					return M_NeedSolidOut;
				default:
					throw new ArgumentException($"Unknown conduit type: {type}");
			}
		}


		public static StatusItem M_NeedLiquidIn, M_NeedGasIn, M_NeedLiquidOut, M_NeedGasOut, M_NeedSolidIn, M_NeedSolidOut;
		public static string
			M_NeedLiquidIn_Key = nameof(M_NeedLiquidIn), M_NeedLiquidOut_Key = nameof(M_NeedLiquidOut),
			M_NeedGasIn_Key = nameof(M_NeedGasIn), M_NeedGasOut_Key = nameof(M_NeedGasOut),
			M_NeedSolidIn_Key = nameof(M_NeedSolidIn), M_NeedSolidOut_Key = nameof(M_NeedSolidOut);

		public static void CreatePortStatusItemsPostfix(BuildingStatusItems __instance)
		{
			M_NeedGasIn = __instance.CreateStatusItem(M_NeedGasIn_Key, "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.GasConduits.ID);
			M_NeedGasIn.resolveStringCallback = delegate (string str, object data)
			{
				Tuple<ConduitType, Tag> tuple2 = (Tuple<ConduitType, Tag>)data;
				string newValue12 = string.Format(NEEDGASIN.LINE_ITEM, tuple2.second.ProperName());
				str = str.Replace("{GasRequired}", newValue12);
				return str;
			};
			M_NeedLiquidIn = __instance.CreateStatusItem(M_NeedLiquidIn_Key, "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.LiquidConduits.ID);
			M_NeedLiquidIn.resolveStringCallback = delegate (string str, object data)
			{
				Tuple<ConduitType, Tag> tuple = (Tuple<ConduitType, Tag>)data;
				string newValue11 = string.Format(NEEDLIQUIDIN.LINE_ITEM, tuple.second.ProperName());
				str = str.Replace("{LiquidRequired}", newValue11);
				return str;
			};
			M_NeedSolidIn = __instance.CreateStatusItem(M_NeedSolidIn_Key, "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.SolidConveyor.ID);
			M_NeedSolidIn.resolveStringCallback = delegate (string str, object data)
			{
				Tuple<ConduitType, Tag> tuple = (Tuple<ConduitType, Tag>)data;
				string newValue11 = string.Format(NEEDLIQUIDIN.LINE_ITEM, tuple.second.ProperName());
				str = str.Replace("{LiquidRequired}", newValue11);
				return str;
			};

			M_NeedGasOut = __instance.CreateStatusItem(M_NeedGasOut_Key, "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.GasConduits.ID);
			M_NeedGasOut.resolveStringCallback = delegate (string str, object data)
			{
				Tuple<ConduitType, List<Tag>> tuple = (Tuple<ConduitType, List<Tag>>)data;
				foreach (var tag in tuple.second)
				{
					str += "\n";
					str += string.Format(NEEDGASIN.LINE_ITEM, tag.ProperName());
				}
				return str;
			};
			M_NeedLiquidOut = __instance.CreateStatusItem(M_NeedLiquidOut_Key, "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.LiquidConduits.ID);
			M_NeedLiquidOut.resolveStringCallback = delegate (string str, object data)
			{
				Tuple<ConduitType, List<Tag>> tuple = (Tuple<ConduitType, List<Tag>>)data;
				foreach (var tag in tuple.second)
				{
					str += "\n";
					str += string.Format(NEEDLIQUIDIN.LINE_ITEM, tag.ProperName());
				}
				return str;
			};
			M_NeedSolidOut = __instance.CreateStatusItem(M_NeedSolidOut_Key, "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.SolidConveyor.ID);
			M_NeedSolidOut.resolveStringCallback = delegate (string str, object data)
			{
				Tuple<ConduitType, List<Tag>> tuple = (Tuple<ConduitType, List<Tag>>)data;
				foreach (var tag in tuple.second)
				{
					str += "\n";
					str += string.Format(NEEDLIQUIDIN.LINE_ITEM, tag.ProperName());
				}
				return str;
			};
		}
		public static void CreateStatusItemStrings()
		{
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDGASIN.NAME", STRINGS.BUILDING.STATUSITEMS.NEEDGASIN.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDGASIN.TOOLTIP", STRINGS.BUILDING.STATUSITEMS.NEEDGASIN.TOOLTIP);

			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDLIQUIDIN.NAME", STRINGS.BUILDING.STATUSITEMS.NEEDLIQUIDIN.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDLIQUIDIN.TOOLTIP", STRINGS.BUILDING.STATUSITEMS.NEEDLIQUIDIN.TOOLTIP);


			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDGASOUT.NAME", STRINGS.BUILDING.STATUSITEMS.NEEDGASOUT.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDGASOUT.TOOLTIP", STRINGS.BUILDING.STATUSITEMS.NEEDGASOUT.TOOLTIP);

			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDLIQUIDOUT.NAME", STRINGS.BUILDING.STATUSITEMS.NEEDLIQUIDOUT.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDLIQUIDOUT.TOOLTIP", STRINGS.BUILDING.STATUSITEMS.NEEDLIQUIDOUT.TOOLTIP);


			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDSOLIDOUT.NAME", STRINGS.BUILDING.STATUSITEMS.NEEDSOLIDOUT.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDSOLIDOUT.TOOLTIP", STRINGS.BUILDING.STATUSITEMS.NEEDSOLIDOUT.TOOLTIP);

			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDSOLIDIN.NAME", STRINGS.BUILDING.STATUSITEMS.NEEDSOLIDIN.NAME);
			Strings.Add("STRINGS.BUILDING.STATUSITEMS.M_NEEDSOLIDIN.TOOLTIP", STRINGS.BUILDING.STATUSITEMS.NEEDSOLIDIN.TOOLTIP);
		}
	}
}
