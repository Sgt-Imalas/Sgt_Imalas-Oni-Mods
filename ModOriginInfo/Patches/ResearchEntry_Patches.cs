using HarmonyLib;

namespace ModOriginInfo.Patches
{

	class ResearchEntry_Patches
	{
		[HarmonyPatch(typeof(ResearchEntry), nameof(ResearchEntry.SetTech))]
		public class ResearchEntry_SetTech_Patch
		{
			/// <summary>
			/// sets the mod info tooltip for a modded building
			/// </summary>
			/// <param name="__instance"></param>
			public static void Postfix(ResearchEntry __instance)
			{
				int index = 1; //child 0 is the icon prefab

				var container = __instance.iconPanel.transform;
				foreach (TechItem unlockedItem in __instance.targetTech.unlockedItems)
				{
					if (!Game.IsCorrectDlcActiveForCurrentSave(unlockedItem))
						continue;
					var child = container.GetChild(index++);
					if (child == null
						|| !ModAssets.IsModdedResearchItem(unlockedItem.Id)
						|| !child.TryGetComponent<ToolTip>(out var tt))
						continue;

					tt.AddMultiStringTooltip(ModAssets.GetResearchModNameIfValid(unlockedItem.Id, 1), PluginAssets.Instance.defaultTextStyleSetting); 
				}
			}
		}
	}
}
