using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static PoisNotIncluded.ModAssets;

namespace PoisNotIncluded
{
	internal class Patches
	{

		public const string StoryTraitsCategory = "pni_storytraits";

		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			static Dictionary<string, string> GravitasBuildingIds = new Dictionary<string, string>() {
				{GravitasCreatureManipulatorConfig.ID,StoryTraitsCategory}// critter flux
				,{MorbRoverConfig.ID ,StoryTraitsCategory }//morb rover maker
				,{GravitasContainerConfig.ID,StoryTraitsCategory } //braintank pajamas cubby
				,{MegaBrainTankConfig.ID,StoryTraitsCategory } //braintank story trait		
				,{LonelyMinionHouseConfig.ID,StoryTraitsCategory } //minion storytrait		
				,{FossilDigSiteConfig.ID,StoryTraitsCategory } //fossil storytrait			
				//,GravitasLabLightConfig.ID ///appears glowing without light		  }
				,{PropGravitasWallConfig.ID   , GameStrings.PlanMenuSubcategory.Tiles}
				,{PropGravitasWallPurpleConfig.ID  , GameStrings.PlanMenuSubcategory.Tiles }
				,{PropGravitasWallPurpleWhiteDiagonalConfig.ID  , GameStrings.PlanMenuSubcategory.Tiles}
				,{POIDlc2ShowroomDoorConfig.ID  , GameStrings.PlanMenuSubcategory.Doors }
				,{GravitasDoorConfig.ID ,GameStrings.PlanMenuSubcategory.Doors }
				,{POIDoorInternalConfig.ID  ,  GameStrings.PlanMenuSubcategory.Doors }
				,{POIBunkerExteriorDoor.ID   , GameStrings.PlanMenuSubcategory.Doors }
				,{POIFacilityDoorConfig.ID   , GameStrings.PlanMenuSubcategory.Doors }
				,{TilePOIConfig.ID  ,GameStrings.PlanMenuSubcategory.Tiles  }
				,{MouldingTileConfig.ID  ,  GameStrings.PlanMenuSubcategory.Tiles  }
				,{PropGravitasLabWallConfig.ID   ,GameStrings.PlanMenuSubcategory.Tiles  }
				,{PropGravitasLabWindowConfig.ID   , GameStrings.PlanMenuSubcategory.Tiles  }
				,{PropGravitasLabWindowHorizontalConfig.ID  ,GameStrings.PlanMenuSubcategory.Tiles }
				,{FacilityBackWallWindowConfig.ID  ,GameStrings.PlanMenuSubcategory.Tiles }
			};

			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix()
			{
				var AllDefs = Assets.BuildingDefs;
				var gravitasDefs = AllDefs.Where(def => GravitasBuildingIds.Keys.Contains(def.PrefabID));
				foreach (var def in gravitasDefs)
					def.ShowInBuildMenu = true;

				var defIds = gravitasDefs.Select(def => def.PrefabID).ToList();


				foreach (var id in new List<string>(defIds))
				{
					if (TUNING.BUILDINGS.PLANORDER.Any(category => category.buildingAndSubcategoryData.Any(comp => comp.Key == id)))
						defIds.Remove(id);
				}

				SgtLogger.l(gravitasDefs.Count() + "x poi buildings");

				PlanScreen.PlanInfo planInfo = new PlanScreen.PlanInfo(POI_Category, false, []);


				TUNING.BUILDINGS.PLANORDER.Add(planInfo);
				foreach(var id in defIds)
				{
					InjectionMethods.AddBuildingToPlanScreen(POI_Category, id, GravitasBuildingIds[id]);
				}
			}
		}


		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Prefix()
			{
				ModAssets.AddSkins();
			}
			public static void Postfix(Db __instance)
			{
				PlanScreen.IconNameMap.Add(HashCache.Get().Add(POI_Category), "icon_category_lights");
			}
		}

		[HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.LoadEntities))]
		public class LegacyModMain_LoadEntities_Patch
		{
			public static void Postfix(LegacyModMain __instance)
			{
				Strings.Add($"STRINGS.UI.NEWBUILDCATEGORIES.{StoryTraitsCategory.ToUpperInvariant()}.NAME", STRINGS.UI.SANDBOXTOOLS.SETTINGS.SPAWN_STORY_TRAIT.NAME);
				Strings.Add($"STRINGS.UI.NEWBUILDCATEGORIES.{StoryTraitsCategory.ToUpperInvariant()}.BUILDMENUTITLE", STRINGS.UI.SANDBOXTOOLS.SETTINGS.SPAWN_STORY_TRAIT.NAME);
				Strings.Add($"STRINGS.UI.NEWBUILDCATEGORIES.{StoryTraitsCategory.ToUpperInvariant()}.TOOLTIP", STRINGS.UI.SANDBOXTOOLS.SETTINGS.SPAWN_STORY_TRAIT.TOOLTIP);
				Strings.Add($"STRINGS.UI.NEWBUILDCATEGORIES.{"exploration".ToUpperInvariant()}.NAME", STRINGS.CODEX.CRITTERSTATUS.CRITTERSTATUS_TITLE);
				Strings.Add($"STRINGS.UI.NEWBUILDCATEGORIES.{"exploration".ToUpperInvariant()}.BUILDMENUTITLE", STRINGS.CODEX.CRITTERSTATUS.CRITTERSTATUS_TITLE);
				Strings.Add($"STRINGS.UI.NEWBUILDCATEGORIES.{"exploration".ToUpperInvariant()}.TOOLTIP", "");
				Strings.Add($"STRINGS.UI.BUILDCATEGORIES.{POI_Category.ToUpperInvariant()}.NAME", STRINGS.UI.CLUSTERMAP.POI.TITLE);
				Strings.Add($"STRINGS.UI.BUILDCATEGORIES.{ POI_Category.ToUpperInvariant()}.TOOLTIP", "");


				ModAssets.TryRegisterDynamicGravitasBuilding(FossilSiteConfig_Ice.ID, StoryTraitsCategory,  isEntitySpawner: true, materialOverride: [SimHashes.Fossil.CreateTag().ToString()], costOverride: [4000]);
				ModAssets.TryRegisterDynamicGravitasBuilding(FossilSiteConfig_Resin.ID, StoryTraitsCategory, isEntitySpawner: true, materialOverride: [SimHashes.Fossil.CreateTag().ToString()], costOverride: [4000]);
				ModAssets.TryRegisterDynamicGravitasBuilding(FossilSiteConfig_Rock.ID, StoryTraitsCategory, isEntitySpawner: true, materialOverride: [SimHashes.Fossil.CreateTag().ToString()], costOverride: [4000]);


				ModAssets.TryRegisterDynamicGravitasBuilding("PropCeresPosterA", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropCeresPosterB", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropCeresPosterLarge", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropClock", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropClothesHanger", GameStrings.PlanMenuSubcategory.Decor, materialOverride: [GameTags.Metal.ToString()]);
				//ModAssets.TryRegisterDynamicGravitasBuilding("PropClothesHanger", animOverride: "off", materialOverride: [GameTags.Metal.ToString()]);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropDesk",  GameStrings.PlanMenuSubcategory.Wellness);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropDlc2GeothermalCart", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropElevator", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropExoShelfLong", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropExoShelfShort", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityChair", GameStrings.PlanMenuSubcategory.Wellness);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityChairFlip", GameStrings.PlanMenuSubcategory.Wellness);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityCouch", GameStrings.PlanMenuSubcategory.Wellness, new(5, 0));
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityDesk", GameStrings.PlanMenuSubcategory.Wellness);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityDisplay", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityDisplay2", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityDisplay3", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasDisplay4", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropDlc2Display1", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityGlobeDroors", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropFacilityPainting", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityStatue", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropFacilityTable", GameStrings.PlanMenuSubcategory.Wellness, new(2, 0));
				ModAssets.TryRegisterDynamicGravitasBuilding("PropFacilityWallDegree", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding_Ceiling("PropGravitasCeilingRobot", GameStrings.PlanMenuSubcategory.Manufacturing, new(0, 5));
				ModAssets.TryRegisterDynamicGravitasBuilding("PropGravitasCreaturePoster", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasDeskPodium", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropGravitasFireExtinguisher", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasFirstAidKit", GameStrings.PlanMenuSubcategory.Medical);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasFloorRobot", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropGravitasHandScanner", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasJar1", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasJar2", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasLabTable", GameStrings.PlanMenuSubcategory.Wellness);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasRobitcTable", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropGravitasShelf", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropGravitasSmallSeedLocker", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropGravitasToolCrate", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropGravitasToolShelf", GameStrings.PlanMenuSubcategory.Manufacturing);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropHumanChesterfieldChair", GameStrings.PlanMenuSubcategory.Wellness, new(3, 2));
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropHumanChesterfieldSofa", GameStrings.PlanMenuSubcategory.Wellness, new(5, 2));
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropHumanMurphyBed", GameStrings.PlanMenuSubcategory.Wellness, new Tuple<int, int>(7, 4));
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropReceptionDesk", GameStrings.PlanMenuSubcategory.Wellness);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropSkeleton", GameStrings.PlanMenuSubcategory.Decor);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropSurfaceSatellite1", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropSurfaceSatellite2", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropSurfaceSatellite3", GameStrings.PlanMenuSubcategory.Exploration);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropTable", GameStrings.PlanMenuSubcategory.Wellness);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropTallPlant", GameStrings.PlanMenuSubcategory.Wellness);

				ModAssets.TryRegisterDynamicGravitasBuilding_Backwall("PropGravitasDecorativeWindow", GameStrings.PlanMenuSubcategory.Tiles);

				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("MissileSetLocker", GameStrings.PlanMenuSubcategory.Storage);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("PropExoSetLocker", GameStrings.PlanMenuSubcategory.Storage);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("SetLocker", GameStrings.PlanMenuSubcategory.Storage);
				ModAssets.TryRegisterDynamicGravitasBuilding_Floor("VendingMachine", GameStrings.PlanMenuSubcategory.Storage);

				ModAssets.TryRegisterDynamicGravitasBuilding("FossilBitsLarge", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, materialOverride: [SimHashes.Fossil.CreateTag().ToString()]);
				ModAssets.TryRegisterDynamicGravitasBuilding("FossilBitsSmall", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, materialOverride: [SimHashes.Fossil.CreateTag().ToString()]);


				ModAssets.TryRegisterDynamicGravitasBuilding("PropLight", GameStrings.PlanMenuSubcategory.Lights, BuildLocationRule.OnCeiling, brokenName: true);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropFacilityHangingLight", GameStrings.PlanMenuSubcategory.Lights, BuildLocationRule.OnCeiling, brokenName: true);
				ModAssets.TryRegisterDynamicGravitasBuilding("PropFacilityChandelier", GameStrings.PlanMenuSubcategory.Lights, BuildLocationRule.OnCeiling, brokenName: true);

				ModAssets.TryRegisterDynamicGravitasBuilding_Lamp("PropLight", "setpiece_light_kanim", new Tuple<string, string>("misc", "on"), "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.NAME", "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.DESC", 1, 1, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, 10, 1800, 8, LIGHT2D.CEILINGLIGHT_OFFSET, new(0, 0), LightShape.Cone);
				ModAssets.TryRegisterDynamicGravitasBuilding_Lamp("CeilingLight_Pretty", "ceilinglight_pretty_kanim", new Tuple<string, string>("off", "on"), "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.NAME", "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.DESC", 1, 1, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, 10, 1800, 8, LIGHT2D.CEILINGLIGHT_OFFSET, new(0, 0), LightShape.Cone);
				ModAssets.TryRegisterDynamicGravitasBuilding_Lamp("GravitasLabLight", "gravitas_lab_light_kanim", new Tuple<string, string>("on", "off"), "STRINGS.BUILDINGS.PREFABS.GRAVITASLABLIGHT.NAME", "STRINGS.BUILDINGS.PREFABS.GRAVITASLABLIGHT.DESC", 1, 1, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, 20, 2400, 10, LIGHT2D.CEILINGLIGHT_OFFSET, new(0, 0), LightShape.Cone);
				ModAssets.TryRegisterDynamicGravitasBuilding_Lamp("PropFacilityHangingLight", "gravitas_light_kanim", new Tuple<string, string>("off", "on"), "STRINGS.BUILDINGS.PREFABS.PROPFACILITYLAMP.NAME", "STRINGS.BUILDINGS.PREFABS.PROPFACILITYLAMP.DESC", 1, 4, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, 120, 8000, 16, new(0, 0.75f), new(0, 4), LightShape.Circle);
				ModAssets.TryRegisterDynamicGravitasBuilding_Lamp("PropFacilityChandelier", "gravitas_chandelier_kanim", new Tuple<string, string>("off", "on"), "STRINGS.BUILDINGS.PREFABS.PROPFACILITYCHANDELIER.NAME", "STRINGS.BUILDINGS.PREFABS.PROPFACILITYCHANDELIER.DESC", 5, 7, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, 480, 18000, 24, new(0, 2), new(0, 6), LightShape.Circle);

				RegisterNewBuilding("WhiteBoard", GameStrings.PlanMenuSubcategory.Decor, BuildLocationRule.OnFloor, "whiteboard_poi_kanim", "off", "STRINGS.BUILDINGS.PREFABS.ROLESTATION.NAME", "STRINGS.BUILDINGS.PREFABS.ROLESTATION.DESC", 2, 2, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2);


			}
		}
	}
}
