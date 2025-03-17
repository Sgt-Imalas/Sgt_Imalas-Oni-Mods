using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace MissingElementFixer
{
	internal class Patches
	{

        [HarmonyPatch(typeof(Constructable), nameof(Constructable.OnSpawn))]
        public class Constructable_OnSpawn_Patch
        {
            public static void Prefix(Constructable __instance)
            {
                var recipe = __instance.Recipe;
                var fixedElementList = new List<Tag>();
                for (int i = 0; i < __instance.SelectedElementsTags.Count(); i++)
                {
                    Recipe.Ingredient ingredient=null;
                    if(recipe.Ingredients.Count > i)
                    {
                        ingredient = recipe.Ingredients[i];                        
                    }


                    var elementInArray = __instance.SelectedElementsTags[i];
                    var Element = ElementLoader.GetElement(elementInArray);
                    if (Element == null)
                    {
                        Tag replacement;
                        var replacementOptions = ingredient.GetElementOptions();
                        if (replacementOptions.Any())
                        {
                            replacement = replacementOptions.First().tag;
						}
                        else
						{
							replacement = ElementLoader.GetElement(SimHashes.Steel.CreateTag()).tag;
							SgtLogger.warning("No replacement element found for " + elementInArray);
                        }

                        SgtLogger.l("Constructable " + STRINGS.UI.StripLinkFormatting(recipe.Name) + " had the invalid element \"" + elementInArray + "\", replacing it with "+ replacement);
                        fixedElementList.Add(replacement);
                    }
                    else
                        fixedElementList.Add(elementInArray);

				}
                __instance.SelectedElementsTags = fixedElementList;
			}
        }
	}
}
