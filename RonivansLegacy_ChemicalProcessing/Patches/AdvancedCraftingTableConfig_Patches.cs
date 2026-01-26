using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class AdvancedCraftingTableConfig_Patches
	{

        [HarmonyPatch(typeof(AdvancedCraftingTableConfig), nameof(AdvancedCraftingTableConfig.ConfigureRecipes))]
        public class AdvancedCraftingTableConfig_ConfigureRecipes_Patch
        {
            public static void Postfix(AdvancedCraftingTableConfig __instance)
            {
                AdditionalRecipes.RegisterRecipes_AdvancedCraftingTable();
			}
        }
	}
}
