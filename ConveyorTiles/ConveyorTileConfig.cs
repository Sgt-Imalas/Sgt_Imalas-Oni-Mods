using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace ConveyorTiles
{
	class ConveyorTileConfig : IBuildingConfig
	{
		public const string ID = "CT_ConveyorTile";
		//public const string LOGIC_ID = "CT_ConveyorTile_RibbonInput";
		public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;
		public override BuildingDef CreateBuildingDef()
		{
			float[] materialMass = new float[]
			{
				125f,
			};
			string[] materialType = new string[]
			{
				MATERIALS.REFINED_METAL
			};
			EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
			EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				id: ID,
				width: 1,
				height: 1,
				anim: "conveyorbelt_kanim",
				hitpoints: 15,
				construction_time: 20f,
				construction_mass: materialMass,
				construction_materials: materialType,
				melting_point: 1600f,
				BuildLocationRule.Tile,
				decor: decorValue,
				noise: noiseLevel);

			BuildingTemplates.CreateFoundationTileDef(buildingDef);

			buildingDef.IsFoundation = true;
			buildingDef.Overheatable = false;
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.PermittedRotations = PermittedRotations.FlipH;
			buildingDef.TileLayer = ObjectLayer.FoundationTile;
			buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
			buildingDef.ForegroundLayer = Grid.SceneLayer.TileFront;

            buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.DefaultAnimState = "off";
			buildingDef.BaseTimeUntilRepair = -1f;

			buildingDef.RequiresPowerInput = true;

			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.EnergyConsumptionWhenActive = Config.Instance.TileWattage * 4;
			buildingDef.SelfHeatKilowattsWhenActive = 0.00f;
			buildingDef.AddLogicPowerPort = false;
			if (!Config.Instance.NoLogicInputs)
			{
				buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
				{
					LogicPorts.Port.RibbonInputPort((HashedString) LogicOperationalController.PORT_ID,
					new CellOffset(0, 0),
					STRINGS.BUILDINGS.PREFABS.CT_CONVEYORTILE.LOGIC_PORT_INPUT,
					STRINGS.BUILDINGS.PREFABS.CT_CONVEYORTILE.LOGIC_PORT_INPUT_ACTIVE_RIBBON,
					STRINGS.BUILDINGS.PREFABS.CT_CONVEYORTILE.LOGIC_PORT_INPUT_INACTIVE_RIBBON)
				};
				GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
			}
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<SimCellOccupier>().doReplaceElement = false;
			go.AddOrGet<TileTemperature>();
			go.AddOrGet<AnimTileable>();
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;

		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			if (!Config.Instance.NoLogicInputs)
				go.AddOrGet<LogicOperationalController>();
			go.AddOrGet<ConveyorTileSM>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
			go.AddComponent<SimTemperatureTransfer>();
			go.AddComponent<ZoneTile>();
			// go.AddOrGet<PortAttachment>();
			//mdh.targetCellOffset = new CellOffset(0, 1);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{

		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;

		}
	}
}
