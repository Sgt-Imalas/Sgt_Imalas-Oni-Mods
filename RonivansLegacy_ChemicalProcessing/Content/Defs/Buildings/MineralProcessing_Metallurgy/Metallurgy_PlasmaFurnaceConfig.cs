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
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;


namespace Metallurgy.Buildings
{
	//===[ CHEMICAL: PLASMA FURNACE CONFIG ]========================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Metallurgy_PlasmaFurnaceConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------------------------------------------
		public static string ID = "Metallurgy_PlasmaFurnace";


		//--[ Special Settings ]------------------------------------------------------------------------------------
		private Tag FUEL_TAG = SimHashes.Hydrogen.CreateTag();
		private static readonly PortDisplayOutput MainOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, -2));
		private static readonly PortDisplayOutput WasteOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(1, -2));

		private static readonly List<Storage.StoredItemModifier> FurnaceStoredItemModifiers;
		static Metallurgy_PlasmaFurnaceConfig()
		{
			Color? MainPortColor = new Color32(255, 69, 56, 255);
			MainOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(-1, -2), null, MainPortColor);

			Color? WastePortColor = new Color32(97, 42, 38, 255);
			WasteOutputPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(1, -2), null, WastePortColor);

			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Preserve);
			list1.Add(Storage.StoredItemModifier.Insulate);
			list1.Add(Storage.StoredItemModifier.Seal);
			FurnaceStoredItemModifiers = list1;
		}

		//--[ Building Definitions ]---------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = [2000f, 1000f];
			string[] textArray1 = [SimHashes.Ceramic.ToString(), SimHashes.Tungsten.ToString()];

			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "plasma_furnace_kanim", 60, 120f, singleArray1, textArray1, 2400f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, noise, 0.2f);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 5000f;
			buildingDef.SelfHeatKilowattsWhenActive = 24f;
			buildingDef.Overheatable = true;
			buildingDef.OverheatTemperature = 363.15f;
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.UtilityInputOffset = new CellOffset(2, 0);
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			return buildingDef;
		}

		//--[ Building Operation Definitions ]-------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			Chemical_GlassForge temperatureHandler = go.AddOrGet<Chemical_GlassForge>();
			temperatureHandler.MeltingTemperature = ElementLoader.FindElementByHash(SimHashes.MoltenGlass).lowTemp;
			temperatureHandler.HeatedOutputOffset = new CellOffset(-1, -2);
			temperatureHandler.HeatedSecondaryOutputOffset = new CellOffset(1, -2);


			var fuelConsumer = go.AddOrGet<Chemical_FueledFabricatorAddon>();
			fuelConsumer.fuelTag = this.FUEL_TAG;

			var furnace = go.AddOrGet<ComplexFabricator>();

			furnace.heatedTemperature = 368.15f;
			furnace.duplicantOperated = true;
			furnace.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			ComplexFabricatorWorkable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			workable.overrideAnims = [Assets.GetAnim("anim_interacts_fabricator_generic_kanim")];
			BuildingTemplates.CreateComplexFabricatorStorage(go, furnace);
			furnace.outStorage.capacityKg = 2000f;
			furnace.inStorage.capacityKg = 2000f;
			furnace.storeProduced = true;
			furnace.inStorage.SetDefaultStoredItemModifiers(FurnaceStoredItemModifiers);
			furnace.buildStorage.SetDefaultStoredItemModifiers(FurnaceStoredItemModifiers);
			furnace.outStorage.SetDefaultStoredItemModifiers(FurnaceStoredItemModifiers);

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.capacityTag = this.FUEL_TAG;
			conduitConsumer.capacityKG = 50f;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.storage = furnace.inStorage;
			conduitConsumer.forceAlwaysSatisfied = true;

			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements =
			[new ElementConverter.ConsumedElement(this.FUEL_TAG, 0.1f, true)];
			elementConverter.outputElements =
			[new ElementConverter.OutputElement(0.025f, SimHashes.CarbonDioxide, 348.15f, false, false, 0f, 2f, 1f, byte.MaxValue, 0, true)];

			PipedConduitDispenser mainOutputPort = go.AddComponent<PipedConduitDispenser>();
			mainOutputPort.storage = furnace.outStorage;
			mainOutputPort.tagFilter = [GameTags.Glass, GameTags.RefinedMetal];

			mainOutputPort.AssignPort(MainOutputPort);
			mainOutputPort.alwaysDispense = true;
			mainOutputPort.SkipSetOperational = true;

			PipedConduitDispenser wasteOutputPort = go.AddComponent<PipedConduitDispenser>();
			wasteOutputPort.storage = furnace.outStorage;
			wasteOutputPort.tagFilter = [GameTags.Glass, GameTags.RefinedMetal];
			wasteOutputPort.invertElementFilter = true;

			wasteOutputPort.AssignPort(WasteOutputPort);
			wasteOutputPort.alwaysDispense = true;
			wasteOutputPort.SkipSetOperational = true;

			PipedOptionalExhaust exhaustGlass = go.AddComponent<PipedOptionalExhaust>();
			exhaustGlass.dispenser = mainOutputPort;
			exhaustGlass.elementTag = SimHashes.MoltenGlass.CreateTag();
			exhaustGlass.capacity = 100f;

			PipedOptionalExhaust exhaustMoltenMetals = go.AddComponent<PipedOptionalExhaust>();
			exhaustGlass.dispenser = mainOutputPort;
			exhaustGlass.elementTag = GameTags.RefinedMetal;
			exhaustGlass.capacity = 100f;

			PipedOptionalExhaust exhaustMoltenSlag = go.AddComponent<PipedOptionalExhaust>();
			exhaustMoltenSlag.dispenser = wasteOutputPort;
			exhaustMoltenSlag.elementTag = ModElements.Slag_Liquid.Tag;
			exhaustMoltenSlag.capacity = 100f;


			this.AttachPort(go);
			Prioritizable.AddRef(go);
			this.ConfigureRecipes();
		}

		//===[ CHEMICAL: PLASMA FURNACE RECIPES ]====================================================================================
		private void ConfigureRecipes()
		{
			bool chemProcActive = Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;
			int index = 0;
			//---- [ Glass Smelting ] ----------------------------------------------------------------------------------------------
			// Ingredient: Sand - 150kg
			//             Borax - 10kg
			// Result: Molten Glass - 100kg
			//         Molten Slag - 60kg
			//----------------------------------------------------------------------------------------------------------------------
			if (chemProcActive)
			{
				RecipeBuilder.Create(ID, 10f)
					.Input(SimHashes.Sand, 150)
					.Input(ModElements.Borax_Solid, 10)
					.Output(SimHashes.MoltenGlass, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(ModElements.Slag_Liquid, 60, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_2_1_1, 2, 2)
					.SortOrder(index++)
					.Build();
			}
			else
			{
				RecipeBuilder.Create(ID, 10f)
					.Input(SimHashes.Sand, 150)
					.Input(SimHashes.Salt, 10)
					.Output(SimHashes.MoltenGlass, 150, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_2_1, 2, 1)
					.SortOrder(index++)
					.Build();
			}

			//---- [ Ore Smelting | Ore to Metal Ratio: 98% ] ] -------------------------------------------------------------
			// Ingredient: Ore - 500kg
			//             Sand - 40kg
			// Result: Molten Refined Metal - 490kg
			//         Molten Slag - 50kg
			//----------------------------------------------------------------------------------------------------------------------
			var specialOres = RefinementRecipeHelper.GetSpecialOres();
			foreach (var element in ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.Ore)))
			{
				if (specialOres.Contains(element.id) || element.HasTag(GameTags.Noncrushable) || element.HasTag(ModAssets.Tags.RandomSand))
				{
					continue;
				}

				Element refinedElementMolten = element.highTempTransition;
				if (refinedElementMolten.IsGas)
					continue;


				if (chemProcActive)
				{
					RecipeBuilder.Create(ID, 10)
						.Input(element.tag, 500f)
						.Input(SimHashes.Sand.CreateTag(), 40f)
						.Output(refinedElementMolten.tag, 490, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
						.Output(ModElements.Slag_Liquid, 50, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
						.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
						.SortOrder(index++)
						.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_2_1_1, 2, 2)
						.Build();
				}
				else
				{
					RecipeBuilder.Create(ID, 10)
						.Input(element.tag, 500f)
						.Output(refinedElementMolten.tag, 500, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
						.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
						.SortOrder(index++)
						.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_1_1, 1, 1)
						.Build();
				}
			}
			//---- [ Electrum Smelting | Ore to Metal Ratio: 98% ]] ---------------------------------------------------------------------------------------------------------
			// Ingredient: Electrum - 100kg
			// Result: Gold - 25kg
			//         Silver - 15kg
			//         Sand - 50g
			//------------------------------------------------------------------------------------------------------------------------------------
			if(chemProcActive)
			{
				RecipeBuilder.Create(ID, 10)
					.Input(SimHashes.Electrum, 500f)
					.Input(SimHashes.Sand.CreateTag(), 40f)
					.Output(SimHashes.MoltenGold, 490f * 0.6f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(ModElements.Silver_Liquid, 490f * 0.4f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(ModElements.Slag_Liquid, 50, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_2_2_1, 2, 3)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.SortOrder(index++)
					.Build();
			}
			else
			{
				RecipeBuilder.Create(ID, 10)
					.Input(SimHashes.Electrum, 500f)
					.Output(SimHashes.MoltenGold, 300, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(SimHashes.MoltenCopper, 200, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_1_2, 1, 2)
					.SortOrder(index++)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Build();
			}

			//---- [ Galena Smelting | Ore to Metal Ratio: 98% ]] ---------------------------------------------------------------------------------------------------------
			// Ingredient: Galena - 100kg
			// Result: Silver - 25kg
			//         Lead - 15kg
			//         Sand - 50g
			//------------------------------------------------------------------------------------------------------------------------------------
			if (chemProcActive)
				RecipeBuilder.Create(ID, 10)
					.Input(ModElements.Galena_Solid, 500f)
					.Input(SimHashes.Sand.CreateTag(), 40f)
					.Output(ModElements.Silver_Liquid, 490f * 0.6f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(SimHashes.MoltenLead, 490f * 0.4f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(ModElements.Slag_Liquid, 50, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_2_2_1, 2, 3)
					.SortOrder(index++)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Build();

			///pyrite is less efficient in other recipes, approximating that here:
			///advanced refinery has 92.5% yield for others while pyrite has 75% there 

			//---- [ Pyrite Smelting | Ore to Metal Ratio: 98%]] -----------------------------------------------------------------------------------------------------
			// Ingredient: Pyrite - 100kg
			// Result: Iron - 30kg
			//         Sand - 70g
			//-------------------------------------------------------------------------------------------------------------------------------
			if (chemProcActive)
				RecipeBuilder.Create(ID, 10)
					.Input(SimHashes.FoolsGold, 500f)
					.Input(SimHashes.Sand.CreateTag(), 40f)
					.Output(SimHashes.MoltenIron, 400f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(ModElements.Slag_Liquid, 140, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_2_1_1, 2, 2)
					.SortOrder(index++)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Build();
			else
				RecipeBuilder.Create(ID, 10)
					.Input(SimHashes.FoolsGold, 500f)
					.Output(SimHashes.MoltenIron, 400f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Output(SimHashes.LiquidSulfur, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
					.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_1_2, 1, 2)
					.SortOrder(index++)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
					.Build();

			//---- [ Steel Smelting ] -------------------------------------------------------------------------------------------
			// Ingredient: Iron - 425kg
			//             Refined Coal - 50kg
			//             Lime - 25kg
			// Result:     Molten Steel - 500kg
			//----------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID,10)
				.Input(SimHashes.Iron, 425f)
				.Input(SimHashes.RefinedCarbon, 50f)
				.Input(SimHashes.Lime, 25f)
				.Output(SimHashes.MoltenSteel, 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_STEEL, 3, 1)
				.SortOrder(index++)
				.Build();

			//---- [ Abyssalite Smelting ] -----------------------------------------------------------------------------------------
			// Ingredient: Abyssalite - 500kg
			// Result: Molten Tungsten - 120kg
			//         Magma - 380kg
			//----------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID,20)
				.Input(SimHashes.Katairite, 500f)
				.Input(SimHashes.Lime,20)
				.Output(SimHashes.MoltenTungsten, 120f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
				.Output(SimHashes.Magma, 380f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted, true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.PLASMAFURNACE_1_1, 1, 1)
				.SortOrder(index++)
				.Build();
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, MainOutputPort);
			controller.AssignPort(go, WasteOutputPort);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			this.AttachPort(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			SymbolOverrideControllerUtil.AddToPrefab(go);
			go.AddOrGetDef<PoweredActiveStoppableController.Def>();
			go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject game_object)
			{
				ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
				component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
				component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
				component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
				component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
				component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
			};
		}
	}
}
