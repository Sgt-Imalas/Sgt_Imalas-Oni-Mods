using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HighCapacityLogisticRails
{
    class HPARailBridgeTileConfig : IBuildingConfig
	{
		public static string ID = "HPA_SolidRailBridgeTile";

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues nONE = NOISE_POLLUTION.NONE;

			string[] mats = [GameTags.BuildableRaw.ToString(),GameTags.Steel.ToString(), MATERIALS.TRANSPARENT];
			float[] costs = [400, 100, 50];

			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "hpa_rail_tile_bridge_kanim", 100, 30f, costs, mats, 1600f, BuildLocationRule.NotInTiles, BUILDINGS.DECOR.NONE, nONE, 0.2f);
			BuildingTemplates.CreateFoundationTileDef(def1);
			def1.Overheatable = false;
			def1.UseStructureTemperature = false;
			def1.Floodable = false;
			def1.Entombable = false;
			def1.ThermalConductivity = 0.01f;
			def1.PermittedRotations = PermittedRotations.R360;
			def1.ObjectLayer = ObjectLayer.SolidConduitConnection;

			def1.InputConduitType = ConduitType.Solid;
			def1.OutputConduitType = ConduitType.Solid;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.AudioCategory = "Metal";
			def1.AudioSize = "small";
			def1.BaseTimeUntilRepair = -1f;
			def1.PermittedRotations = PermittedRotations.R360;
			def1.UtilityInputOffset = new CellOffset(-1, 0);
			def1.UtilityOutputOffset = new CellOffset(1, 0);
			//def1.SceneLayer = Grid.SceneLayer.SolidConduitBridges;
			def1.SceneLayer = Grid.SceneLayer.Wires;
			def1.ForegroundLayer = Grid.SceneLayer.TileMain;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("logistic_bridge_kanim", "utilities_conveyorbridge_kanim");
			return def1;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

			var cap = go.AddOrGet<ConduitCapacityDescriptor>();
			cap.Conduit = ConduitType.Solid;
			cap.CachedConduitCapacity = HighPressureConduitRegistration.SolidCap_HP;

			go.AddOrGet<Insulator>();
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();

			simCellOccupier.doReplaceElement = true;
			simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT_MODIFIERS.PENALTY_3;
			simCellOccupier.notifyOnMelt = true;
			go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
			go.AddOrGet<TileTemperature>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<SolidConduitBridge>();
			go.AddOrGet<HighPressureConduit>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
		}
	}
}
