using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class SaveGame_Patches
	{
		[HarmonyPatch(typeof(SaveGame), nameof(SaveGame.OnPrefabInit))]
		public class SaveGame_OnPrefabInit_Patch
		{
			public static void Postfix(SaveGame __instance)
			{
				__instance.gameObject.AddOrGet<RandomTickManager>();
			}
		}
	}
}
