using HarmonyLib;
using STRINGS;

namespace DlcSwapButton
{
	internal class Patches
	{
		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnPrefabInit))]
		public static class MainMenu_addOldButton
		{
			public static void Postfix(MainMenu __instance)
			{
				bool expansion1Active = DlcManager.IsExpansion1Active();
				System.Action action = () => { DlcManager.ToggleDLC("EXPANSION1_ID"); };

				var btn = __instance.MakeButton(new MainMenu.ButtonInfo(
					expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1,
					action,
					22, __instance.normalButtonStyle)
				);
				btn.gameObject.SetActive(false);

				btn.gameObject.SetActive(true);
			}
		}
	}
}
