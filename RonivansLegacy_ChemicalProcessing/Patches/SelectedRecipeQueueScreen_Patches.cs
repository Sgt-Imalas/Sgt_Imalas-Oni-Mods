using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class SelectedRecipeQueueScreen_Patches
    {

        [HarmonyPatch(typeof(SelectedRecipeQueueScreen), nameof(SelectedRecipeQueueScreen.GetResultDescriptions))]
        public class SelectedRecipeQueueScreen_GetResultDescriptions_Patch
        {
            public static void Postfix(SelectedRecipeQueueScreen __instance, List<SelectedRecipeQueueScreen.DescriptorWithSprite> __result, ComplexRecipe recipe)
			{
				if (RandomRecipeResults.GetRandomOccurencesforRecipe(recipe, out var occurence))
				{
					__result.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(
						new(occurence.GetOccurenceCompositionName(),
						occurence.GetOccurenceCompositionDescription()),
						new(Assets.GetSprite("icon_mining_occurence"), UIUtils.rgb(204, 127, 5)))
						);
				}
				if (RandomRecipeResults.GetRandomResultsforRecipe(recipe, out var result))
				{
                    __result.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(
                        new(result.GetProductCompositionName(),
						result.GetProductCompositionDescription()),
                        new(Assets.GetSprite("unknown"), Color.black))
                        );
				}
			}
        }
    }
}
