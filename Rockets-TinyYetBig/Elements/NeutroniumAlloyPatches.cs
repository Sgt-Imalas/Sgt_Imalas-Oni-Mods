using HarmonyLib;
using Klei.AI;
using System.Collections.Generic;
using UtilLibs;
using static ComplexRecipe;

namespace Rockets_TinyYetBig.Elements
{
	public class NeutroniumAlloyPatches
	{
		[HarmonyPatch(typeof(SupermaterialRefineryConfig))]
		[HarmonyPatch(nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
		public static class Patch_CraftingTableConfig_ConfigureRecipes
		{
			public static void Postfix()
			{
				AddAlloyRecipe();
			}
			private static void AddAlloyRecipe()
			{
				RecipeElement[] input = new RecipeElement[]
				{
					new RecipeElement(ModElements.UnobtaniumDust.Tag, 10f),
					new RecipeElement(SimHashes.Steel.CreateTag(), 90f),
				};

				RecipeElement[] output = new RecipeElement[]
				{
					new RecipeElement(ModElements.UnobtaniumAlloy.Tag, 100f)
				};

				string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
				new ComplexRecipe(recipeID, input, output)
				{
					time = 40f,
					description = STRINGS.ELEMENTS.UNOBTANIUMALLOY.RECIPE_DESCRIPTION,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
				}.requiredTech = GameStrings.Technology.ColonyDevelopment.DurableLifeSupport;
			}
		}

		[HarmonyPatch(typeof(LegacyModMain))]
		[HarmonyPatch(nameof(LegacyModMain.ConfigElements))]
		public static class Add_NeutroniumAlloy_Effects
		{
			public static void Postfix()
			{
				var UnobtaniumAlloy = ModElements.UnobtaniumAlloy.Get();

				//string OverheatId = Db.Get().BuildingAttributes.OverheatTemperature.Id;
				//Dictionary<SimHashes, float> ModifierOverrides = new Dictionary<SimHashes, float>();
				//ModifierOverrides.Add(SimHashes.Copper, 200f);
				//ModifierOverrides.Add(SimHashes.Gold, 200f);
				//ModifierOverrides.Add(SimHashes.Lead, 200f);

				//foreach(var elementOverride in ModifierOverrides)
				//{
				//    var ElementEntry = ElementLoader.GetElement(elementOverride.Key.CreateTag());
				//    if(ElementEntry != null)
				//    {
				//        ElementEntry.attributeModifiers.RemoveAll(item => item.AttributeId == OverheatId);
				//        ElementEntry.attributeModifiers.Add(new AttributeModifier(OverheatId, elementOverride.Value, ElementEntry.name));
				//    }
				//}


				UnobtaniumAlloy.attributeModifiers.Add(new AttributeModifier("Decor", 1.0f, UnobtaniumAlloy.name, true));
				UnobtaniumAlloy.attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, 2000f, UnobtaniumAlloy.name));
			}
		}
	}
}
