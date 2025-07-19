using Mineral_Processing_Mining.Buildings;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.MINING_AUGUR_DRILL;


namespace Mineral_Processing
{
	public class Mining_MineralDrillConfig : IBuildingConfig
	{
		public static string ID = "Mining_MineralDrill";
		private static readonly List<Storage.StoredItemModifier> DrillStoredItemModifiers = new List<Storage.StoredItemModifier>()
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Seal
		};
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 5, "mineral_drill_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER6, ["Steel"], 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, tieR2);
			BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
			buildingDef.EnergyConsumptionWhenActive = 1200f;
			buildingDef.SelfHeatKilowattsWhenActive = 20f;
			buildingDef.PowerInputOffset = new CellOffset(1, 1);
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = true;
			buildingDef.AudioCategory = "Metal";
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.BuildLocationRule = BuildLocationRule.OnFloor;
			SoundUtils.CopySoundsToAnim("mineral_drill_kanim", "rockrefinery_kanim");
			//buildingDef.OnePerWorld = true;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Prioritizable.AddRef(go);
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);


			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			//component.AddTag(GameTags.UniquePerWorld);

			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;

			ComplexFabricatorRandomOutput randomFabricator = go.AddOrGet<ComplexFabricatorRandomOutput>();
			randomFabricator.ByproductSpawnIntervalSeconds = 10;
			randomFabricator.StoreRandomOutputs = true;

			randomFabricator.heatedTemperature = 346.15f;
			randomFabricator.duplicantOperated = false;
			BuildingTemplates.CreateComplexFabricatorStorage(go, randomFabricator);
			randomFabricator.inStorage.SetDefaultStoredItemModifiers(DrillStoredItemModifiers);
			randomFabricator.buildStorage.SetDefaultStoredItemModifiers(DrillStoredItemModifiers);
			randomFabricator.buildStorage.allowItemRemoval = false;
			randomFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			randomFabricator.fetchChoreTypeIdHash = Db.Get().ChoreTypes.FabricateFetch.IdHash;

			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();


			var worldElementDropper = go.AddOrGet<WorldElementDropper>();
			worldElementDropper.DropGases = true;
			worldElementDropper.DropLiquids = true;
			worldElementDropper.DropSolids = true;
			worldElementDropper.TargetStorage = randomFabricator.outStorage;
			worldElementDropper.SpawnOffset = new CellOffset(1, 0);

			this.ConfigureRecipes();
		}

		private void ConfigureRecipes()
		{
			RecipeBuilder.Create(ID, 150)
				.Input(SimHashes.Copper, 400)
				.Output(SimHashes.CrushedRock, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.NameOverride(BASIC_DRILLING)
				.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, SimHashes.Copper.CreateTag(), global::STRINGS.ELEMENTS.COPPER.NAME))
				.Build();

			RecipeBuilder.Create(ID, 150)
				.Input(SimHashes.Iron, 400)
				.Output(SimHashes.CrushedRock, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.NameOverride(BASIC_DRILLING)
				.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, SimHashes.Iron.CreateTag(), global::STRINGS.ELEMENTS.IRON.NAME))
				.Build();

			RecipeBuilder.Create(ID, 240)
				.Input(SimHashes.Steel, 200)
				.Output(SimHashes.CrushedRock, 100, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Ingredient)
				.NameOverride(BASIC_DRILLING)
				.Description(RandomRecipeProducts.GetAugerDrillRandomResultString(ID, SimHashes.Steel.CreateTag(), global::STRINGS.ELEMENTS.STEEL.NAME))
				.Build();
		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<ActiveLightController>();
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
			{
				Light2D light2D = go.AddComponent<Light2D>();
				light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
				light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
				light2D.Range = 8f;
				light2D.Angle = 2.6f;
				light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
				light2D.Offset = new(0, 4);
				light2D.shape = LightShape.Cone;
				light2D.drawOverlay = true;
				light2D.Lux = 1800;
				light2D.autoRespondToOperational = false;
			}
			{
				Light2D light2D = go.AddComponent<Light2D>();
				light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
				light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
				light2D.Range = 8f;
				light2D.Angle = 2.6f;
				light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
				light2D.Offset = new(1,4);
				light2D.shape = LightShape.Cone;
				light2D.drawOverlay = true;
				light2D.Lux = 1800;
				light2D.autoRespondToOperational = false;
			}
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => base.DoPostConfigurePreview(def, go);

		public override void DoPostConfigureUnderConstruction(GameObject go) => base.DoPostConfigureUnderConstruction(go);
	}
}
