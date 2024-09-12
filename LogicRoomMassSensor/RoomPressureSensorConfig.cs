using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace LogicRoomMassSensor
{
	internal class RoomPressureSensorConfig : IBuildingConfig
	{
		public static string ID = "LRMS_RoomPressureSensor";
		public static float maxValue = 100000000;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR0_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
			string[] refinedMetals = TUNING.MATERIALS.REFINED_METALS;
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues tieR0_2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER0;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "switchliquidpressure_kanim", 30, 30f, tieR0_1, refinedMetals, 1600f, BuildLocationRule.Anywhere, tieR0_2, noise);
			buildingDef.Overheatable = false;
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.ViewMode = OverlayModes.Logic.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.AlwaysOperational = true;
			buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
	{
	  LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.LRMS_ROOMPRESSURESENSOR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.LRMS_ROOMPRESSURESENSOR.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LRMS_ROOMPRESSURESENSOR.LOGIC_PORT_INACTIVE, true)
	};
			SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
			SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
			GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var logicPressureSensor = go.AddOrGet<RoomPressureSensor>();
			logicPressureSensor.rangeMin = 0.0f;
			// 10.000.000 <- 10k Tons
			logicPressureSensor.rangeMax = maxValue;
			logicPressureSensor.Threshold = 1000f;
			logicPressureSensor.ActivateAboveThreshold = false;
			logicPressureSensor.manuallyControlled = false;
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
		}
	}

}
