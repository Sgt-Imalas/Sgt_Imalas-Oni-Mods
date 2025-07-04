using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
    class RetiredColonyInfoScreen_Patches
    {
		[HarmonyPatch(typeof(RetiredColonyInfoScreen), nameof(RetiredColonyInfoScreen.IsAchievementValidForDLCContext))]
		public static class Fix_existing_games_with_demolior
		{
			public static void Postfix(RetiredColonyInfoScreen __instance, ref bool __result, IHasDlcRestrictions restrictions, string clusterTag)
			{
				if (__result == false
					&& clusterTag != null
					&& DlcManager.IsContentSubscribed(DlcManager.DLC4_ID)
					&& Game.IsCorrectDlcActiveForCurrentSave(restrictions)
					&& clusterTag == "DemoliorImperative"
					&& CustomGameSettings.Instance != null
					)
				{
					__result = CustomGameSettings.Instance.GetCurrentStories().Contains(CGMWorldGenUtils.CGM_Impactor_StoryTrait);
				}
			}
		}
	}
}
