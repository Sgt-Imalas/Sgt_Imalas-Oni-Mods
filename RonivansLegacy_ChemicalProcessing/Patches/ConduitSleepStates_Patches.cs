using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class ConduitSleepStates_Patches
	{

		[HarmonyPatch(typeof(ConduitSleepStates), nameof(ConduitSleepStates.GetSleepingLayer))]
		public class ConduitSleepStates_GetSleepingLayer_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.Drywall_Hides_Pipes;
			public static void Postfix(ref Grid.SceneLayer __result)
			{
				__result = ModAssets.AboveDrywallLayer;
			}
		}
	}
}
