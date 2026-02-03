using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;


namespace Biochemistry.Buildings
{
	//===[ OIL PRESSER CONFIG ]===================================================================================
	public class Biochemistry_ExpellerPressConfig : IBuildingConfig
	{
		public static string ID = "Biochemistry_ExpellerPress";
		private static readonly PortDisplayInput waterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-1, 1), null, new Color32(3, 148, 252, 255));

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
			SoundUtils.CopySoundsToAnim("oil_presser_kanim", "fertilizer_maker_kanim");
			return buildingDef;
		}
		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, waterInputPort);
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;




			//----------------------------- Fabricator Section
			ComplexFabricator oilPress = go.AddOrGet<ComplexFabricator>();
			oilPress.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			oilPress.duplicantOperated = false;
			oilPress.heatedTemperature = 298.15f;
			BuildingTemplates.CreateComplexFabricatorStorage(go, oilPress);
			oilPress.storeProduced = true;
			oilPress.keepExcessLiquids = true;
			oilPress.inStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			oilPress.buildStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			oilPress.outStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			oilPress.outputOffset = new Vector3(1f, 0.5f);
			//-----------------------------

			PortConduitConsumer waterInput = go.AddComponent<PortConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 10f;
			waterInput.capacityKG = 40f;
			waterInput.storage = oilPress.inStorage;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			waterInput.SkipSetOperational = true;
			waterInput.AssignPort(waterInputPort);

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.storage = oilPress.outStorage;
			dispenser.elementFilter = [ModElements.VegetableOil_Liquid, SimHashes.NaturalResin, SimHashes.PhytoOil, SimHashes.Milk];

			var dropper = go.AddOrGet<WorldElementDropper>();
			dropper.DropSolids = true;
			dropper.TargetStorage = oilPress.outStorage;

			this.ConfigureRecipes();
			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			this.AttachPort(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			this.AttachPort(go);
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>() ;
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}

		//===[ EXPELLER PRESS RECIPES ]===============================================================================
		private void ConfigureRecipes()
		{
			///Recipes are moved to ModDb.AdditionalRecipes to be generated dynamically			
		}
	}
}
