using CannedFoods.Foods;
using HarmonyLib;
using UtilLibs;

namespace CannedFoods
{
    class StringPatches
    {
		[HarmonyPatch(typeof(EntityConfigManager))]
		[HarmonyPatch(nameof(EntityConfigManager.LoadGeneratedEntities))]
		public class EntityConfigManager_LoadGeneratedEntities_Patch
		{
			public static void Prefix()
			{
				InjectionMethods.AddFoodStrings(CannedBBQConfig.ID, STRINGS.ITEMS.FOOD.CF_CANNEDBBQ.NAME, STRINGS.ITEMS.FOOD.CF_CANNEDBBQ.DESC);
				InjectionMethods.AddFoodStrings(CannedTunaConfig.ID, STRINGS.ITEMS.FOOD.CF_CANNEDTUNA.NAME, STRINGS.ITEMS.FOOD.CF_CANNEDTUNA.DESC);
			}
		}
	}
}
