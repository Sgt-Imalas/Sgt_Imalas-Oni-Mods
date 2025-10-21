using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HighCapacityLogisticRails
{
    class HPARailInsulatedConfig : IBuildingConfig
	{
		public static string ID = "HPA_SolidRail_Insulated";
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			string[] mats = [ModElements.SteelAndTungstenMaterial, MATERIALS.TRANSPARENT];
			float[] costs = [125, 25];
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "hpa_rail_insulated_kanim", 10, 3f, costs, mats, 1600f, BuildLocationRule.NotInTiles, BUILDINGS.DECOR.NONE, nONE, 0.2f);
			def1.Overheatable = false;
			def1.Floodable = false;
			def1.Entombable = false;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.ObjectLayer = ObjectLayer.SolidConduit;
			def1.TileLayer = ObjectLayer.SolidConduitTile;
			def1.ReplacementLayer = ObjectLayer.ReplacementSolidConduit;
			def1.AudioCategory = "Metal";
			def1.AudioSize = "small";
			def1.BaseTimeUntilRepair = 0f;
			def1.UtilityInputOffset = new CellOffset(0, 0);
			def1.UtilityOutputOffset = new CellOffset(0, 0);
			def1.SceneLayer = Grid.SceneLayer.SolidConduits;
			def1.ForegroundLayer = Grid.SceneLayer.SolidConduitBridges;
			def1.isKAnimTile = true;
			def1.isUtility = true;
			def1.DragBuild = true;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("hpa_rail_insulated_kanim", "utilities_conveyor_kanim");
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
			KAnimGraphTileVisualizer local1 = go.AddComponent<KAnimGraphTileVisualizer>();
			local1.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Solid;
			local1.isPhysicalBuilding = true;
			LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<SolidConduit>();
			go.AddOrGet<HighPressureConduit>().InsulateSolidContents = true;

			var cap = go.AddOrGet<ConduitCapacityDescriptor>();
			cap.Conduit = ConduitType.Solid;
			cap.CachedConduitCapacity = HighPressureConduitRegistration.SolidCap_HP;
			//go.AddOrGet<Insulator>();
			go.AddOrGet<HPA_InsulationCapHandler>();
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			KAnimGraphTileVisualizer local1 = go.AddComponent<KAnimGraphTileVisualizer>();
			local1.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Solid;
			local1.isPhysicalBuilding = false;
			go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
		}
	}
}
