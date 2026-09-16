using BioluminescentDupes.Content.Scripts;
using HarmonyLib;
using UnityEngine;

namespace BioluminescentDupes.Patches
{
	internal class BaseMinionPatch
	{
		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.BaseMinion))]
		public class Minion_AddFlipper_Patch
		{
			public static void Postfix(GameObject __result)
			{
				__result.AddOrGet<BioluminescenceColorSelectable>();
			}

		}
	}
}
