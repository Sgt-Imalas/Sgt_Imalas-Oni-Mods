using LogicSatelites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatelites.Entities
{
    class SatelliteComponentConfig : IEntityConfig
    {
        public const string ID = "LS_ClusterSatelitePart";
        public const string NAME = "Satellite Parts";
        public const string DESC = "A bunch of duct taped electronics and parts found in the back of the storage bin.\nMaybe these will come in handy for a satellite?";
        public const float MASS = 30f;

        public static ComplexRecipe recipe;

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
                   isPickupable: true,
                   element: SimHashes.Steel,
                   additionalTags: new List<Tag>()
                   {
                      GameTags.IndustrialIngredient
                   });

            looseEntity.AddOrGet<EntitySplitter>();
           // looseEntity.AddOrGet<SatelliteGridStates>();
            return looseEntity;
        }


        public void OnPrefabInit(GameObject inst)
        {
        }
        public void OnSpawn(GameObject inst) { }
    }
}
