using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryopod.Entities
{
    class FrozenDupeConfig : IEntityConfig
    {
        public const string ID = "CRY_FrozenDupe";
        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY; 
        public GameObject CreatePrefab()
        {
            string name = (string)STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CRY_FROZENDUPE.NAME;
            string desc = (string)STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CRY_FROZENDUPE.DESC;
            KAnimFile anim = Assets.GetAnim((HashedString)"fruitcake_kanim");
            GameObject placedEntity = EntityTemplates.CreateLooseEntity(ID, name, desc, 30f,true, anim,"object", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 2.0f, true, element: SimHashes.Creature);
            placedEntity.AddComponent<frozenDupe>();
            return placedEntity;
        }

        public void OnPrefabInit(GameObject go)
        {
            var storage =go.AddOrGet<MinionStorage>();
            var dup = go.AddOrGet<frozenDupe>();
            dup.DupeStorage = storage;
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
