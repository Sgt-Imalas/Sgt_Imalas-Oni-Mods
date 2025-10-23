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
	internal class SulphuricAcidRecipeSelector : RecipeSelectorBase
	{
		const string SulphurRecipeID = "SulphuricAcid_from_Sulphur";
		const string PyriteRecipeID = "SulphuricAcid_from_Pyrite";

		[SerializeField] public ManualDeliveryKG sulphurDelivery, pyriteDelivery;
		[SerializeField] public ElementConverter sulphurConverter, pyriteConverter;

		[MyCmpGet] Storage storage;

		public override FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			return [
				new (SulphurRecipeID, GetLabel(SimHashes.Sulfur),Def.GetUISprite(ElementLoader.GetElement(SimHashes.Sulfur.CreateTag())),GetDescriptor(sulphurConverter)),
				new (PyriteRecipeID, GetLabel(SimHashes.FoolsGold),Def.GetUISprite(ElementLoader.GetElement(SimHashes.FoolsGold.CreateTag())),GetDescriptor(pyriteConverter)),
			];
		}
		public override void OnOptionSelected(Tag t)
		{
			SelectedRecipe = t;
			switch (t.ToString())
			{
				case SulphurRecipeID:
					sulphurDelivery.Pause(false, "needed");
					pyriteDelivery.Pause(true, "not needed");
					storage.DropHasTags([SimHashes.FoolsGold.CreateTag()]);
					storage.DropHasTags([SimHashes.Rust.CreateTag()]);
					sulphurConverter.SetWorkSpeedMultiplier(1);
					pyriteConverter.SetWorkSpeedMultiplier(0);
					break;
				case PyriteRecipeID:
					sulphurDelivery.Pause(true, "Not needed");
					pyriteDelivery.Pause(false, "needed");
					storage.DropHasTags([SimHashes.Sulfur.CreateTag()]);
					sulphurConverter.SetWorkSpeedMultiplier(0);
					pyriteConverter.SetWorkSpeedMultiplier(1);
					break;
				default:
					Debug.LogWarning($"[SulphuricAcidRecipeSelector] Unknown recipe selected: {t}");
					break;
			}
		}
	}
}
