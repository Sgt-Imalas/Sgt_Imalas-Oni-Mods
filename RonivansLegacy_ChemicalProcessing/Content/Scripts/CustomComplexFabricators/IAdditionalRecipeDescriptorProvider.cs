using System.Collections.Generic;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	public interface IAdditionalRecipeDescriptorProvider
	{
		public List<Descriptor> GetAdditionalRecipeEffects(ComplexRecipe recipe);
	}
}
