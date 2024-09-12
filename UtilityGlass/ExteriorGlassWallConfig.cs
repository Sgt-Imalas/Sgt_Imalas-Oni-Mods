using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace UtilityGlass
{
	internal class ExteriorGlassWallConfig : IBuildingConfig
	{
		public const string ID = "UG_ExteriorGlassWall";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] buildingMaterials = MATERIALS.TRANSPARENTS;
			EffectorValues noise = NOISE_POLLUTION.NONE;
			EffectorValues decor = new EffectorValues()
			{
				amount = 15,
				radius = 0
			};
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "ug_exteriorglasswall_kanim", 20, 3f, tieR2, buildingMaterials, 800f, BuildLocationRule.NotInTiles, decor, noise);
			buildingDef.Entombable = false;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "Glass";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.DefaultAnimState = "off";
			buildingDef.ObjectLayer = ObjectLayer.Backwall;
			buildingDef.SceneLayer = Grid.SceneLayer.Backwall;
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.ReplacementLayer = ObjectLayer.ReplacementBackwall;
			buildingDef.ReplacementCandidateLayers = new List<ObjectLayer>()
			{
				ObjectLayer.FoundationTile,
				ObjectLayer.Backwall
			};
			buildingDef.ReplacementTags = new List<Tag>()
			{
				GameTags.FloorTiles,
				GameTags.Backwall
			};
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
			go.AddComponent<ZoneTile>();
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.Backwall);
			GeneratedBuildings.RemoveLoopingSounds(go);
		}
	}

}
