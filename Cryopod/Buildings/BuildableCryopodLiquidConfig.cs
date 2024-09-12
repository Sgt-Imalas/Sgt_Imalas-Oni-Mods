using System.Collections.Generic;
using UnityEngine;

namespace Cryopod.Buildings
{
	class BuildableCryopodLiquidConfig : IBuildingConfig
	{
		public const string ID = "CRY_BuildableCryoTankLiquid";
		public const float MetalCost = 800f;

		public override BuildingDef CreateBuildingDef()
		{
			float[] mass = {
				MetalCost,
				200f,
				200f,
			};
			string[] material = {
				"RefinedMetal"
				,"Glass"
				,"Plastic"
			};
			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER1;
			EffectorValues decor = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 3, "cryo_chamber_buildable_liquid_kanim", 100, 30f, mass, material, 1600f, BuildLocationRule.OnFloor, decor, noise);

			buildingDef.RequiresPowerInput = true;
			buildingDef.AddLogicPowerPort = false;
			buildingDef.OverheatTemperature = 348.15f;
			buildingDef.EnergyConsumptionWhenActive = 320f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.ExhaustKilowattsWhenActive = 0.0f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.Floodable = false;
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>(){
				LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), (string) global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, (string) global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, (string) global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE)
			};

			buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.InputPort(CryopodReusable.PORT_ID, new CellOffset(0, 0),
				(string) STRINGS.BUILDINGS.PREFABS.CRY_BUILDABLECRYOTANK.INPUT_LOGIC_PORT,
				(string) STRINGS.BUILDINGS.PREFABS.CRY_BUILDABLECRYOTANK.INPUT_LOGIC_PORT_ACTIVE,
				(string) STRINGS.BUILDINGS.PREFABS.CRY_BUILDABLECRYOTANK.INPUT_LOGIC_PORT_INACTIVE)
			};
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(2, 2);
			buildingDef.UtilityOutputOffset = new CellOffset(2, 0);

			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
			//go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<RequireInputs>().SetRequirements(true, false);
			UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireOutputs>());
			UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
			UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());

			var ownable = go.AddOrGet<Ownable>();
			ownable.tintWhenUnassigned = false;
			ownable.slotID = Db.Get().AssignableSlots.WarpPortal.Id;
			go.AddOrGet<EnergyConsumer>();
			go.AddOrGet<MinionStorage>();
			var cryopod = go.AddOrGet<CryopodReusable>();
			cryopod.dropOffset = new CellOffset(1, 0);
			cryopod.InternalTemperatureKelvin = CryopodReusable.InternalTemperatureKelvinUpperLimit;
			cryopod.buildingeMode = CryopodReusable.BuildingeMode.Piped;
			cryopod.powerSaverEnergyUsage = 80f;
			go.AddOrGet<CryopodLiquidPortAddon>();
			go.AddOrGet<CryopodFreezeWorkable>();
			go.AddOrGet<OpenCryopodWorkable>();
			go.AddOrGet<Prioritizable>();
		}

	}
}
