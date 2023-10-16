using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace NeutroniumTrashCan
{
    internal class SolidTrashCanConfig : IBuildingConfig
    {
        public const string ID = "NTC_SolidTrashCan";

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] rawMinerals = MATERIALS.ALL_METALS;
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.
                CreateBuildingDef(
                ID,
                1,
                1,
                "trash_can_kanim",
                30,
                10f,
                tieR4,
                rawMinerals,
                8000f,
                BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = false;
            buildingDef.InputConduitType = ConduitType.Solid;
            buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SoundEventVolumeCache.instance.AddVolume("trash_can_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

            storage.capacityKg = 9999999;
            storage.showInUI = true;
            storage.allowItemRemoval = false;
            storage.showDescriptor = true;
            SolidConduitConsumer conduitConsumer = go.AddOrGet<SolidConduitConsumer>();
            conduitConsumer.alwaysConsume = true;
        }

        public override void DoPostConfigureComplete(GameObject go) => go.AddOrGet<NeutroniumTrashCan>().tint = Color.red;
    }
}
