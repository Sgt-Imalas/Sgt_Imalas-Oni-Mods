using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShockWormMob.OreDeposits
{
    internal class OreDepositsConfig : IMultiEntityConfig
    {
        public static Tag DepositSolidAttachmentTag = TagManager.Create("depositMineralSolidAttachment");
        public static Tag DepositLiquidAttachmentTag = TagManager.Create("depositMineralLiquidAttachment");
        public static Tag DepositGasAttachmentTag = TagManager.Create("depositMineralGasAttachment");
        public const string KatheriumDepositID = "Katherium";

        public List<GameObject> CreatePrefabs()
        {
            List<GameObject> prefabs = new List<GameObject>();
            foreach (var config in GenerateConfigs())
                prefabs.Add(CreateHarvestablePOI(config));
            return prefabs;
        }

        public static GameObject CreateHarvestablePOI(HarvestableDepositConfig config)
        {
            GameObject entity = EntityTemplates.CreatePlacedEntity(
                config.id,
                config.name,
                config.description,
                2000f,
                Assets.GetAnim(config.animName),
                "off",
                Grid.SceneLayer.BuildingBack,
                4,
                2,
                TUNING.BUILDINGS.DECOR.BONUS.TIER1);

            if(entity.TryGetComponent<PrimaryElement>(out var primElement))
            {
                primElement.SetElement(SimHashes.Unobtanium, true); 
                primElement.Temperature = 272.15f; 
            }
            if(entity.TryGetComponent<OccupyArea>(out var occupyArea))
            {
                occupyArea.objectLayers = new ObjectLayer[1] { ObjectLayer.Building};
            }
            entity.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[] { new BuildingAttachPoint.HardPoint(CellOffset.none, config.attachmentTag, null) };

            entity.AddOrGet<SaveLoadRoot>();
            var deposit = entity.AddOrGet<OreDeposit>();
            deposit.MiningRates = config.mineableElements;
            return entity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

        private List<HarvestableDepositConfig> GenerateConfigs()
        {
            List<HarvestableDepositConfig> configs = new List<HarvestableDepositConfig>();
            configs.Add(new HarvestableDepositConfig(KatheriumDepositID,"Katherium Deposit","A vast mineral deposit that spreads through other regions of the asteroid","geyser_side_oil_kanim", 
                new Dictionary<SimHashes, float>()
                {
                    {
                    SimHashes.RefinedCarbon,//replace with katherium simhash
                    1.0f
                    }
                }, DepositSolidAttachmentTag));
            return configs;
        }

        public struct HarvestableDepositConfig
        {
            public string id;
            public string name;
            public string animName;
            public string description;
            public Dictionary<SimHashes, float> mineableElements;
            public Tag attachmentTag;

            public HarvestableDepositConfig(string id, string name, string desc, string anim, Dictionary<SimHashes, float> mineableElements, Tag _attachmentTag)
            {
                this.id = "HarvestableDeposit_" + id;
                this.name = name;
                animName = anim;
                description = desc;
                this.mineableElements = mineableElements;
                attachmentTag = _attachmentTag;
            }
        }
    }
}
