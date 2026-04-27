using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TMPro;
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

				TMP_FontAsset text = __instance.materialPrefab.GetComponent<HierarchyReferences>().GetReference<LocText>("Amount").font;



				if (RecipeCondenser.IsDerivedRecipe(__instance.complexRecipe, out ComplexRecipe sourceRecipe))
				{
					SgtLogger.l("derived recipe detected: " + __instance.complexRecipe.id + " is derived from " + sourceRecipe.id);
					foreach (var uiEntiry in __instance.ingredientsContainer.transform)
					{
						(uiEntiry as Transform).gameObject.SetActive(false);
					}
					AddMultiIngredientVisualizers(__instance, __instance.complexRecipe);
				}


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

					var containerLE = __instance.fabricatorContainer.GetComponent<LayoutElement>();
					containerLE.preferredWidth = containerLE.preferredWidth + 40f;

					var amount = component.GetReference<LocText>("Amount");
					amount.text = occurence.GetOccurenceCompositionName(true);
					amount.color = Color.black;
					amount.textWrappingMode = TextWrappingModes.NoWrap;

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
					amount.textWrappingMode = TextWrappingModes.NoWrap;

					var containerLE = __instance.resultsContainer.GetComponent<LayoutElement>();
					containerLE.preferredWidth = containerLE.preferredWidth + 40f;

					component.GetReference<ToolTip>("Tooltip").toolTip = result.GetProductCompositionDescription();
					component.GetReference<KButton>("Button").interactable = false;

					__instance.title.text = __instance.complexRecipe.ingredients[0].material.ProperName();
				}
			}
			static GameObject MultiEntryPrefab = null;
			//static void InstantiatePrefab(CodexRecipePanel instance)
			//{
			//	if (MultiEntryPrefab == null)
			//	{
			//		//MultiEntryPrefab = ModAssets.MultiIngredientCodexVisualizer;
			//		MultiEntryPrefab = Util.KInstantiateUI(instance.materialPrefab);
			//		var le = MultiEntryPrefab.GetComponent<LayoutElement>();
			//		le.minWidth = 90f;
			//		le.minHeight = 90f;

			//		var container = new GameObject("RotatableContainer");
			//		container.transform.SetParent(MultiEntryPrefab.transform);
			//		var iconRef = MultiEntryPrefab.GetComponent<HierarchyReferences>().GetReference<Image>("Icon");
			//		var icon = Util.KInstantiateUI(iconRef.gameObject, container, true);
			//		icon.name = "RotatablePrefab";
			//		var imgLE = icon.GetComponent<LayoutElement>();
			//		imgLE.minWidth = 20f;
			//		imgLE.minHeight = 20f;
			//	}

			//}

			private static void AddMultiIngredientVisualizers(CodexRecipePanel instance, ComplexRecipe originalRecipe)
			{
				//InstantiatePrefab(instance);

				var ingredientsMap = RecipeCondenser.GetAllIngredientVariants(originalRecipe);
				for (int i = 0; i < ingredientsMap.Length; i++)
				{
					var ingredientVariants = ingredientsMap[i];

					var originalIngredient = originalRecipe.ingredients[i];

					MultiIngredientCodexVisualizer visualizer = Util.KInstantiateUI<MultiIngredientCodexVisualizer>(ModAssets.MultiIngredientCodexVisualizer, instance.ingredientsContainer, true);
					visualizer.SetDisplayedIngredients(ingredientVariants, originalIngredient.material);
				}
			}
		}
	}
}
