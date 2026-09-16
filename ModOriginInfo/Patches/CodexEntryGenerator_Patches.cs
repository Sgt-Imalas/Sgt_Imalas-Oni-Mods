using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace ModOriginInfo.Patches
{
	internal class CodexEntryGenerator_Patches
	{

		[HarmonyPatch(typeof(CodexEntryGenerator), nameof(CodexEntryGenerator.GenerateBuildingDescriptionContainers))]
		public class CodexEntryGenerator_GenerateBuildingDescriptionContainers_Patch
		{
			public static void Postfix(BuildingDef def, List<ContentContainer> containers)
			{
				string modOrigin = ModAssets.GetModNameIfValid(def, 0);
				if (modOrigin.Any() && containers.Any())
				{
					containers.Add(new ContentContainer(new List<ICodexWidget>
						{
							new CodexSpacer(),
							new CodexText(modOrigin)
						}, ContentContainer.ContentLayout.Vertical));
				}
			}
		}


		[HarmonyPatch(typeof(CodexEntryGenerator_Elements), nameof(CodexEntryGenerator_Elements.GenerateMadeAndUsedContainers))]
		public class CodexEntryGenerator_Elements_GenerateMadeAndUsedContainers_Patch
		{
			public static void Prefix(Tag tag, List<ContentContainer> containers)
			{
				if (!ModAssets.IsBuilding(tag) && ModAssets.IsModded(tag, out _))
				{
					string modOrigin = ModAssets.GetModNameIfValid(tag, 0);
					if (modOrigin.Any() && containers.Any())
					{
						containers.Add(new ContentContainer(new List<ICodexWidget>
						{
							new CodexSpacer(),
							new CodexText(modOrigin)
						}, ContentContainer.ContentLayout.Vertical));
					}
				}
			}
		}
	}
}
