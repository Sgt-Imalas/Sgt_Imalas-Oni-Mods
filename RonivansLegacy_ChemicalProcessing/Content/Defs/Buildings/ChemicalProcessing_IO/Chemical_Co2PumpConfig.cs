using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;


namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//==== [ CHEMICAL: CO2 DISSIPATOR CONFIG ] ====================================================================
	public class Chemical_Co2PumpConfig : IBuildingConfig
	{
		public static string ID = "Chemical_Co2Pump";
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER2;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "co2_pump_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER0, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.EnergyConsumptionWhenActive = 10f;
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.SelfHeatKilowattsWhenActive = 0f;
			def1.Floodable = true;
			def1.ViewMode = OverlayModes.GasConduits.ID;
			def1.AudioCategory = "Metal";
			def1.OutputConduitType = ConduitType.Gas;
			def1.UtilityOutputOffset = new CellOffset(0, 0);
			def1.PowerInputOffset = new CellOffset(0, 0);
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			SoundUtils.CopySoundsToAnim("co2_pump_kanim", "minigaspump_kanim");
			return def1;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<Pump>();
			go.AddOrGet<Storage>().capacityKg = 2f;
			ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
			elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
			elementConsumer.consumptionRate = 0.1f;
			elementConsumer.storeOnConsume = true;
			elementConsumer.showInStatusPanel = false;
			elementConsumer.consumptionRadius = 3;
			elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Gas;
			conduitDispenser.alwaysDispense = true;
			conduitDispenser.elementFilter = null;
			go.AddOrGetDef<OperationalController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}
	}
}
