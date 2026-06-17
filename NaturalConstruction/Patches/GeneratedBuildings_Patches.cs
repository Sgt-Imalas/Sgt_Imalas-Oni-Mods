using HarmonyLib;
using NaturalConstruction.Content.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace NaturalConstruction.Patches
{
	internal class GeneratedBuildings_Patches
	{

		[HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, NaturalTileBuildingConfig.ID, TileConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, NaturalBackwallBuildingConfig.ID, ExteriorWallConfig.ID);

				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.BasicFarming, NaturalTileBuildingConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.BasicFarming, NaturalBackwallBuildingConfig.ID);
			}
		}
	}
}
