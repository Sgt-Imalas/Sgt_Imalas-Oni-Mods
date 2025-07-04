using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
    class ColonyAchievment_Patches
    {
		[HarmonyPatch(typeof(ColonyAchievement), nameof(ColonyAchievement.IsValidForSave))]
		public static class SteamAheadAchievment_ExistingSaves
		{
			public static void Postfix(ColonyAchievement __instance, ref bool __result)
			{
				if (__result == false
					&& __instance.clusterTag != null
					&& DlcManager.IsContentSubscribed(DlcManager.DLC4_ID)
					&& Game.IsDlcActiveForCurrentSave(DlcManager.DLC4_ID)
					&& CustomGameSettings.Instance != null
					&& __instance.clusterTag == "DemoliorImperative"
					)
				{
					__result = CustomGameSettings.Instance.GetCurrentStories().Contains(CGMWorldGenUtils.CGM_Impactor_StoryTrait);
				}
			}
		}
    }
}
