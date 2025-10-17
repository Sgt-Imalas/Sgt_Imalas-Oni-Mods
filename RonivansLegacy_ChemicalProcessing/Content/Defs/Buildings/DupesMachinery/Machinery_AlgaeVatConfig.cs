using HarmonyLib;
using KSerialization;
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


namespace Dupes_Machinery.Biological_Vats
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Machinery_AlgaeVatConfig : IBuildingConfig
	{
		public static string ID = "AlgaeVat";

		private static readonly PortDisplayOutput outputPort = new PortDisplayOutput(ConduitType.Gas, new CellOffset(0, 3));

		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = [200f, 300f];
			string[] textArray1 = ["RefinedMetal", "Farmable"];

			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 4, "bio_algae_vat_kanim", 30, 90f, singleArray1, textArray1, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.Floodable = true;
			def1.ViewMode = OverlayModes.Oxygen.ID;
			def1.AudioCategory = "HollowMetal";
			def1.EnergyConsumptionWhenActive = 60f;
			def1.SelfHeatKilowattsWhenActive = 2f;
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.InputConduitType = ConduitType.Liquid;
			def1.OutputConduitType = ConduitType.Liquid;
			def1.UtilityInputOffset = new CellOffset(-1, 0);
			def1.UtilityOutputOffset = new CellOffset(1, 0);
			def1.PowerInputOffset = new CellOffset(0, 0);
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			SoundEventVolumeCache.instance.AddVolume("bio_algae_vat_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("bio_algae_vat_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
			SoundEventVolumeCache.instance.AddVolume("bio_algae_vat_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
			SoundUtils.CopySoundsToAnim("bio_algae_vat_kanim", "algaefarm_kanim");
			return def1;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);

			ManualDeliveryKG algaeDelivery = go.AddComponent<ManualDeliveryKG>();
			algaeDelivery.SetStorage(storage);
			algaeDelivery.RequestedItemTag = SimHashes.Algae.CreateTag();
			algaeDelivery.capacity = 500f;
			algaeDelivery.refillMass = 100f;
			algaeDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer waterConduitConsumer = go.AddComponent<ConduitConsumer>();
			waterConduitConsumer.capacityTag = SimHashes.Water.CreateTag();
			waterConduitConsumer.capacityKG = 500f;
			waterConduitConsumer.forceAlwaysSatisfied = true;
			waterConduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			float multiplier = 2.5f;

			///reducing consumption and production of water by 1kg to make it "more efficient", apart from that use old rates * 2.5
			ElementConverter mainConverter = go.AddComponent<ElementConverter>();
			mainConverter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Algae.CreateTag(), 0.060f * multiplier),
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 0.800f * multiplier - 1)
				];
			mainConverter.outputElements = [new ElementConverter.OutputElement(0.160f * multiplier, SimHashes.Oxygen, 303.15f, false, true),
				new ElementConverter.OutputElement(0.77422f * multiplier - 1, SimHashes.DirtyWater, 303.15f, false, true)];

			ElementConverter secondaryConverter = go.AddComponent<ElementConverter>();
			secondaryConverter.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.CarbonDioxide.CreateTag(), 0.00859f * multiplier)];
			secondaryConverter.outputElements = [new ElementConverter.OutputElement(0.02578f * multiplier, SimHashes.DirtyWater, 303.15f, false, true)];

			ConduitDispenser pWaterDispenser = go.AddOrGet<ConduitDispenser>();
			pWaterDispenser.conduitType = ConduitType.Liquid;
			pWaterDispenser.elementFilter = [SimHashes.DirtyWater];

			ElementConsumer co2ElementConsumer = go.AddOrGet<ElementConsumer>();
			co2ElementConsumer.elementToConsume = SimHashes.CarbonDioxide;
			co2ElementConsumer.consumptionRate = 0.009f * multiplier;
			co2ElementConsumer.consumptionRadius = 6;
			co2ElementConsumer.storeOnConsume = true;
			co2ElementConsumer.capacityKG = 10f;
			co2ElementConsumer.showInStatusPanel = true;
			co2ElementConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
			co2ElementConsumer.isRequired = false;


			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;

			PipedConduitDispenser oxygenDispenser = go.AddComponent<PipedConduitDispenser>();
			oxygenDispenser.elementFilter = [SimHashes.Oxygen];
			oxygenDispenser.AssignPort(outputPort);
			oxygenDispenser.alwaysDispense = true;
			oxygenDispenser.SkipSetOperational = true;

			PipedOptionalExhaust exhaust = go.AddComponent<PipedOptionalExhaust>();
			exhaust.dispenser = oxygenDispenser;
			exhaust.elementTag = SimHashes.Oxygen.CreateTag();
			exhaust.capacity = 0.40f;
			this.AttachPort(go);

			go.AddOrGet<ElementConversionBuilding>().UsePrimaryConverterOnly = true; //Handles element converter

			Prioritizable.AddRef(go);
		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, outputPort);
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
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			AttachPort(go);
		}
	}
}
