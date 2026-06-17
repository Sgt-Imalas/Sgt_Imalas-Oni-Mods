using NaturalConstruction.Content.Scripts;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace NaturalConstruction.Content.Defs
{
	internal class NaturalTileBuildingConfig : IBuildingConfig
	{
		public static string ID = "NC_NaturalTile";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = [100];
			string[] materials = [GameTags.Solid.ToString()];
			EffectorValues none1 = NOISE_POLLUTION.NONE;
			EffectorValues none2 = TUNING.BUILDINGS.DECOR.NONE;
			EffectorValues noise = none1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "nc_natural_tile_kanim", 100, 30f, tieR2, materials, 1600f, BuildLocationRule.Tile, none2, noise);
			BuildingTemplates.CreateFoundationTileDef(buildingDef);
			buildingDef.Floodable = false;
			buildingDef.Entombable = false;
			buildingDef.Overheatable = false;
			buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
			buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
			buildingDef.DragBuild = true;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			Prioritizable.AddRef(go);
		}
		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			UnityEngine.Object.DestroyImmediate(go.GetComponent<Constructable>());
			go.AddComponent<ConstructableNaturalSpawner>();
		}


		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KBatchedAnimController>().initialBlendParameters = 4;
			go.AddComponent<CompleteBuildingNaturalSpawner>();
		}
	}
}
