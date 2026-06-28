using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Patches.MoisturizingWorkablePatches
{
	internal class SodaFountainWorkable_Patches
	{

        [HarmonyPatch(typeof(SodaFountainWorkable), nameof(SodaFountainWorkable.OnCompleteWork))]
        public class SodaFountainWorkable_OnCompleteWork_Patch
        {
            public static void Postfix(SodaFountainWorkable __instance, WorkerBase worker) => ModAssets.TriggerDrinkMoisturization(worker?.gameObject);
		}
	}
}
