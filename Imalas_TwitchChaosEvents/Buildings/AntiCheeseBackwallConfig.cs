using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Buildings
{
	internal class AntiCheeseBackwallConfig : IBuildingConfig
	{
		public static string ID = "ChaosTwitch_AntiCheeseBackwall";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] rawMineralsOrWood = MATERIALS.RAW_MINERALS_OR_WOOD;
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues decor = new EffectorValues()
			{
				amount = 10,
				radius = 0
			};
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "walls_kanim", 30, 3f, tieR2, rawMineralsOrWood, 9900f, BuildLocationRule.NotInTiles, decor, noise);
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
			buildingDef.Invincible = true;
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
			go.AddOrGet<AntiCheeseBackWall>();
			GeneratedBuildings.RemoveLoopingSounds(go);

		}
	}
}
