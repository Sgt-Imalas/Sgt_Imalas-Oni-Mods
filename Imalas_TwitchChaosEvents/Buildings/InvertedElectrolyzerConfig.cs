using Imalas_TwitchChaosEvents.Elements;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Buildings
{
    class InvertedElectrolyzerConfig : IBuildingConfig
	{
		public const string ID = "ITCE_ElectrolyzerInverted";
		public const float WATER2OXYGEN_RATIO = 0.888f;
		public const float OXYGEN_TEMPERATURE = 343.15f;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR3_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
			string[] allMetals = TUNING.MATERIALS.ALL_METALS;
			EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
			EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
			EffectorValues noise = tieR3_2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "electrolyzer_invertedwater_kanim", 30, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnCeiling, tieR1, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.PowerInputOffset = new CellOffset(1, 1);
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 0.25f;
			buildingDef.SelfHeatKilowattsWhenActive = 1f;
			buildingDef.ViewMode = OverlayModes.Oxygen.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 1);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.OXYGEN);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			CellOffset cellOffset = new CellOffset(0, 1);
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
			electrolyzer.maxMass = 1.8f;
			electrolyzer.hasMeter = true;
			electrolyzer.emissionOffset = cellOffset;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 1f;
			conduitConsumer.capacityTag = ModElements.InverseWater.Tag;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = 2f;
			storage.showInUI = true;
			storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Insulate
			});
			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
			{
				new ElementConverter.ConsumedElement(new Tag( ModElements.InverseWater.Tag), 1f)
			};
			elementConverter.outputElements = new ElementConverter.OutputElement[]
			{
				new ElementConverter.OutputElement(0.888f, SimHashes.Oxygen, 343.15f, outputElementOffsetx: ((float) cellOffset.x), outputElementOffsety: ((float) cellOffset.y)),
				new ElementConverter.OutputElement(0.112f, SimHashes.Hydrogen, 343.15f, outputElementOffsetx: ((float) cellOffset.x), outputElementOffsety: ((float) cellOffset.y)),
				new (0.001f, SimHashes.Unobtanium, 343.15f, outputElementOffsetx: ((float) cellOffset.x), outputElementOffsety: ((float) cellOffset.y))
			};
			var flip = go.AddOrGet<InvertedBuilding>();			
			flip.yOffset = 2;
			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
			var flip = go.AddOrGet<InvertedBuilding>();
			flip.yOffset = 2;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}
	}

}
