using PeterHan.PLib.Options;
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
	public class PressureGasPumpConfig : IBuildingConfig
	{
		public static string ID = "PressureGasPump";

		public override BuildingDef CreateBuildingDef()
		{
			float[] quantity1 = [400f, 200f];
			string[] materials1 = [GameTags.Steel.ToString(), GameTags.Plastic.ToString()];
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER2;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "pressure_gas_pump_kanim", 30, 30f, quantity1, materials1, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
			def1.RequiresPowerInput = true;
			def1.Overheatable = false;
			def1.EnergyConsumptionWhenActive = Config.Instance.HPA_Pump_Base_Mult_Gas * HighPressureConduit.GetConduitMultiplier(ConduitType.Gas);
			def1.ExhaustKilowattsWhenActive = 0f;
			def1.SelfHeatKilowattsWhenActive = 0f;
			def1.OutputConduitType = ConduitType.Gas;
			def1.Floodable = true;
			def1.ViewMode = OverlayModes.GasConduits.ID;
			def1.AudioCategory = "Metal";
			def1.PowerInputOffset = new CellOffset(0, 1);
			def1.UtilityOutputOffset = new CellOffset(0, 2);
			def1.PermittedRotations = PermittedRotations.R90;
			def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<Pump>();
			go.AddOrGet<Storage>().capacityKg = Config.Instance.HPA_Capacity_Gas;
			ElementConsumer local1 = go.AddOrGet<ElementConsumer>();
			local1.configuration = ElementConsumer.Configuration.AllGas;
			local1.consumptionRate = Config.Instance.HPA_Capacity_Gas;
			local1.storeOnConsume = true;
			local1.showInStatusPanel = false;
			local1.consumptionRadius = 12;
			ConduitDispenser local2 = go.AddOrGet<ConduitDispenser>();
			local2.conduitType = ConduitType.Gas;
			local2.alwaysDispense = true;
			local2.elementFilter = null;
			go.AddOrGetDef<OperationalController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			go.AddOrGet<HPA_ConduitRequirement>().RequiresHighPressureOutput = true;
			UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());//handled by HighPressureOutput
		}
	}
}
