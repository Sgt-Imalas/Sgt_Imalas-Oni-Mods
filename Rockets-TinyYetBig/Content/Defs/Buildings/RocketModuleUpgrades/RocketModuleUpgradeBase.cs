using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings;
using Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.RocketModuleUpgrades
{
	internal static class RocketModuleUpgradeBase
	{
		public static BuildingDef CreateRocketModuleUpgrade(RocketModuleUpgrade upgradeBase, string anim, float constructionTime = 60f)
		{
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(upgradeBase.ID, 1, 1, anim, 1000, constructionTime, upgradeBase.BuildCosts, upgradeBase.BuildTags.Select(t => t.ToString()).ToArray(), 9999f, BuildLocationRule.BuildingAttachPoint, DECOR.NONE, NOISE_POLLUTION.NONE);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.AttachmentSlotTag = ModAssets.Tags.AttachmentSlotRocketModuleUpgrades;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
			return buildingDef;
		}

		public static void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();

		}

		public static void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<RocketModuleUpgradeBuilding>();
		}
	}
}
