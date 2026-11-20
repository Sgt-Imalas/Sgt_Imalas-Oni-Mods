using OniRetroEdition.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace OniRetroEdition.Buildings.Splitters
{
	internal class BaseSplitterConfig
	{
		public static BuildingDef CreateBuildingDef(string id, string anim, ConduitType type, bool mirrored)
		{
			var buildingDef = BuildingTemplates.CreateBuildingDef(
				id,
				width: 2,
				height: 2,
				anim,
				hitpoints: BUILDINGS.HITPOINTS.TIER0,
				construction_time: BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER0,
				construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
				construction_materials: MATERIALS.RAW_MINERALS,
				melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER0,
				build_location_rule: BuildLocationRule.Conduit,
				decor: DECOR.NONE,
				noise: NOISE_POLLUTION.NONE);

			bool gasSplitter = type == ConduitType.Gas;

			buildingDef.InputConduitType = type;
			buildingDef.OutputConduitType = type;
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.Overheatable = false;
			buildingDef.ViewMode = gasSplitter ? OverlayModes.GasConduits.ID : OverlayModes.LiquidConduits.ID;
			buildingDef.ObjectLayer = gasSplitter ? ObjectLayer.GasConduitConnection : ObjectLayer.LiquidConduitConnection;
			buildingDef.SceneLayer = gasSplitter ? Grid.SceneLayer.GasConduitBridges : Grid.SceneLayer.LiquidConduitBridges;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.UtilityInputOffset = !mirrored ? new CellOffset(0, 0) : new CellOffset(1,0);
			buildingDef.UtilityOutputOffset = !mirrored ? new CellOffset(1, 0) : new CellOffset(0, 0);
			GeneratedBuildings.RegisterWithOverlay(gasSplitter ? OverlayScreen.GasVentIDs : OverlayScreen.LiquidVentIDs, id);

			return buildingDef;
		}

		public static ConduitPortInfo GetPortInfo(ConduitType type, bool mirrored) => new ConduitPortInfo(type, new CellOffset(mirrored ? 0 : 1, 1));

		public static void AttachPort(GameObject go, ConduitPortInfo thirdPort)
		{
			go.AddComponent<ConduitSecondaryOutput>().portInfo = thirdPort;
		}
		public static void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag, ConduitPortInfo thirdPort, bool flipped)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			var logic = go.AddOrGet<ConduitSplitter>();
			logic.ConduitType = thirdPort.conduitType;
			logic.SecondaryPort = thirdPort;
			logic.Flipped = flipped;
		}
		public static void DoPostConfigureComplete(GameObject go)
		{
			UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
			UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
			UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
		}
	}
}
