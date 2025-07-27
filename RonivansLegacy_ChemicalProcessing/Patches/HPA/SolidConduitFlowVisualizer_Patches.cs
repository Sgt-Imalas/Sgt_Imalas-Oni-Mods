using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.Patches.HPA.SolidConduitBridge_Patches;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	class SolidConduitFlowVisualizer_Patches
	{

		/// <summary>
		/// this patch allows hiding the rail basked on insulated solid conduits, but I dont like the look of it
		/// </summary>
		[HarmonyPatch(typeof(SolidConduitFlowVisualizer), nameof(SolidConduitFlowVisualizer.Render))]
		public class SolidConduitFlowVisualizer_Render_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => false;// Config.Instance.HighPressureApplications_Enabled;
			internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				MethodInfo Handle_IsValid = AccessTools.Method(typeof(SolidConduitFlowVisualizer.ConduitFlowMesh), nameof(SolidConduitFlowVisualizer.ConduitFlowMesh.AddQuad));
				MethodInfo setMaxFlowForBridge = AccessTools.Method(typeof(SolidConduitFlowVisualizer_Render_Patch), nameof(HideInsulatedCages));

				foreach (CodeInstruction original in instructions)
				{
					if (original.Calls(Handle_IsValid))
					{
						SgtLogger.l("SolidConduitFlowVisualizer.Render - Replacing AddQuad to hide it from insulated pipes");
						//yield return original; // Handle.IsValid
						yield return new CodeInstruction(OpCodes.Ldloc_S, 15); //load current cell
						yield return new CodeInstruction(OpCodes.Call, setMaxFlowForBridge).MoveLabelsFrom(original); // mark handle as invalid to hide the cage if its insulated solid conduit
					}
					else
						yield return original;

				}
			}

			private static void HideInsulatedCages(SolidConduitFlowVisualizer.ConduitFlowMesh mesh, Vector2 pos, Color32 color, float size, float is_foreground, float highlight, Vector2I uvbl, Vector2I uvtl, Vector2I uvbr, Vector2I uvtr, int cell)
			{
				if (HighPressureConduitRegistration.IsInsulatedRail(cell))
				{
					return; //hide insulated cages
				}
				mesh.AddQuad(pos, color, size, is_foreground, highlight, uvbl, uvtl, uvbr, uvtr);
			}
		}

	}
}
