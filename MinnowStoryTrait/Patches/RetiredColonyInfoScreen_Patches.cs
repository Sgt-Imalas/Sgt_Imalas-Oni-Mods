using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace MinnowStoryTrait.Patches
{
    class RetiredColonyInfoScreen_Patches
	{
		[HarmonyPatch(typeof(RetiredColonyInfoScreen), nameof(RetiredColonyInfoScreen.IsAchievementValidForDLCContext))]
		public static class Fix_existing_games_with_aquatic
		{
			public static void Postfix(RetiredColonyInfoScreen __instance, ref bool __result, IHasDlcRestrictions restrictions, string clusterTag)
			{
				if (__result == false
					&& clusterTag != null
					&& DlcManager.IsContentSubscribed(DlcManager.DLC5_ID)
					&& Game.IsCorrectDlcActiveForCurrentSave(restrictions)
					&& clusterTag == ModAssets.ACHIEVMENT_TAG
					&& CustomGameSettings.Instance != null
					)
				{
					__result = CustomGameSettings.Instance.GetCurrentStories().Contains(CGMWorldGenUtils.CGM_Minnow_StoryTrait);
				}
			}
		}		
	}
}
