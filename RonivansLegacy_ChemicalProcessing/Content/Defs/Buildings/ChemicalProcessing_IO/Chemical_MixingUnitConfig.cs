using KSerialization;
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
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS;
using static UtilLibs.UIcmp.FSlider;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_MixingUnitConfig : IBuildingConfig
	{
		public static string ID = "Chemical_MixingUnit";

		public static readonly List<Storage.StoredItemModifier> MixerStoredItemModifiers = new List<Storage.StoredItemModifier>()
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate
			};
		private static readonly PortDisplayInput liquidWaterInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(-2, 1), color: new Color?((Color)new Color32((byte)66, (byte)135, (byte)245, byte.MaxValue)));
		private static readonly PortDisplayInput liquidPetrolInputPort = new PortDisplayInput(ConduitType.Liquid, new CellOffset(3, 2), color: new Color?((Color)new Color32(byte.MaxValue, (byte)224, (byte)20, byte.MaxValue)));
		private static readonly PortDisplayInput gasNitrogenInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 1), color: new Color?((Color)new Color32((byte)205, (byte)194, byte.MaxValue, byte.MaxValue)));
		private static readonly PortDisplayInput gasAmmoniaInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(3, 0), color: new Color?((Color)new Color32((byte)215, (byte)227, (byte)252, byte.MaxValue)));
		private static readonly PortDisplayInput gasPropaneInputPort = new PortDisplayInput(ConduitType.Gas, new CellOffset(-2, 0), color: new Color?((Color)new Color32((byte)50, (byte)84, (byte)207, byte.MaxValue)));
		private static readonly PortDisplayOutput simpleOutputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(1, 0), color: new Color?((Color)new Color32((byte)17, (byte)210, (byte)132, byte.MaxValue)));


		private void ConfigureRecipes()
		{
			RecipeBuilder.Create(ID, 20)
				.Input(SimHashes.Sulfur, 20)
				.Input(SimHashes.Water, 30)
				.Output(ModElements.SulphuricAcid_Liquid, 50, storeElement: true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description2I1O(CHEMICAL_MIXINGUNIT_2_1)
				.Build();

			RecipeBuilder.Create(ID, 20)
				.Input(ModElements.Nitrogen_Gas, 20)
				.Input(SimHashes.Water, 30)
				.Output(ModElements.NitricAcid_Liquid, 50, storeElement: true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description2I1O(CHEMICAL_MIXINGUNIT_2_1)
				.Build();

			RecipeBuilder.Create(ID,40)
				.Input(SimHashes.Petroleum, 30)
				.Input(ModElements.Nitrogen_Gas,19)
				.Input(SimHashes.Fullerene,1)
				.Output(SimHashes.SuperCoolant, 50, storeElement: true)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(CHEMICAL_MIXINGUNIT_3_1,3,1)
				.RequiresTech(GameStrings.Technology.Gases.Catalytics)
				.Build();

			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.Propane, 50)
				.Input(SimHashes.Petroleum, 49)
				.Input(SimHashes.Fullerene, 1)
				.Output(ModElements.Isopropane_Gas, 100f, storeElement: true)
				.Description(CHEMICAL_MIXINGUNIT_3_1, 3, 1)
				.RequiresTech(GameStrings.Technology.Gases.Catalytics)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.IngredientToResult)
				.Build();

			RecipeBuilder.Create(ID,20)
				.Input(ModElements.Ammonia_Gas, 5)
				.Input(SimHashes.Dirt, 35)
				.Input(SimHashes.Phosphorus,5)
				.Input(SimHashes.Sulfur,5)
				.Output(SimHashes.Fertilizer,50)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(CHEMICAL_MIXINGUNIT_FERTILIZER, 4, 1)
				.Build();

		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [400f, 600f];
			string[] construction_materials =
			[
				SimHashes.Steel.ToString(),
				"RefinedMetal"
			];
			EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Chemical_MixingUnit", 6, 4, "chemical_plant_kanim", 100, 30f, construction_mass, construction_materials, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tieR6);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 800f;
			buildingDef.ExhaustKilowattsWhenActive = 8f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			SoundUtils.CopySoundsToAnim("chemical_plant_kanim", "chemistry_lab_kanim");
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{

			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			PipedComplexFabricator fabricator = go.AddOrGet<PipedComplexFabricator>();


			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();



			var workable = go.AddOrGet<ComplexFabricatorWorkable>();
			workable.overrideAnims =
			[
				Assets.GetAnim((HashedString) "anim_interacts_research2_kanim")
			];
			fabricator.duplicantOperated = true;
			fabricator.heatedTemperature = 298.15f;
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.keepExcessLiquids = true;
			fabricator.keepExcessGasses = true;	
			fabricator.inStorage.capacityKg = 1000f;
			fabricator.buildStorage.capacityKg = 1000f;
			fabricator.outStorage.capacityKg = 1000f;
			fabricator.storeProduced = true;
			fabricator.inStorage.SetDefaultStoredItemModifiers(MixerStoredItemModifiers);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(MixerStoredItemModifiers);
			fabricator.outStorage.SetDefaultStoredItemModifiers(MixerStoredItemModifiers);
			fabricator.outputOffset = new Vector3(1f, 0.5f);
			var inputStorage = fabricator.inStorage;
			var outpuStorage = fabricator.outStorage;

			PortConduitConsumer portConduitConsumer1 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer1.conduitType = ConduitType.Gas;
			portConduitConsumer1.consumptionRate = 10f;
			portConduitConsumer1.capacityKG = 50f;
			portConduitConsumer1.storage = inputStorage;
			portConduitConsumer1.capacityTag = ModElements.Nitrogen_Gas.Tag;
			portConduitConsumer1.forceAlwaysSatisfied = true;
			portConduitConsumer1.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer1.AssignPort(gasNitrogenInputPort);

			PortConduitConsumer portConduitConsumer2 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer2.conduitType = ConduitType.Gas;
			portConduitConsumer2.consumptionRate = 10f;
			portConduitConsumer2.capacityKG = 50f;
			portConduitConsumer2.storage = inputStorage;
			portConduitConsumer2.capacityTag = ModElements.Ammonia_Gas.Tag;
			portConduitConsumer2.forceAlwaysSatisfied = true;
			portConduitConsumer2.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer2.AssignPort(gasAmmoniaInputPort);

			PortConduitConsumer portConduitConsumer3 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer3.conduitType = ConduitType.Liquid;
			portConduitConsumer3.consumptionRate = 10f;
			portConduitConsumer3.capacityKG = 50f;
			portConduitConsumer3.storage = inputStorage;
			portConduitConsumer3.capacityTag = SimHashes.Propane.CreateTag();
			portConduitConsumer3.forceAlwaysSatisfied = true;
			portConduitConsumer3.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer3.AssignPort(gasPropaneInputPort);

			PortConduitConsumer portConduitConsumer4 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer4.conduitType = ConduitType.Liquid;
			portConduitConsumer4.consumptionRate = 10f;
			portConduitConsumer4.capacityKG = 100f;
			portConduitConsumer4.storage = inputStorage;
			portConduitConsumer4.capacityTag = SimHashes.Water.CreateTag();
			portConduitConsumer4.forceAlwaysSatisfied = true;
			portConduitConsumer4.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer4.AssignPort(liquidWaterInputPort);

			outpuStorage.SetDefaultStoredItemModifiers(MixerStoredItemModifiers);


			PortConduitConsumer portConduitConsumer5 = go.AddComponent<PortConduitConsumer>();
			portConduitConsumer5.conduitType = ConduitType.Liquid;
			portConduitConsumer5.consumptionRate = 10f;
			portConduitConsumer5.capacityKG = 100f;
			portConduitConsumer5.storage = inputStorage;
			portConduitConsumer5.capacityTag = SimHashes.Petroleum.CreateTag();
			portConduitConsumer5.forceAlwaysSatisfied = true;
			portConduitConsumer5.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			portConduitConsumer5.AssignPort(liquidPetrolInputPort);

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.storage = outpuStorage;
			conduitDispenser.elementFilter =[SimHashes.SuperCoolant, ModElements.SulphuricAcid_Liquid, ModElements.NitricAcid_Liquid];

			PipedConduitDispenser pipedDispenser = go.AddComponent<PipedConduitDispenser>();
			pipedDispenser.conduitType = ConduitType.Gas;
			pipedDispenser.storage = outpuStorage;
			pipedDispenser.alwaysDispense = true;
			pipedDispenser.elementFilter =
			[
				ModElements.Isopropane_Gas
			];
			pipedDispenser.AssignPort(simpleOutputPort);
			Prioritizable.AddRef(go);
			this.AttachPort(go);
			ConfigureRecipes();
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController displayController = go.AddComponent<PortDisplayController>();
			displayController.Init(go);
			displayController.AssignPort(go, liquidWaterInputPort);
			displayController.AssignPort(go, liquidPetrolInputPort);
			displayController.AssignPort(go, gasNitrogenInputPort);
			displayController.AssignPort(go, gasAmmoniaInputPort);
			displayController.AssignPort(go, gasPropaneInputPort);
			displayController.AssignPort(go, simpleOutputPort);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;

			go.GetComponent<KPrefabID>().prefabInitFn += (KPrefabID.PrefabFn)(inst => { inst.GetComponent<ComplexFabricatorWorkable>().SetOffsets([new(1, 0)]); });
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			this.AttachPort(go);
			go.AddOrGet<PortPreviewVisualizer>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			this.AttachPort(go);
		}
	}
}
