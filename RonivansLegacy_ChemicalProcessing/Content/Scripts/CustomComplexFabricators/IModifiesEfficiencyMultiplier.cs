using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal interface IModifiesEfficiencyMultiplier
	{
		float ApplyEfficiencyModifierChanges(float modifier);
		bool Multiplicative { get; }
	}
}
