using AkisDecorPackB.Content.Defs.Buildings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisDecorPackB.Patches
{
	internal class GeneratedBuildings_Patch
	{

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, FountainConfig.ID,  MarbleSculptureConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, FossilDisplayConfig.ID,  CeilingFossilSculptureConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, OilLanternConfig.ID,  FloorLampConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, PotConfig.ID, StorageLockerConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, GiantFossilDisplayConfig.ID, FossilDisplayConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, FloorLightConfig.ID, TileConfig.ID);

				InjectionMethods.AddBuildingToTechnology( GameStrings.Technology.Decor.FineArt, FountainConfig.ID);
				InjectionMethods.AddBuildingToTechnology( GameStrings.Technology.Decor.EnvironmentalAppreciation, FossilDisplayConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.EnvironmentalAppreciation, GiantFossilDisplayConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, FloorLightConfig.ID);
			}
		}
	}
}
