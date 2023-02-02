using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static Storage;

namespace Rockets_TinyYetBig.Buildings.CargoBays
{
    class FridgeModuleConfig : IBuildingConfig
    {
        public const string ID = "RTB_FridgeCargoBay";
        public float CAPACITY = 300f;

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] hollowTieR1 = BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER2;
            string[] refinedMetals = {
                MATERIALS.REFINED_METAL,
                MATERIALS.PLASTIC
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "rocket_fridge_module_small_kanim", 1000, 30f, hollowTieR1, refinedMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.Invincible = true;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerInput = false;
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
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, (AttachableBuilding) null)
            };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go = BuildingTemplates.ExtendBuildingToClusterCargoBay(go, this.CAPACITY, STORAGEFILTERS.FOOD, CargoBay.CargoType.Solids);
            go.TryGetComponent<Storage>(out var freezerStorage);
            freezerStorage.SetDefaultStoredItemModifiers(new List<StoredItemModifier>
            {
                StoredItemModifier.Hide,
                StoredItemModifier.Seal,
                StoredItemModifier.Preserve,
                StoredItemModifier.Insulate
            });
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MINOR_PLUS);
        }
    }
}
