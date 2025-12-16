using HarmonyLib;
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
	class SpacerWallConfig : IBuildingConfig
	{
		public static string ID = "SpacerWall";
		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "spacer_wall_small_kanim", 30, 3f, [10], [GameTags.Steel.ToString()], 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
			{
				amount = 15,
				radius = 0
			});
			def.Entombable = false;
			def.Floodable = false;
			def.Overheatable = false;
			def.AudioCategory = "Metal";
			def.AudioSize = "small";
			def.BaseTimeUntilRepair = -1f;
			def.DefaultAnimState = "off";
			def.ObjectLayer = ObjectLayer.Backwall;
			def.SceneLayer = Grid.SceneLayer.Backwall;
			def.PermittedRotations = PermittedRotations.R360;
			def.ReplacementLayer = ObjectLayer.ReplacementBackwall;
			def.ReplacementCandidateLayers = [ObjectLayer.FoundationTile,ObjectLayer.Backwall];
			def.ReplacementTags = [GameTags.FloorTiles,GameTags.Backwall];
			def.AddSearchTerms(SEARCH_TERMS.TILE);
			ModAssets.MakeWallHidePipesIfEnabled(def);
			return def;
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
