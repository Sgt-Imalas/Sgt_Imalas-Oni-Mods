using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.RecipeElementConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class NitricAcidRecipeSelector : RecipeSelectorBase
	{
		const string AmmoniaRecipeID = "NitricAcid_from_Ammonia";
		const string AcidRecipeID = "NitricAcid_from_SulphuricAcid";

		[SerializeField] public ManualDeliveryKG saltDelivery;
		[SerializeField] public ElementConverter ammoniaConverter, acidConverter;

		public override FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			return [
				new (AcidRecipeID, GetLabel(ModElements.SulphuricAcid_Liquid),Def.GetUISprite(ElementLoader.GetElement(ModElements.SulphuricAcid_Liquid.Tag)),GetDescriptor(acidConverter)),
				new (AmmoniaRecipeID, GetLabel(ModElements.Ammonia_Gas),Def.GetUISprite(ElementLoader.GetElement(ModElements.Ammonia_Gas.Tag)),GetDescriptor(ammoniaConverter)),
			];
		}
		public override void OnOptionSelected(Tag t)
		{
			SelectedRecipe = t;
			switch (t.ToString())
			{
				case AmmoniaRecipeID:
					saltDelivery.Pause(true, "Not needed");
					ammoniaConverter.SetWorkSpeedMultiplier(1);
					acidConverter.SetWorkSpeedMultiplier(0);
					break;
				case AcidRecipeID:
					saltDelivery.Pause(false, "needed");
					ammoniaConverter.SetWorkSpeedMultiplier(0);
					acidConverter.SetWorkSpeedMultiplier(1);
					break;
				default:
					Debug.LogWarning($"[NitricAcidRecipeSelector] Unknown recipe selected: {t}");
					break;
			}
		}
	}
}
