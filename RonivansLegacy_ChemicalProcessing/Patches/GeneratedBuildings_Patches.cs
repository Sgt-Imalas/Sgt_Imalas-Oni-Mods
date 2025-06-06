using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	public class GeneratedBuildings_Patches
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
				BuildingInjection.AddBuildingsToPlanscreen();
			}
		}
	}
}
