using LogicSatelites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatelites.Entities
{
    class SatelliteLogicConfig : IEntityConfig
    {
        public const string ID = "LS_ClusterSateliteLogic";
        public const float MASS = 600f;

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public GameObject CreatePrefab()
        {
            GameObject looseEntity = EntityTemplates.CreateLooseEntity(
                   id: ID,
                   name: STRINGS.ITEMS.SATELLITE.TITLE,
                   desc: STRINGS.ITEMS.SATELLITE.DESC,
                   mass: MASS,
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
                      GameTags.IndustrialIngredient
                   }); 

            //looseEntity.AddOrGet<LogicBroadcaster>(); needs custom made comp.

            //looseEntity.AddOrGet<LogicBroadcastReceiver>();
            return looseEntity;
        }


    public void OnPrefabInit(GameObject inst)
    {
    }
    public void OnSpawn(GameObject inst) { }
    }
}
