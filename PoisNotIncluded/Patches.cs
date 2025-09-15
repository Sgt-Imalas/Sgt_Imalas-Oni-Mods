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
				
				//,GravitasLabLightConfig.ID ///appears glowing without light		  }
				{PropGravitasWallConfig.ID   , GameStrings.PlanMenuSubcategory.Tiles}
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
				//,{TemporalTearOpenerConfig.ID  ,GameStrings.PlanMenuSubcategory.Exploration }
			};
			static HashSet<string> StoryTraitBuildings = [
				GravitasCreatureManipulatorConfig.ID,// critter flux
				MorbRoverMakerConfig.ID ,//morb rover maker
				GravitasContainerConfig.ID,//braintank pajamas cubby
				 MegaBrainTankConfig.ID, //braintank story trait		
				 LonelyMinionHouseConfig.ID, //minion storytrait		
				 LonelyMinionMailboxConfig.ID,//minion storytrait		
				 FossilDigSiteConfig.ID //fossil storytrait			
				];


			[HarmonyPatch(typeof(TilePOIConfig), nameof(TilePOIConfig.CreateBuildingDef))]
			public class TilePOIConfig_CreateBuildingDef_Patch
			{
				public static void Postfix(BuildingDef __result)
				{
					BuildingTemplates.CreateFoundationTileDef(__result);
					__result.DebugOnly = false;
				}
			}

			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix()
			{

				var buildingIDs = GravitasBuildingIds.Keys.ToList();
				if (Config.Instance.ConstructableStoryTraits)
					buildingIDs.AddRange(StoryTraitBuildings);
				var AllDefs = Assets.BuildingDefs;
				var gravitasDefs = AllDefs.Where(def => buildingIDs.Contains(def.PrefabID));
				foreach (var def in gravitasDefs)
				{
					def.ShowInBuildMenu = true;
					def.DebugOnly = false;
				}

				var defIds = gravitasDefs.Select(def => def.PrefabID).ToList();


				foreach (var id in new List<string>(defIds))
				{
					if (TUNING.BUILDINGS.PLANORDER.Any(category => category.buildingAndSubcategoryData.Any(comp => comp.Key == id)))
						defIds.Remove(id);
				}

				SgtLogger.l(gravitasDefs.Count() + "x poi buildings");

				PlanScreen.PlanInfo planInfo = new PlanScreen.PlanInfo(POI_Category, false, []);


				TUNING.BUILDINGS.PLANORDER.Add(planInfo);
				foreach (var id in defIds)
				{
					string subcategoryID;
					if (!GravitasBuildingIds.TryGetValue(id, out subcategoryID))
						subcategoryID = StoryTraitsCategory;

					InjectionMethods.AddBuildingToPlanScreen(POI_Category, id, subcategoryID);
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
				Strings.Add($"STRINGS.UI.BUILDCATEGORIES.{POI_Category.ToUpperInvariant()}.NAME", STRINGS.UI.SANDBOXTOOLS.FILTERS.ENTITIES.GRAVITAS);
				Strings.Add($"STRINGS.UI.BUILDCATEGORIES.{POI_Category.ToUpperInvariant()}.TOOLTIP", "");


				if (Config.Instance.ConstructableStoryTraits)
				{
					TryRegisterDynamicGravitasBuilding(FossilSiteConfig_Ice.ID, StoryTraitsCategory, BuildLocationRule.OnFloor, isEntitySpawner: true, materialOverride: [SimHashes.Fossil.CreateTag().ToString()], costOverride: [4000]);
					TryRegisterDynamicGravitasBuilding(FossilSiteConfig_Resin.ID, StoryTraitsCategory, BuildLocationRule.OnFloor, isEntitySpawner: true, materialOverride: [SimHashes.Fossil.CreateTag().ToString()], costOverride: [4000]);
					TryRegisterDynamicGravitasBuilding(FossilSiteConfig_Rock.ID, StoryTraitsCategory, BuildLocationRule.OnFloor, isEntitySpawner: true, materialOverride: [SimHashes.Fossil.CreateTag().ToString()], costOverride: [4000]);
				}
				RegisterNewBuilding("TemporalTearOpener", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, "temporal_tear_opener_kanim", "off", "STRINGS.BUILDINGS.PREFABS.TEMPORALTEAROPENER.NAME", "STRINGS.BUILDINGS.PREFABS.TEMPORALTEAROPENER.DESC", 3, 4, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, decorName: true, altAnims: ["off", "on", "inert", "working_loop"]);


				TryRegisterDynamicGravitasBuilding("PropCeresPosterA", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding("PropCeresPosterB", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding("PropCeresPosterLarge", GameStrings.PlanMenuSubcategory.Decor, backwall: true);
				TryRegisterDynamicGravitasBuilding("PropClock", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding("PropClothesHanger", GameStrings.PlanMenuSubcategory.Storage, materialOverride: [GameTags.Metal.ToString()], altAnims: ["on", "off"]);
				TryRegisterDynamicGravitasBuilding_Floor("PropDesk", GameStrings.PlanMenuSubcategory.Wellness);
				TryRegisterDynamicGravitasBuilding_Floor("PropDlc2GeothermalCart", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding_Floor("PropElevator", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding("PropExoShelfLong", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding("PropExoShelfShort", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityChair", GameStrings.PlanMenuSubcategory.Wellness);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityChairFlip", GameStrings.PlanMenuSubcategory.Wellness);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityCouch", GameStrings.PlanMenuSubcategory.Wellness, new(5, 0));
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityDesk", GameStrings.PlanMenuSubcategory.Wellness);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityDisplay", GameStrings.PlanMenuSubcategory.Exploration);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityDisplay2", GameStrings.PlanMenuSubcategory.Exploration);
				TryRegisterDynamicGravitasBuilding("PropFacilityDisplay3", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.NotInTiles);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasDisplay4", GameStrings.PlanMenuSubcategory.Exploration);
				TryRegisterDynamicGravitasBuilding_Floor("PropDlc2Display1", GameStrings.PlanMenuSubcategory.Exploration);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityGlobeDroors", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding("PropFacilityPainting", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityStatue", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding_Floor("PropFacilityTable", GameStrings.PlanMenuSubcategory.Wellness, new(2, 0));
				TryRegisterDynamicGravitasBuilding("PropFacilityWallDegree", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding_Ceiling("PropGravitasCeilingRobot", GameStrings.PlanMenuSubcategory.Manufacturing, new(0, 5));
				TryRegisterDynamicGravitasBuilding("PropGravitasCreaturePoster", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasDeskPodium", GameStrings.PlanMenuSubcategory.Manufacturing);
				TryRegisterDynamicGravitasBuilding("PropGravitasFireExtinguisher", GameStrings.PlanMenuSubcategory.Manufacturing);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasFirstAidKit", GameStrings.PlanMenuSubcategory.Medical);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasFloorRobot", GameStrings.PlanMenuSubcategory.Manufacturing);
				TryRegisterDynamicGravitasBuilding("PropGravitasHandScanner", GameStrings.PlanMenuSubcategory.Manufacturing);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasJar1", GameStrings.PlanMenuSubcategory.Exploration);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasJar2", GameStrings.PlanMenuSubcategory.Exploration);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasLabTable", GameStrings.PlanMenuSubcategory.Wellness);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasRobitcTable", GameStrings.PlanMenuSubcategory.Manufacturing);
				TryRegisterDynamicGravitasBuilding("PropGravitasShelf", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding_Floor("PropGravitasToolCrate", GameStrings.PlanMenuSubcategory.Manufacturing);
				TryRegisterDynamicGravitasBuilding("PropGravitasToolShelf", GameStrings.PlanMenuSubcategory.Manufacturing);
				TryRegisterDynamicGravitasBuilding_Floor("PropHumanChesterfieldChair", GameStrings.PlanMenuSubcategory.Wellness, new(3, 2));
				TryRegisterDynamicGravitasBuilding_Floor("PropHumanChesterfieldSofa", GameStrings.PlanMenuSubcategory.Wellness, new(5, 2));
				TryRegisterDynamicGravitasBuilding_Floor("PropHumanMurphyBed", GameStrings.PlanMenuSubcategory.Wellness, new Tuple<int, int>(7, 4));
				TryRegisterDynamicGravitasBuilding_Floor("PropReceptionDesk", GameStrings.PlanMenuSubcategory.Wellness);
				TryRegisterDynamicGravitasBuilding_Floor("PropSkeleton", GameStrings.PlanMenuSubcategory.Decor);
				TryRegisterDynamicGravitasBuilding("PropSurfaceSatellite1", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true);
				TryRegisterDynamicGravitasBuilding("PropSurfaceSatellite2", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true);
				TryRegisterDynamicGravitasBuilding("PropSurfaceSatellite3", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true);
				TryRegisterDynamicGravitasBuilding_Floor("PropTable", GameStrings.PlanMenuSubcategory.Wellness);
				TryRegisterDynamicGravitasBuilding_Floor("PropTallPlant", GameStrings.PlanMenuSubcategory.Wellness);

				//TryRegisterDynamicGravitasBuilding("CryoTank", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, animOverride: "on", isEntitySpawner:true);
				TryRegisterDynamicGravitasBuilding("CryoTank", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, animOverride: "on", decorName: true, altAnims: ["off", "on"]);

				TryRegisterDynamicGravitasBuilding_Backwall("PropGravitasDecorativeWindow", GameStrings.PlanMenuSubcategory.Tiles);

				TryRegisterDynamicGravitasBuilding("MissileSetLocker", GameStrings.PlanMenuSubcategory.Storage, BuildLocationRule.OnFloor, altAnims: ["off", "on"]);
				TryRegisterDynamicGravitasBuilding("PropExoSetLocker", GameStrings.PlanMenuSubcategory.Storage, BuildLocationRule.OnFloor, altAnims: ["off", "on"]);
				TryRegisterDynamicGravitasBuilding("SetLocker", GameStrings.PlanMenuSubcategory.Storage, BuildLocationRule.OnFloor);
				TryRegisterDynamicGravitasBuilding("VendingMachine", GameStrings.PlanMenuSubcategory.Storage, BuildLocationRule.OnFloor, altAnims: ["off", "on"]);
				TryRegisterDynamicGravitasBuilding("PropGravitasSmallSeedLocker", GameStrings.PlanMenuSubcategory.Storage, altAnims: ["empty", "on"]);

				TryRegisterDynamicGravitasBuilding("FossilBitsLarge", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, materialOverride: [SimHashes.Fossil.CreateTag().ToString()]);
				TryRegisterDynamicGravitasBuilding("FossilBitsSmall", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, materialOverride: [SimHashes.Fossil.CreateTag().ToString()]);


				TryRegisterDynamicGravitasBuilding("PropLight", GameStrings.PlanMenuSubcategory.Lights, BuildLocationRule.OnCeiling, brokenName: true, altAnims: ["off", "misc"]);
				TryRegisterDynamicGravitasBuilding("PropFacilityHangingLight", GameStrings.PlanMenuSubcategory.Lights, BuildLocationRule.OnCeiling, brokenName: true);
				TryRegisterDynamicGravitasBuilding("PropFacilityChandelier", GameStrings.PlanMenuSubcategory.Lights, BuildLocationRule.OnCeiling, brokenName: true);

				TryRegisterDynamicGravitasBuilding_Lamp("PropLight", "setpiece_light_kanim", new Tuple<string, string>("misc", "on"), "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.NAME", "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.DESC", 1, 1, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, 10, 1800, 8, LIGHT2D.CEILINGLIGHT_OFFSET, new(0, 0), LightShape.Cone);
				TryRegisterDynamicGravitasBuilding_Lamp("CeilingLight_Pretty", "ceilinglight_pretty_kanim", new Tuple<string, string>("off", "on"), "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.NAME", "STRINGS.BUILDINGS.PREFABS.PROPLIGHT.DESC", 1, 1, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, 10, 1800, 8, LIGHT2D.CEILINGLIGHT_OFFSET, new(0, 0), LightShape.Cone);
				TryRegisterDynamicGravitasBuilding_Lamp("GravitasLabLight", "gravitas_lab_light_kanim", new Tuple<string, string>("on", "off"), "STRINGS.BUILDINGS.PREFABS.GRAVITASLABLIGHT.NAME", "STRINGS.BUILDINGS.PREFABS.GRAVITASLABLIGHT.DESC", 1, 1, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, 20, 2400, 10, LIGHT2D.CEILINGLIGHT_OFFSET, new(0, 0), LightShape.Cone);
				TryRegisterDynamicGravitasBuilding_Lamp("PropFacilityHangingLight", "gravitas_light_kanim", new Tuple<string, string>("off", "on"), "STRINGS.BUILDINGS.PREFABS.PROPFACILITYLAMP.NAME", "STRINGS.BUILDINGS.PREFABS.PROPFACILITYLAMP.DESC", 1, 4, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, 120, 8000, 16, new(0, 0.75f), new(0, 4), LightShape.Circle);
				TryRegisterDynamicGravitasBuilding_Lamp("PropFacilityChandelier", "gravitas_chandelier_kanim", new Tuple<string, string>("off", "on"), "STRINGS.BUILDINGS.PREFABS.PROPFACILITYCHANDELIER.NAME", "STRINGS.BUILDINGS.PREFABS.PROPFACILITYCHANDELIER.DESC", 5, 7, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, 480, 18000, 24, new(0, 2), new(0, 6), LightShape.Circle);

				RegisterNewBuilding("WhiteBoard", GameStrings.PlanMenuSubcategory.Decor, BuildLocationRule.OnFloor, "whiteboard_poi_kanim", "off", "STRINGS.BUILDINGS.PREFABS.ROLESTATION.NAME", "STRINGS.BUILDINGS.PREFABS.ROLESTATION.DESC", 2, 2, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, false);
				RegisterNewBuilding("WidePedestal", GameStrings.PlanMenuSubcategory.Wellness, BuildLocationRule.OnFloor, "gravitas_pedestal_regular_kanim", "pedestal_regular", "STRINGS.BUILDINGS.PREFABS.GRAVITASPEDESTAL.NAME", "STRINGS.BUILDINGS.PREFABS.GRAVITASPEDESTAL.DESC", 3, 1, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3);

				RegisterNewBuilding("WarpConduitSender", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, "warp_conduit_sender_kanim", "idle", "STRINGS.BUILDINGS.PREFABS.WARPCONDUITSENDER.NAME", "STRINGS.BUILDINGS.PREFABS.WARPCONDUITSENDER.DESC", 4, 3, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, decorName: true, altAnims: ["off", "idle", "working_loop"]);
				RegisterNewBuilding("WarpConduitReceiver", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, "warp_conduit_receiver_kanim", "off", "STRINGS.BUILDINGS.PREFABS.WARPCONDUITRECEIVER.NAME", "STRINGS.BUILDINGS.PREFABS.WARPCONDUITRECEIVER.DESC", 4, 3, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, decorName: true, altAnims: ["off", "idle", "working_loop"]);
				RegisterNewBuilding("GeneShuffler", GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, "geneshuffler_kanim", "on", "STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.NAME", "STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.DESC", 4, 3, MATERIALS.ALL_METALS, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, decorName: true, altAnims: ["on", "off", "recharging"]);
				TryRegisterDynamicGravitasBuilding(WarpPortalConfig.ID, GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true, altAnims: ["recharge", "idle"]);
				TryRegisterDynamicGravitasBuilding(WarpReceiverConfig.ID, GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true, altAnims: ["off", "idle"]);
				TryRegisterDynamicGravitasBuilding(PioneerLanderConfig.ID, GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true);
				TryRegisterDynamicGravitasBuilding(ScoutLanderConfig.ID, GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true);
				TryRegisterDynamicGravitasBuilding(SapTreeConfig.ID, GameStrings.PlanMenuSubcategory.Exploration, BuildLocationRule.OnFloor, decorName: true, altAnims: ["off","idle", "eat_loop", "ooze_loop", "attacking_loop", "attack_cooldown", "withered"]);

			}
		}
	}
}
