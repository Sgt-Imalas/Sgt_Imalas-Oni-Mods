using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.UI;

namespace Rockets_TinyYetBig.Patches
{
	internal class LaunchPadSideScreen_Patches
	{
        static KButton BlueprintsButton;

        [HarmonyPatch(typeof(LaunchPadSideScreen), nameof(LaunchPadSideScreen.OnSpawn))]
        public class LaunchPadSideScreen_OnSpawn_Patch
        {
            public static void Postfix(LaunchPadSideScreen __instance)
            {
				InitBlueprintsButton(__instance);
            }

			private static void InitBlueprintsButton(LaunchPadSideScreen instance)
			{
                var reference = instance.startNewRocketbutton;
                BlueprintsButton = Util.KInstantiateUI<KButton>(reference.gameObject, reference.transform.parent.gameObject, true);

                UIUtils.TryChangeText(BlueprintsButton.transform, "Label", Strings.Get("STRINGS.UI.ROCKETBLUEPRINTS_SECONDARYSIDESCREEN.TITLE.TITLETEXT").ToString().ToUpperInvariant());
                BlueprintsButton.ClearOnClick();
                BlueprintsButton.onClick += ()=> OnBlueprintsButtonClicked(instance);
				BlueprintsButton.interactable = true;
			}
		}
        static void OnBlueprintsButtonClicked(LaunchPadSideScreen instance)
        {
			RocketBlueprintsSecondarySidescreen newScreen = (RocketBlueprintsSecondarySidescreen)DetailsScreen.Instance.SetSecondarySideScreen(ModAssets.RocketBlueprintSecondarySideScreen, "Rocket Blueprints");
			newScreen.OpenedFrom(instance);
		}
	}
}
