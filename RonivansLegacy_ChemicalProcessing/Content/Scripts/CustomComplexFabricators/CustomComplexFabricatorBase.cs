using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class CustomComplexFabricatorBase : ComplexFabricator
	{
		IAdditionalRecipeDescriptorProvider[] additionalRecipeDescriptors;
		IOnRecipeCompeteActionProvider[] onRecipeCompeteActionProviders;
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
				GameObject storageItem = storage.items[num];
				if (storageItem != null)
				{
					bool shouldDrop = true;
					if (storageItem.TryGetComponent<PrimaryElement>(out var primaryElement))
					{
						if (keepExcessLiquids && primaryElement.Element.IsLiquid
						|| keepExcessGasses && primaryElement.Element.IsGas)
							shouldDrop = false;
					}
					if (storageItem.TryGetComponent<KPrefabID>(out var prefabID))
					{
						if (shouldDrop && hashSet.Contains(prefabID.PrefabID()))
							shouldDrop = false;
						if (shouldDrop)
						{
							foreach (var tag in prefabID.Tags)
							{
								if (hashSet.Contains(tag))
								{
									shouldDrop = false;
									break;
								}
							}
						}
					}
					if (shouldDrop)
						storage.Drop(storageItem);
				}
			}
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			additionalRecipeDescriptors = GetComponents<IAdditionalRecipeDescriptorProvider>();
			onRecipeCompeteActionProviders = GetComponents<IOnRecipeCompeteActionProvider>();
		}
		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			foreach(var provider in onRecipeCompeteActionProviders)
				provider.OnRecipeCompletedAction(recipe);
			return base.SpawnOrderProduct(recipe);
		}

		public override List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
		{
			var descriptors = base.AdditionalEffectsForRecipe(recipe);

			foreach (var provider in additionalRecipeDescriptors)
			{
				var additionalDesc = provider.GetAdditionalRecipeEffects(recipe);
				if(additionalDesc.Any())
					descriptors.AddRange(additionalDesc);
			}
			return descriptors;
		}
	}
}
