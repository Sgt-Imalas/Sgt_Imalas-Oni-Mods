using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction.ModuleBuildings
{
	public class Part_A_1_Config : IBuildingConfig
	{
		public const string ID = "RTB_Part_A_1";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] ResourceCosts = ModuleParts_Central.StationParts["PartCostA"].SplitMaterialCosts();
			string[] BuildingMats = ModuleParts_Central.StationParts["PartCostA"].SplitMaterials();
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "RTB_Part_A_kanim", 1000, 120f, ResourceCosts, BuildingMats, 9999f, BuildLocationRule.BuildingAttachPoint, none, noise);
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.AttachmentSlotTag = ModuleParts_Central.RTB_Part_Base;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.RequiresPowerInput = false;
			buildingDef.DefaultAnimState = "on";
			buildingDef.CanMove = false;
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), ModuleParts_Central.RTB_Part_A_1, (AttachableBuilding) null)
			};
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var part = go.AddOrGet<StationPartBuilding>();
			part.PositionIndex = 1;
			part.SetStationPart("PartCostA");
		}
	}
}
