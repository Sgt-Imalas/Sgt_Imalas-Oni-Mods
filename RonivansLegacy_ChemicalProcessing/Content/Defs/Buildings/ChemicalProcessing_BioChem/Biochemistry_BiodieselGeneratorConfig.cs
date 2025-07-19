using HarmonyLib;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
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
	public class Biochemistry_BiodieselGeneratorConfig : IBuildingConfig
	{
		public static string ID = "Biochemistry_BiodieselGenerator";

		private static readonly List<Storage.StoredItemModifier> BioGeneratorStoredItemModifiers;

		static Biochemistry_BiodieselGeneratorConfig()
		{


			List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
			list1.Add(Storage.StoredItemModifier.Hide);
			list1.Add(Storage.StoredItemModifier.Preserve);
			list1.Add(Storage.StoredItemModifier.Insulate);
			list1.Add(Storage.StoredItemModifier.Seal);
			BioGeneratorStoredItemModifiers = list1;
		}


		public override BuildingDef CreateBuildingDef()
		{
			GeneratorList.AddGeneratorToIgnore(ID);
			GeneratorList.AddCombustionGenerator(ID);
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 4, "biodiesel_generator_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier);
			buildingDef.GeneratorWattageRating = 3200f;
			buildingDef.GeneratorBaseCapacity = 3200f;
			buildingDef.ExhaustKilowattsWhenActive = 2f;
			buildingDef.SelfHeatKilowattsWhenActive = 2f;
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

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			Storage storage = go.AddOrGet<Storage>();
			storage.SetDefaultStoredItemModifiers(BioGeneratorStoredItemModifiers);
			BuildingDef def = go.GetComponent<Building>().Def;
			go.AddOrGet<LoopingSounds>();

			ManualDeliveryKG ykg = go.AddOrGet<ManualDeliveryKG>();
			ykg.SetStorage(storage);
			ykg.RequestedItemTag = ModAssets.Tags.Biodiesel_Composition;
			ykg.capacity = 0f;
			ykg.refillMass = 0f;
			ykg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

			// 

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = def.InputConduitType;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityTag = ModAssets.Tags.Biodiesel_Composition;
			conduitConsumer.capacityKG = 32f;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
			energyGenerator.powerDistributionOrder = 8;
			energyGenerator.ignoreBatteryRefillPercent = true;
			energyGenerator.hasMeter = true;
			energyGenerator.formula = new EnergyGenerator.Formula
			{
				inputs =
				[
				new EnergyGenerator.InputItem( ModAssets.Tags.Biodiesel_Composition, 0.38f, 32f)
				],
				outputs =
				[
				new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.14428f, true, new CellOffset(1, 1), 313.15f)
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
