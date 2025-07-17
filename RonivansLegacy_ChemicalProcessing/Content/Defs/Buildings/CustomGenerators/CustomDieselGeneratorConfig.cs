using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomGenerators
{
	class CustomDieselGeneratorConfig : IBuildingConfig, IHasConfigurableWattage
	{
		public const float SizeMultiplier = 1f / 5f; // 1/5 of the width

		public static float Wattage = 2000f * SizeMultiplier;
		public float GetWattage() => Wattage;
		public void SetWattage(float mass) => Wattage = mass;

		public static string ID = "CustomPetroleumGenerator";

		private static readonly PortDisplayOutput pWaterPort = new PortDisplayOutput(ConduitType.Liquid, new CellOffset(0, 1));

		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [200];
			string[] construction_materials = [GameTags.Metal.ToString()];

			EffectorValues decor = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = 
				BuildingTemplates.CreateBuildingDef(ID, 3, 4,
				"custom_petro_generator_kanim", 
				(int)(100*SizeMultiplier), 
				(int)(480f*SizeMultiplier),
				construction_mass, construction_materials,
				2400f, BuildLocationRule.OnFloor, decor, noise);

			buildingDef.GeneratorWattageRating = GetWattage();
			buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
			buildingDef.ExhaustKilowattsWhenActive = 4f * SizeMultiplier;
			buildingDef.SelfHeatKilowattsWhenActive = 16f * SizeMultiplier;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.RequiresPowerOutput = true;
			buildingDef.PowerOutputOffset = new CellOffset(0, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.POWER);
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.GENERATOR);
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			this.AttachPort(go); 
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();

			var kprefab = go.GetComponent<KPrefabID>();
			kprefab.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

			go.AddOrGet<LoopingSounds>();
			Storage storage = go.AddOrGet<Storage>();
			float num = 20f;
			go.AddOrGet<LoopingSounds>();
			ConduitConsumer consumer = go.AddOrGet<ConduitConsumer>();
			consumer.conduitType = go.GetComponent<Building>().Def.InputConduitType;
			consumer.consumptionRate = 10f;
			consumer.capacityTag = GameTags.CombustibleLiquid;
			consumer.capacityKG = num;
			consumer.forceAlwaysSatisfied = true;
			consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ManualDeliveryKG ykg = go.AddOrGet<ManualDeliveryKG>();
			ykg.SetStorage(storage);
			ykg.RequestedItemTag = GameTags.CombustibleLiquid;
			ykg.capacity = 0f;
			ykg.refillMass = 0f;
			ykg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			EnergyGenerator generator = go.AddOrGet<EnergyGenerator>();
			generator.powerDistributionOrder = 8;
			generator.ignoreBatteryRefillPercent = true;
			generator.hasMeter = true;
			EnergyGenerator.Formula formula = new EnergyGenerator.Formula();
			formula.inputs = new EnergyGenerator.InputItem[] { new EnergyGenerator.InputItem(GameTags.CombustibleLiquid, 0.667f, num) };
			formula.outputs = new EnergyGenerator.OutputItem[] { new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.17f, true, new CellOffset(0, 0), 383.15f), new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.25f, true, new CellOffset(0, 0), 313.15f) };
			generator.formula = formula;
			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Gas;
			dispenser.invertElementFilter = false;
			dispenser.elementFilter = new SimHashes[] { SimHashes.CarbonDioxide };

			//===> Multiple Outputs <=================================
			PipedDispenser liquidDispenser = go.AddComponent<PipedDispenser>();
			liquidDispenser.elementFilter = new SimHashes[] { SimHashes.DirtyWater };
			liquidDispenser.AssignPort(pWaterPort);
			liquidDispenser.alwaysDispense = true;

			PipedOptionalExhaust exhaust = go.AddComponent<PipedOptionalExhaust>();
			exhaust.dispenser = liquidDispenser;
			exhaust.elementHash = SimHashes.DirtyWater;
			exhaust.elementTag = SimHashes.DirtyWater.CreateTag();
			exhaust.capacity = 10f;

			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			Tinkerable.MakePowerTinkerable(go);


			go.AddOrGetDef<PoweredActiveController.Def>();
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);
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
