using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class NitricAcidRecipeSelector : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		const string AmmoniaRecipeID = "NitricAcid_from_Ammonia";
		const string AcidRecipeID = "NitricAcid_from_SulphuricAcid";

		[SerializeField] public ManualDeliveryKG saltDelivery;
		[SerializeField] public ElementConverter ammoniaConverter, acidConverter;

		[Serialize] Tag SelectedRecipe = AcidRecipeID;
		static StringBuilder sb = new();

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			return new FewOptionSideScreen.IFewOptionSideScreen.Option[] {
				new (AmmoniaRecipeID, GetLabel(ModElements.Ammonia_Gas),Def.GetUISprite(ElementLoader.GetElement(ModElements.Ammonia_Gas.Tag)),GetDescriptor(ammoniaConverter)),
				new (AcidRecipeID, GetLabel(ModElements.SulphuricAcid_Liquid),Def.GetUISprite(ElementLoader.GetElement(ModElements.SulphuricAcid_Liquid.Tag)),GetDescriptor(acidConverter)),
			};
		}
		public Tag GetSelectedOption() => SelectedRecipe;
		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option) => OnOptionSelected(option.tag);
		static string GetLabel(SimHashes element)
		{
			return Strings.Get($"STRINGS.ELEMENTS.{element.ToString().ToUpperInvariant()}.NAME") + " → " + STRINGS.ELEMENTS.LIQUIDNITRIC.NAME;
		}
		string GetDescriptor(ElementConverter e)
		{
			sb.Clear();
			foreach (var desc in e.GetDescriptors(gameObject))
			{
				sb.AppendLine(desc.text);
			}
			return sb.ToString().TrimEnd();
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			OnOptionSelected(SelectedRecipe);
		}
		void OnOptionSelected(Tag t)
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
