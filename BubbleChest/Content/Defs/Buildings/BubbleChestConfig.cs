using BubbleChest.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace BubbleChest.Content.Defs.Buildings
{
	internal class BubbleChestConfig : IBuildingConfig
	{
		public static readonly string ID = "BC_BubbleChest";
		const float Capacity = 5f;
		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR1_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
			string[] allMetals = MATERIALS.ALL_METALS;
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR1_2 = BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "bc_bubblechest_kanim", 30, 30f, tieR1_1, allMetals, 1600f, BuildLocationRule.OnFloor, tieR1_2, noise);
			buildingDef.InputConduitType = ConduitType.Gas;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<LoopingSounds>();
			go.AddOrGet<LogicOperationalController>();
			var storage = go.AddComponent<Storage>();
			storage.capacityKg = Capacity;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Gas;
			conduitConsumer.ignoreMinMassCheck = true;
			conduitConsumer.keepZeroMassObject = false;
			conduitConsumer.capacityKG = Capacity;
			go.AddComponent<ChestBubbler>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
		}
	}
}
