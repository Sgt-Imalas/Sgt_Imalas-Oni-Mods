using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.BuildingConfigInterfaces;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
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
    class CustomSolidGeneratorConfig : IBuildingConfig, IHasConfigurableWattage, IGeneratorBuilding
	{
		public static float Wattage = 750;
		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;

		public static string ID = "CustomSolidGenerator";

		private static readonly PortDisplayOutput co2Port = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3));

		static CustomSolidGeneratorConfig()
		{
			//hide coal gen slider
			//GeneratorList.AddGeneratorToIgnore(ID);
			GeneratorList.AddCombustionGenerator(ID);
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [200];
			string[] construction_materials = [GameTags.Metal.ToString()];

			EffectorValues decor = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef =
				BuildingTemplates.CreateBuildingDef(ID, 2, 4,
				"custom_solid_generator_kanim",
				(int)(100 ),
				(int)120,
				construction_mass, construction_materials,
				2400f, BuildLocationRule.OnFloor, decor, noise);

			buildingDef.GeneratorWattageRating = GetWattage();
			buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
			buildingDef.ExhaustKilowattsWhenActive = 8f;
			buildingDef.SelfHeatKilowattsWhenActive = 1f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.RequiresPowerOutput = true;
			buildingDef.PowerOutputOffset = new CellOffset(0, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.POWER);
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.GENERATOR);

			SoundUtils.CopySoundsToAnim("custom_solid_generator_kanim", "generatorpetrol_kanim");
			return buildingDef;

		}
		public override void DoPostConfigureComplete(GameObject go)
		{

			var kprefab = go.GetComponent<KPrefabID>();
			kprefab.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			kprefab.AddTag(RoomConstraints.ConstraintTags.PowerBuilding);
			kprefab.AddTag(RoomConstraints.ConstraintTags.GeneratorType);
			kprefab.AddTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType);

			Storage fuelStorage = go.AddOrGet<Storage>();
			go.AddOrGet<LoopingSounds>();


			var fuel = SimHashes.RefinedCarbon.CreateTag();
			float maxFuelCapacity = 600;

			ManualDeliveryKG ykg = go.AddOrGet<ManualDeliveryKG>();
			ykg.SetStorage(fuelStorage);
			ykg.RequestedItemTag = fuel;
			ykg.capacity = maxFuelCapacity;
			ykg.refillMass = 100f;
			ykg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			EnergyGenerator generator = go.AddOrGet<EnergyGenerator>();
			generator.powerDistributionOrder = 9;
			//generator.ignoreBatteryRefillPercent = true;
			generator.hasMeter = true;

			generator.formula = new EnergyGenerator.Formula()
			{
				inputs = [new(fuel, 0.75f, maxFuelCapacity)],
				outputs = [new(SimHashes.CarbonDioxide, 0.01f, true, new CellOffset(0, 2), 383.15f)]
			};

			PipedConduitDispenser co2Dispenser = go.AddOrGet<PipedConduitDispenser>();
			co2Dispenser.AssignPort(co2Port);
			co2Dispenser.elementFilter = [SimHashes.CarbonDioxide];
			co2Dispenser.alwaysDispense = true;
			co2Dispenser.SkipSetOperational = true;


			PipedOptionalExhaust co2Exhaust = go.AddComponent<PipedOptionalExhaust>();
			co2Exhaust.dispenser = co2Dispenser;
			co2Exhaust.elementTag = SimHashes.CarbonDioxide.CreateTag();
			co2Exhaust.capacity = 1f;
			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;

			Tinkerable.MakePowerTinkerable(go);

			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			this.AttachPort(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddOrGet<PortDisplayController>();
			controller.Init(go);
			controller.AssignPort(go, co2Port);
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
