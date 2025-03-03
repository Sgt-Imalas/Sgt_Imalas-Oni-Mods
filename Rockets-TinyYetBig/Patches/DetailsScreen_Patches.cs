using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    internal class DetailsScreen_Patches
	{
		[HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.OnPrefabInit))]
		public static class CustomSideScreenPatch_InitCustomScreens
		{
			public static void Postfix(DetailsScreen __instance)
			{
				SgtLogger.l("Registering RE sidescreens");
				UIUtils.AddCustomSideScreen<DockingSidescreen>("RTB_DockingSidescreen", ModAssets.DockingSideScreenWindowPrefab);
				UIUtils.AddCustomSideScreen<SpaceConstructionSidescreen>("RTB_SpaceConstructionSidescreen", ModAssets.SpaceConstructionSideScreenWindowPrefab);
			}
		}
	}
}
