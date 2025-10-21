using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications
{
	public class HighPressureLiquidConduitConfig : IBuildingConfig
	{
		public static string ID = "HighPressureLiquidConduit";
		private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

		public override BuildingDef CreateBuildingDef()
		{
			int width = 1;
			int height = 1;
			string anim = "pressure_liquid_pipe_kanim";
			int hitpoints = 10;
			float construction_time = 30f;
			float[] tIER = [10f, 5f];
			string[] constructionMaterial = [ModElements.SteelAndTungstenMaterial, MATERIALS.PLASTIC];
			float melting_point = 1600f;
			BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, constructionMaterial, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, nONE);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.ThermalConductivity = 1.3f;
			buildingDef.ObjectLayer = ObjectLayer.LiquidConduit;
			buildingDef.TileLayer = ObjectLayer.LiquidConduitTile;
			buildingDef.ReplacementLayer = ObjectLayer.ReplacementLiquidConduit;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduits;
			buildingDef.isKAnimTile = true;
			buildingDef.isUtility = true;
			buildingDef.DragBuild = true;
			buildingDef.ReplacementTags = new List<Tag>();
			buildingDef.ReplacementTags.Add(GameTags.Pipes);
			buildingDef.ThermalConductivity = 1.0e-05f;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			Conduit conduit = go.AddOrGet<Conduit>();
			conduit.type = CONDUIT_TYPE;
			var cap = go.AddOrGet<ConduitCapacityDescriptor>();
			cap.Conduit = CONDUIT_TYPE;
			cap.CachedConduitCapacity = HighPressureConduitRegistration.CachedHPAConduitCapacity(CONDUIT_TYPE);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
			go.AddComponent<EmptyConduitWorkable>();
			KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
			kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
			kAnimGraphTileVisualizer.isPhysicalBuilding = true;
			go.GetComponent<KPrefabID>().AddTag(GameTags.Pipes);
			go.AddOrGet<HighPressureConduit>();
			LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
			kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
			kAnimGraphTileVisualizer.isPhysicalBuilding = false;
		}
	}
}
