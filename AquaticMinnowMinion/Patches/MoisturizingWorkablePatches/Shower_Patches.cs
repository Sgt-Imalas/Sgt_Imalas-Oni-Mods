using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Patches.MoisturizingWorkablePatches
{
	internal class Shower_Patches
	{

        [HarmonyPatch(typeof(Shower), nameof(Shower.OnStartWork))]
        public class Shower_OnStartWork_Patch
		{
			public static void Postfix(WorkerBase worker) => ModAssets.StartMoisturizing(worker);
        }
	}
}
