using LogicSatelites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LogicSatelites.STRINGS.ITEMS;

namespace LogicSatelites.Entities
{
    public class SatelliteGridConfig : IEntityConfig
    {
        public const string ID = "LS_SatelliteGrid";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateBasicEntity(
                   id: ID,
                   name: SATELLITE.NAME,
                   desc: SATELLITE.DESC,
                   mass: 600,
                   unitMass: true,
                   anim: Assets.GetAnim("space_satellite_kanim"),
                   initialAnim: "object",
                   sceneLayer: Grid.SceneLayer.Creatures,
                   element: SimHashes.Steel,
                   additionalTags: new List<Tag>()
                   {
                      GameTags.IgnoreMaterialCategory,
                      GameTags.Experimental
                   });

            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(looseEntity);
            defaultStorage.showInUI = false;
            defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            defaultStorage.capacityKg = 5000f;

            var entity = looseEntity.AddOrGet<SatelliteGridEntity>();
            entity.clusterAnimName = "space_satellite_kanim";
            entity.isWorldEntity = true;
            entity.enabled = true;
            entity.nameKey = new StringKey("STRINGS.ITEMS.SATELLITE.TITLE");
            return looseEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}