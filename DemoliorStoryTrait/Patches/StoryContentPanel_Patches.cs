using HarmonyLib;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StoryContentPanel;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{

	class StoryContentPanel_Patches
	{
		static bool PendingOnToggle = false;
		/// <summary>			
		/// disable custom impactor story trait if it got rolled randomly on a cluster that already has an impactor asteroid			
		/// </summary>
		[HarmonyPatch(typeof(StoryContentPanel), nameof(StoryContentPanel.SelectRandomStories))]
		public class StoryContentPanel_SelectRandomStories_Patch
		{
			public static void Postfix(StoryContentPanel __instance, bool useBias)
			{
				string clusterId = __instance.mainScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.ClusterLayout);
				DisableImpactorOnRelica(clusterId, __instance);
			}
		}


		[HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.SetQualitySetting), [typeof(SettingConfig), typeof(string), typeof(bool)])]
		public class CustomGameSettings_SetQualitySetting_Patch
		{
			public static void Postfix(CustomGameSettings __instance, SettingConfig config, string value)
			{
				if (config != CustomGameSettingConfigs.ClusterLayout)
					return;

				DisableImpactorOnRelica(value);
			}
		}

		[HarmonyPatch(typeof(StoryContentPanel), nameof(StoryContentPanel.SetStoryState))]
		public class StoryContentPanel_SetStoryState_Patch
		{
			public static bool Prefix(StoryContentPanel __instance, string storyId, StoryState state)
			{
				if (storyId != CGMWorldGenUtils.CGM_Impactor_StoryTrait)
					return true;

				if (state == StoryState.Forbidden)
					return true;

				var clusterId = __instance.mainScreen.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.ClusterLayout);
				if (CGMWorldGenUtils.HasImpactorShowerInCluster(clusterId))
				{
					PendingOnToggle = true;
					return false;
				}
				return true;
			}
		}

		public static void DisableImpactorOnRelica(string clusterId, StoryContentPanel __instance = null)
		{
			var settings = CustomGameSettings.Instance;
			if (!settings.StorySettings.ContainsKey(CGMWorldGenUtils.CGM_Impactor_StoryTrait))
			{
				return;
			}

			if (CGMWorldGenUtils.HasImpactorShowerInCluster(clusterId))
			{
				var story = settings.StorySettings[CGMWorldGenUtils.CGM_Impactor_StoryTrait];
				var storyState = settings.GetCurrentStoryTraitSetting(story);

				bool currentlyEnabled = storyState.id == "Guaranteed";
				//SgtLogger.l("story trait is enabled: " + currentlyEnabled);
				if (!currentlyEnabled && !PendingOnToggle)
					return;

				if (__instance == null)
				{
					settings.SetStorySetting(story, false);
					foreach (var setting in settings.StorySettings)
					{
						SgtLogger.l(setting.Key + " " + setting.Value.id + " -> " + settings.GetCurrentStoryTraitSetting(setting.Key).id);
					}

					var currentlyDisabledTraits = settings.StorySettings
							.Where(pair => settings.GetCurrentStoryTraitSetting(pair.Key).id == "Disabled" && pair.Key != CGMWorldGenUtils.CGM_Impactor_StoryTrait);

					if (currentlyDisabledTraits == null || !currentlyDisabledTraits.Any())
						return;

					var enableRandomOtherInstead = currentlyDisabledTraits
						.GetRandom();

					settings.SetStorySetting(enableRandomOtherInstead.Value, true);
				}
				else
				{
					__instance.SetStoryState(CGMWorldGenUtils.CGM_Impactor_StoryTrait, StoryContentPanel.StoryState.Forbidden);

					var currentlyDisabledTraits = __instance.storyStates
						.Where(pair => pair.Value == StoryContentPanel.StoryState.Forbidden && pair.Key != CGMWorldGenUtils.CGM_Impactor_StoryTrait);

					if (currentlyDisabledTraits == null || !currentlyDisabledTraits.Any())
						return;

					var enableRandomOtherInstead = currentlyDisabledTraits
						.GetRandom();

					__instance.SetStoryState(enableRandomOtherInstead.Key, StoryContentPanel.StoryState.Guaranteed);
				}
				PendingOnToggle = false;
			}
		}
	}
}
