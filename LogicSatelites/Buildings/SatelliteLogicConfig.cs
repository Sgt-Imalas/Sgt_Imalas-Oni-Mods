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
        public const string NAME = "Logic Satellite";
        public const string DESC = "Deploy this satellite on the star map to create a logic relay";
        public const float MASS = 600f;

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public GameObject CreatePrefab()
        {
            GameObject looseEntity = EntityTemplates.CreateLooseEntity(
                   id: ID,
                   name: NAME,
                   desc: DESC,
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


            var entity = looseEntity.AddOrGet<SatelliteGridEntity>();
            entity.name = "Satellite";
            entity.clusterAnimName = "space_satellite_kanim";

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
