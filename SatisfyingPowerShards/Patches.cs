using HarmonyLib;

namespace SatisfyingPowerShards
{
	internal class Patchess
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
				//ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
			}
		}
	}
}
