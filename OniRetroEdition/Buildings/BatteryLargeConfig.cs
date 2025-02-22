using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace OniRetroEdition.Buildings
{
	internal class BatteryLargeConfig:BaseBatteryConfig
	{
		public static string ID = "RetroOni_BatteryLarge";
		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "batterylg_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ALL_METALS, 3200f, BuildLocationRule.OnFloor, new EffectorValues()
			{
				amount = -20,
				radius = 4
			}, NOISE_POLLUTION.NOISY.TIER3);
			buildingDef.Floodable = true;
			buildingDef.Overheatable = true;
			buildingDef.AudioCategory = "Metal";
			buildingDef.RequiresPowerOutput = true;
			buildingDef.UseWhitePowerOutputConnectorColour = true;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.SelfHeatKilowattsWhenActive = 1.35f;
			buildingDef.AudioSize = "large";
			SoundEventVolumeCache.instance.AddVolume("batterylg_kanim", "Battery_rattle", NOISE_POLLUTION.NOISY.TIER3);
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
			buildingDef.DefaultAnimState = "off";
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Battery battery = go.AddOrGet<Battery>();
			battery.capacity = 60000f;
			battery.joulesLostPerSecond = battery.capacity * (0.05f / 600.0f);
			battery.powerSortOrder = 1600;
			base.DoPostConfigureComplete(go);
		}
	}
}
