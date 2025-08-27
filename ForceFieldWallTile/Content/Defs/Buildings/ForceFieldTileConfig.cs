using ForceFieldWallTile.Content.Scripts;
using ForceFieldWallTile.Content.Scripts.MeshGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ForceFieldWallTile.Content.Defs.Buildings
{
	internal class ForceFieldTileConfig : IBuildingConfig
	{
		public static string ID = "FFT_ForceFieldProjector";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] rawMineralsOrWood = TUNING.MATERIALS.RAW_MINERALS_OR_WOOD;
			EffectorValues none = NOISE_POLLUTION.NONE;
			EffectorValues decor = new EffectorValues()
			{
				amount = 10,
				radius = 0
			};
			EffectorValues noise = none;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "gas_element_sensor_kanim", 30, 3f, tieR2, rawMineralsOrWood, 1600f, BuildLocationRule.NotInTiles, decor, noise);
			buildingDef.Entombable = false;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.DefaultAnimState = "off";
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.TILE);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Building;
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);
			go.AddOrGet<ForceFieldSpriteRenderer>();
			go.AddOrGet<ShieldGrid>();
			go.AddOrGet<ForceFieldTile>();
		}
	}
}
