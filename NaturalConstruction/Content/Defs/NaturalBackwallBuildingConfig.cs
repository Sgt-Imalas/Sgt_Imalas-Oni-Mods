using NaturalConstruction.Content.Scripts;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace NaturalConstruction.Content.Defs
{
	internal class NaturalBackwallBuildingConfig : IBuildingConfig
	{
		public static string ID = "NC_NaturalBackwall";
		public override BuildingDef CreateBuildingDef()
		{
			float[] mass = [100];
			string[] mats = [GameTags.Solid.ToString()];
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues decor = DECOR.NONE;
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "nc_natural_backwall_kanim", 30, 30f, mass, mats, 1600f, BuildLocationRule.NotInTiles, decor, noise);
			buildingDef.Entombable = false;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "Metal";
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
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.TILE);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>();
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
			go.AddComponent<ZoneTile>();
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
			UnityEngine.Object.DestroyImmediate(go.GetComponent<Constructable>());
			go.AddComponent<ConstructableNaturalSpawner>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<KBatchedAnimController>().initialBlendParameters = 0;
			go.AddComponent<CompleteBuildingNaturalSpawner>();
		}
	}
}

