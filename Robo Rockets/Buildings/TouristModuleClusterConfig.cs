
using TUNING;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoboRockets.Buildings
{
    public class TouristModuleClusterConfig : IBuildingConfig
    {
        public const string ID = "TouristModuleCluster";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] denseTieR1 = BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER1;
            string[] construction_materials = new string[1]
            {
      SimHashes.Steel.ToString()
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("TouristModuleCluster", 5, 5, "rocket_tourist_kanim", 1000, 60f, denseTieR1, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerInput = false;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            buildingDef.ShowInBuildMenu = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            //go.AddOrGet<TouristModule>();
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
            };
            go.AddOrGet<Storage>();
            go.AddOrGet<MinionStorage>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MAJOR);
            Ownable ownable = go.AddOrGet<Ownable>();
            ownable.slotID = Db.Get().AssignableSlots.HabitatModule.Id;
            ownable.canBePublic = false;
            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[5]
            {
      new CellOffset(-2, -1),
      new CellOffset(-1, -1),
      new CellOffset(0, -1),
      new CellOffset(1, -1),
      new CellOffset(2, -1)
            };
            fakeFloorAdder.initiallyActive = false;
            go.AddOrGet<BuildingCellVisualizer>();
        }
    }
}