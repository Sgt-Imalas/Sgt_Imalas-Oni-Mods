using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics
{
    class LogisticBridgeConfig : IBuildingConfig
	{
		public static string ID = "LogisticBridge";
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

			var cap = go.AddOrGet<ConduitCapacityDescriptor>();
			cap.Conduit = ConduitType.Solid;
			cap.CachedConduitCapacity = HighPressureConduitRegistration.SolidCap_Logistic;
		}

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "logistic_bridge_kanim", 10, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Conduit, BUILDINGS.DECOR.NONE, nONE, 0.2f);
			def1.ObjectLayer = ObjectLayer.SolidConduitConnection;
			def1.SceneLayer = Grid.SceneLayer.SolidConduitBridges;
			def1.InputConduitType = ConduitType.Solid;
			def1.OutputConduitType = ConduitType.Solid;
			def1.Floodable = false;
			def1.Entombable = false;
			def1.Overheatable = false;
			def1.ViewMode = OverlayModes.SolidConveyor.ID;
			def1.AudioCategory = "Metal";
			def1.AudioSize = "small";
			def1.BaseTimeUntilRepair = -1f;
			def1.PermittedRotations = PermittedRotations.R360;
			def1.UtilityInputOffset = new CellOffset(-1, 0);
			def1.UtilityOutputOffset = new CellOffset(1, 0);
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
			SoundUtils.CopySoundsToAnim("logistic_bridge_kanim", "utilities_conveyorbridge_kanim");
			return def1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<SolidConduitBridge>();
			go.AddOrGet<LogisticConduit>();
			go.AddOrGet<HPA_SolidBridgeRequirement>().IsLogisticRail = true;
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
		}
	}
}
