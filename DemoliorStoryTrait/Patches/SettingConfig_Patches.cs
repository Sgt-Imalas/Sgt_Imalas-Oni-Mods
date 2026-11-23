using HarmonyLib;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
	internal class SettingConfig_Patches
	{

		[HarmonyPatch(typeof(SettingConfig), nameof(SettingConfig.IsAvailableForCluster))]
		public class SettingConfig_IsAvailableForCluster_Patch
		{
			public static void Postfix(SettingConfig __instance, ref bool __result)
			{
				if (__result)
					return;
				if (__instance.id != "DemoliorDifficulty")
					return;

				if (CustomGameSettings.Instance.GetCurrentStories().Contains(CGMWorldGenUtils.CGM_Impactor_StoryTrait))
					__result = true;
			}
		}
	}
}
