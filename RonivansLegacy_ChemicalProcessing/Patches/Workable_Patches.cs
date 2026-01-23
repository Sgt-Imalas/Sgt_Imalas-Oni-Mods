using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Workable_Patches
	{

        [HarmonyPatch(typeof(Workable), nameof(Workable.GetEfficiencyMultiplier))]
        public class Workable_GetEfficiencyMultiplier_Patch
        {
			public static void Postfix(Workable __instance, WorkerBase worker, ref float __result)
			{
				if (worker.TryGetComponent<VariableCapacityForTransferArm>(out var cmp))
					__result *= cmp.GetWorkSpeedMultiplier();
			}
		}
	}
}
