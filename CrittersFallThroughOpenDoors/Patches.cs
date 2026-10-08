using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CrittersFallThroughOpenDoors
{
	internal class Patches
	{
		static readonly HashSet<int> OpenDoorCells = [];

		[HarmonyPatch(typeof(Game), nameof(Game.OnLoadLevel))]
		public class Game_OnLoadLevel_Patch
		{
			public static void Postfix() => OpenDoorCells.Clear();
		}

		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				DoorPatches.Execute();
			}
		}
		public class DoorPatches
		{
			internal static void Execute()
			{
				Mod.Harmony.Patch(
					AccessTools.Method(typeof(Door), nameof(Door.SetPassableState)),
					postfix: new(AccessTools.Method(typeof(DoorPatches), nameof(SetWorldStatePatch))));
			}
			static void SetWorldStatePatch(bool is_door_open, IList<int> cells)
			{
				foreach (int cell in cells)
				{
					if (is_door_open)
						OpenDoorCells.Add(cell);
					else
						OpenDoorCells.Remove(cell);
				}
			}
		}


		[HarmonyPatch(typeof(GameNavGrids.FloorValidator), nameof(GameNavGrids.FloorValidator.IsWalkableCell))]
		public class GameNavGrids_FloorValidator_IsWalkableCell_Patch
		{
			public static void Postfix(int anchor_cell, bool is_dupe, ref bool __result)
			{
				if (is_dupe || !Grid.IsWorldValidCell(anchor_cell))
					return;
				if (Grid.FakeFloor[anchor_cell] && OpenDoorCells.Contains(anchor_cell))
					__result = false;
			}
		}
		//[HarmonyPatch(typeof(GameNavGrids.WallValidator), nameof(GameNavGrids.WallValidator.IsWalkableCell))]
		//public class GameNavGrids_WallValidator_IsWalkableCell_Patch
		//{
		//	public static void Postfix(int cell, int anchor_cell, ref bool __result)
		//	{
		//		if (is_dupe)
		//			return;
		//		if (Grid.FakeFloor[anchor_cell] && OpenDoorCells.Contains(anchor_cell))
		//			__result = false;
		//	}
		//}
		[HarmonyPatch(typeof(GameNavGrids.CeilingValidator), nameof(GameNavGrids.CeilingValidator.IsWalkableCell))]
		public class GameNavGrids_CeilingValidator_IsWalkableCell_Patch
		{
			public static void Postfix(int anchor_cell, ref bool __result)
			{
				if (!__result || !Grid.IsWorldValidCell(anchor_cell))
					return;
				if (Grid.FakeFloor[anchor_cell] && OpenDoorCells.Contains(anchor_cell))
					__result = false;
			}
		}

		//[HarmonyPatch(typeof(CreatureFallMonitor.Instance), nameof(CreatureFallMonitor.Instance.ShouldFall))]
		//public class CreatureFallMonitor_Instance_ShouldFall_Patch
		//{
		//	public static void Postfix(CreatureFallMonitor.Instance __instance, ref bool __result)
		//	{
		//		if (__result || __instance.navigator.IsMoving() || __instance.kprefabId.HasTag(GameTags.Stored))
		//			return;

		//		Vector3 pos = __instance.smi.transform.GetPosition();
		//		pos.y += CreatureFallMonitor.FLOOR_DISTANCE;
		//		int cell = Grid.PosToCell(pos);
		//		if(OpenDoorCells.Contains(cell))
		//			__result = true;
		//	}
		//}


		//[HarmonyPatch(typeof(GravityComponents), nameof(GravityComponents.FixedUpdate))]
		//public class GravityComponents_FixedUpdate_Patch
		//{
		//	public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
		//	{
		//		var codes = orig.ToList();
		//		MethodInfo fakeFloorIndexer = AccessTools.Method(typeof(Grid.BuildFlagsFakeFloorIndexer), "get_Item");
		//		MethodInfo injectedMethod = AccessTools.Method(typeof(GravityComponents_FixedUpdate_Patch), nameof(CheckIfOpenDoor));

		//		int callIndex = codes.FindIndex(ci => ci.Calls(fakeFloorIndexer));
		//		if (callIndex == -1)
		//		{
		//			SgtLogger.error("TRANSPILER ERROR: could not find call for BuildFlagsFakeFloorIndexer");
		//			return orig;
		//		}
		//		var callCi = codes[callIndex];

		//		//localCellVarIndex == 14 in current code
		//		int localCellVarIndex = TranspilerHelper.FindIndexOfNextLocalIndex(orig.ToList(), callCi);

		//		codes.InsertRange(callIndex + 1,
		//			[
		//			new CodeInstruction(OpCodes.Ldloc_S, localCellVarIndex),
		//			new CodeInstruction(OpCodes.Call,injectedMethod)
		//			]);

		//		TranspilerHelper.PrintInstructions(codes);
		//		return codes;
		//	}

		//	private static bool CheckIfOpenDoor(bool isFakeFloor, int cell)
		//	{
		//		if (OpenDoorCells.Contains(cell))
		//			return false;
		//		return isFakeFloor;
		//	}
		//}
	}
}
