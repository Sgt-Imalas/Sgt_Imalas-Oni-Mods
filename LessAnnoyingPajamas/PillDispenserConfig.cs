using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace ClothingLockerMod
{
    internal class PillDispenserConfig : IBuildingConfig
    {
        public const string ID = "DE_PillDispenser";

        public override BuildingDef CreateBuildingDef()
        {
            string[] refinedMetals = MATERIALS.RAW_METALS;
            float[] construction_mass = new float[1]
            {
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
            };
            string[] construction_materials = refinedMetals;
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR1 = BUILDINGS.DECOR.BONUS.TIER1;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "handsanitizer_kanim", 30, 30f, construction_mass, construction_materials, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.PreventIdleTraversalPastBuilding = true;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, ID);
            return buildingDef;
        }


        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.capacityKg = 20;
            storage.showCapacityStatusItem = true;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.storageFilters = new List<Tag>() 
            { 
                GameTags.Medicine 
            };

            go.AddOrGet<StorageLocker>();
            Prioritizable.AddRef(go);
            go.AddOrGet<DirectionControl>();

            go.AddOrGet<TreeFilterable>();
            var pd = go.AddOrGet<PillDispenser>();
            PillDispenser.PopPillWorkable work = go.AddOrGet<PillDispenser.PopPillWorkable>();
            work.overrideAnims = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "anim_interacts_washbasin_kanim")
            };

            SymbolOverrideControllerUtil.AddToPrefab(go);
        }
    }

}

