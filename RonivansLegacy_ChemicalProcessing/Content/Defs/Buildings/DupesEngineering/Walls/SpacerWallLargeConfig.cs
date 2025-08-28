using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Walls
{
	class SpacerWallLargeConfig : IBuildingConfig
	{
		public static string ID = "SpacerWallLarge";
		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "spacer_wall_large_kanim", 120, 3f, [25], [GameTags.Steel.ToString()], 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
			{
				amount = 20,
				radius = 1
			});
			obj.Entombable = false;
			obj.Floodable = false;
			obj.Overheatable = false;
			obj.AudioCategory = "Metal";
			obj.AudioSize = "small";
			obj.BaseTimeUntilRepair = -1f;
			obj.DefaultAnimState = "off";
			obj.ObjectLayer = ObjectLayer.Backwall;
			obj.SceneLayer = Grid.SceneLayer.Backwall;
			obj.PermittedRotations = PermittedRotations.R360;
			obj.ReplacementLayer = ObjectLayer.ReplacementBackwall;
			obj.ReplacementCandidateLayers = [ObjectLayer.FoundationTile,ObjectLayer.Backwall];
			obj.ReplacementTags = [GameTags.FloorTiles,GameTags.Backwall];
			obj.AddSearchTerms(SEARCH_TERMS.TILE);
			return obj;
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
