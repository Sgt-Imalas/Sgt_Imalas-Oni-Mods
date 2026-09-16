using HarmonyLib;
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
