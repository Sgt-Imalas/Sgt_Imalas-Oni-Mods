using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SaveGame_Patches
	{
		[HarmonyPatch(typeof(SaveGame), nameof(SaveGame.OnPrefabInit))]
		public class SaveGame_OnPrefabInit_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Toxicity;
			public static void Postfix(SaveGame __instance)
			{
				__instance.gameObject.AddOrGet<ToxicityGrid>();
			}
		}
	}
}
