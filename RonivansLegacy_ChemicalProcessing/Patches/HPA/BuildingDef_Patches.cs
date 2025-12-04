using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Grid.Restriction;
using static Operational;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	/// <summary>
	/// partially HPA, partially Structural Tile and building anim offset related patches 
	/// </summary>
	class BuildingDef_Patches
	{
		static Dictionary<BuildingDef, bool> HasStructuralTileMarkerCache = [];
		static bool DefHasStructuralTileMarker(BuildingDef def)
		{
			if (!HasStructuralTileMarkerCache.TryGetValue(def, out bool hasMarker))
			{
				hasMarker = def.BuildingComplete.TryGetComponent<StructuralTileMarker>(out _);
				HasStructuralTileMarkerCache[def] = hasMarker;
			}
			return hasMarker;
		}

		[HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.IsValidTileLocation))]
		public class BuildingDef_IsValidTileLocation_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HPA_Rails_Mod_Enabled;
			public static void Postfix(BuildingDef __instance, GameObject source_go, int cell, bool replacement_tile, ref string fail_reason, ref bool __result)
			{
				switch (__instance.BuildLocationRule)
				{
					case BuildLocationRule.OnFloor:
					case BuildLocationRule.OnCeiling:
					case BuildLocationRule.OnWall:
					case BuildLocationRule.OnFloorOverSpace:
					case BuildLocationRule.InCorner:
						return;
				}

				if (!__result && fail_reason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION && DefHasStructuralTileMarker(__instance))
				{
					fail_reason = "";
					__result = true;
					return;
				}

				if (!__result || StructuralTileMarker.TileAtCell(cell))
					return;

				var solidBridge = Grid.Objects[cell, (int)ObjectLayer.SolidConduitConnection];
				if (solidBridge != null && solidBridge != source_go && solidBridge.TryGetComponent<Building>(out var building) && building.Def.BuildLocationRule == BuildLocationRule.NotInTiles)
				{
					fail_reason = STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_HPA_RAIL;
					__result = false;
				}
				var solidRail = Grid.Objects[cell, (int)ObjectLayer.SolidConduit];
				if (solidRail != null && solidRail != source_go && solidRail.TryGetComponent<Building>(out var building2)
					&& building2.Def.BuildLocationRule == BuildLocationRule.NotInTiles
					&& !DefHasStructuralTileMarker(__instance))
				{
					fail_reason = STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_HPA_RAIL;
					__result = false;
				}
			}
		}

		static bool ReplacableReason(ref string fail_reason)
		{
			return fail_reason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES
				|| fail_reason == global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_WIRE_OBSTRUCTION
				|| fail_reason.IsNullOrWhiteSpace();
		}

		[HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.IsValidBuildLocation), [typeof(GameObject), typeof(int), typeof(Orientation), typeof(bool), typeof(string)], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out])]
		public class BuildingDef_IsValidBuildLocation_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;
			/// <summary>
			/// allow placing wires inside of structure tiles
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="cell"></param>
			/// <param name="source_go"></param>
			/// <param name="orientation"></param>
			/// <param name="fail_reason"></param>
			/// <param name="__result"></param>
			public static void Postfix(BuildingDef __instance, int cell, GameObject source_go, Orientation orientation, ref string fail_reason, ref bool __result)
			{
				switch (__instance.BuildLocationRule)
				{
					case BuildLocationRule.OnFloor:
					case BuildLocationRule.OnCeiling:
					case BuildLocationRule.OnWall:
					case BuildLocationRule.OnFloorOverSpace:
					case BuildLocationRule.InCorner:
						return;
				}

				if (!__result && ReplacableReason(ref fail_reason) && (StructuralTileMarker.TileAtCell(cell) || DefHasStructuralTileMarker(__instance)))
				{
					fail_reason = "";
					__result = true;
					return;
				}

				if (!__result)
					return;
				__result = IsValidHPABridgeLocation(__instance, ref cell, source_go, ref orientation, ref fail_reason);
			}
		}
		static GameObject inputCellTile, outputCellTile;
		static CellOffset outputOffset, inputOffset;
		static int utility_output_cell, utility_input_cell;
		static bool IsValidHPABridgeLocation(BuildingDef __instance, ref int cell, GameObject source_go, ref Orientation orientation, ref string fail_reason)
		{
			if (__instance.BuildLocationRule == BuildLocationRule.NotInTiles && __instance.ObjectLayer == ObjectLayer.SolidConduitConnection)
			{
				//SgtLogger.l("Checking Bridge Validity for cell: " + cell + " with orientation: " + orientation);

				outputOffset = Rotatable.GetRotatedCellOffset(__instance.UtilityOutputOffset, orientation);
				utility_output_cell = Grid.OffsetCell(cell, outputOffset);

				inputOffset = Rotatable.GetRotatedCellOffset(__instance.UtilityInputOffset, orientation);
				utility_input_cell = Grid.OffsetCell(cell, inputOffset);

				inputCellTile = Grid.Objects[utility_input_cell, (int)ObjectLayer.FoundationTile];
				outputCellTile = Grid.Objects[utility_output_cell, (int)ObjectLayer.FoundationTile];

				//SgtLogger.l("Input Cell Tile: " + (inputCellTile != null) + " at cell: " + utility_input_cell);
				//SgtLogger.l("Output Cell Tile: " + (outputCellTile != null) + " at cell: " + utility_output_cell);

				if (inputCellTile != null || Grid.HasDoor[utility_input_cell]
					|| outputCellTile != null || Grid.HasDoor[utility_output_cell])
				{
					fail_reason = global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_NOT_IN_TILES;
					return false;
				}
			}
			return true;
		}

		[HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.IsValidPlaceLocation),
			[typeof(GameObject), typeof(int), typeof(Orientation), typeof(bool), typeof(string), typeof(bool)], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal])]
		public class BuildingDef_IsValidPlaceLocation_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled || Config.Instance.HPA_Rails_Mod_Enabled;

			public static void Postfix(BuildingDef __instance, GameObject source_go, int cell, Orientation orientation, bool replace_tile, ref string fail_reason, bool restrictToActiveWorld, ref bool __result)
			{
				switch (__instance.BuildLocationRule)
				{
					case BuildLocationRule.OnFloor:
					case BuildLocationRule.OnCeiling:
					case BuildLocationRule.OnWall:
					case BuildLocationRule.OnFloorOverSpace:
					case BuildLocationRule.InCorner:
						return;
				}

				if (!__result && ReplacableReason(ref fail_reason) && (StructuralTileMarker.TileAtCell(cell) || DefHasStructuralTileMarker(__instance)))
				{
					fail_reason = "";
					__result = true;
					return;
				}

				if (!__result)
					return;
				__result = IsValidHPABridgeLocation(__instance, ref cell, source_go, ref orientation, ref fail_reason);
			}
		}
	}
}
