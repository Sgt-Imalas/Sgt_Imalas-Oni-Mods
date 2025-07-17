using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Dupes_Machinery.Ethanol_Still
{

	public class Machinery_EthanolStillConfig : IBuildingConfig
	{
		public static string ID = "EthanolStill";
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
			go.AddOrGet<WaterPurifier>();
			Prioritizable.AddRef(go);

			SimHashes WasteElement = DlcManager.IsExpansion1Active() 
				? SimHashes.ToxicMud : SimHashes.ToxicSand; //pdirt as base game fallback

			ElementConverter converter = go.AddOrGet<ElementConverter>();
			converter.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Sucrose.CreateTag(), 0.2f),
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 1f), 
				new ElementConverter.ConsumedElement(SimHashes.SlimeMold.CreateTag(), 0.05f)];
			converter.outputElements = [
				new ElementConverter.OutputElement(0.8f, SimHashes.Ethanol, 347.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0), 
				new ElementConverter.OutputElement(0.2f, SimHashes.CarbonDioxide, 315.15f, false, false, 0f, 0.5f, 0.75f, 0xff, 0), 
				new ElementConverter.OutputElement(0.2f, WasteElement, 327.15f, false, true, 0f, 0.5f, 0.75f, 0xff, 0)];

			ElementDropper dropper = go.AddComponent<ElementDropper>();
			dropper.emitMass = 10f;
			dropper.emitTag = WasteElement.CreateTag();
			dropper.emitOffset = new Vector3(0f, 1f, 0f);

			ManualDeliveryKG ykg = go.AddComponent<ManualDeliveryKG>();
			ykg.SetStorage(storage);
			ykg.RequestedItemTag = SimHashes.Sucrose.CreateTag();
			ykg.capacity = 500f;
			ykg.refillMass = 200f;
			ykg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ManualDeliveryKG ykg2 = go.AddComponent<ManualDeliveryKG>();
			ykg2.SetStorage(storage);
			ykg2.RequestedItemTag = SimHashes.SlimeMold.CreateTag();
			ykg2.capacity = 200f;
			ykg2.refillMass = 50f;
			ykg2.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			ConduitConsumer consumer = go.AddOrGet<ConduitConsumer>();
			consumer.conduitType = ConduitType.Liquid;
			consumer.consumptionRate = 10f;
			consumer.capacityKG = 20f;
			consumer.capacityTag = SimHashes.Water.CreateTag();
			consumer.forceAlwaysSatisfied = true;
			consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.elementFilter = [SimHashes.Ethanol];
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER3;
			BuildingDef def = BuildingTemplates.CreateBuildingDef("EthanolStill", 4, 5, "ethanol_still_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, noise, 0.2f);
			def.RequiresPowerInput = true;
			def.EnergyConsumptionWhenActive = 120f;
			def.ExhaustKilowattsWhenActive = 21f;
			def.SelfHeatKilowattsWhenActive = 9f;
			def.Overheatable = true;
			def.OverheatTemperature = 333.15f;
			def.InputConduitType = ConduitType.Liquid;
			def.OutputConduitType = ConduitType.Liquid;
			def.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
			def.ViewMode = OverlayModes.LiquidConduits.ID;
			def.AudioCategory = "HollowMetal";
			def.PowerInputOffset = new CellOffset(-1, 0);
			def.UtilityInputOffset = new CellOffset(0, 0);
			def.UtilityOutputOffset = new CellOffset(2, 0);
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "EthanolStill");
			return def;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}
	}
}
