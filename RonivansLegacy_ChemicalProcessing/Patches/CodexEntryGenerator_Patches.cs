using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class CodexEntryGenerator_Patches
	{

		[HarmonyPatch(typeof(CodexEntryGenerator_Elements), nameof(CodexEntryGenerator_Elements.GenerateMadeAndUsedContainers))]
		public class CodexEntryGenerator_Elements_GenerateMadeAndUsedContainers_Patch
		{
			public static void Postfix(Tag tag, List<ContentContainer> containers)
			{
				List<ICodexWidget> randomProductEntries = new List<ICodexWidget>();
				List<ICodexWidget> randomOccurenceEntries = new List<ICodexWidget>();
				foreach (ComplexRecipe recipe in ComplexRecipeManager.Get().recipes)
				{
					if (Game.IsCorrectDlcActiveForCurrentSave(recipe))
					{
						if (RandomRecipeProducts.GetRandomResultsforRecipe(recipe, out var randomResults) && randomResults.HasTagInList(tag))
						{
							randomProductEntries.Add(new CodexRecipePanel(recipe, true));
						}
						if (RandomRecipeProducts.GetRandomOccurencesforRecipe(recipe, out var randomOccurences) && randomOccurences.HasTagInList(tag))
						{
							randomProductEntries.Add(new CodexRecipePanel(recipe, true));
						}
					}
				}
				ContentContainer contentsRandomProducts = new ContentContainer(randomProductEntries, ContentContainer.ContentLayout.Vertical);
				if (randomProductEntries.Any())
				{
					containers.Add(new ContentContainer(new List<ICodexWidget>()
					{
						new CodexSpacer(),
						new CodexCollapsibleHeader(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.NAME,string.Empty), contentsRandomProducts)
					}, ContentContainer.ContentLayout.Vertical));
					containers.Add(contentsRandomProducts);
				}
				ContentContainer contentsRandomOccurences = new ContentContainer(randomOccurenceEntries, ContentContainer.ContentLayout.Vertical);
				if (randomOccurenceEntries.Any())
				{
					containers.Add(new ContentContainer(new List<ICodexWidget>()
					{
						new CodexSpacer(),
						new CodexCollapsibleHeader(string.Format(STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS.RANDOMRECIPERESULT.NAME_OCCURENCE_FORMAT,string.Empty), contentsRandomOccurences)
					}, ContentContainer.ContentLayout.Vertical));
					containers.Add(contentsRandomOccurences);
				}
			}
		}
	}
}
