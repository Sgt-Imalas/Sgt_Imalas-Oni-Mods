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
