using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Scripts.PathSystem;
using TUNING;
using UnityEngine;

namespace TrainMod.Content.Defs.Buildings
{
    class RailCurveConfig : IBuildingConfig
	{
		public static string ID = "Rail_Curve";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] rawMetals = MATERIALS.RAW_METALS;
			EffectorValues none1 = NOISE_POLLUTION.NONE;
			EffectorValues none2 = BUILDINGS.DECOR.NONE;
			EffectorValues noise = none1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "rail_curve_kanim", 200, 3f, tieR2, rawMetals, 1600f, BuildLocationRule.Anywhere, none2, noise);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = 0.0f;
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.PermittedRotations = PermittedRotations.R360;
			buildingDef.ConstructionOffsetFilter = [new(0, -2)];
			//buildingDef.DragBuild = true;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var track = go.AddOrGet<TrackPiece>();
			track.PathCost = 3;
			track.InputCellOffset = new CellOffset(-1, 0);
			track.InputCellOffsetConnectsTo = new CellOffset(-2, 0);
			track.OutputCellOffsets = [ new(1, 2)];
			track.OutputCellOffsetsConnectsTo = [new(1, 3)];
		}
	}
}
