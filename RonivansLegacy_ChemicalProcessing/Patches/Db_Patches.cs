using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Db_Patches
	{
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Prefix()
			{
			}
			public static void Postfix(Db __instance)
			{
				ModPersonalities.Register(__instance.Personalities);
				BuildingInjection.AddBuildingsToTech();
				ModElements.OverrideDebrisAnims();
				HarvestablePOIAdditions.AddExtraPOIElements();
			}
		}
	}
}
