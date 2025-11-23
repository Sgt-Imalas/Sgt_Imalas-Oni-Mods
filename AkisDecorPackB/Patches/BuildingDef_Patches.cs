using AkisDecorPackB.Content.ModDb;
using AkisDecorPackB.Content.Scripts.BigFossil;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Patches
{
	internal class BuildingDef_Patches
	{
		private static void CheckCustomRules(BuildingDef __instance, ref bool __result, int cell, Orientation orientation, ref string fail_reason)
		{
			if (!Grid.IsValidBuildingCell(cell))
				return;

			if (__instance.BuildLocationRule == Mod_Db.BuildLocationRules.GiantFossilRule)
				__result = __result && IsValidFossilSpace(__instance, cell, orientation, ref fail_reason);
			else if (__instance.BuildLocationRule == Mod_Db.BuildLocationRules.OnAnyWall)
				__result = __result && CheckIfNextToWall(__instance, cell, orientation, ref fail_reason);
		}
		private static bool IsValidFossilSpace(BuildingDef instance, int cell, Orientation orientation, ref string fail_reason)
		{
			if (!BigFossil.isActivePreviewHangable && !IsOnFloor(instance, cell, orientation, ref fail_reason))
			{
				fail_reason = STRINGS.UI.DECORPACKB.BUILD_LOCATION_RULE_COMPLAINTS.WALLS;
				return false;
			}

			return true;
		}

		private static bool IsOnFloor(BuildingDef instance, int cell, Orientation orientation, ref string fail_reason)
		{
			if (!BuildingDef.CheckFoundation(cell, orientation, BuildLocationRule.OnFloor, instance.WidthInCells, instance.HeightInCells))
			{
				fail_reason = STRINGS.UI.DECORPACKB.BUILD_LOCATION_RULE_COMPLAINTS.HANGABLE;
				return false;
			}

			return true;
		}
		private static bool CheckIfNextToWall(BuildingDef __instance, int cell, Orientation orientation, ref string fail_reason)
		{
			// If the building is not next to a wall
			if (!IsNextToWall(__instance.WidthInCells, __instance.HeightInCells, cell, orientation))
			{
				fail_reason = STRINGS.UI.DECORPACKB.BUILD_LOCATION_RULE_COMPLAINTS.WALLS;
				return false;
			}

			return true;
		}

		private static bool IsNextToWall(int width, int height, int cell, Orientation orientation)
		{
			return BuildingDef.CheckFoundation(cell, orientation, BuildLocationRule.OnFloor, width, height) ||
				BuildingDef.CheckFoundation(cell, orientation, BuildLocationRule.OnCeiling, width, height) ||
				BuildingDef.CheckWallFoundation(cell, width, height, true) ||
				BuildingDef.CheckWallFoundation(cell, width, height, false);
		}

		[HarmonyPatch(
			typeof(BuildingDef),
			nameof(BuildingDef.IsValidPlaceLocation),
			[
				typeof(GameObject),
				typeof(int),
				typeof(Orientation),
				typeof(bool),
				typeof(string),
				typeof(bool)
			],
			[
				ArgumentType.Normal,
				ArgumentType.Normal,
				ArgumentType.Normal,
				ArgumentType.Normal,
				ArgumentType.Out,
				ArgumentType.Normal,
			])]
		public class BuildingDefuildingDef_IsValidPlaceLocation_Patch
		{
			public static void Postfix(BuildingDef __instance, ref bool __result, int cell, Orientation orientation, ref string fail_reason)
			{
				CheckCustomRules(__instance, ref __result, cell, orientation, ref fail_reason);
			}
		}
		[HarmonyPatch(typeof(BuildingDef),nameof(BuildingDef.IsValidBuildLocation),
			[typeof(GameObject),
				typeof(int),
				typeof(Orientation),
				typeof(bool),
				typeof(string)],
	[ArgumentType.Normal,
				ArgumentType.Normal,
				ArgumentType.Normal,
				ArgumentType.Normal,
				ArgumentType.Out])]
		public class BuildingDefuildingDef_IsValidBuildLocation_Patch
		{
			public static void Postfix(BuildingDef __instance, ref bool __result, int cell, Orientation orientation, ref string fail_reason)
			{
				CheckCustomRules(__instance, ref __result, cell, orientation, ref fail_reason);
			}
		}
	}
}
