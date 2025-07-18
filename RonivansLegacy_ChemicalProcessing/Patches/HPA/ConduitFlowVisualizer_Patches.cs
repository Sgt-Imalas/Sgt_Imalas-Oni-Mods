using HarmonyLib;
using Mono.Cecil.Cil;
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
using static ConduitFlowVisualizer;

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
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static bool Prefix(ConduitFlowVisualizer __instance, int cell, ref Color32 __result)
			{
				if (HighPressureConduit.HasHighPressureConduitAt(cell, __instance.flowManager.conduitType, __instance.showContents, out var changedTint))
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
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			static OverlayModes.ConduitMode Instance;
            public static void Prefix(OverlayModes.ConduitMode __instance)
            {
                Instance = __instance;
            }

            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var Colorset_GetColorByName = AccessTools.Method(typeof(ColorSet),nameof(ColorSet.GetColorByName));
                var ReplaceConduitColor = AccessTools.Method(typeof(OverlayModes_ConduitMode_Update_Patch),nameof(ReplaceHPConduitColor));

				var GetColorByName = AccessTools.Method(typeof(ColorSet), nameof(ColorSet.GetColorByName));
				//var set_TintColour = AccessTools.PropertySetter(typeof(KAnimControllerBase), nameof(KAnimControllerBase.TintColour));

				var codes = orig.ToList();

				// SaveLoadRoot layerTarget;
				int layerTargetIdx = 12;
				foreach (CodeInstruction original in orig)
				{
					if (original.Calls(GetColorByName))
					{
						yield return original; //puts the color on the stack
						yield return new CodeInstruction(OpCodes.Ldloc_S, layerTargetIdx); //current layer target 
						yield return new CodeInstruction(OpCodes.Call, ReplaceConduitColor); //ReplaceHPConduitColor
					}
					else
						yield return original;
				}
			}

			private static Color32 ReplaceHPConduitColor(Color32 oldColor, SaveLoadRoot currentItem)
            {
				if (HighPressureConduit.IsHighPressureConduit(currentItem.gameObject))
				{
					if (Instance.ViewMode() == OverlayModes.LiquidConduits.ID)
					{
						return HighPressureConduit.GetColorForConduitType(ConduitType.Liquid, true);
					}
					else if (Instance.ViewMode() == OverlayModes.GasConduits.ID)
					{
						return HighPressureConduit.GetColorForConduitType(ConduitType.Gas, true);
					}
				}
				return oldColor;
            }

        }

		/// <summary>
		/// This transpiler adjusts the blob filled state on high pressure pipes
		/// its remade since the original target no longer exists
		/// </summary>
		[HarmonyPatch(typeof(ConduitFlowVisualizer.RenderMeshBatchJob), nameof(ConduitFlowVisualizer.RenderMeshBatchJob.RunItem))]
		public class ConduitFlowVisualizer_RenderMeshBatchJob_TargetMethod_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;

			static ConduitType CurrentConduitType;
			public static void Prefix(RenderMeshContext shared_data)
			{
				CurrentConduitType = shared_data.outer.flowManager.conduitType;
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				MethodInfo ConduitFlowVisualite_CalculateMassScale = AccessTools.Method(typeof(ConduitFlowVisualizer), nameof(ConduitFlowVisualizer.CalculateMassScale));
				MethodInfo Conduit_GetCell = AccessTools.Method(typeof(ConduitFlow.Conduit), nameof(ConduitFlow.Conduit.GetCell));
				MethodInfo normalizeConduitFillState = AccessTools.Method(typeof(ConduitFlowVisualizer_RenderMeshBatchJob_TargetMethod_Patch), nameof(NormalizeFillState));

				var codes = orig.ToList();

				int searchIndex = codes.FindIndex(ci => ci.Calls(Conduit_GetCell));

				//either 17 or 7
				int cellidx_index = TranspilerHelper.FindIndexOfNextLocalIndex(codes, searchIndex,false);
				SgtLogger.l("RenderMeshBatchJob cellidx: " + cellidx_index);

				///Inject a check if the pipe is a high pressure pipe.
				///if yes, divide the actual pipe mass by the capacity multiplier, so it calculates the blob size based on the relative fill state
				foreach (CodeInstruction original in orig)
				{
					if (original.Calls(ConduitFlowVisualite_CalculateMassScale))
					{
						yield return new CodeInstruction(OpCodes.Ldloc_S,cellidx_index); //result of conduit.GetCell(shared_data.outer.flowManager)
						yield return new CodeInstruction(OpCodes.Call, normalizeConduitFillState); //normalizes the amount injected into CalculateMassScale
						yield return original; //ConduitFlowVisualizer.CalculateMassScale
					}
					else
						yield return original;
				}
			}

			//switches out the high pressure amount with the fullness amount a regular pipe would have relative to its max capacity
			private static float NormalizeFillState(float absPipeMass, int cell)
			{
				if(HighPressureConduit.HasHighPressureConduitAt(cell, CurrentConduitType))
				{
					return HighPressureConduit.GetNormalizedPercentageMass(absPipeMass, CurrentConduitType);
				}

				return absPipeMass;
			}
		}
	}
}
