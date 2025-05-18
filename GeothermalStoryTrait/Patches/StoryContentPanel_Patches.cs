using HarmonyLib;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static StoryContentPanel;

namespace GeothermalStoryTrait.Patches
{
	class StoryContentPanel_Patches
	{

		[HarmonyPatch(typeof(ColonyDestinationSelectScreen), nameof(ColonyDestinationSelectScreen.OnAsteroidClicked))]
		public class ColonyDestinationSelectScreen_OnAsteroidClicked_Patch
		{
			public static void Postfix(ColonyDestinationSelectScreen __instance)
			{
				DisableHeatpumpOnCeres(__instance.storyContentPanel);
			}
		}

		/// <summary>			
		/// disable custom heatpump story trait if it got rolled randomly on a cluster that already has a heatpump			
		/// </summary>
		[HarmonyPatch(typeof(StoryContentPanel), nameof(StoryContentPanel.SelectRandomStories))]
		public class StoryContentPanel_SelectRandomStories_Patch
		{
			public static void Postfix(StoryContentPanel __instance, bool useBias)
			{
				DisableHeatpumpOnCeres(__instance);
			}
		}


		[HarmonyPatch(typeof(StoryContentPanel), nameof(StoryContentPanel.SetStoryState))]
		public class StoryContentPanel_SetStoryState_Patch
		{
			public static bool Prefix(StoryContentPanel __instance, string storyId, StoryState state)
			{
				if (storyId != CGMWorldGenUtils.CGM_Heatpump_StoryTrait)
					return true;

				if (state == StoryState.Forbidden)
					return true;

				var clusterId = __instance.mainScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.ClusterLayout);
				if (CGMWorldGenUtils.HasGeothermalPumpInCluster(clusterId))
					return false;
				return true;
			}
		}

		public static void DisableHeatpumpOnCeres(StoryContentPanel __instance)
		{
			if (!__instance.storyStates.ContainsKey(CGMWorldGenUtils.CGM_Heatpump_StoryTrait))
				return;
			var clusterId = __instance.mainScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.ClusterLayout);

			if (CGMWorldGenUtils.HasGeothermalPumpInCluster(clusterId))
			{
				if (__instance.storyStates[CGMWorldGenUtils.CGM_Heatpump_StoryTrait] == StoryContentPanel.StoryState.Forbidden)
					return;

				__instance.SetStoryState(CGMWorldGenUtils.CGM_Heatpump_StoryTrait, StoryContentPanel.StoryState.Forbidden);

				var currentlyDisabledTraits = __instance.storyStates
					.Where(pair => pair.Value == StoryContentPanel.StoryState.Forbidden && pair.Key != CGMWorldGenUtils.CGM_Heatpump_StoryTrait);
				if (currentlyDisabledTraits == null || !currentlyDisabledTraits.Any())
					return;

					var enableRandomOtherInstead = currentlyDisabledTraits
						.Select(pair => pair.Key)
						.GetRandom();
				__instance.SetStoryState(enableRandomOtherInstead, StoryContentPanel.StoryState.Guaranteed);

			}
		}
	}
}
