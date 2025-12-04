using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.Patches.CodexEntryGenerator_Patches;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class RequiresFoundation_Patches
	{

		[HarmonyPatch(typeof(RequiresFoundation), nameof(RequiresFoundation.Add))]
		public class RequiresFoundation_Add_Patch
		{
			/// <summary>
			/// modifies the cells that refresh the foundation checks for buildings with offset colliders
			/// </summary>
			/// <param name="_"></param>
			/// <param name="orig"></param>
			/// <returns></returns>
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{

				//MethodInfo modifyOffsetCheck = AccessTools.Method(typeof(RequiresFoundation_Add_Patch), nameof(ModifyCellOffset));
				MethodInfo modifyRootCell = AccessTools.Method(typeof(RequiresFoundation_Add_Patch), nameof(ModifyRootCell));


				//var Rotatable_GetRotatedCellOffset = AccessTools.Method(typeof(Rotatable), nameof(Rotatable.GetRotatedCellOffset), [typeof(CellOffset), typeof(Orientation)]);
				var Grid_PosToCell = AccessTools.Method(typeof(Grid), nameof(Grid.PosToCell), [typeof(Vector3)]);

				foreach (CodeInstruction original in orig)
				{
					//if (original.Calls(Rotatable_GetRotatedCellOffset))
					//{
					//	yield return original;
					//	yield return new CodeInstruction(OpCodes.Ldarg_1); //GameObject go
					//	yield return new CodeInstruction(OpCodes.Call, modifyOffsetCheck);
					//}
					if (original.Calls(Grid_PosToCell))
					{
						yield return original;
						yield return new CodeInstruction(OpCodes.Ldarg_1); //GameObject go
						yield return new CodeInstruction(OpCodes.Call, modifyRootCell);
					}
					else
						yield return original;
				}
			}
			public static int ModifyRootCell(int original, GameObject go)
			{
				if (!go.TryGetComponent<ColliderOffsetHandler>(out var handler))
				{
					return original;
				}
				var offset = handler.GetRotatedOffset();
				int offsetCell = Grid.OffsetCell(original, offset);
				SgtLogger.l("Modifying RequiresFoundation root cell from " + original + " by " + offset.ToString() + " to " + offsetCell + " for " + go.name);
				return offsetCell;
			}

			//public static CellOffset ModifyCellOffset(CellOffset original, GameObject go)
			//{
			//	if (!go.TryGetComponent<ColliderOffsetHandler>(out var handler))
			//	{
			//		return original;
			//	}
			//	var offset = handler.GetRotatedOffset();
			//	//SgtLogger.l("Modifying RequiresFoundation offset from " + original + "by " + offset.ToString() + " for " + go.name);
			//	return new CellOffset(original.x + offset.x, original.y + offset.y);
			//}
		}

	}
}
