using BubbleChest.Content.Defs.Buildings;
using HarmonyLib;
using UtilLibs;

namespace BubbleChest
{
	internal class Patches_
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
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, BubbleChestConfig.ID, ParkSignConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.InteriorDecor, BubbleChestConfig.ID);
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
