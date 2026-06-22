using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace MinnowStoryTrait.Patches
{
	internal class ColonyAchievment_Patches
	{
		[HarmonyPatch(typeof(ColonyAchievement), nameof(ColonyAchievement.IsValidForSave))]
		public static class BetterTogether_ExistingSaves
		{
			public static void Postfix(ColonyAchievement __instance, ref bool __result)
			{
				if (__result == false
					&& __instance.clusterTag != null
					&& DlcManager.IsContentSubscribed(DlcManager.DLC5_ID)
					&& CustomGameSettings.Instance != null
					&& Game.IsDlcActiveForCurrentSave(DlcManager.DLC5_ID)
					&& __instance.clusterTag == ModAssets.ACHIEVMENT_TAG
					)
				{
					__result = CustomGameSettings.Instance.GetCurrentStories().Contains(CGMWorldGenUtils.CGM_Minnow_StoryTrait);
				}
			}
		}
	}
}
