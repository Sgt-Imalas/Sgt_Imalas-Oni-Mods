using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Db_Patches
	{
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Prefix()
			{
				SkinDatabase.AddSkins();
			}
			public static void Postfix(Db __instance)
			{
				BuildingManager.AddBuildingsToTechs();
				ModElements.OverrideDebrisAnims();
				SpaceMiningAdditions.AddExtraPOIElements();
				StatusItemsDatabase.CreateStatusItems();
				ModTechs.RegisterCustomEntries();
				ModElements.RegisterTraitElementBandModifications();
			}
		}
	}
}
