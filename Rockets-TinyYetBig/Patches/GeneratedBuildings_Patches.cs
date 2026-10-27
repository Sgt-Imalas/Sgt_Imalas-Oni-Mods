using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations;

namespace Rockets_TinyYetBig.Patches
{
	internal class GeneratedBuildings_Patches
	{
		/// <summary>
		/// This patch registers all buildings to the build screen 
		/// and all rocket modules to the rocket module selection screen and their building categories
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Postfix()
			{
				BuildingDatabase.MoveVanillaBuildings();
				BuildingManager.AddBuildingsToPlanScreen();
			}
		}
	}
}
