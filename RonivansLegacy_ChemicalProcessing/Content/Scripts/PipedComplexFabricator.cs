using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class PipedComplexFabricator : ComplexFabricator
	{
		[SerializeField]
		public bool keepExcessGasses = false;

		public new void DropExcessIngredients(Storage storage)
		{
			HashSet<Tag> hashSet = new HashSet<Tag>();
			if (keepAdditionalTag != Tag.Invalid)
			{
				hashSet.Add(keepAdditionalTag);
			}

			for (int i = 0; i < recipe_list.Length; i++)
			{
				ComplexRecipe complexRecipe = recipe_list[i];
				if (IsRecipeQueued(complexRecipe))
				{
					ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
					foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
					{
						hashSet.Add(recipeElement.material);
					}
				}
			}

			for (int num = storage.items.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = storage.items[num];
				if (!(gameObject == null))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (!(component == null) && (!keepExcessLiquids || !component.Element.IsLiquid) && (!keepExcessGasses || !component.Element.IsGas))
					{
						KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
						if ((bool)component2 && !hashSet.Contains(component2.PrefabID()))
						{
							storage.Drop(gameObject);
						}
					}
				}
			}
		}
	}
}
