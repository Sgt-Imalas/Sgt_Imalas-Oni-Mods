using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static CargoBay;

namespace Rockets_TinyYetBig.Buildings.Utility
{
    public class DrillconeStorageModuleConfig : IBuildingConfig
    {
        public const string ID = "RTB_DrillConeDiamondStorage";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] cargoMass = new float[] { 500f };
            string[] construction_materials = new string[1]
            {
                "RefinedMetal"
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "rocket_drillcone_cargo_bay_kanim", 1000, 60f, cargoMass, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
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
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
            };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = (float)Config.Instance.DrillconeSupportDiamondMass;
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.showCapacityStatusItem = true;
            storage.storageFilters = new List<Tag>() { SimHashes.Diamond.CreateTag() };
            storage.allowSettingOnlyFetchMarkedItems = false;
            storage.useWideOffsets = true;
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.RequestedItemTag = SimHashes.Diamond.CreateTag();
            manualDeliveryKg.MinimumMass = storage.capacityKg;
            manualDeliveryKg.capacity = storage.capacityKg;
            manualDeliveryKg.refillMass = storage.capacityKg;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            go.AddOrGet<DrillConeAssistentModule>(); 
            go.AddOrGet<DrillConeModeHandler>(); 

            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
        }
    }

}
