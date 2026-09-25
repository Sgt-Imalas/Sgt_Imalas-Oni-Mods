using Rockets_TinyYetBig.Content.Defs.Entities;
using Rockets_TinyYetBig.Elements;
using UtilLibs;
using static ComplexRecipe;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class ModRecipes
	{
		internal static void AdditionalRecipes_SupermaterialRefinery()
		{
			if (!Config.Instance.NeutroniumMaterial)
				return;
			RecipeBuilder.Create(SupermaterialRefineryConfig.ID,40f)
				.Input(ModElements.UnobtaniumDust, 10f)
				.Input(SimHashes.Steel, 90f)
				.Output(ModElements.UnobtaniumAlloy, 100f)
				.Description(STRINGS.ELEMENTS.UNOBTANIUMALLOY.RECIPE_DESCRIPTION)
				.NameDisplay(RecipeNameDisplay.Result)
				.RequiresTech(GameStrings.Technology.ColonyDevelopment.DurableLifeSupport)
				.Build();
		}
		internal static void AdditionalRecipes_CraftingTable()
		{
			if(!Config.Instance.SpaceStationsAndTech)
				return;

			RecipeBuilder.Create(CraftingTableConfig.ID, "Create " + STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RTB_EMPTYDATACARD.NAME_PLURAL + " from steel and plastic metals. Recipe wip.", 100)
				.Input(SimHashes.Steel.CreateTag(), 4)
				.Input(SimHashes.Polypropylene.CreateTag(), 6)
				.Output(EmptyDataCardConfig.TAG, 1)
				.Build();
		}
		public static void AdditionalRecipes_ClothingFabricator()
		{
			if (!Config.Instance.SpaceStationsAndTech)
				return;

			RecipeBuilder.Create(ClothingFabricatorConfig.ID,100)
				.Input(SimHashes.RefinedCarbon,95)
				.Input(SimHashes.NaturalResin, 5)
				.Output(SimHashes.CarbonFibre, 50)
				.Build();
		}
	}
}
