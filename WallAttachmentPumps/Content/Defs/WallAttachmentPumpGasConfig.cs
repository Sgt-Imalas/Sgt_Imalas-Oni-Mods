using STRINGS;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;
using WallAttachmentPumps.Content.Scripts;

namespace WallAttachmentPumps.Content.Defs
{
	internal class WallAttachmentPumpGasConfig : IBuildingConfig
	{
		public static readonly string ID = "WAP_GasPump";

		public override BuildingDef CreateBuildingDef()
		{
			float[] mass = [50, 1];
			string[] mats = [MATERIALS.REFINED_METAL, MATERIALS.GASKET];
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "wap_gas_kanim", 60, 60f, mass, mats, 1600f, BuildLocationRule.OnFoundationRotatable, tieR1, noise);
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 0;
			buildingDef.SelfHeatKilowattsWhenActive = 0;
			buildingDef.OutputConduitType = ConduitType.Gas;
			buildingDef.Floodable = true;
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<EnergyConsumer>();

			var pumpOffset = new CellOffset(0, -2);
			go.AddOrGet<RotatablePump>().PumpOffset = pumpOffset;
			go.AddOrGet<Storage>().capacityKg = 0.5f;
			go.AddTag(GameTags.CorrosionProof);

			ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
			elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
			elementConsumer.consumptionRate = 0.25f;
			elementConsumer.storeOnConsume = true;
			elementConsumer.showInStatusPanel = false;
			elementConsumer.consumptionRadius = (byte)2;

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Gas;
			conduitDispenser.alwaysDispense = true;
			conduitDispenser.elementFilter = (SimHashes[])null;

			go.AddOrGetDef<OperationalController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
		}
	}
}
