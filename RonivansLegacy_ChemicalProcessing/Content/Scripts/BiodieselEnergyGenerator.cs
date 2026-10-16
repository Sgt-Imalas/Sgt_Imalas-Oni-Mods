using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class BiodieselEnergyGenerator : EnergyGenerator
	{
		[SerializeField]
		public Formula vanillaDieselFormula;
		[SerializeField]
		public Formula modDieselFormula;

		public override void EnergySim200ms(float dt)
		{
			SetCurrentFormula();
			base.EnergySim200ms(dt);
		}
		void SetCurrentFormula()
		{
			float modDieselAmount = storage.GetAmountAvailable(ModElements.BioDiesel_Liquid.Tag);
			float vanillaDieselAmount = storage.GetAmountAvailable(SimHashes.RefinedLipid.CreateTag());
			bool useModDiesel = modDieselAmount >= vanillaDieselAmount;
			formula = useModDiesel ? modDieselFormula : vanillaDieselFormula;
		}
	}
}
