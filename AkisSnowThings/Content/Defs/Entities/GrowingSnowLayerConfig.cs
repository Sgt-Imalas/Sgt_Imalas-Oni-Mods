using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AkisSnowThings.Content.Defs.Entities
{
    internal class GrowingSnowLayerConfig : IEntityConfig
    {
        public const string ID = "SnowSculptures_GrowingSnowLayer";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            string name = STRINGS.ENTITIES.PREFABS.SNOWSCULPTURES_GROWINGSNOWLAYER.NAME;
            string desc = STRINGS.ENTITIES.PREFABS.SNOWSCULPTURES_GROWINGSNOWLAYER.DESC;
            EffectorValues decor = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
            EffectorValues noisePollution = NOISE_POLLUTION.NONE;
            KAnimFile anim = Assets.GetAnim((HashedString)"geyser_side_chlorine_kanim");
            EffectorValues noise = noisePollution;
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, 1, anim, "inactive", Grid.SceneLayer.Ore, 4, 2, decor, noise,SimHashes.Snow);
            
           
            placedEntity.AddOrGet<LoopingSounds>();
            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
