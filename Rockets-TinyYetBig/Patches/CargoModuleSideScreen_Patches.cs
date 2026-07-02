using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class CargoModuleSideScreen_Patches
	{

        [HarmonyPatch(typeof(CargoModuleSideScreen), nameof(CargoModuleSideScreen.RefreshProgressBars))]
        public class CargoModuleSideScreen_RefreshProgressBars_Patch
        {
            public static void Postfix(CargoModuleSideScreen __instance)
			{
				if (__instance.targetCraft.IsNullOrDestroyed() || ClusterMapSelectTool.Instance.GetSelected() == null || !__instance.IsValidForTarget(ClusterMapSelectTool.Instance.GetSelected().gameObject))
					return;

				foreach(var collector in __instance.modulePanels.Keys)
				{
					if(collector is ExplorerModuleTelescope.Instance)
					{
						var progressBar = __instance.modulePanels[collector]?.GetComponent<HierarchyReferences>()?.GetReference<GenericUIProgressBar>("gatheringProgressBar");
						float remainingCapacity = collector.GetCapacity() - collector.GetMassStored();
						if (progressBar != null)
						{
							if (collector.CheckIsCollecting())
								progressBar.label.SetText(STRINGS.UI_MOD.UISIDESCREENS.SCANNERMODULE_GATHERINGSTRINGS.GATHERING_IN_PROGRESS);
							else if(remainingCapacity > 0f)
								progressBar.label.SetText(STRINGS.UI_MOD.UISIDESCREENS.SCANNERMODULE_GATHERINGSTRINGS.GATHERING_STOPPED);
						}
					}
				}

			}
        }
	}
}
