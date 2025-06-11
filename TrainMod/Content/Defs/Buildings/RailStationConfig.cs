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
	class RailStationConfig : IBuildingConfig
	{
		public static string ID = "Rail_Station";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] rawMetals = MATERIALS.RAW_METALS;
			EffectorValues none1 = NOISE_POLLUTION.NONE;
			EffectorValues none2 = BUILDINGS.DECOR.NONE;
			EffectorValues noise = none1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "rail_piece_kanim", 200, 3f, tieR2, rawMetals, 1600f, BuildLocationRule.Anywhere, none2, noise);
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = 0.0f;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.PermittedRotations = PermittedRotations.R90;
			buildingDef.DragBuild = true;
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
			var track = go.AddOrGet<TrackStation>();
			track.PathCost = 1;
			track.InputCellOffset = new CellOffset(-0, 0);
			track.InputCellOffsetConnectsTo = new CellOffset(-1, 0);
			track.OutputCellOffsets = [new CellOffset(0, 0)];
			track.OutputCellOffsetsConnectsTo = [new CellOffset(1,0)];
		}
	}
}
