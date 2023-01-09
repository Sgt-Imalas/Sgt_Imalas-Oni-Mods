using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace ShockWormMob
{
    internal class GroneHogMoundConfig : IEntityConfig
    {

        public const string ID = "GroneHogMound";
        public const string BASE_TRAIT_ID = "GroneHogMoundBaseTrait";
        private const int WIDTH = 3;
        private const int HEIGHT = 3;
        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public GameObject CreatePrefab()
        {
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID, (string)"Volgus Cave", (string)"nutty putty cave", 100f, Assets.GetAnim((HashedString)"gronehogmound_kanim"), "idle", Grid.SceneLayer.Creatures, WIDTH, HEIGHT, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, defaultTemperature: TUNING.CREATURES.TEMPERATURE.FREEZING_3);
            KPrefabID kprefabId = placedEntity.AddOrGet<KPrefabID>();
            kprefabId.AddTag(GameTags.Experimental);
            kprefabId.AddTag(GameTags.Creature);
            placedEntity.AddOrGet<GroneHogMound>();
            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst) => inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
        {
            ObjectLayer.Building
        };

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
