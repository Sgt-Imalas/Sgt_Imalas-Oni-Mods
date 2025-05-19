using HarmonyLib;
using UtilLibs;
using static ClaimNotification.STRINGS;

namespace ClaimNotification
{
	internal class Patches
	{
		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(TopLeftControlScreen))]
		[HarmonyPatch(nameof(TopLeftControlScreen.RefreshKleiItemDropButton))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			static bool HasShownInfo = false;
			public static void Postfix()
			{
				//SgtLogger.l("check run");
				if (
					!KleiItemDropScreen.HasItemsToShow() &&
					HasShownInfo)
				{
					//SgtLogger.l("no item found");
					HasShownInfo = false;
				}
				else
				{

					if (
						!HasShownInfo
						&& KleiItemDropScreen.HasItemsToShow()
						)
					{
						//SgtLogger.l("item found!");
						System.Action ShowScreen = () => UnityEngine.Object.FindObjectOfType<KleiItemDropScreen>(true).Show(true);
						System.Action close = () => { };

						KMod.Manager.Dialog(GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay),
					   CLAIMNOTIFICATION.TITLE,
					   CLAIMNOTIFICATION.TEXT,
				CLAIMNOTIFICATION.CONFIRM,
					   ShowScreen
					   , global::STRINGS.UI.CREDITSSCREEN.CLOSEBUTTON,
					   close);
						HasShownInfo = true;
					}
				}
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
