using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace GeothermalStoryTrait.Patches
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
					&& DlcManager.IsContentSubscribed(DlcManager.DLC2_ID)
					&& CustomGameSettings.Instance != null
					&& Game.IsDlcActiveForCurrentSave(DlcManager.DLC2_ID)
					&& __instance.clusterTag == "GeothermalImperative"
					)
				{
					__result = CustomGameSettings.Instance.GetCurrentStories().Contains(CGMWorldGenUtils.CGM_Heatpump_StoryTrait);
				}
			}
		}
	}
}
