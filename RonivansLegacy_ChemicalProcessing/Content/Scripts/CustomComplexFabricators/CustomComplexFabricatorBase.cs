using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class CustomComplexFabricatorBase : ComplexFabricator
	{
		IAdditionalRecipeDescriptorProvider[] additionalRecipeDescriptors;
		public override void OnSpawn()
		{
			base.OnSpawn();
			additionalRecipeDescriptors = GetComponents<IAdditionalRecipeDescriptorProvider>();
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
