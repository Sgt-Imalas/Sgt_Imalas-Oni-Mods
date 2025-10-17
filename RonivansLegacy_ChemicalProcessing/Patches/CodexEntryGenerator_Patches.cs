using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ModInfo;
using static RonivansLegacy_ChemicalProcessing.Patches.HPA.ConduitBridge_Patches;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class CodexEntryGenerator_Patches
	{
		[HarmonyPatch(typeof(CodexTemperatureTransitionPanel), nameof(CodexTemperatureTransitionPanel.ConfigureResults))]
		public class CodexTemperatureTransitionPanel_ConfigureResults_Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{

				MethodInfo replaceTransitionOre = AccessTools.Method(typeof(CodexTemperatureTransitionPanel_ConfigureResults_Patch), nameof(ReplaceTransitionOre));


				var lowTempOreTransition = AccessTools.Field(typeof(Element), nameof(Element.lowTempTransitionOreID));
				var highTempOreTransition = AccessTools.Field(typeof(Element), nameof(Element.highTempTransitionOreID));

				foreach (CodeInstruction original in orig)
				{
					if (original.LoadsField(lowTempOreTransition) || original.LoadsField(highTempOreTransition))
					{
						yield return original;
						yield return new CodeInstruction(OpCodes.Ldarg_0); //this CodexTemperatureTransitionPanel
						yield return new CodeInstruction(OpCodes.Call, replaceTransitionOre);
					}
					else
						yield return original;
				}
			}

			static SimHashes ReplaceTransitionOre(SimHashes transitionsIntoOriginalSimHash, CodexTemperatureTransitionPanel instance)
			{
				var transitionTarget = ElementLoader.FindElementByHash(transitionsIntoOriginalSimHash);
				if (transitionTarget == null)
					return transitionsIntoOriginalSimHash;


				var transitionThreshold = (instance.transitionType == CodexTemperatureTransitionPanel.TransitionType.HEAT) ? instance.sourceElement.highTemp : instance.sourceElement.lowTemp;

				if (!ModElements.IsModElement(transitionsIntoOriginalSimHash) && !ModElements.IsModElement(instance.sourceElement.id))
					return transitionsIntoOriginalSimHash;

				if (transitionThreshold > transitionTarget.highTemp && transitionTarget.highTempTransition != null)
					return transitionTarget.highTempTransition.id;
				else if (transitionThreshold < transitionTarget.lowTemp && transitionTarget.lowTempTransition != null)
					return transitionTarget.lowTempTransition.id;

				return transitionsIntoOriginalSimHash;
			}
		}


		[HarmonyPatch(typeof(CodexEntryGenerator_Elements), nameof(CodexEntryGenerator_Elements.GenerateElementDescriptionContainers))]
		public class CodexEntryGenerator_Elements_GenerateElementDescriptionContainers_Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				MethodInfo replaceTransitionOre_high = AccessTools.Method(typeof(CodexEntryGenerator_Elements_GenerateElementDescriptionContainers_Patch), nameof(ReplaceTransitionOre_HIGH));
				MethodInfo replaceTransitionOre_low = AccessTools.Method(typeof(CodexEntryGenerator_Elements_GenerateElementDescriptionContainers_Patch), nameof(ReplaceTransitionOre_LOW));
				MethodInfo ElementLoader_FindElementByHash = AccessTools.Method(typeof(ElementLoader), nameof(ElementLoader.FindElementByHash));

				var lowTempOreTransition = AccessTools.Field(typeof(Element), nameof(Element.lowTempTransitionOreID));
				var highTempOreTransition = AccessTools.Field(typeof(Element), nameof(Element.highTempTransitionOreID));


				foreach (CodeInstruction original in orig)
				{
					if (original.LoadsField(lowTempOreTransition))
					{
						yield return new CodeInstruction(OpCodes.Ldarg_0); //element
						yield return new CodeInstruction(OpCodes.Call, replaceTransitionOre_low);
						//yield return original;
					}
					else if (original.LoadsField(highTempOreTransition))
					{
						yield return new CodeInstruction(OpCodes.Ldarg_0); //element
						yield return new CodeInstruction(OpCodes.Call, replaceTransitionOre_high);
						//yield return original;
					}
					else
						yield return original;
				}
			}
			static SimHashes ReplaceTransitionOre_HIGH(Element transitionElement, Element currentElement)
			{
				var highTempOreTransition = ElementLoader.FindElementByHash(transitionElement.highTempTransitionOreID);

				if (highTempOreTransition == null)
					return transitionElement.highTempTransitionOreID;

				if (!ModElements.IsModElement(highTempOreTransition.id) && !ModElements.IsModElement(currentElement.id) && !ModElements.IsModElement(transitionElement.id))
					return transitionElement.highTempTransitionOreID;

				//highTempOreTransition check
				//one of the elements involved is from this mod, check if it has to be replaced with its low state transition

				//when the melting temp of the ore is lower than the melting temp of the material, show its melted state transition
				if (highTempOreTransition.highTemp + 3 < transitionElement.highTemp && highTempOreTransition.highTempTransition != null)
				{
					return highTempOreTransition.highTempTransition.id;
				}

				return transitionElement.highTempTransitionOreID;
			}
			static SimHashes ReplaceTransitionOre_LOW(Element element1, Element currentElement)
			{
				var lowTempOreTransition = ElementLoader.FindElementByHash(element1.lowTempTransitionOreID);

				if (lowTempOreTransition == null)
					return element1.lowTempTransitionOreID;

				if (!ModElements.IsModElement(lowTempOreTransition.id) && !ModElements.IsModElement(currentElement.id) && !ModElements.IsModElement(element1.id))
					return element1.lowTempTransitionOreID;

				//lowTempOreTransition check
				//one of the elements involved is from this mod, check if it has to be replaced with its high state transition

				//when the freezing temp of the ore is higher than the freezing temp of the material, show its frozen state transition
				if (lowTempOreTransition.lowTemp < element1.lowTemp && lowTempOreTransition.lowTempTransition != null)
				{
					return lowTempOreTransition.lowTempTransition.id;
				}


				return element1.lowTempTransitionOreID;
			}
		}


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
