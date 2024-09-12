using HarmonyLib;
using UtilLibs;

namespace LogicRoomMassSensor
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
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Automation, RoomPressureSensorConfig.ID);
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
		[HarmonyPatch(typeof(ThresholdSwitchSideScreen), "OnSpawn")]
		public static class adjustlimit
		{
			public static void Postfix(ThresholdSwitchSideScreen __instance)
			{
				if (__instance.numberInput.maxValue < RoomPressureSensorConfig.maxValue)
					__instance.numberInput.maxValue = RoomPressureSensorConfig.maxValue;
			}
		}
	}
}
