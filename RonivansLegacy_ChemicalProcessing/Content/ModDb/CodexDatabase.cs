using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class CodexDatabase
	{
		static bool entriesGenerated = false;
		internal static void GenerateGuidanceDeviceEntries()
		{
			if (entriesGenerated)
				return;
			entriesGenerated = true;

			string parentID = CodexCache.FormatLinkID(ModAssets.Tags.MineralProcessing_GuidanceUnit.ToString());

			var entry = CodexCache.FindEntry(parentID);

			foreach (GameObject go1 in Assets.GetPrefabsWithComponent<ProgrammableGuidanceModule>())
			{
				var prefabTag = go1.GetComponent<KPrefabID>().PrefabTag;
				SgtLogger.l("Creating sub entry for " + prefabTag);

				List<ContentContainer> contentContainerList = new List<ContentContainer>();
				CodexEntryGenerator.GenerateTitleContainers(go1.GetProperName(), contentContainerList);
				Sprite first = Def.GetUISprite(go1).first;
				CodexEntryGenerator.GenerateImageContainers(first, contentContainerList);
				List<ICodexWidget> content = new List<ICodexWidget>();
				content.Add((ICodexWidget)new CodexText(go1.GetComponent<InfoDescription>().description));
				content.Add((ICodexWidget)new CodexSpacer());


				//content.Add(new CodexEntry_MadeAndUsed() { tag = prefabTag.ToString() });
				contentContainerList.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
				CodexEntryGenerator_Elements.GenerateMadeAndUsedContainers(prefabTag, contentContainerList);
				string id = prefabTag.ToString();
				if (entry.subEntries.Find((Predicate<SubEntry>)(x => x.id == id)) == null)
					CodexCache.FindEntry(parentID).subEntries.Add(new SubEntry(id, parentID, contentContainerList, go1.GetProperName())
					{
						icon = first
					});
			}

		}
	}
}
