using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS;
using TUNING;
using UnityEngine;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.NuclearProcessing
{
	public class LightReactorConfig : IBuildingConfig
	{

		public static string ID = "LightReactor";
		public override string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 6, "light_reactor_kanim", 100, 480f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, TUNING.MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tieR5);
			
			buildingDef.RequiresPowerInput = false;
			buildingDef.RequiresPowerOutput = false;
			buildingDef.UseStructureTemperature = false;
			buildingDef.ThermalConductivity = 0.1f;
			buildingDef.ExhaustKilowattsWhenActive = 0.0f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.Overheatable = false;
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.UtilityInputOffset = new CellOffset(1, 0);
			List<LogicPorts.Port> portList = [LogicPorts.Port.InputPort(NuclearReactorConfig.INPUT_PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.NUCLEARREACTOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.NUCLEARREACTOR.INPUT_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.NUCLEARREACTOR.INPUT_PORT_INACTIVE, display_custom_name: true)];
			buildingDef.LogicInputPorts = portList;
			buildingDef.ViewMode = OverlayModes.Temperature.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.Breakable = false;
			buildingDef.Invincible = true;
			buildingDef.Deprecated = !Sim.IsRadiationEnabled();
			SoundUtils.CopySoundsToAnim("light_reactor_kanim", "generatornuclear_kanim");

			ColliderOffsetHandler.GenerateBuildingDefOffsets(buildingDef, -3, 0);

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			UnityEngine.Object.Destroy(go.GetComponent<BuildingEnabledButton>());
			RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			radiationEmitter.emitRadiusX = 4;
			radiationEmitter.emitRadiusY = 4;
			radiationEmitter.emitRads = 0.0f;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emitRate = 0.0f;
			radiationEmitter.emissionOffset = new Vector3(0.35f, -2f, 0.0f);

			var modifers = Storage.StandardInsulatedStorage;

			Storage supplyStorage = go.AddComponent<Storage>();			
			Storage reactionStorage = go.AddComponent<Storage>();			
			Storage wasteStorage = go.AddComponent<Storage>();
			supplyStorage.SetDefaultStoredItemModifiers(modifers);
			reactionStorage.SetDefaultStoredItemModifiers(modifers);
			wasteStorage.SetDefaultStoredItemModifiers(modifers);

			ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
			manualDeliveryKg.RequestedItemTag = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).tag;
			manualDeliveryKg.SetStorage(supplyStorage);
			manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;
			manualDeliveryKg.capacity = 72f;
			manualDeliveryKg.MinimumMass = 0.5f;
			go.AddOrGet<LightReactor>();
			go.AddOrGet<LoopingSounds>();
			Prioritizable.AddRef(go);
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 10f;
			conduitConsumer.capacityKG = 36f;
			conduitConsumer.capacityTag = GameTags.AnyWater;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			conduitConsumer.storage = supplyStorage;
		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddTag(GameTags.CorrosionProof);
			MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
			def.occupyFoundationLayer = false;
			def.solidOffsets = new CellOffset[2];
			for (int x = 0; x < 2; ++x)
				def.solidOffsets[x] = new CellOffset(x, 0);
			go.AddOrGet<ColliderOffsetHandler>().ColliderOffsetY = -3;
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.AddOrGet<ColliderOffsetHandler>().ColliderOffsetY = -3;
		}
	}
}
