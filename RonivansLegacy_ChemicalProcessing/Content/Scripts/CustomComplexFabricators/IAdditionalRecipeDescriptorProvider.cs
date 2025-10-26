using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	public interface IAdditionalRecipeDescriptorProvider
	{
		public List<Descriptor> GetAdditionalRecipeEffects(ComplexRecipe recipe);
	}
}
