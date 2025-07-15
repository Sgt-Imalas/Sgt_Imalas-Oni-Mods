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
	class HPAVentGasConfig : IBuildingConfig
	{
		public static string ID = "HPAVentGas";
		public override BuildingDef CreateBuildingDef()
		{
			float[] construction_mass = [200f, 100f];
			string[] construction_materials = [GameTags.Steel.ToString(), GameTags.Plastic.ToString()];
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "hpa_vent_gas_kanim", 30, 30f, construction_mass, construction_materials, 1600f, BuildLocationRule.Anywhere, tieR1, noise);
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.EnergyConsumptionWhenActive = 120;
			buildingDef.RequiresPowerInput = true;
			buildingDef.ExhaustKilowattsWhenActive = 0f;
			buildingDef.SelfHeatKilowattsWhenActive = 0f;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			SoundEventVolumeCache.instance.AddVolume("hpa_vent_gas_kanim", "GasVent_clunk", NOISE_POLLUTION.NOISY.TIER0);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<PoweredExhaust>();
			go.AddOrGet<LogicOperationalController>();
			Vent vent = go.AddOrGet<Vent>();
			vent.conduitType = ConduitType.Gas;
			vent.endpointType = Endpoint.Sink;
			vent.overpressureMass = HighPressureConduitComponent.GetConduitMultiplier(ConduitType.Gas) * GasVentHighPressureConfig.OVERPRESSURE_MASS;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.ignoreMinMassCheck = true;
			BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
			go.AddOrGet<SimpleVent>();
			var inputs = go.AddOrGet<RequireInputs>();
			inputs.requireConduitHasMass = false;
			inputs.requireConduit = false; //handled by highPressureInput
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<VentController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
			go.AddOrGet<HighPressureInput>();
		}
	}
}
