namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal interface IModifiesEfficiencyMultiplier
	{
		float ApplyEfficiencyModifierChanges(float modifier);
		bool Multiplicative { get; }
	}
}
