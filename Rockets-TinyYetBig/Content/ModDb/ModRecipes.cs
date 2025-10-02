using Rockets_TinyYetBig.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ComplexRecipe;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class ModRecipes
	{
		internal static void AdditionalRecipes_SupermaterialRefinery()
		{
			RecipeBuilder.Create(SupermaterialRefineryConfig.ID,40f)
				.Input(ModElements.UnobtaniumDust, 10f)
				.Input(SimHashes.Steel, 90f)
				.Output(ModElements.UnobtaniumAlloy, 100f)
				.Description(STRINGS.ELEMENTS.UNOBTANIUMALLOY.RECIPE_DESCRIPTION)
				.NameDisplay(RecipeNameDisplay.Result)
				.RequiresTech(GameStrings.Technology.ColonyDevelopment.DurableLifeSupport)
				.Build();
		}
	}
}
