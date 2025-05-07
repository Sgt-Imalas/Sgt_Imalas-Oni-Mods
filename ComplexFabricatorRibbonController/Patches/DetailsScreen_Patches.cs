using ComplexFabricatorRibbonController.Content.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ComplexFabricatorRibbonController.Patches
{
    class DetailsScreen_Patches
    {
		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class DetailsScreen_OnPrefabInit_Patch
		{
			public static void Postfix()
			{
				UIUtils.AddCustomSideScreen<RibbonRecipeController_Sidescreen>("ComplexFabricatorRibbonControlSidescreen", ModAssets.RecipeSelectionSidescreenGO);
			}
		}
	}
}
