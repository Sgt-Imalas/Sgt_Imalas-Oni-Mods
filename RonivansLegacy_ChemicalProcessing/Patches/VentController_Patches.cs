using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class VentController_Patches
	{

		[HarmonyPatch(typeof(VentController), nameof(VentController.InitializeStates))]
		public class VentController_InitializeStatesd_Patch
		{
			public static void Postfix(VentController __instance)
			{
				__instance.working_loop.Enter(smi => SetPoweredExhaustActive(smi, true));
			}
		}
		static void SetPoweredExhaustActive(VentController.Instance smi, bool setActive)
		{
			if(smi.exhaust is PoweredExhaust)
			{
				smi.exhaust.operational.SetActive(smi.exhaust.operational.IsOperational && setActive);
			}
		}
	}
}
