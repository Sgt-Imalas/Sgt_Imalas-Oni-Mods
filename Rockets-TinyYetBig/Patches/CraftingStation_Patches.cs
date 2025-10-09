using HarmonyLib;
using Rockets_TinyYetBig.Content.Defs.Entities;
using Rockets_TinyYetBig.Content.ModDb;
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
				ModRecipes.AdditionalRecipes_CraftingTable();
			}
		}
	}
}
