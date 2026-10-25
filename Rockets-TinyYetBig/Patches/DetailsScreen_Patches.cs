using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens;
using System.Collections.Generic;
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
				
				UIUtils.AddCustomSideScreen<UpgradeListSidescreen>("RTB_ModuleUpgradeSidescreen", ModAssets.UpgradeSideScreenWindowPrefab);
				UIUtils.AddCustomSideScreen<DockingSidescreen>("RTB_DockingSidescreen", ModAssets.DockingSideScreenWindowPrefab);
				UIUtils.AddCustomSideScreen<SpaceConstructionSidescreen>("RTB_SpaceConstructionSidescreen", ModAssets.SpaceConstructionSideScreenWindowPrefab);

				UIUtils.AddClonedSideScreen<ResourceHarvestModuleHEPSideScreen>("HarvestModuleHEPSideScreen", "HarvestModuleSideScreen", typeof(HarvestModuleSideScreen));
				UIUtils.AddClonedSideScreen<DisableableCheckboxControlSideScreen>("DisableableCheckboxControlSideScreen", "SingleCheckboxSideScreen", typeof(SingleCheckboxSideScreen));
				UIUtils.AddClonedSideScreen<SpaceStationSideScreen>("SpaceStationSideScreen", "ClusterGridWorldSideScreen", typeof(ClusterGridWorldSideScreen));
			}
		}   
	}
}
