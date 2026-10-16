using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SaveManager_Patches
	{
		/// <summary>
		/// register legacy building ids for save migration for merged buildings
		/// </summary>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.OnPrefabInit))]
        public class SaveManager_OnPrefabInit_Patch
        {
            public static void Postfix(SaveManager __instance)
			{
				BuildingManager.RegisterLegacyMigrations();
				BuildingDatabase.RegisterAdditionalMigrations();
			}
        }
	}
}
