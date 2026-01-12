using HarmonyLib;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UtilLibs;

namespace ItemDropPrevention.Patches
{
	internal class PauseScreen_Patches
	{
		private static readonly KButtonMenu.ButtonInfo CustomSettingsButton = new KButtonMenu.ButtonInfo(STRINGS.IDP_MOD_CONFIG.PAUSE_MENU_OPEN_CONFIG_BUTTONTEXT, Action.NumActions, new UnityAction(OnCustomMenuButtonPressed));


		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				PauseScreen_OnPrefabInit_Patch.ExecutePatch(Mod.Harmony);
			}
		}
		private static class PauseScreen_OnPrefabInit_Patch
		{
			public static void ExecutePatch(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("PauseScreen, Assembly-CSharp:ConfigureButtonInfos");
				var m_Postfix = AccessTools.Method(typeof(PauseScreen_OnPrefabInit_Patch), "Postfix");

				harmony.Patch(m_TargetMethod,
					null,//new HarmonyMethod(m_Prefix),
					new HarmonyMethod(m_Postfix),
					null //new HarmonyMethod(m_Transpiler)
						 );
			}

			public static void Postfix(ref IList<KButtonMenu.ButtonInfo> ___buttons)
			{
				SgtLogger.l("adding custom settings button");
				List<KButtonMenu.ButtonInfo> list = [.. ___buttons];
				CustomSettingsButton.isEnabled = true;

				var optionsButtons = list.FirstOrDefault(button => button.text == global::STRINGS.UI.FRONTEND.PAUSE_SCREEN.OPTIONS);
				int index = optionsButtons != null ? list.IndexOf(optionsButtons) + 1 : 5;

				list.Insert(index, CustomSettingsButton);
				___buttons = list;
			}
		}
		private static void OnCustomMenuButtonPressed()
		{
			PauseScreen.Instance?.Show(false);
			if (!SpeedControlScreen.Instance.IsPaused)
				SpeedControlScreen.Instance.Pause(false);
			POptions.ShowDialog(typeof(Config));
		}
	}
}
