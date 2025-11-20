using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// Fixes the recipe material list extending beyond the height of the screen
	/// </summary>
	public sealed class SelectedRecipeQueueScreenSizeFix : PForwardedComponent
	{
		public static void Register()
		{
			new SelectedRecipeQueueScreenSizeFix().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);
		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod = AccessTools.Method(typeof(SelectedRecipeQueueScreen), nameof(SelectedRecipeQueueScreen.RefreshSizeScrollContainerSize));
				var postfixMethod = AccessTools.Method(typeof(SelectedRecipeQueueScreenSizeFix), nameof(SelectedRecipeQueueScreen_Postfix));
				plibInstance.Patch(targetMethod, postfix: new(postfixMethod));
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		public static void SelectedRecipeQueueScreen_Postfix(SelectedRecipeQueueScreen __instance)
		{
			float currentHeight = __instance.scrollContainer.minHeight;
			float uiScale = KPlayerPrefs.GetFloat(KCanvasScaler.UIScalePrefKey) / 100f;
			if (uiScale > 1f)
				uiScale *= uiScale; ///stronger reduction at higher ui scales to account for other elements above and below it taking more space

			float maxAvailableScreenSpaceAt1080p = 1080 - 448;
			if (__instance.RefreshMinionDisplayAnim())
				maxAvailableScreenSpaceAt1080p -= 128;

			float screenSizeRatio = maxAvailableScreenSpaceAt1080p / 1080f;

			float actualScreenHeight = Screen.height * screenSizeRatio;
			actualScreenHeight /= uiScale;

			__instance.scrollContainer.minHeight = Mathf.Min(currentHeight, actualScreenHeight);
		}
	}
}
