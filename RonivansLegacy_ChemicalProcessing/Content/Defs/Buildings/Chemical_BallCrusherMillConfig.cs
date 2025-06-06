using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//==== [ CHEMICAL: BALL CRUSHER MILL CONFIG ] ===================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_BallCrusherMillConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_BallCrusherMill";
		
		public static readonly List<Storage.StoredItemModifier> BallmillStoredItemModifiers;

		//--[ Special Settings ]-----------------------------------------------
		private static readonly PortDisplayInput sulfuricAcidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-2, 1));
		private static readonly PortDisplayInput nitricAcidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-2, 0));

		static Chemical_BallCrusherMillConfig()
		{
			Color? sulfuricPortColor = new Color32(252, 252, 3, 255);
			sulfuricAcidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-2, 1), null, sulfuricPortColor);
			Color? nitricPortColor = new Color32(255, 68, 0, 255);
			nitricAcidInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-2, 0), null, nitricPortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Seal);
			list1.Add(Storage.StoredItemModifier.Insulate);
			BallmillStoredItemModifiers = list1;
		}

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] ingredient_mass = [300f, 100f];
			string[] ingredient_types = ["RefinedMetal", SimHashes.Steel.ToString()];

			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 3, "ball_mill_kanim", 100, 30f, ingredient_mass, ingredient_types, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier, 0.2f);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 800f;
			buildingDef.ExhaustKilowattsWhenActive = 12f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(-2, 2);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(3, 2);
			return buildingDef;
		}

		//--[ Building Operation Definitions ]---------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{

			Storage liquidStorage = go.AddOrGet<Storage>();
			liquidStorage.SetDefaultStoredItemModifiers(BallmillStoredItemModifiers);
			liquidStorage.showCapacityStatusItem = true;
			liquidStorage.showCapacityAsMainStatus = true;
			liquidStorage.showDescriptor = true;

			ConduitConsumer waterInput = go.AddOrGet<ConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 10f;
			waterInput.capacityKG = 100f;
			waterInput.storage = liquidStorage;
			waterInput.capacityTag = SimHashes.Water.CreateTag(); ;
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			PortConduitConsumer sulfuricInput = go.AddComponent<PortConduitConsumer>();
			sulfuricInput.conduitType = ConduitType.Liquid;
			sulfuricInput.consumptionRate = 10f;
			sulfuricInput.capacityKG = 50f;
			sulfuricInput.storage = liquidStorage;
			sulfuricInput.capacityTag = ModElements.SulphuricAcid_Liquid.Tag;
			sulfuricInput.forceAlwaysSatisfied = true;
			sulfuricInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			sulfuricInput.AssignPort(sulfuricAcidInputPort);

			PortConduitConsumer nitricInput = go.AddComponent<PortConduitConsumer>();
			nitricInput.conduitType = ConduitType.Liquid;
			nitricInput.consumptionRate = 10f;
			nitricInput.capacityKG = 50f;
			nitricInput.storage = liquidStorage;
			nitricInput.capacityTag = ModElements.NitricAcid_Liquid.Tag;
			nitricInput.forceAlwaysSatisfied = true;
			nitricInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			nitricInput.AssignPort(nitricAcidInputPort);

			Storage toxicStorage = go.AddOrGet<Storage>();
			toxicStorage.SetDefaultStoredItemModifiers(BallmillStoredItemModifiers);
			toxicStorage.showCapacityStatusItem = true;
			toxicStorage.showCapacityAsMainStatus = true;
			toxicStorage.showDescriptor = true;

			//----------------------------- Fabricator Section
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Chemical_BallCrusherMill_RandomFabricator ballMill = go.AddOrGet<Chemical_BallCrusherMill_RandomFabricator>();
			ballMill.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			ballMill.duplicantOperated = false;
			ballMill.heatedTemperature = 298.15f;
			BuildingTemplates.CreateComplexFabricatorStorage(go, ballMill);
			ballMill.keepExcessLiquids = true;
			ballMill.inStorage.capacityKg = 1000f;
			ballMill.buildStorage.capacityKg = 1000f;
			ballMill.outStorage.capacityKg = 1000f;
			ballMill.storeProduced = true;
			ballMill.inStorage.SetDefaultStoredItemModifiers(BallmillStoredItemModifiers);
			ballMill.buildStorage.SetDefaultStoredItemModifiers(BallmillStoredItemModifiers);
			ballMill.outStorage.SetDefaultStoredItemModifiers(BallmillStoredItemModifiers);
			ballMill.inStorage = liquidStorage;
			ballMill.outStorage = toxicStorage;
			ballMill.outputOffset = new Vector3(1f, 0.5f);
			//-----------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.storage = toxicStorage;
			dispenser.elementFilter = [ModElements.ToxicMix_Liquid];

			Prioritizable.AddRef(go);
			this.AttachPort(go);
			this.ConfigureRecipes();

		}

		//==== [ CHEMICAL: BALL CRUSHER MILL RECIPES ] ==================================================================
		private void ConfigureRecipes()
		{
			////---- [ Crushed Rock Milling ] -----------------------------------------------------------------------------
			//// Ingredient: Crushed Rock - 500kg
			//// Result: Sand - 500kg
			////-----------------------------------------------------------------------------------------------------------
			//ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
			//{
			//    new ComplexRecipe.RecipeElement(SimHashes.CrushedRock.CreateTag(), 500f)
			//};
			//ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
			//{
			//    new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			//};
			//var recipe_1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array, array2), array, array2)
			//{
			//    time = 25f,
			//    description = string.Format(string.Concat(new string[]
			//    {
			//    "Further crushes ",SimHashes.CrushedRock.CreateTag().ProperName()," to " +
			//    "",SimHashes.Sand.CreateTag().ProperName(),"."})),
			//    nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient,
			//    fabricators = new List<Tag> { ID },
			//    sortOrder = 1
			//};

			////---- [ Obsidian Milling ] ---------------------------------------------------------------------------------
			//// Ingredient: Obsidian - 500kg
			//// Result: Sand - 500kg
			////-----------------------------------------------------------------------------------------------------------
			//ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
			//{
			//    new ComplexRecipe.RecipeElement(SimHashes.Obsidian.CreateTag(), 500f)
			//};
			//ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
			//{
			//    new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			//};
			//var recipe_2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, array3, array4), array3, array4)
			//{
			//    time = 25f,
			//    description = string.Format(string.Concat(new string[]
			//    {
			//    "Crushes ",SimHashes.Obsidian.CreateTag().ProperName()," to " +
			//    "",SimHashes.Sand.CreateTag().ProperName(),"."})),
			//    nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient,
			//    fabricators = new List<Tag> { ID },
			//    sortOrder = 2
			//};

			//---- [ Sandstone Milling ] ---------------------------------------------------------------------------------
			// Ingredient: Sandstone - 300kg
			//             Water - 100kg
			//             Sulfuric Acid - 50kg
			// Random Results: Toxic Slurry, Fertilizer, Low-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.SandStone, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.SulphuricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Sedimentary Rock Milling ] -----------------------------------------------------------------------------
			// Ingredient: Sedimentary Rock - 300kg
			//             Water - 100kg
			//             Sulfuric Acid - 50kg
			// Random Results: Toxic Slurry, Low-Grade Metallic Sand, Base-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//---------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.SedimentaryRock, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.SulphuricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Shale, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.SulphuricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Granite Milling ] ------------------------------------------------------------------------------------------
			// Ingredient: Granite - 300kg
			//             Water - 100kg
			//             Sulfuric Acid - 50kg
			// Random Results: Toxic Slurry, Base-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//-------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Granite, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.SulphuricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Igneous Rock Milling ] ----------------------------------------------------------------------------------------
			// Ingredient: Igneous Rock - 300kg
			//             Water - 100kg
			//             Nitric Acid - 50kg
			// Random Results: Toxic Slurry, Sulfur, Base-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//----------------------------------------------------------------------------------------------------------------------
			
			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.IgneousRock, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.NitricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Mafic Rock Milling ] -----------------------------------------------------------------------------------------------
			// Ingredient: Mafic Rock - 300kg
			//             Water - 100kg
			//             Nitric Acid - 50kg
			// Random Results: Toxic Slurry, Phosphorus, Low-Grade Metallic Sand, Base-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//----------------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.MaficRock, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.NitricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Abyssalite Milling ] ----------------------------------------------------------------------------------------------------
			// Ingredient: Abyssalite - 300kg
			//             Water - 100kg
			//             Sulfuric Acid - 25kg
			//             Nitric Acid - 25kg
			// Random Results: Toxic Slurry, Phosphorus, Diamond, Low-Grade Metallic Sand, Base-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//--------------------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Katairite, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.NitricAcid_Liquid, 25f)
				.Input(ModElements.SulphuricAcid_Liquid, 25f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Regolith Milling ] ------------------------------------------------------------------------------------------------------
			// Ingredient: Abyssalite - 300kg
			//             Water - 100kg
			//             Sulfuric Acid - 50kg
			//             Nitric Acid - 50kg
			// Random Results: Toxic Slurry, Low-Grade Metallic Sand, Base-Grade Metallic Sand, High-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//--------------------------------------------------------------------------------------------------------------------------------

			RecipeBuilder.Create(ID, 50)
				.Input(SimHashes.Regolith, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.NitricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();

			//---- [ Meteor Ore Milling ] -------------------------------------------------------------------------------------------------------
			// Ingredient: Meteor Ore - 300kg
			//             Water - 100kg
			//             Sulfuric Acid - 50kg
			//             Nitric Acid - 50kg
			// Random Results: Toxic Slurry, Base-Grade Metallic Sand, High-Grade Metallic Sand
			// Assured Result: Toxic Slurry - 10kg
			//-----------------------------------------------------------------------------------------------------------------------------------
			
			RecipeBuilder.Create(ID, 50)
				.Input(ModElements.MeteorOre_Solid, 300f)
				.Input(SimHashes.Water, 100f)
				.Input(ModElements.NitricAcid_Liquid, 50f)
				.Input(ModElements.SulphuricAcid_Liquid, 50f)
				.Output(ModElements.ToxicMix_Liquid, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(RandomRecipeResults.GetBallCrusherResultsString)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.Build();
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, sulfuricAcidInputPort);
			controller.AssignPort(go, nitricAcidInputPort);
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
