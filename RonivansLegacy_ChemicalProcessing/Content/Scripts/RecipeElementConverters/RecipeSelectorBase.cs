using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.RecipeElementConverters
{
	public abstract class RecipeSelectorBase : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen
	{
		const string AmmoniaRecipeID = "NitricAcid_from_Ammonia";
		const string AcidRecipeID = "NitricAcid_from_SulphuricAcid";

		[Serialize] protected Tag SelectedRecipe;
		protected static StringBuilder sb = new();

		public abstract FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions();
		public Tag GetSelectedOption() => SelectedRecipe;
		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option) => OnOptionSelected(option.tag);
		public static string GetLabel(SimHashes element)
		{
			return Strings.Get($"STRINGS.ELEMENTS.{element.ToString().ToUpperInvariant()}.NAME");// + " → " + STRINGS.ELEMENTS.LIQUIDNITRIC.NAME;
		}
		public string GetDescriptor(ElementConverter e)
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
			if(SelectedRecipe == null)
				SelectedRecipe = GetOptions().First().tag;

			OnOptionSelected(SelectedRecipe);
		}
		public abstract void OnOptionSelected(Tag t);
	}
}
