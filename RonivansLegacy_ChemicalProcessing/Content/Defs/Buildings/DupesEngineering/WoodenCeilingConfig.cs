using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering
{
    class WoodenCeilingConfig : IBuildingConfig
	{
		public static string ID = "WoodenCeiling";
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration, false);
			go.AddOrGet<AnimTileable>();
		}

		public override BuildingDef CreateBuildingDef()
		{
			float[] cost =[ 50f, 50f ];
			string[] material = [MATERIALS.RAW_MINERALS_OR_WOOD.First(), GameTags.BuildingWood.ToString()];

			EffectorValues decor = new()
			{
				amount = 5,
				radius = 3
			};
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "wooden_ceiling_kanim", 10, 30f, cost, material, 800f, BuildLocationRule.OnCeiling, decor, NOISE_POLLUTION.NONE, 0.2f);
			def.DefaultAnimState = "S_U";
			def.Floodable = false;
			def.Overheatable = false;
			def.ViewMode = OverlayModes.Decor.ID;
			def.AudioCategory = "Metal";
			def.AudioSize = "small";
			return def;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}
