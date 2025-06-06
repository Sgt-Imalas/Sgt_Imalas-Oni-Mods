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
				//SpaceStationRoomsDB.PatchRoomsConstructor();
			}
			public static void Postfix(Db __instance)
			{
				BuildingInjection.AddBuildingsToTech();
			}
		}
	}
}
