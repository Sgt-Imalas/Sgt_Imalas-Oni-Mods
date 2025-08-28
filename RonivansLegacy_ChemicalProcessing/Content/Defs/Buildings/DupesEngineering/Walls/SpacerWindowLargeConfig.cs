using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Walls
{
	internal class SpacerWindowLargeConfig : IBuildingConfig
	{
		public static string ID = "SpacerWindowLarge";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = [45,15];
			string[] buildingMaterials = [GameTags.Transparent.ToString(), GameTags.Steel.ToString()];
			EffectorValues noise = NOISE_POLLUTION.NONE;
			EffectorValues decor = new EffectorValues()
			{
				amount = 15,
				radius = 1
			};
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "spacer_window_large_kanim", 20, 3f, tieR2, buildingMaterials, 800f, BuildLocationRule.NotInTiles, decor, noise);
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
