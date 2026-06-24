using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Patches.MoisturizingWorkablePatches
{
	internal class SaunaWorkable_Patches
	{

        [HarmonyPatch(typeof(SaunaWorkable), nameof(SaunaWorkable.OnStartWork))]
        public class SaunaWorkable_OnStartWork_Patch
		{
			public static void Postfix(WorkerBase worker) => ModAssets.StartMoisturizing(worker);
		}
	}
}
