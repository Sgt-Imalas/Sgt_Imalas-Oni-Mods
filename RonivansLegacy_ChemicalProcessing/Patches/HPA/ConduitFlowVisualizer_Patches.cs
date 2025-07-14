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
		/// <summary>
		/// This patch overrides the colored border of the blobs to the custom overlay colors of the HPA conduits
		/// </summary>
		[HarmonyPatch(typeof(ConduitFlowVisualizer), nameof(ConduitFlowVisualizer.GetCellTintColour))]
		public class ConduitFlowVisualizer_GetCellTintColour_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications;
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


		/// <summary>
		/// this patch overrides the tinting color the high pressure conduits receive in their respective conduit overlays
		/// </summary>
        [HarmonyPatch(typeof(OverlayModes.ConduitMode), nameof(OverlayModes.ConduitMode.Update))]
        public class OverlayModes_ConduitMode_Update_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications;
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

				// SaveLoadRoot layerTarget;
				int layerTargetIdx = 12;
				foreach (CodeInstruction original in orig)
				{
					if (original.Calls(Colorset_GetColorByName))
					{
						yield return original; //puts the color on the stack
						yield return new CodeInstruction(OpCodes.Ldloc_S, layerTargetIdx); //current layer target 
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
