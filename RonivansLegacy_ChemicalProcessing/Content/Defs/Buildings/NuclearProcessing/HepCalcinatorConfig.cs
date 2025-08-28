using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.NuclearProcessing
{
	public class HepCalcinatorConfig : IBuildingConfig
	{
		public static string ID = "HepCalcinator";
		private static readonly int HEP_STORAGE_CAPACITY = 1000;
		private static readonly List<Storage.StoredItemModifier> CalcinatorItemModifiers = new()
		{
		  Storage.StoredItemModifier.Hide,
		  Storage.StoredItemModifier.Preserve,
		  Storage.StoredItemModifier.Insulate,
		  Storage.StoredItemModifier.Seal
		};

		public override string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass =
			[
				1000f,
				600f
			];
			string[] construction_materials =
			[
				"RefinedMetal",
				SimHashes.Ceramic.ToString()
			];
			EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 3, "hep_calcinator_kanim", 100, 90f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tieR5);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 3000f;
			buildingDef.BaseMeltingPoint = 2400f;
			buildingDef.ExhaustKilowattsWhenActive = 16f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.UseHighEnergyParticleInputPort = true;
			buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 2);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.LogicOutputPorts = [LogicPorts.Port.OutputPort(HEPStorageThreshold.PORT_ID, new CellOffset(1, 1), global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, global::STRINGS. BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)];

			SoundUtils.CopySoundsToAnim("hep_calcinator_kanim", "suit_maker_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			fabricator.duplicantOperated = true;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<CopyBuildingSettings>();
			HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
			energyParticleStorage.capacity = HEP_STORAGE_CAPACITY;
			energyParticleStorage.autoStore = true;
			energyParticleStorage.showCapacityStatusItem = true;
			Workable workable = (Workable)go.AddOrGet<ComplexFabricatorWorkable>();

			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.inStorage.SetDefaultStoredItemModifiers(HepCalcinatorConfig.CalcinatorItemModifiers);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(HepCalcinatorConfig.CalcinatorItemModifiers);
			workable.overrideAnims = [Assets.GetAnim((HashedString) "anim_interacts_material_research_centre_kanim")];
			BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
			buildingElementEmitter.emitRate = 0.1f;
			buildingElementEmitter.temperature = 338.15f;
			buildingElementEmitter.element = SimHashes.CarbonDioxide;
			buildingElementEmitter.emitDiseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id);
			buildingElementEmitter.emitDiseaseCount = 1000;
			buildingElementEmitter.modifierOffset = new Vector2(-3.0f, 1f);

			go.AddOrGet<HEPStorageMeterHandler>();
			go.AddOrGet<HEPStorageThreshold>();

			go.AddOrGet<HEPEmitterOperationalController>();

			RadiationEmitter radiationEmitter = go.AddOrGet<RadiationEmitter>();
			radiationEmitter.emitRadiusX = (short)7;
			radiationEmitter.emitRadiusY = (short)7;
			radiationEmitter.emitRate = 3f;
			radiationEmitter.emitRads = 200.0f;
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Pulsing;
			radiationEmitter.emissionOffset = new Vector3(0.5f, 1f, 0.0f);

			Prioritizable.AddRef(go);
			ConfigureRecipes();
		}

		private void ConfigureRecipes()
		{
			if(DlcManager.IsPureVanilla())
				return;

			RecipeBuilder.Create(ID, 80)
				.Input(SimHashes.UraniumOre, 100)
				.Input(SimHashes.OxyRock, 100)
				.InputHEP(20)
				.Output(SimHashes.Yellowcake, 100)
				.Description(CALCINATOR_1_1, 1, 1)				
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			RecipeBuilder.Create(ID, 80)
				.Input(SimHashes.DepletedUranium, 10)
				.Input(SimHashes.SolidNuclearWaste, 90)
				.Input(SimHashes.OxyRock, 100)
				.InputHEP(50)
				.Output(SimHashes.Yellowcake, 100)
				.Description(CALCINATOR_2_1, 2, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			SymbolOverrideControllerUtil.AddToPrefab(go);
			go.GetComponent<KPrefabID>().prefabSpawnFn += (KPrefabID.PrefabFn)(game_object =>
			{
				ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
				component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
				component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
				component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
				component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
				component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;				
			});
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => base.DoPostConfigurePreview(def, go);

		public override void DoPostConfigureUnderConstruction(GameObject go) => base.DoPostConfigureUnderConstruction(go);
	}
}
