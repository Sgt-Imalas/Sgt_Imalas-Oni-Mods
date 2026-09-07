using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.BuildingConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;


namespace Biochemistry.Buildings
{
	public class Biochemistry_BiodieselGeneratorConfig : IBuildingConfig, IHasConfigurableRateMultiplier
	{
		public static string ID = "Biochemistry_BiodieselGenerator";

		public static float RateMultiplier = 1f;
		public void SetMultiplier(float multiplier)
		{
			RateMultiplier = multiplier;
		}
		public Func<BuildingConfigurationEntry, string> GetCurrentRateDescription()
		{
			return (entry) =>
			{
				string toFormat = RonivansLegacy_ChemicalProcessing.STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RATESETTING_GENERATOR_TOOLTIP;

				float currentMultiplier = entry.GetRateMultiplier();

				return toFormat
				.Replace("{PERCENTAGE}", GameUtil.GetFormattedPercent(currentMultiplier * 100f))
				.Replace("{WATTAGE}", GameUtil.GetFormattedWattage(3200f * currentMultiplier))
				.Replace("{FUEL}", RonivansLegacy_ChemicalProcessing.STRINGS.ELEMENTS.LIQUIDBIODIESEL.NAME)
				.Replace("{RATE}", GameUtil.GetFormattedMass(0.300f * currentMultiplier))
				;
			};
		}
		public string GetRateLabel() => RonivansLegacy_ChemicalProcessing.STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RATESETTING_GENERATOR;

		static Biochemistry_BiodieselGeneratorConfig()
		{
			GeneratorList.AddGeneratorToIgnore(ID);
			//GeneratorList.AddCombustionGenerator(ID);
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 4, "biodiesel_generator_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.GeneratorWattageRating = 3200f * RateMultiplier;
			buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
			buildingDef.ExhaustKilowattsWhenActive = 2f * RateMultiplier;
			buildingDef.SelfHeatKilowattsWhenActive = 2f * RateMultiplier;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
			buildingDef.RequiresPowerOutput = true;
			buildingDef.PowerOutputOffset = new CellOffset(-1, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			SoundUtils.CopySoundsToAnim("biodiesel_generator_kanim", "generatorpetrol_kanim");
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			base.ConfigureBuildingTemplate(go, prefab_tag);

			var kprefab = go.GetComponent<KPrefabID>();
			kprefab.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			kprefab.AddTag(RoomConstraints.ConstraintTags.PowerBuilding);
			kprefab.AddTag(RoomConstraints.ConstraintTags.GeneratorType);
			kprefab.AddTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			BuildingDef def = go.GetComponent<Building>().Def;
			go.AddOrGet<LoopingSounds>();

			ManualDeliveryKG ykg = go.AddOrGet<ManualDeliveryKG>();
			ykg.SetStorage(storage);
			ykg.RequestedItemTag = ModElements.BioDiesel_Liquid.Tag;
			ykg.capacity = 0f;
			ykg.refillMass = 0f;
			ykg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			// 

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = def.InputConduitType;
			conduitConsumer.consumptionRate = 10f * RateMultiplier;
			conduitConsumer.capacityTag = ModAssets.Tags.AIO_BioFuel;// ModElements.BioDiesel_Liquid.Tag;
			conduitConsumer.capacityKG = 32f * RateMultiplier;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			BiodieselEnergyGenerator energyGenerator = go.AddOrGet<BiodieselEnergyGenerator>();
			energyGenerator.powerDistributionOrder = 8;
			energyGenerator.ignoreBatteryRefillPercent = true;
			energyGenerator.hasMeter = true;
			energyGenerator.formula = new EnergyGenerator.Formula
			{
				inputs =
				[
				new EnergyGenerator.InputItem(ModElements.BioDiesel_Liquid.Tag, 0.300f* RateMultiplier, 32f* RateMultiplier)
				//new EnergyGenerator.InputItem( ModElements.BiodieselGroup, 3.2f, 32f)
				],
				outputs =
				[
				new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.14428f* RateMultiplier, true, new CellOffset(1, 1), 313.15f)
				//new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 1.215f, true, new CellOffset(1, 1), 313.15f)
				]
			};
			energyGenerator.modDieselFormula = energyGenerator.formula;
			energyGenerator.vanillaDieselFormula = new EnergyGenerator.Formula
			{
				inputs =
				[
				new EnergyGenerator.InputItem(SimHashes.RefinedLipid.CreateTag(), 3.200f* RateMultiplier, 32f* RateMultiplier)
				],
				outputs =
				[
				new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.4f*3.2f* RateMultiplier, true, new CellOffset(1, 1), 313.15f)
				]
			};

			Tinkerable.MakePowerTinkerable(go);
			go.AddOrGetDef<PoweredActiveController.Def>();

			ConduitDispenser biodieselDispenser = go.AddOrGet<ConduitDispenser>();
			biodieselDispenser.conduitType = ConduitType.Liquid;
			biodieselDispenser.alwaysDispense = true;
			biodieselDispenser.elementFilter = [SimHashes.DirtyWater];

		}
	}
}
