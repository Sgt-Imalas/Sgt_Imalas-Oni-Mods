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
                   anim: Assets.GetAnim("seed_saltPlant_kanim"),
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


            return looseEntity;
        }


    public void OnPrefabInit(GameObject inst)
    {
    }
    public void OnSpawn(GameObject inst) { }
    }
}
