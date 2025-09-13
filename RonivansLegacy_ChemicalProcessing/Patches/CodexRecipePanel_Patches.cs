using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class CodexRecipePanel_Patches
	{
		[HarmonyPatch(typeof(CodexRecipePanel), nameof(CodexRecipePanel.ConfigureComplexRecipe))]
		public class CodexRecipePanel_ConfigureComplexRecipe_Patch
		{
			public static void Postfix(CodexRecipePanel __instance)
			{
				if (__instance == null || __instance.complexRecipe == null)
					return;

				if (RandomRecipeProducts.GetRandomOccurencesforRecipe(__instance.complexRecipe, out var occurence))
				{
					//expand the width of the container to fit the occurence entry
					__instance.fabricatorContainer.gameObject.GetComponent<LayoutElement>().preferredWidth = 160f;

					HierarchyReferences component = Util.KInstantiateUI(__instance.materialPrefab, __instance.fabricatorContainer, true)
						.GetComponent<HierarchyReferences>();
					Tuple<Sprite, Color> uiSprite = new(Assets.GetSprite("icon_mining_occurence"), UIUtils.rgb(204, 127, 5));
					var icon = component.GetReference<Image>("Icon");
					icon.sprite = uiSprite.first;
					icon.color = uiSprite.second;

					var amount = component.GetReference<LocText>("Amount");
					amount.text = occurence.GetOccurenceCompositionName(true);
					amount.color = Color.black;
					amount.enableWordWrapping = false;

					component.GetReference<ToolTip>("Tooltip").toolTip = occurence.GetOccurenceCompositionDescription();
					component.GetReference<KButton>("Button").interactable = false;
				}
				if (RandomRecipeProducts.GetRandomResultsforRecipe(__instance.complexRecipe, out var result))
				{
					HierarchyReferences component = Util.KInstantiateUI(__instance.materialPrefab, __instance.resultsContainer, true)
						.GetComponent<HierarchyReferences>();
					Tuple<Sprite, Color> uiSprite = new(Assets.GetSprite("unknown"), Color.black);
					var icon = component.GetReference<Image>("Icon");
					icon.sprite = uiSprite.first;
					icon.color = uiSprite.second;

					var amount = component.GetReference<LocText>("Amount");
					amount.text = result.GetProductCompositionName(true);
					amount.color = Color.black;

					component.GetReference<ToolTip>("Tooltip").toolTip = result.GetProductCompositionDescription();
					component.GetReference<KButton>("Button").interactable = false;

					__instance.title.text = __instance.complexRecipe.ingredients[0].material.ProperName();
				}
			}
		}
	}
}
