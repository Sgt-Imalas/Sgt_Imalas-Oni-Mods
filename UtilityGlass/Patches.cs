using HarmonyLib;
using UtilLibs;

namespace UtilityGlass
{
	internal class Patches
	{
		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, ReinforcedGlassConfig.ID, BunkerTileConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, ExteriorGlassWallConfig.ID, ExteriorWallConfig.ID);
			}
		}


		/// <summary>
		/// Adds buildings to technology tree
		/// </summary>
		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				//add buildings to technology tree
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.GlassBlowing, ExteriorGlassWallConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, ReinforcedGlassConfig.ID);
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}
