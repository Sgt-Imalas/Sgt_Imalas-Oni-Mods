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

			ManualDeliveryKG local1 = go.AddComponent<ManualDeliveryKG>();
			local1.SetStorage(storage);
			local1.RequestedItemTag = SimHashes.Algae.CreateTag();
			local1.capacity = 500f;
			local1.refillMass = 100f;
			local1.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer local2 = go.AddComponent<ConduitConsumer>();
			local2.capacityTag = SimHashes.Water.CreateTag();
			local2.capacityKG = 500f;
			local2.forceAlwaysSatisfied = true;
			local2.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ElementConverter converter = go.AddComponent<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Algae.CreateTag(), 0.075f), 
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 0.8f)
				];
			converter.outputElements = [new ElementConverter.OutputElement(0.20f, SimHashes.Oxygen, 303.15f, false, true, 0f, 1f, 1f, 0xff, 0),
				new ElementConverter.OutputElement(0.9625f, SimHashes.DirtyWater, 303.15f, false, true, 0f, 1f, 1f, 0xff, 0)];

			ElementConverter converter2 = go.AddComponent<ElementConverter>();
			converter2.consumedElements = [new ElementConverter.ConsumedElement(SimHashes.CarbonDioxide.CreateTag(), 0.0085f)];
			converter2.outputElements = [new ElementConverter.OutputElement(0.03f, SimHashes.DirtyWater, 303.15f, false, true, 0f, 1f, 1f, 0xff, 0)];

			ConduitDispenser local5 = go.AddOrGet<ConduitDispenser>();
			local5.conduitType = ConduitType.Liquid;
			local5.elementFilter = [SimHashes.DirtyWater];

			ElementConsumer local6 = go.AddOrGet<ElementConsumer>();
			local6.elementToConsume = SimHashes.CarbonDioxide;
			local6.consumptionRate = 0.009f;
			local6.consumptionRadius = 6;
			local6.storeOnConsume = true;
			local6.capacityKG = 10f;
			local6.showInStatusPanel = true;
			local6.sampleCellOffset = new Vector3(0f, 1f, 0f);
			local6.isRequired = false;


			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			go.AddOrGet<AnimTileable>();

			PipedConduitDispenser dispenser = go.AddComponent<PipedConduitDispenser>();
			dispenser.elementFilter = [SimHashes.Oxygen];
			dispenser.AssignPort(outputPort);
			dispenser.alwaysDispense = true;
			dispenser.SkipSetOperational = true;

			PipedOptionalExhaust exhaust = go.AddComponent<PipedOptionalExhaust>();
			exhaust.dispenser = dispenser;
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
