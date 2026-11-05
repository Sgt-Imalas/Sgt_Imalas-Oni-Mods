using ShapeProjector.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;

namespace ShapeProjector.Content.Defs.Buildings
{
	internal class ShapeProjectorConfig : IBuildingConfig
	{
		public static string ID = "SP_PlanShapeProjector";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] materials = TUNING.MATERIALS.ALL_METALS;
			EffectorValues decor = new() { amount = -5, radius = 0 };
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "sp_shape_projector_kanim", 30, 5f, tieR2, materials, 1600f, BuildLocationRule.Anywhere, decor, NOISE_POLLUTION.NONE);
			buildingDef.Entombable = false;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;
			buildingDef.DefaultAnimState = "off";
			buildingDef.ObjectLayer = ObjectLayer.Gantry;
			buildingDef.SceneLayer = Grid.SceneLayer.Backwall;
			//buildingDef.AddLogicPowerPort = true;

			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			GeneratedBuildings.RemoveLoopingSounds(go);
			go.AddOrGet<ShapeProjectorLogic>();
		}
	}
}
