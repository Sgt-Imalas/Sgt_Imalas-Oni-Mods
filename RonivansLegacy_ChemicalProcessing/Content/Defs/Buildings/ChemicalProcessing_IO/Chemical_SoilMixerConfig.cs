using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
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
	//==== [ CHEMICAL: SOIL MIXER CONFIG ] =================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_SoilMixerConfig : IBuildingConfig
	{
		//--[ Base Information ]----------------------------------------------------------------------------
		public static string ID = "Chemical_SoilMixer";

		//--[ Identification and DLC stuff ]----------------------------------------------
		public static readonly List<Storage.StoredItemModifier> SoilMixerStoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------------------------------------
		private static readonly PortDisplayInput waterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(1, 1));
		private static readonly PortDisplayInput gasAmmoniaInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(1, 1));

		static Chemical_SoilMixerConfig()
		{
			Color? waterPortColor = new Color32(3, 148, 252, 255);
			waterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(1, 1), null, waterPortColor);
			Color? gasCo2PortColor = new Color32(215, 227, 252, 255);
			gasAmmoniaInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(1, 1), null, gasCo2PortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			SoilMixerStoredItemModifiers = list1;
		}

		//--[ Building Definitions ]--------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 4, "soil_mixer_kanim", 100, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 480f;
			buildingDef.ExhaustKilowattsWhenActive = 8f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(1, 0);
			SoundUtils.CopySoundsToAnim("soil_mixer_kanim", "fertilizer_maker_kanim");
			return buildingDef;
		}

		//--[ Building Operation Definitions ]-----------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{

			Storage liquidStorage = go.AddOrGet<Storage>();
			liquidStorage.SetDefaultStoredItemModifiers(SoilMixerStoredItemModifiers);
			//liquidStorage.showCapacityStatusItem = true;
			//liquidStorage.showCapacityAsMainStatus = true;
			//liquidStorage.showDescriptor = true;

			ConduitConsumer pollutedWaterInput = go.AddOrGet<ConduitConsumer>();
			pollutedWaterInput.conduitType = ConduitType.Liquid;
			pollutedWaterInput.consumptionRate = 10f;
			pollutedWaterInput.capacityKG = 100f;
			pollutedWaterInput.storage = liquidStorage;
			pollutedWaterInput.capacityTag = SimHashes.DirtyWater.CreateTag(); ;
			pollutedWaterInput.forceAlwaysSatisfied = true;
			pollutedWaterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer waterInput = go.AddComponent<PortConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 10f;
			waterInput.capacityKG = 100f;
			waterInput.storage = liquidStorage;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			waterInput.AssignPort(waterInputPort);

			PortConduitConsumer gasAmmoniaInput = go.AddComponent<PortConduitConsumer>();
			gasAmmoniaInput.conduitType = ConduitType.Gas;
			gasAmmoniaInput.consumptionRate = 10f;
			gasAmmoniaInput.capacityKG = 50f;
			gasAmmoniaInput.storage = liquidStorage;
			gasAmmoniaInput.capacityTag = ModElements.Ammonia_Gas.Tag;
			gasAmmoniaInput.forceAlwaysSatisfied = true;
			gasAmmoniaInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			gasAmmoniaInput.AssignPort(gasAmmoniaInputPort);

			//----------------------------- Fabricator Section
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			var soilMixer = go.AddOrGet<ComplexFabricator>();
			soilMixer.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			soilMixer.duplicantOperated = false;
			soilMixer.heatedTemperature = 298.15f;
			BuildingTemplates.CreateComplexFabricatorStorage(go, soilMixer);
			soilMixer.keepExcessLiquids = true;
			soilMixer.keepAdditionalTag = ModElements.Ammonia_Gas.Tag;
			soilMixer.inStorage.capacityKg = 1000f;
			soilMixer.buildStorage.capacityKg = 1000f;
			soilMixer.outStorage.capacityKg = 1000f;
			soilMixer.storeProduced = false;
			soilMixer.buildStorage.SetDefaultStoredItemModifiers(SoilMixerStoredItemModifiers);
			soilMixer.outStorage.SetDefaultStoredItemModifiers(SoilMixerStoredItemModifiers);
			soilMixer.inStorage = liquidStorage;
			soilMixer.inStorage.SetDefaultStoredItemModifiers(SoilMixerStoredItemModifiers);
			soilMixer.outputOffset = new Vector3(-1f, 0.5f);
			//-----------------------------
			Prioritizable.AddRef(go);
			this.AttachPort(go);
			this.ConfigureRecipes();
		}

		//==== [ CHEMICAL: SOIL MIXER RECIPES ] ==================================================================
		private void ConfigureRecipes()
		{
			bool chemproc = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;
			//---- [ Nitrate Fertilizer ] ------------------------------------------------------------------------
			// Ingredient: Nitrate Nodules - 25kg
			//             Phosphate Nodules - 25kg
			//             Sulfur - 25kg
			//             Polluted Water - 25kg
			// Result: Fertilizer - 100kg
			//-----------------------------------------------------------------------------------------------------
			if (chemproc)
			{
				RecipeBuilder.Create(ID, 80)
					.Input(ModElements.AmmoniumSalt_Solid, 25)
					.Input(SimHashes.PhosphateNodules, 25)
					.Input(SimHashes.Sulfur, 25)
					.Input(SimHashes.DirtyWater, 25)
					.Output(SimHashes.Fertilizer, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(SOILMIXER_4_1, 4, 1)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Build();

			}

			//---- [ Ammonia Fertilizer ] ------------------------------------------------------------------------
			// Ingredient: Ammonia Gas - 25kg
			//             Phosphate Nodules - 25kg
			//             Sulfur - 25kg
			//             Polluted Water - 25kg
			// Result: Fertilizer - 100kg
			//-----------------------------------------------------------------------------------------------------
			if (chemproc)
			{
				RecipeBuilder.Create(ID, 80)
				.Input(ModElements.Ammonia_Gas, 25)
				.Input(SimHashes.PhosphateNodules, 25)
				.Input(SimHashes.Sulfur, 25)
				.Input(SimHashes.DirtyWater, 25)
				.Output(SimHashes.Fertilizer, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(SOILMIXER_4_1, 4, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();
			}

			//---- [ Phosphate Nodules from Phosphorus ] --------------------------------------------------------------
			// Ingredient: Phosphorus - 50kg
			//             Crushed Rock - 40kg
			//             Water - 10kg
			// Result: Phosphate Nodules - 100kg
			//---------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 60)
				.Input(SimHashes.Phosphorus, 50)
				.Input(SimHashes.CrushedRock, 40)
				.Input(SimHashes.Water, 10)
				.Output(SimHashes.PhosphateNodules, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(SOILMIXER_3_1, 3, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			//---- [ Phosphorite ] ---------------------------------------------------------------------------------------
			// Ingredient: Phosphate Nodules - 50kg
			//             Dirt - 40kg
			//             Water - 10kg
			// Result: Phosphorite - 100kg
			//------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 60)
				.Input(SimHashes.PhosphateNodules, 50)
				.Input(SimHashes.Dirt, 40)
				.Input(SimHashes.Water, 10)
				.Output(SimHashes.Phosphorite, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(SOILMIXER_3_1, 3, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			//---- [ Dirt Recipe A ] ---------------------------------------------------------------------------------------
			// Ingredient: Crushed Rock - 40kg
			//             Polluted Dirt - 20kg
			//             Coal - 20kg
			//             Water - 20kg
			// Result: Dirt - 100kg
			//---------------------------------------------------------------------------------------------------------------

			//---- [ Dirt Recipe B ] -------------------------------------------------------------------------------------------
			// Ingredient: Crushed Rock - 40kg
			//             Polluted Dirt - 20kg
			//             Wood - 20kg
			//             Water - 20kg
			// Result: Dirt - 100kg
			//------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 60)
				.Input(SimHashes.CrushedRock, 40)
				.Input(SimHashes.ToxicSand, 20)
				.Input([SimHashes.Carbon, SimHashes.Peat,SimHashes.WoodLog], 20, GameTags.CombustibleSolid)
				.Input(SimHashes.Water, 20)
				.Output(SimHashes.Dirt, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(SOILMIXER_4_1, 4, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();


			//---- [ Lumber to Polluted Dirt ] -----------------------------------------------------------------------------------
			// Ingredient: Crushed Rock - 30kg
			//             Lumber - 60kg
			//             Polluted Water - 10kg
			// Result: Polluted Dirt - 100kg
			//--------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 60)
				.Input(SimHashes.WoodLog, 60)
				.Input(SimHashes.CrushedRock, 30)
				.Input(SimHashes.DirtyWater, 10)
				.Output(SimHashes.ToxicSand, 100)
				.Description(SOILMIXER_3_1, 3, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				//---- [ Dirt Recipe C ] -------------------------------------------------------------------------------------------
				// Ingredient: Crushed Rock - 50kg
				//             Biomass - 40kg
				//             Water - 10kg
				// Result: Dirt - 100kg
				//------------------------------------------------------------------------------------------------------------------

				RecipeBuilder.Create(ID, 60)
					.Input(ModElements.BioMass_Solid, 40)
					.Input(SimHashes.CrushedRock, 50)
					.Input(SimHashes.Water, 10)
					.Output(SimHashes.Dirt, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
					.Description(SOILMIXER_3_1, 3, 1)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Build();
			}
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, waterInputPort);
			controller.AssignPort(go, gasAmmoniaInputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}
