using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications
{
	public class HighPressureLiquidConduitBridgeConfig : IBuildingConfig
	{
		public static string ID = "HighPressureLiquidConduitBridge";
		private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

		public override BuildingDef CreateBuildingDef()
		{
			int width = 3;
			int height = 1;
			string anim = "pressure_liquid_bridge_kanim";
			int hitpoints = 10;
			float construction_time = 45f;
			float[] tIER = [10f, 5f];
			string[] constructionMaterial = [SimHashes.Steel.ToString(), MATERIALS.PLASTIC];
			float melting_point = 1600f;
			BuildLocationRule build_location_rule = BuildLocationRule.Conduit;
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, constructionMaterial, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, nONE);
			buildingDef.ObjectLayer = ObjectLayer.LiquidConduitConnection;
			buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduitBridges;
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			buildingDef.ThermalConductivity = 1.0e-05f;
			SoundUtils.CopySoundsToAnim(anim, "utilitygasbridge_kanim");
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			ConduitBridge conduitBridge = go.AddOrGet<ConduitBridge>();
			conduitBridge.type = CONDUIT_TYPE;
			go.AddOrGet<HighPressureConduit>();
			var cap = go.AddOrGet<ConduitCapacityDescriptor>();
			cap.Conduit = CONDUIT_TYPE;
			cap.CachedConduitCapacity = HighPressureConduitRegistration.CachedHPAConduitCapacity(CONDUIT_TYPE);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
			UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
			UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		}
	}
}
