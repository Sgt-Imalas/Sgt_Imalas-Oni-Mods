using HarmonyLib;
using Mono.Cecil.Cil;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	class ConduitFlowVisualizer_Patches
	{

		[HarmonyPatch(typeof(ConduitFlowVisualizer), nameof(ConduitFlowVisualizer.GetCellTintColour))]
		public class ConduitFlowVisualizer_GetCellTintColour_Patch
		{
			public static bool Prefix(ConduitFlowVisualizer __instance, int cell, ref Color32 __result)
			{
				if (HighPressureConduitComponent.HasHighPressureConduitAt(cell, __instance.flowManager.conduitType, __instance.showContents, out var changedTint))
				{
					__result = changedTint;
					return false;
				}
				return true;
			}
		}

        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Update))]
        public class OverlayModes_ConduitMode_Update_Patch
        {
            static OverlayModes.ConduitMode Instance;
            public static void Prefix(OverlayModes.ConduitMode __instance)
            {
                Instance = __instance;
            }

            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var Colorset_GetColorByName = AccessTools.Method(typeof(ColorSet),nameof(ColorSet.GetColorByName));
                var ReplaceConduitColor = AccessTools.Method(typeof(OverlayModes_ConduitMode_Update_Patch),nameof(ReplaceHPConduitColor));

				var GetComponent = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), []).MakeGenericMethod(typeof(IBridgedNetworkItem));

				var codes = orig.ToList();

				var locIndexSearch = codes.FindIndex(ci => ci.Calls(GetComponent));
				///this is broken, fix!
				int locIndex = TranspilerHelper.FindIndexOfNextLocalIndex(codes,locIndexSearch);

				locIndex = 12;
				foreach (CodeInstruction original in orig)
				{
					if (original.Calls(Colorset_GetColorByName))
					{
						yield return original; //puts the color on the stack
						yield return new CodeInstruction(OpCodes.Ldloc_S,locIndex); //current layer target 
						yield return new CodeInstruction(OpCodes.Call, ReplaceConduitColor); //SetMaxFlow(flowManager.GetContents
					}
					else
						yield return original;
				}
			}

            private static Color32 ReplaceHPConduitColor(Color32 oldColor, SaveLoadRoot currentItem)
            {
				if (HighPressureConduitComponent.IsHighPressureConduit(currentItem.gameObject))
				{
					if (Instance.ViewMode() == OverlayModes.LiquidConduits.ID)
					{
						return HighPressureConduitComponent.GetColorForConduitType(ConduitType.Liquid, true);
					}
					else if (Instance.ViewMode() == OverlayModes.GasConduits.ID)
					{
						return HighPressureConduitComponent.GetColorForConduitType(ConduitType.Gas, true);
					}
				}
				return oldColor;
            }
        }
	}
}
