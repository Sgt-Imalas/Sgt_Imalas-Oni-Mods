using BubbleChest.Content.Scripts;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace BubbleChest.Content.Defs.Buildings
{
	internal class BubbleChestConfig : IBuildingConfig
	{
		public static readonly string ID = "BC_BubbleChest";

		public static readonly string EFFECT_ID = "BC_InteractedWithBubbleChest";
		const float Capacity = 5f;
		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR1_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
			string[] allMetals = MATERIALS.ALL_METALS;
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR1_2 = BUILDINGS.DECOR.BONUS.TIER2;
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
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.ignoreMinMassCheck = true;
			conduitConsumer.keepZeroMassObject = false;
			conduitConsumer.capacityKG = Capacity;
			go.AddComponent<ChestBubbler>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);

			new EffectBuilder(BubbleChestConfig.EFFECT_ID, 300, false)
				.Name(STRINGS.CREATURES.MODIFIERS.BC_INTERACTEDWITHBUBBLECHEST.NAME)
				.Description(STRINGS.CREATURES.MODIFIERS.BC_INTERACTEDWITHBUBBLECHEST.TOOLTIP)
				.Modifier(Db.Get().CritterAttributes.Happiness.Id, 1)
				.Add(Db.Get(), out _);
		}
	}
}
