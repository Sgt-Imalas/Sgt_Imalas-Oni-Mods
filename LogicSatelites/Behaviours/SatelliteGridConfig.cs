using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatelites.Behaviours
{
    public class SatelliteGridConfig : IEntityConfig
    {
        public const string ID = "LS_SatelliteOnGrid";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateLooseEntity(
                   id: ID,
                   name: STRINGS.ITEMS.SATELLITE.TITLE,
                   desc: STRINGS.ITEMS.SATELLITE.DESC,
                   mass: 600,
                   unitMass: true,
                   anim: Assets.GetAnim("space_satellite_kanim"),
                   initialAnim: "object",
                   sceneLayer: Grid.SceneLayer.Front,
                   collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                   width: 1f,
                   height: 1f,
                   isPickupable: false,
                   element: SimHashes.Steel,
                   additionalTags: new List<Tag>()
                   {
                      GameTags.IgnoreMaterialCategory,
                      GameTags.Experimental
                   });

            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(looseEntity);
            defaultStorage.showInUI = false;
            defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            defaultStorage.allowSettingOnlyFetchMarkedItems = false;
            defaultStorage.allowItemRemoval = false;
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