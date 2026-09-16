using HarmonyLib;
using UnityEngine;

namespace ModOriginInfo.Patches
{
	internal class SimpleInfoScreen_Patches
	{

		const string panelID = "ModOriginInfo_PanelLabel";
		static string ModnameDesc = null;
		[HarmonyPatch(typeof(SimpleInfoScreen), nameof(SimpleInfoScreen.RefreshInfoPanel))]
		public class SimpleInfoScreen_RefreshInfoPanel_Patch
		{
			public static void Prefix(SimpleInfoScreen __instance, CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
			{
				if (targetEntity == null)
					return;
				if (targetEntity.TryGetComponent<KPrefabID>(out var kprefabid) && ModAssets.IsModded(kprefabid))
				{
					ModnameDesc = ModAssets.GetModNameIfValid(kprefabid, 1);
				}
			}
		}

		[HarmonyPatch(typeof(CollapsibleDetailContentPanel), nameof(CollapsibleDetailContentPanel.Commit))]
		public class CollapsibleDetailContentPanel_Commit_Patch
		{
			public static void Prefix(CollapsibleDetailContentPanel __instance)
			{
				if (ModnameDesc == null)
					return;
				__instance.SetLabel(panelID, ModnameDesc, string.Empty);
				ModnameDesc = null;
			}
		}
	}
}
