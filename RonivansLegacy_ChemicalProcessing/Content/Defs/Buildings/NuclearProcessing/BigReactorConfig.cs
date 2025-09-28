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

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.NuclearProcessing
{
	/// <summary>
	/// This is experimental, based on
	/// https://ko-fi.com/post/Nuclear-Processing-Mod-Update-Ideas-N4N0TDXRY
	/// https://ko-fi.com/post/Nuclear-Processing-Update-1-X8X3VDU0O
	/// and 
	/// https://ko-fi.com/post/Nuclear-Processing-Update-2-N4N2VGLHY
	/// </summary>
	internal class BigReactorConfig : IBuildingConfig
	{
		public static string ID = "AIO_BigNuclearReactor";

		private static readonly PortDisplayInput WaterLeft = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-2, 11), null, new Color32(61, 142, 255, 255));
		private static readonly PortDisplayInput WaterRight = new PortDisplayInput(ConduitType.Liquid, new CellOffset(2, 11), null, new Color32(61, 142, 255, 255));
		public override string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass =
			[
				10000f,
				2000f
			];
			string[] construction_materials =
			[
				"Steel",
				SimHashes.Ceramic.ToString()
			];
			EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 11, "big_nuclear_reactor_kanim", 100, 90f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tieR5);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 2000f;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.Deprecated = true;//hide it from codex 
			SoundUtils.CopySoundsToAnim("hep_calcinator_kanim", "suit_maker_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			PipedComplexFabricator fabricator = go.AddOrGet<PipedComplexFabricator>();
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			fabricator.duplicantOperated = false;
			fabricator.keepExcessLiquids = true;
			fabricator.keepExcessGasses = true;
			fabricator.storeProduced = true;
			fabricator.heatedTemperature = UtilMethods.GetKelvinFromC(1600);

			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<CopyBuildingSettings>();


			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.inStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			fabricator.outStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

			//BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
			//buildingElementEmitter.emitRate = 0.1f;
			//buildingElementEmitter.temperature = 338.15f;
			//buildingElementEmitter.element = SimHashes.CarbonDioxide;
			//buildingElementEmitter.emitDiseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id);
			//buildingElementEmitter.emitDiseaseCount = 1000;
			//buildingElementEmitter.modifierOffset = new Vector2(-3.0f, 1f);


			RadiationEmitter radiationEmitter = go.AddOrGet<RadiationEmitter>();
			radiationEmitter.emitRadiusX = 50;
			radiationEmitter.emitRadiusY = 50;
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emissionOffset = new Vector3(0.0f, 5f, 0.0f);
			radiationEmitter.emitRads = 0;

			PortConduitConsumer waterRight = go.AddComponent<PortConduitConsumer>();
			waterRight.conduitType = ConduitType.Liquid;
			waterRight.consumptionRate = 50f;
			waterRight.capacityKG = 10000f;
			waterRight.capacityTag = SimHashes.Water.CreateTag();
			waterRight.forceAlwaysSatisfied = true;
			waterRight.alwaysConsume = true;
			waterRight.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			waterRight.storage = fabricator.inStorage;
			waterRight.AssignPort(WaterRight);

			PortConduitConsumer waterLeft = go.AddComponent<PortConduitConsumer>();
			waterLeft.conduitType = ConduitType.Liquid;
			waterLeft.consumptionRate = 50f;
			waterLeft.capacityKG = 10000f;
			waterLeft.capacityTag = SimHashes.Water.CreateTag();
			waterLeft.forceAlwaysSatisfied = true;
			waterLeft.alwaysConsume = true;
			waterLeft.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			waterLeft.storage = fabricator.inStorage;
			waterLeft.AssignPort(WaterLeft);

			var dropper = go.AddOrGet<WorldElementDropper>();
			dropper.DropGases = true;
			dropper.TargetStorage = fabricator.outStorage;
			dropper.SpawnOffset = new CellOffset(0, -2);

			go.AddOrGet<ActiveRadEmitter>();

			Prioritizable.AddRef(go);
			ConfigureRecipes();
			AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, WaterLeft);
			controller.AssignPort(go, WaterRight);
		}
		private void ConfigureRecipes()
		{
			if (DlcManager.IsPureVanilla())
				return;

			//RecipeBuilder.Create(ID, 200)
			//	.Input(SimHashes.Water, 4000)
			//	.Input(SimHashes.EnrichedUranium, 10)
			//	.Output(SimHashes.Steam, 4000, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
			//	.Description("Test Reaction")
			//	.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
			//	.Build();

		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>();
			SymbolOverrideControllerUtil.AddToPrefab(go);
			AttachPort(go);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => AttachPort(go);

		public override void DoPostConfigureUnderConstruction(GameObject go) => AttachPort(go);

	}
}
