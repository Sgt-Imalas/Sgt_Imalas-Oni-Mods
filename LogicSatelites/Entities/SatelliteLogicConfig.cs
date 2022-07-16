using LogicSatelites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LogicSatelites.Behaviours.ModAssets;
using static LogicSatelites.STRINGS.ITEMS;

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
                   name: SATELLITE.TITLE,
                   desc: SATELLITE.DESC,
                   mass: MASS,
                   unitMass: true,
                   anim: Assets.GetAnim("space_satellite_kanim"),
                   initialAnim: "object",
                   sceneLayer: Grid.SceneLayer.Creatures,
                   collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                   element: SimHashes.Steel,
                   additionalTags: new List<Tag>()
                   {
                       Tags.LS_Satellite,
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
