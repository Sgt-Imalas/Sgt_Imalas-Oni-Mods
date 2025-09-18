using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.MINING_AUGUR_DRILL;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;
using RonivansLegacy_ChemicalProcessing;


namespace Mineral_Processing_Mining.Buildings
{
	public class Mining_AugerDrillConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Mining_AugerDrill";

		[SerializeField]
		public Storage buildStorage;


		private Tag fuelTag = GameTags.CombustibleLiquid;

		//--[ Building Definitions ]---------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			string[] construction_materials = [SimHashes.Steel.ToString()];
			float[] material_amount = [5000f];

			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 13, 8, "auger_drill_kanim", 100, 480f, material_amount, construction_materials, 9999f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, noise);
			def.RequiresPowerInput = false;
			def.ObjectLayer = ObjectLayer.Building;
			def.UseStructureTemperature = false;
			def.InputConduitType = ConduitType.Liquid;
			def.UtilityInputOffset = new CellOffset(0, 0);
			def.OutputConduitType = ConduitType.Solid;
			def.UtilityOutputOffset = new CellOffset(-3, 4);
			def.ViewMode = OverlayModes.Power.ID;
			def.AudioCategory = "HollowMetal";
			def.AudioSize = "large";
			def.Overheatable = false;
			def.Floodable = false;
			def.Entombable = false;
			def.Breakable = false;
			def.Invincible = true;
			def.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(5, 1));
			def.LogicOutputPorts = ComplexFabricatorActiveLogicOutput.CreateSingleOutputPortList(new CellOffset(5, 2));
			//this ONLY toggles of the build tool afterwards, you need to add the UniquePerWorld tag to the building definition to make it a world exclusive building.
			def.OnePerWorld = true;
			SoundUtils.CopySoundsToAnim("auger_drill_kanim", "rockrefinery_kanim");


			return def;
		}

		//--[ Building Operation Definitions ]-----------------------------------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{

			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(GameTags.UniquePerWorld);

			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Chemical_FueledFabricatorAddon fuelConsumer = go.AddOrGet<Chemical_FueledFabricatorAddon>();
			fuelConsumer.fuelTag = this.fuelTag;

			ComplexFabricatorRandomOutput drillRig = go.AddOrGet<ComplexFabricatorRandomOutput>();
			drillRig.StoreRandomOutputs = true;
			drillRig.heatedTemperature = 368.15f;
			drillRig.duplicantOperated = false;
			drillRig.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			ComplexFabricatorWorkable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, drillRig);
			drillRig.keepExcessLiquids = true;
			drillRig.storeProduced = true;
			drillRig.inStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			drillRig.buildStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			drillRig.outStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			//drillRig.showProgressBar = true;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.capacityTag = this.fuelTag;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			conduitConsumer.capacityKG = 50f;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.storage = drillRig.inStorage;
			conduitConsumer.forceAlwaysSatisfied = true;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements =
			[ ///Equivalent of 3KW
			new ElementConverter.ConsumedElement(this.fuelTag, 3f, true)
			];
			elementConverter.outputElements =
			[
			new ElementConverter.OutputElement(0.75f, SimHashes.CarbonDioxide, UtilMethods.GetKelvinFromC(80), false, false, 0f, 2f, 1f, byte.MaxValue, 0, true)
			];

			ConfigurableSolidConduitDispenser solidDispenser = go.AddOrGet<ConfigurableSolidConduitDispenser>();
			solidDispenser.alwaysDispense = true;
			solidDispenser.massDispensed = HighPressureConduitRegistration.SolidCap_HP;
			solidDispenser.storage = drillRig.outStorage;
			solidDispenser.solidOnly = true;
			solidDispenser.elementFilter = null;

			var worldElementDropper = go.AddOrGet<WorldElementDropper>();
			worldElementDropper.DropGases = true;
			worldElementDropper.DropLiquids = true;
			worldElementDropper.TargetStorage = drillRig.outStorage;
			worldElementDropper.SpawnOffset = new CellOffset(4, 1);
			

			var guidanceDeviceHandler = go.AddOrGet<GuidanceDeviceWearHandler>();
			guidanceDeviceHandler.SourceStorage = drillRig.outStorage;
			guidanceDeviceHandler.TargetStorage = drillRig.inStorage;

			go.AddOrGet<HPA_SolidConduitRequirement>().RequiresHighPressureOutput = true;
			Prioritizable.AddRef(go);
			this.ConfigureRecipes();

		}

		private void ConfigureRecipes()
		{
			int count = 0;
			//---- [ Basic Drilling ] ----------------------------------------------------------------------------------------------
			// Ingredient:       < Basic Drillbits >
			// High Chance:        Dirt
			//                     Toxic Dirt
			//                     Clay
			//                     Sand
			//                     Coal
			//
			// Low Chance          Copper Ore
			//                     Pyrite
			//                     Aluminum Ore
			//                     Gold Amalgam
			//
			// Rare Occurance:     Water
			//                     Polluted Water
			//------------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 120)
				.Input(Mining_Drillbits_Basic_ItemConfig.TAG, 1)
				.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.SortOrder(count++)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.IconPrefabOverride(Mining_Drillbits_Basic_ItemConfig.TAG)
				.NameOverride(BASIC_DRILLING)
				.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID,Mining_Drillbits_Basic_ItemConfig.TAG, MINING_DRILLBITS_BASIC_ITEM.NAME))
				.Build();


			//---- [ Steel Drilling ] ---------------------------------------------------------------------------------------------
			// Ingredient:       < Steel Drillbits >
			// High Chance:        Copper Ore
			//                     Aluminum Ore
			//                     Iron Ore
			//                     Electrum
			//                     Gold Amalgam
			//
			// Low Chance          Igneous Rock
			//                     Granite
			//                     Sandstone
			//                     Sulfur
			//                     Wolframite
			//
			// Rare Occurance:     Steam
			//                     Carbon Dioxide
			//----------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 120)
				.Input(Mining_Drillbits_Steel_ItemConfig.TAG, 1)
				.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.SortOrder(count++)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.IconPrefabOverride(Mining_Drillbits_Steel_ItemConfig.TAG)
				.NameOverride(STEEL_DRILLING)
				.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_Steel_ItemConfig.TAG, MINING_DRILLBITS_STEEL_ITEM.NAME))
				.Build();

			//---- [ Tungsten Drilling ] ---------------------------------------------------------------------------------------------
			// Ingredient:       < Tungsten Drillbits >
			// High Chance:        Wolframite
			//                     Abyssalite
			//                     Obsidian
			//                     Rust
			//                     Salt
			//
			// Low Chance          Diamond
			//                     Fossil
			//                     Refined Coal
			//                     Lead
			//
			// Rare Occurance:     Methane Gas
			//                     Sulfur Gas
			//----------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 120)
			.Input(Mining_Drillbits_Tungsten_ItemConfig.TAG, 1)
			.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
			.SortOrder(count++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.IconPrefabOverride(Mining_Drillbits_Tungsten_ItemConfig.TAG)
			.NameOverride(TUNGSTEN_DRILLING)
			.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_Tungsten_ItemConfig.TAG, MINING_DRILLBITS_TUNGSTEN_ITEM.NAME))
			.Build();

			//---- [ Smart Drilling: Soft Stratum ] --------------------------------------------------------------------------------
			// Ingredient:       < Basic Drillbits >
			//                   < Guidance Device >
			// High Chance:        Dirt
			//                     Sand
			//                     Phosphorite
			//                     Salt
			//                     Clay
			//                     Coal
			//                     Polluted Dirt
			//                     Algae
			//                     Slime
			//
			// Rare Occurance:     Polluted Oxygen
			//                     Chlorine Gas
			//                     Carbon Dioxide
			//                     Hydrogen
			//----------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 120)
			.Input(Mining_Drillbits_GuidanceDevice_ItemConfig.SoftStratumTag, 1, doNotConsume: true)
			.Input(Mining_Drillbits_Basic_ItemConfig.TAG, 1)
			.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
			.SortOrder(count++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.NameOverride(string.Format(SMART_DRILLING, Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(Mining_Drillbits_GuidanceDevice_ItemConfig.SoftStratumTag)))
			.IconPrefabOverride(Mining_Drillbits_GuidanceDevice_ItemConfig.SoftStratumTag)
			.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_GuidanceDevice_ItemConfig.SoftStratumTag, MINING_DRILLBITS_BASIC_ITEM.NAME, true))
			.Build();

			//---- [ Smart Drilling: Aquifers ] --------------------------------------------------------------------------------
			// Ingredient:       < Basic Drillbits >
			//                   < Guidance Device >
			// High Chance:        Sand
			//                     Sedimentary Rock
			//                     Sandstone
			//                     Granite
			//                     Clay
			//                     Polluted Dirt
			//
			// Rare Occurance:     Water
			//                     Polluted Water
			//                     Salt Water
			//                     Brine
			//----------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 120)
			.Input(Mining_Drillbits_GuidanceDevice_ItemConfig.AquiferTag, 1, doNotConsume: true)
			.Input(Mining_Drillbits_Basic_ItemConfig.TAG, 1)
			.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
			.SortOrder(count++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.NameOverride(string.Format(SMART_DRILLING, Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(Mining_Drillbits_GuidanceDevice_ItemConfig.AquiferTag)))
			.IconPrefabOverride(Mining_Drillbits_GuidanceDevice_ItemConfig.AquiferTag)
			.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_GuidanceDevice_ItemConfig.AquiferTag, MINING_DRILLBITS_BASIC_ITEM.NAME, true, true))
			.Build();

			//---- [ Smart Drilling: Hard Stratum ] --------------------------------------------------------------------------------
			// Ingredient:       < Steel Drillbits >
			//                   < Guidance Device >
			// High Chance:        Copper
			//                     Aluminum Ore
			//                     Iron Ore
			//                     Gold Amalgam
			//                     Wolframite
			//                     Abyssalite
			//                     Igneous Rock
			//                     Obsidian
			//                     Granite
			//                     Rust
			//                     Fossil
			//
			// Rare Occurance:     Carbon Dioxide
			//                     Liquid Sulfur
			//                     Mercury Gas
			//----------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 120)
			.Input(Mining_Drillbits_GuidanceDevice_ItemConfig.HardStratumTag, 1, doNotConsume: true)
			.Input(Mining_Drillbits_Steel_ItemConfig.TAG, 1)
			.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
			.SortOrder(count++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.NameOverride(string.Format(SMART_DRILLING, Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(Mining_Drillbits_GuidanceDevice_ItemConfig.HardStratumTag)))
			.IconPrefabOverride(Mining_Drillbits_GuidanceDevice_ItemConfig.HardStratumTag)
			.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_GuidanceDevice_ItemConfig.HardStratumTag, MINING_DRILLBITS_STEEL_ITEM.NAME, true))
			.Build();

			//---- [ Smart Drilling: Oil Reserves ] --------------------------------------------------------------------------------
			// Ingredient:       < Steel Drillbits >
			//                   < Guidance Device >
			// High Chance:        Igneous Rock
			//                     Obsidian
			//                     Granite
			//                     Sulfur
			//                     Fossil
			//
			// Rare Occurance:     Crude Oil
			//                     Methane Gas
			//                     Sour Gas
			//----------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 120)
			.Input(Mining_Drillbits_GuidanceDevice_ItemConfig.OilReservesTag, 1, doNotConsume: true)
			.Input(Mining_Drillbits_Steel_ItemConfig.TAG, 1)
			.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
			.SortOrder(count++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.NameOverride(string.Format(SMART_DRILLING, Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(Mining_Drillbits_GuidanceDevice_ItemConfig.OilReservesTag)))
			.IconPrefabOverride(Mining_Drillbits_GuidanceDevice_ItemConfig.OilReservesTag)
			.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_GuidanceDevice_ItemConfig.OilReservesTag, MINING_DRILLBITS_STEEL_ITEM.NAME, true, true))
			.Build();

			//---- [ Smart Drilling: Cryosphere ] --------------------------------------------------------------------------------
			// Ingredient:       < Tungsten Drillbits >
			//                   < Guidance Device >
			// High Chance:        Snow
			//                     Ice
			//                     Polluted Ice
			//                     Brine Ice
			//                     Sand
			//                     Regolith
			//                     Fossil
			//
			// Low Chance          Solid Crude Oil
			//                     Solid Carbon Dioxide
			//                     Solid Chlorine
			//                     Solid Mercury
			//                     Solid Methane
			//----------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 120)
			.Input(Mining_Drillbits_GuidanceDevice_ItemConfig.CryosphereTag, 1, doNotConsume: true)
			.Input(Mining_Drillbits_Tungsten_ItemConfig.TAG, 1)
			.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
			.SortOrder(count++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.NameOverride(string.Format(SMART_DRILLING, Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(Mining_Drillbits_GuidanceDevice_ItemConfig.CryosphereTag)))
			.IconPrefabOverride(Mining_Drillbits_GuidanceDevice_ItemConfig.CryosphereTag)
			.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_GuidanceDevice_ItemConfig.CryosphereTag, MINING_DRILLBITS_TUNGSTEN_ITEM.NAME, true))
			.Build();

			//---- [ Smart Drilling: Mantle ] -------------------------------------------------------------------------------------
			// Ingredient:       < Tungsten Drillbits >
			//                   < Guidance Device >
			// High Chance:        Diamond
			//                     Abyssalite
			//                     Obsidian
			//                     Refined Coal
			//                     Wolframite
			//                     Gold
			//                     Iron
			//
			// Low Chance          Niobiun
			//                     Fullerene
			//
			// Rare Occurance:     Sulfur Gas
			//                     Magma
			//                     Rock Gas
			//----------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 120)
			.Input(Mining_Drillbits_GuidanceDevice_ItemConfig.MantleTag, 1, doNotConsume: true)
			.Input(Mining_Drillbits_Tungsten_ItemConfig.TAG, 1)
			.Output(SimHashes.CrushedRock.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
			.SortOrder(count++)
			.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
			.NameOverride(string.Format(SMART_DRILLING, Mining_Drillbits_GuidanceDevice_ItemConfig.GetTargetName(Mining_Drillbits_GuidanceDevice_ItemConfig.MantleTag)))
			.IconPrefabOverride(Mining_Drillbits_GuidanceDevice_ItemConfig.MantleTag)
			.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, Mining_Drillbits_GuidanceDevice_ItemConfig.MantleTag, MINING_DRILLBITS_TUNGSTEN_ITEM.NAME, true, false, true))
			.Build();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;

			if (Config.Instance.HPA_Rails_Mod_Enabled)
				go.AddOrGet<HPA_SolidConduitRequirement>().RequiresHighPressureOutput = true;
			AddFloor(go);
			go.AddOrGet<ComplexFabricatorActiveLogicOutput>();

		}

		void AddFloor(GameObject go)
		{
			FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
			fakeFloorAdder.floorOffsets =
			[
			new CellOffset(-6, 0),
			new CellOffset(-5, 0),
			new CellOffset(-4, 0),
			new CellOffset(-3, 0),
			new CellOffset(-2, 0),
			new CellOffset(-1, 0),
			new CellOffset(0, 0),
			new CellOffset(1, 0),
			new CellOffset(2, 0),
			new CellOffset(3, 0),
			new CellOffset(4, 0),
			new CellOffset(5, 0),
			new CellOffset(6, 0)
			];
			fakeFloorAdder.initiallyActive = true;
		}


		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			AddFloor(go);
			base.DoPostConfigureUnderConstruction(go);
		}
	}
}
