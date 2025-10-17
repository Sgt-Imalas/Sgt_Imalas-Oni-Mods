using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Pump_Patches
	{

		[HarmonyPatch(typeof(Pump), nameof(Pump.IsPumpable))]
		public class Pump_IsPumpable_Patch
		{
			public static void Postfix(Pump __instance, Element.State expected_state, ref bool __result)
			{
				if (__instance is FilterablePump pump)
					__result = pump.CanPump();
			}
		}
	}
}
