using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction.ModuleBuildings
{
	internal class PartWorkshopConfig : IBuildingConfig
	{
		public const string ID = "RTB_PartWorkshopBase";
		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
			string[] refinedMetals = TUNING.MATERIALS.REFINED_METALS;
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 7, 1, "rtb_station_part_manufacturer_kanim", 1000, 120f, tieR5, refinedMetals, 9999f, BuildLocationRule.OnFloor, none, noise);
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.UseStructureTemperature = false;
			//buildingDef.AttachmentSlotTag = RTB_AttachmentTagBase;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.attachablePosition = new CellOffset(0, 0);
			buildingDef.RequiresPowerInput = false;
			buildingDef.DefaultAnimState = "off";
			buildingDef.CanMove = false;
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 1), ModuleParts_Central.RTB_Part_Base, (AttachableBuilding) null)
			};
			var part = go.AddOrGet<StationPartWorkshopBuilding>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}
