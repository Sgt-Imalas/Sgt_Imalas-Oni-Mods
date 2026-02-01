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
				if (HighPressureConduitRegistration.HasHighPressureConduitAt(cell, __instance.flowManager.conduitType, __instance.showContents, out var changedTint))
				{
					__result = changedTint;
					return false;
				}
				return true;
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
			public static void Prefix(ref RenderMeshContext shared_data)
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
				if(HighPressureConduitRegistration.HasHighPressureConduitAt(cell, CurrentConduitType))
				{
					return HighPressureConduitRegistration.GetNormalizedPercentageMass(absPipeMass, CurrentConduitType);
				}

				return absPipeMass;
			}
		}
	}
}
