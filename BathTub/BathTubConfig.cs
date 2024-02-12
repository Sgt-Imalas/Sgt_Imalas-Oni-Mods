using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace BathTub
{
    internal class BathTubConfig : IBuildingConfig
    {
        public static string ID = "SgtImalas_BathTub";
        public static float BathingTime = 60f;
        public static float WaterConsumedPerBath = 7 * 16;


        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[2] { 500f, 200f };
            string[] construction_materials = new string[2]
            {
                "PreciousRock",
                "Metal"
            };
            EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
            EffectorValues tieR1 = BUILDINGS.DECOR.BONUS.TIER3;
            EffectorValues noise = tieR3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 2, "hottub_kanim", 30, 10f, construction_mass, construction_materials, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;


            buildingDef.Overheatable = false;
            buildingDef.EnergyConsumptionWhenActive = 240f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;
            buildingDef.ExhaustKilowattsWhenActive = 2f;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(2, 0);
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(-2, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.WashStation);
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.AdvancedWashStation);


            //Shower shower = go.AddComponent<Shower>();
            //shower.overrideAnims = new KAnimFile[1]
            //{
            //    Assets.GetAnim((HashedString) "anim_interacts_shower_kanim")
            //};
            //shower.workTime = 15f;
            //shower.outputTargetElement = SimHashes.DirtyWater;
            //shower.fractionalDiseaseRemoval = 0.99f;
            //shower.absoluteDiseaseRemoval = -2000;
            //shower.workLayer = Grid.SceneLayer.BuildingFront;
            //shower.trackUses = true;


            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
            conduitConsumer.capacityKG = 100f;
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.invertElementFilter = true;
            conduitDispenser.elementFilter = new SimHashes[1]
            {
                SimHashes.Water
            };
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
            {
                new ElementConverter.ConsumedElement(new Tag("Water"), WaterConsumedPerBath/BathingTime)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[1]
            {
                new ElementConverter.OutputElement(WaterConsumedPerBath/BathingTime, SimHashes.DirtyWater, 0.0f, storeOutput: true)
            };
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 200f;
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;

            RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
            roomTracker.requiredRoomType = Db.Get().RoomTypes.PlumbedBathroom.Id;
            roomTracker.requirement = RoomTracker.Requirement.Recommended;

            var bathtub = go.AddOrGet<BathTub>();
            bathtub.waterStorage   = storage;


            go.AddOrGetDef<RocketUsageRestriction.Def>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }

}
