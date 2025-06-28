using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;


namespace Biochemistry.Buildings
{
	//===[ OIL PRESSER CONFIG ]===================================================================================
	public class Biochemistry_ExpellerPressConfig : IBuildingConfig
	{
		public static string ID = "Biochemistry_ExpellerPress";
		
		private static readonly List<Storage.StoredItemModifier> ExpellerStoredItemModifiers;

		static Biochemistry_ExpellerPressConfig()
		{
			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Preserve);
			list1.Add(Storage.StoredItemModifier.Insulate);
			list1.Add(Storage.StoredItemModifier.Seal);
			ExpellerStoredItemModifiers = list1;
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "oil_presser_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 0.2f;
			buildingDef.SelfHeatKilowattsWhenActive = 3f;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.PowerInputOffset = new CellOffset(-1, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(2, 0);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			Storage liquidStorage = go.AddOrGet<Storage>();
			liquidStorage.SetDefaultStoredItemModifiers(ExpellerStoredItemModifiers);
			liquidStorage.showCapacityStatusItem = false;
			liquidStorage.showCapacityAsMainStatus = false;
			liquidStorage.showDescriptor = false;

			Storage vegStorage = go.AddOrGet<Storage>();
			vegStorage.SetDefaultStoredItemModifiers(ExpellerStoredItemModifiers);
			vegStorage.showCapacityStatusItem = false;
			vegStorage.showCapacityAsMainStatus = false;
			vegStorage.showDescriptor = false;

			//----------------------------- Fabricator Section
			ComplexFabricator oilPress = go.AddOrGet<ComplexFabricator>();
			oilPress.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			oilPress.duplicantOperated = false;
			oilPress.heatedTemperature = 298.15f;
			BuildingTemplates.CreateComplexFabricatorStorage(go, oilPress);
			oilPress.inStorage.capacityKg = 1000f;
			oilPress.buildStorage.capacityKg = 1000f;
			oilPress.outStorage.capacityKg = 1000f;
			oilPress.storeProduced = true;
			oilPress.keepExcessLiquids = true;
			oilPress.inStorage.SetDefaultStoredItemModifiers(ExpellerStoredItemModifiers);
			oilPress.buildStorage.SetDefaultStoredItemModifiers(ExpellerStoredItemModifiers);
			oilPress.outStorage.SetDefaultStoredItemModifiers(ExpellerStoredItemModifiers);
			oilPress.inStorage = liquidStorage;
			oilPress.outStorage = vegStorage;
			oilPress.outputOffset = new Vector3(1f, 0.5f);
			//-----------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.storage = vegStorage;
			dispenser.elementFilter = [ModElements.VegetableOil_Liquid];

			this.ConfigureRecipes();
			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}

		//===[ EXPELLER PRESS RECIPES ]===============================================================================
		private void ConfigureRecipes()
		{
			///Recipes are moved to ModDb.AdditionalRecipes to be generated dynamically			
		}
	}	
}
