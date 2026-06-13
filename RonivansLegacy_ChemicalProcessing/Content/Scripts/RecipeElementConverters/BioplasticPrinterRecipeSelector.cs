using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.Gaskets;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.RecipeElementConverters
{
	internal class BioplasticPrinterRecipeSelector : RecipeSelectorBase
	{
		const string ElementRecipeID = "BioplasticElementRecipe";
		const string GasketRecipeID = "BioplasticGasketRecipe";

		[SerializeField] public ElementConverter bioplasticConverter, gasketConverter;

		public override FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			return [
				new (ElementRecipeID, GetLabel(ModElements.BioPlastic_Solid),Def.GetUISprite(ElementLoader.GetElement(ModElements.BioPlastic_Solid.Tag)),GetDescriptor(bioplasticConverter)),
				new (GasketRecipeID, STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.AIO_BIOPLASTICGASKET.NAME ,Def.GetUISprite(BioPlasticGasketConfig.ID),GetDescriptor(gasketConverter)),
					];
		}
		public override void OnOptionSelected(Tag t)
		{
			SelectedRecipe = t;
			switch (t.ToString())
			{
				case ElementRecipeID:
					bioplasticConverter.SetWorkSpeedMultiplier(1);
					gasketConverter.SetWorkSpeedMultiplier(0);
					break;
				case GasketRecipeID:
					bioplasticConverter.SetWorkSpeedMultiplier(0);
					gasketConverter.SetWorkSpeedMultiplier(1);
					break;
				default:
					Debug.LogWarning($"[NitricAcidRecipeSelector] Unknown recipe selected: {t}");
					break;
			}
		}
	}
}
