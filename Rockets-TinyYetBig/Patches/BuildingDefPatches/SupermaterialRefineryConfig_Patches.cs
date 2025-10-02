using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;
using UtilLibs;
using Rockets_TinyYetBig.Elements;
using Rockets_TinyYetBig.Content.ModDb;

namespace Rockets_TinyYetBig.Patches.ElementPatches
{
	internal class SupermaterialRefineryConfig_Patches
	{
		/// <summary>
		/// adding the neutronium alloy recipe to the supermaterial refinery
		/// </summary>
		[HarmonyPatch(typeof(SupermaterialRefineryConfig), nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
		public static class Patch_SupermaterialRefineryConfig_ConfigureRecipes
		{
			public static void Postfix() => ModRecipes.AdditionalRecipes_SupermaterialRefinery();
		}
	}
}
