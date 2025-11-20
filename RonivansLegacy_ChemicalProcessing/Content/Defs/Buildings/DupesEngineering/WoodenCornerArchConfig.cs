using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering
{
    class WoodenCornerArchConfig : IBuildingConfig
	{
		public static string ID = "WoodenCornerArch";

		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues decor = new() 
			{
				amount = 5,
				radius = 3
			};
			float[] cost = [150f, 50f];
			string[] material = [MATERIALS.RAW_MINERALS_OR_WOOD.First(), GameTags.BuildingWood.ToString()];

			BuildingDef def1 = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "wooden_corner_arch_kanim", 10, 30f, cost, material, 800f, BuildLocationRule.InCorner, decor, NOISE_POLLUTION.NONE, 0.2f);
			def1.DefaultAnimState = "corner";
			def1.Floodable = false;
			def1.Overheatable = false;
			def1.ViewMode = OverlayModes.Decor.ID;
			def1.AudioCategory = "Metal";
			def1.AudioSize = "small";
			def1.PermittedRotations = PermittedRotations.FlipH;
			return def1;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration, false);
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}
