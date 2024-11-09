using HarmonyLib;
using Klei.CustomSettings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UtilLibs;

namespace CustomGameSettingsModifier
{
	internal class Patches
	{
		// [HarmonyPatch(typeof(CustomGameSettings))]
		//[HarmonyPatch(nameof(CustomGameSettings.OnDeserialized))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("CustomGameSettings, Assembly-CSharp:OnDeserialized");
				//var m_Transpiler = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Transpiler");
				//var m_Prefix = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Prefix");
				var m_Postfix = AccessTools.Method(typeof(GeneratedBuildings_LoadGeneratedBuildings_Patch), "Postfix");

				harmony.Patch(m_TargetMethod,
					null,//new HarmonyMethod(m_Prefix),
					new HarmonyMethod(m_Postfix),
					null //new HarmonyMethod(m_Transpiler)
						 );
			}
			public static void Postfix(CustomGameSettings __instance)
			{
				if (!__instance.CurrentQualityLevelsBySetting.ContainsKey(CustomGameSettingConfigs.MeteorShowers.id))
				{
					__instance.CurrentQualityLevelsBySetting.Add(CustomGameSettingConfigs.MeteorShowers.id, "Default");
				}
			}
		}

		[HarmonyPatch(typeof(PauseScreen))]
		[HarmonyPatch(nameof(PauseScreen.OnKeyDown))]
		public static class CatchGoingBack
		{
			public static bool Prefix(KButtonEvent e)
			{
				if (CustomSettingsController.Instance != null && CustomSettingsController.Instance.CurrentlyActive)
					return false;
				return true;
			}
		}

		private static readonly KButtonMenu.ButtonInfo CustomSettingsButton = new KButtonMenu.ButtonInfo((string)STRINGS.UI.CUSTOMGAMESETTINGSCHANGER.BUTTONTEXT, Action.NumActions, new UnityAction(OnCustomMenuButtonPressed));
		private static void OnCustomMenuButtonPressed()
		{
			PauseScreen.Instance.RefreshButtons();
			CustomSettingsController.ShowWindow();
			GameScheduler.Instance.ScheduleNextFrame("OpenCustomSettings", (System.Action<object>)(_ =>
			{
				PauseScreen.Instance.RefreshButtons();
			}));
		}

		[HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
		public static class OnASsetPrefabPatch
		{
			public static void Postfix()
			{
				PauseScreen_OnPrefabInit_Patch.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
				GeneratedBuildings_LoadGeneratedBuildings_Patch.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
			}
		}



		//[HarmonyPatch(typeof(PauseScreen), "OnShow")]
		private static class PauseScreen_OnPrefabInit_Patch
		{
			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("PauseScreen, Assembly-CSharp:OnShow");
				//var m_Transpiler = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Transpiler");
				//var m_Prefix = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Prefix");
				var m_Postfix = AccessTools.Method(typeof(PauseScreen_OnPrefabInit_Patch), "Postfix");

				harmony.Patch(m_TargetMethod,
					null,//new HarmonyMethod(m_Prefix),
					new HarmonyMethod(m_Postfix),
					null //new HarmonyMethod(m_Transpiler)
						 );
			}

			private static void Postfix(ref IList<KButtonMenu.ButtonInfo> ___buttons)
			{
				SgtLogger.l("adding custom settings button");
				List<KButtonMenu.ButtonInfo> list = ___buttons.ToList<KButtonMenu.ButtonInfo>();
				CustomSettingsButton.isEnabled = true;
				list.Insert(5, CustomSettingsButton);
				___buttons = (IList<KButtonMenu.ButtonInfo>)list;
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}

