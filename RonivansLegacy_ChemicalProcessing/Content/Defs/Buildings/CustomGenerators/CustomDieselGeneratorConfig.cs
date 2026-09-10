using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.BuildingConfigInterfaces;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using RonivansLegacy_ChemicalProcessing.Patches;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomGenerators
{
	class CustomDieselGeneratorConfig : IBuildingConfig, IHasConfigurableWattage, IGeneratorBuilding, IHasConfigurableRateMultiplier
	{
		public const float SizeMultiplier = 1f / 3f; // percentage of the vanilla gen

		public static float Wattage = 2000f * SizeMultiplier;
		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;

		public static string ID = "CustomPetroleumGenerator";

		private static readonly PortDisplayOutput pWaterPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 1));
		private static readonly PortDisplayOutput co2Port = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3));

		public static float RateMultiplier = 1f;
		public static float conduitInputRate = 10 * SizeMultiplier * RateMultiplier;
		public void SetMultiplier(float multiplier)
		{
			RateMultiplier = multiplier;
		}

		public Func<BuildingConfigurationEntry, string> GetCurrentRateDescription()
		{
			return (entry) =>
			{
				string toFormat = STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RATESETTING_GENERATOR_TOOLTIP;

				float currentMultiplier = entry.GetRateMultiplier();

				return toFormat
				.Replace("{PERCENTAGE}", GameUtil.GetFormattedPercent(currentMultiplier * 100f))
				.Replace("{WATTAGE}", GameUtil.GetFormattedWattage(entry.GetWattage() * currentMultiplier))
				.Replace("{FUEL}", global::STRINGS.MISC.TAGS.COMBUSTIBLELIQUID)
				.Replace("{RATE}", GameUtil.GetFormattedMass(2f * SizeMultiplier * currentMultiplier,GameUtil.TimeSlice.PerSecond))
				;
			};
		}
		public string GetRateLabel() => STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RATESETTING_GENERATOR;

		static CustomDieselGeneratorConfig()
		{
			//hide coal gen slider
			GeneratorList.AddGeneratorToIgnore(ID);
			GeneratorList.AddCombustionGenerator(ID);
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [200];
			string[] construction_materials = [GameTags.Metal.ToString()];

			EffectorValues decor = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef =
				BuildingTemplates.CreateBuildingDef(ID, 1, 4,
				"custom_petro_generator_kanim",
				(int)(100 * SizeMultiplier),
				(int)(480f * SizeMultiplier),
				construction_mass, construction_materials,
				2400f, BuildLocationRule.OnFloor, decor, noise);

			buildingDef.GeneratorWattageRating = GetWattage() * RateMultiplier;
			buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
			buildingDef.ExhaustKilowattsWhenActive = 4f * SizeMultiplier * RateMultiplier;
			buildingDef.SelfHeatKilowattsWhenActive = 16f * SizeMultiplier * RateMultiplier;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.RequiresPowerOutput = true;
			buildingDef.PowerOutputOffset = new CellOffset(0, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.POWER);
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.GENERATOR);

			SoundUtils.CopySoundsToAnim("custom_petro_generator_kanim", "generatorpetrol_kanim");
			return buildingDef;

		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();

			var kprefab = go.GetComponent<KPrefabID>();
			kprefab.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			kprefab.AddTag(RoomConstraints.ConstraintTags.PowerBuilding);
			kprefab.AddTag(RoomConstraints.ConstraintTags.GeneratorType);
			kprefab.AddTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType);

			Storage fuelStorage = go.AddOrGet<Storage>();
			fuelStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			go.AddOrGet<LoopingSounds>();
			ConduitConsumer consumer = go.AddOrGet<ConduitConsumer>();
			consumer.conduitType = go.GetComponent<Building>().Def.InputConduitType;
			consumer.consumptionRate = conduitInputRate * RateMultiplier;
			consumer.capacityTag = GameTags.CombustibleLiquid;
			consumer.capacityKG = conduitInputRate * 2 * RateMultiplier;
			consumer.forceAlwaysSatisfied = true;
			consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			//ManualDeliveryKG ykg = go.AddOrGet<ManualDeliveryKG>();
			//ykg.SetStorage(fuelStorage);
			//ykg.RequestedItemTag = GameTags.CombustibleLiquid;
			//ykg.capacity = 0f;
			//ykg.refillMass = 0f;
			//ykg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			EnergyGenerator generator = go.AddOrGet<EnergyGenerator>();
			generator.powerDistributionOrder = 8;
			generator.ignoreBatteryRefillPercent = true;
			generator.hasMeter = true;

			generator.formula = new EnergyGenerator.Formula()
			{
				inputs = [new(GameTags.CombustibleLiquid, 2f * SizeMultiplier * RateMultiplier, conduitInputRate * 2 * RateMultiplier)],
				outputs = [
					new(SimHashes.CarbonDioxide, 0.5f * SizeMultiplier* RateMultiplier, true, new CellOffset(0, 0), 383.15f),
					new (SimHashes.DirtyWater, 0.75f * SizeMultiplier* RateMultiplier, true, new CellOffset(0, 0), 313.15f)
					]
			};

			PipedConduitDispenser co2Dispenser = go.AddOrGet<PipedConduitDispenser>();
			co2Dispenser.AssignPort(co2Port);
			co2Dispenser.elementFilter = [SimHashes.CarbonDioxide];
			co2Dispenser.alwaysDispense = true;
			co2Dispenser.SkipSetOperational = true;


			//===> Multiple Outputs <=================================
			PipedConduitDispenser pWaterDispenser = go.AddComponent<PipedConduitDispenser>();
			pWaterDispenser.elementFilter = [SimHashes.DirtyWater];
			pWaterDispenser.AssignPort(pWaterPort);
			pWaterDispenser.alwaysDispense = true;
			pWaterDispenser.SkipSetOperational = true;

			PipedOptionalExhaust pWaterExhaust = go.AddComponent<PipedOptionalExhaust>();
			pWaterExhaust.dispenser = pWaterDispenser;
			pWaterExhaust.elementTags = [SimHashes.DirtyWater.CreateTag()];
			pWaterExhaust.capacity = 10f;

			PipedOptionalExhaust co2Exhaust = go.AddComponent<PipedOptionalExhaust>();
			co2Exhaust.dispenser = co2Dispenser;
			co2Exhaust.elementTags = [SimHashes.CarbonDioxide.CreateTag()];
			co2Exhaust.capacity = 1f;
			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;

			Tinkerable.MakePowerTinkerable(go);
			go.AddOrGetDef<PoweredActiveController.Def>();
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddOrGet<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, co2Port);
			controller.AssignPort(go, pWaterPort);
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
