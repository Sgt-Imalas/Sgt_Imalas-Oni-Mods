using HarmonyLib;
using KSerialization;
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
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: SYNGAS REFINERY CONFIG ]=============================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_SyngasRefineryConfig : IBuildingConfig
	{
		//--[ Base Information ]------------------------------------------------------------------------------------------
		public static string ID = "Chemical_SyngasRefinery";
		private static readonly List<Storage.StoredItemModifier> RefineryStorageModifier;

		private static readonly PortDisplayOutput LiquidOilOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 0));

		//--[ Identification and DLC stuff ]-------------------------------------------------------------------------------
		static Chemical_SyngasRefineryConfig()
		{
			Color? OilPortColor = new Color32(255, 216, 43, 255);
			LiquidOilOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 0), null, OilPortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Preserve);
			list1.Add(Storage.StoredItemModifier.Insulate);
			list1.Add(Storage.StoredItemModifier.Seal);
			RefineryStorageModifier = list1;
		}

		//--[ Building Definitions ]----------------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 2, 4, "syngas_distillery_kanim", 100, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, noise,	0.2f);
			def1.Overheatable = false;
			def1.RequiresPowerInput = true;
			def1.EnergyConsumptionWhenActive = 60f;
			def1.ExhaustKilowattsWhenActive = 2f;
			def1.SelfHeatKilowattsWhenActive = 4f;
			def1.AudioCategory = "HollowMetal";
			def1.ViewMode = OverlayModes.LiquidConduits.ID;
			def1.OutputConduitType = ConduitType.Gas;
			def1.PowerInputOffset = new CellOffset(0, 0);
			def1.UtilityOutputOffset = new CellOffset(1, 0);
			SoundUtils.CopySoundsToAnim("syngas_distillery_kanim", "algae_distillery_kanim");
			return def1;
		}

		//--[ Building Operation Definitions ]------------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();

			ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			fabricator.duplicantOperated = false;
			fabricator.heatedTemperature = 320.15f;

			Storage standardStorage = go.AddOrGet<Storage>();
			standardStorage.capacityKg = 5000f;

			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.outStorage.capacityKg = 5000f;
			fabricator.heatedTemperature = 326.15f;
			fabricator.keepExcessLiquids = true;
			fabricator.keepAdditionalTag = SimHashes.Syngas.CreateTag();
			fabricator.inStorage.SetDefaultStoredItemModifiers(RefineryStorageModifier);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(RefineryStorageModifier);
			fabricator.outStorage = standardStorage;
			fabricator.outputOffset = new Vector3(0f, 0f);

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.storage = standardStorage;
			conduitDispenser.conduitType = ConduitType.Gas;
			conduitDispenser.invertElementFilter = false;
			conduitDispenser.elementFilter = [SimHashes.Syngas];

			PipedConduitDispenser LiquidDispenser = go.AddComponent<PipedConduitDispenser>();
			LiquidDispenser.elementFilter = [SimHashes.Petroleum];
			LiquidDispenser.AssignPort(LiquidOilOutputPort);
			LiquidDispenser.alwaysDispense = true;
			LiquidDispenser.SkipSetOperational = true;

			Prioritizable.AddRef(go);
			this.AttachPort(go);
			this.ConfigureRecipes();
		}

		//===[ CHEMICAL: SYNGAS REFINERY RECIPES ]========================================================================
		private void ConfigureRecipes()
		{
			//---- [ Syngas from Lumber ] --------------------------------------------------------------------------------
			// Ingredient: Wood Lumber   - 100kg
			// Result: Syngas            - 25kg
			//         Polluted Dirt     - 75kg
			//------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID,50)
				.Input(SimHashes.WoodLog,100)
				.Output(SimHashes.Syngas,25, ComplexRecipe.RecipeElement.TemperatureOperation.Heated,true)
				.Output(SimHashes.ToxicSand,75, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(SYNGASREFINERY_1_1_1,1,2)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Syngas from Bitumen ] ----------------------------------------------------------------------------------
			// Ingredient: Bitumen     - 100kg
			// Result: Syngas          - 25kg
			//         Refined Coal    - 75kg
			//---------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Bitumen, 100)
				.Output(SimHashes.Syngas, 25, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.Output(SimHashes.RefinedCarbon, 75, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(SYNGASREFINERY_1_1_1, 1, 2)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Syngas from Oil Shale ] -----------------------------------------------------------------------------------
			// Ingredient: Oil Shale    - 100kg
			// Result: Syngas           - 50kg
			//         Petroleum        - 30kg
			//         Refined Coal     - 20kg
			//-------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(ModElements.OilShale_Solid, 100)
				.Output(SimHashes.Syngas, 50, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.Output(SimHashes.Petroleum, 30, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.Output(SimHashes.RefinedCarbon, 20, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(SYNGASREFINERY_1_1_2, 1, 3)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				//---- [ BIOMASS TO SYNGAS ] -------------------------------------------------------------------------------------------
				// Ingredient: Compressed Biomass - 100kg
				// Result: Syngas - 25kg
				//         Polluted Dirt - 75kg
				//----------------------------------------------------------------------------------------------------------------------
				RecipeBuilder.Create(ID, 30)
					.Input(ModElements.BioMass_Solid, 100)
					.Output(SimHashes.Syngas,25, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
					.Output(SimHashes.ToxicSand,75)
					.Description(SYNGASREFINERY_1_1_1,1,2)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
					.Build();
			}
		}



		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, LiquidOilOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			this.AttachPort(go);
		}
	}
}
