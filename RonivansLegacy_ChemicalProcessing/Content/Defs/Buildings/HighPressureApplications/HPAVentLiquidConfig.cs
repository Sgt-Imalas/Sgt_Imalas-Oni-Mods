using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications
{
    class HPAVentLiquidConfig : IBuildingConfig
	{
		public static string ID = "HPAVentLiquid";
		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = new float[] { 200f, 100f };
			string[] construction_materials = new string[] { GameTags.Steel.ToString(), GameTags.Plastic.ToString() };
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "hpa_vent_liquid_kanim", 30, 30f, construction_mass, construction_materials, 1600f, BuildLocationRule.Anywhere, tieR1, noise);
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.EnergyConsumptionWhenActive = 120;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			SoundEventVolumeCache.instance.AddVolume("hpa_vent_liquid_kanim", "GasVent_clunk", NOISE_POLLUTION.NOISY.TIER0);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<Exhaust>();
			go.AddOrGet<LogicOperationalController>();
			Vent vent = go.AddOrGet<Vent>();
			vent.conduitType = ConduitType.Liquid;
			vent.endpointType = Endpoint.Sink;
			vent.overpressureMass = Config.Instance.HPA_Capacity_Liquid * LiquidVentConfig.OVERPRESSURE_MASS / 10;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.ignoreMinMassCheck = true;
			BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
			go.AddOrGet<SimpleVent>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<VentController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
			go.AddOrGet<HighPressurePipeRequirement>();
		}
	}
}
