using HarmonyLib;
using Rockets_TinyYetBig.Content.Defs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	class CraftingStation_Patches
	{

		[HarmonyPatch(typeof(CraftingTableConfig), nameof(CraftingTableConfig.ConfigureRecipes))]
		public class CraftingTableConfig_ConfigureRecipes_Patch
		{
			/// <summary>
			/// placeholder recipe for empty data card, to be replaced by a proper recipe later.
			/// </summary>
			/// <param name="__instance"></param>
			public static void Postfix(CraftingTableConfig __instance)
			{
				RecipeBuilder.Create(CraftingTableConfig.ID, "Create " + STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RTB_EMPTYDATACARD.NAME_PLURAL + " from steel and plastic metals. Recipe wip.", 100)
					.Input(SimHashes.Steel.CreateTag(), 4)
					.Input(SimHashes.Polypropylene.CreateTag(), 6)
					.Output(EmptyDataCardConfig.TAG, 1)
					.Build();
			}
		}
	}
}
