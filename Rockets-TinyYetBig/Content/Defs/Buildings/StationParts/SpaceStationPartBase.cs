using Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;
using static ResearchTypes;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.StationParts
{
	internal class SpaceStationPartBase
	{
		public static BuildingDef CreateStationPartDef(string Id, string[] materials , float[] matCosts, float constructionTime = 60f)
		{
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(Id, 3, 4, "railgun_emptier_kanim", 1000, constructionTime, matCosts, materials, 9999f, BuildLocationRule.BuildingAttachPoint, DECOR.NONE, NOISE_POLLUTION.NONE);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.DefaultAnimState = "grounded";
			buildingDef.AttachmentSlotTag = ModAssets.Tags.AttachmentSlotStationParts;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.DefaultAnimState = "on";
			buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
			return buildingDef;
		}

		public static void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

		}

		public static void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<SelfStoringPart>();
		}
	}
}
