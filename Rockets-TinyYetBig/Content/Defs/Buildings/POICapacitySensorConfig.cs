using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	public class POICapacitySensorConfig : IBuildingConfig
	{
		public const string ID = "RTB_PoiCapacitySensor";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;


		public static readonly HashedString PORT_ID_MASS_THRESHOLD = (HashedString)"RTB_" + nameof(PORT_ID_MASS_THRESHOLD);
		public static readonly HashedString PORT_ID_ARTIFACT = (HashedString)"RTB_" + nameof(PORT_ID_ARTIFACT);

		public override BuildingDef CreateBuildingDef()
		{
			float[] materialMass = new float[]
			{
				75f,
				25f
			};
			string[] materialType = new string[]
			{
				"RefinedMetal",
				"Plastic"
			};
			EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
			EffectorValues _decor = BUILDINGS.DECOR.PENALTY.TIER1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 2,
				height: 4,
				anim: "poi_capacity_scanner_kanim",
				hitpoints: 30,
				construction_time: 30f,
				construction_mass: materialMass,
				construction_materials: materialType,
				melting_point: 1600f,
				BuildLocationRule.OnFloor,
				decor: _decor,
				noise: noiseLevel);

			buildingDef.Overheatable = false;
			buildingDef.Floodable = true;
			buildingDef.Entombable = true;
			buildingDef.RequiresPowerInput = true;
			buildingDef.AddLogicPowerPort = false;
			buildingDef.EnergyConsumptionWhenActive = 120f;

			buildingDef.ViewMode = OverlayModes.Logic.ID;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;

			buildingDef.PermittedRotations = PermittedRotations.Unrotatable;

			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
			{
				LogicPorts.Port.OutputPort(PORT_ID_MASS_THRESHOLD, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.LOGIC_PORT_CAPACITY, (string) STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.LOGIC_PORT_CAPACITY_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.LOGIC_PORT_CAPACITY_INACTIVE, true)
				,LogicPorts.Port.OutputPort(PORT_ID_ARTIFACT, new CellOffset(1, 0), (string) STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.LOGIC_PORT_ARTIFACT, (string) STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.LOGIC_PORT_ARTIFACT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.RTB_POICAPACITYSENSOR.LOGIC_PORT_ARTIFACT_INACTIVE, false)
			};

			GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.TryGetComponent<KPrefabID>(out var kPrefabID);
			kPrefabID.AddTag(GameTags.NotRocketInteriorBuilding);
			kPrefabID.AddTag(GameTags.OverlayBehindConduits);
			var selector = go.AddComponent<ClusterDestinationSelector>();
			selector.assignable = true;
			selector.requireAsteroidDestination = false;
			SymbolOverrideControllerUtil.AddToPrefab(go);

			go.AddComponent<POICapacitySensorSM>();
		}
	}
}

