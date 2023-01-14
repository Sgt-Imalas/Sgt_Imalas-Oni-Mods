using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.CargoBays
{
    public class CritterContainmentModuleConfigOLD : IBuildingConfig
    {
        public const string ID = "RTB_CritterContainmentModule";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] MatCosts = {
                600f,
                300f
            };
            string[] Materials =
            {
                "RefinedMetal",
                "Plastic"
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER1;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "rocket_storage_live_kanim", 1000, 30f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.DefaultAnimState = "grounded";
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket,  null)
            };
            go = ExtendBuildingToDeliverableStorage(go, Config.Instance.CritterStorageCapacity);

        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            ///NEEDS REWORK

            Prioritizable.AddRef(go);


            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MODERATE);

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
        }

        public static GameObject ExtendBuildingToDeliverableStorage(GameObject template, float capacity)
        {


            Storage storage = template.AddComponent<Storage>();
            storage.capacityKg = capacity;
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            storage.showCapacityStatusItem = true;
            storage.storageFilters = GetCritterTags();
            TreeFilterable treeFilterable = template.AddOrGet<TreeFilterable>();
            treeFilterable.dropIncorrectOnFilterChange = true;
            treeFilterable.autoSelectStoredOnLoad = false;

            template.AddOrGet<StorageLocker>();
            return template;
        }
        public static List<Tag> GetCritterTags()
        {
            List<Tag> tagList = new List<Tag>();
            tagList.AddRange(STORAGEFILTERS.BAGABLE_CREATURES);
            tagList.AddRange(STORAGEFILTERS.SWIMMING_CREATURES);
            return tagList;
        }

    }
}
