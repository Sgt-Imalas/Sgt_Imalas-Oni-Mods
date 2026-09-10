using AkisDecorPackB.Content.Defs.Buildings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisDecorPackB.Content.ModDb
{
	internal class Mod_Db
	{
		public static FloorLampPanes FloorLampPanes { get; set; }
		public static BigFossilVariants BigFossils { get; set; }

		public static Dictionary<SimHashes, List<IWeighted>> treasureHunterLoottable = new Dictionary<SimHashes, List<IWeighted>>()
		{

		};

		public static void PostDbInit(global::Db __instance)
		{
			FloorLampPanes = new FloorLampPanes();
			BigFossils = new BigFossilVariants();
			ModSkillPerks.Register(__instance);
			ModStatusItems.Register(__instance.BuildingStatusItems);
			RegisterBuildings();
		}

		static void RegisterBuildings()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, FountainConfig.ID, MarbleSculptureConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, FossilDisplayConfig.ID, CeilingFossilSculptureConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, OilLanternConfig.ID, FloorLampConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, PotConfig.ID, StorageLockerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, GiantFossilDisplayConfig.ID, FossilDisplayConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, FloorLightConfig.ID, TileConfig.ID);

			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.FineArt, FountainConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.EnvironmentalAppreciation, FossilDisplayConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.EnvironmentalAppreciation, GiantFossilDisplayConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, FloorLightConfig.ID);

		}

		public static class BuildLocationRules
		{
			public static BuildLocationRule OnAnyWall = (BuildLocationRule)(-1569291063);
			public static BuildLocationRule GiantFossilRule = (BuildLocationRule)Hash.SDBMLower("DecorPackB_FloorOrHanging");
		}
	}
}
