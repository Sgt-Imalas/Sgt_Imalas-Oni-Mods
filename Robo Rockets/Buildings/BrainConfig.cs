using RoboRockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Robo_Rockets
{
    class BrainConfig : IEntityConfig
    {
        public const string ID = "RR_BrainFlyer";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            GameObject prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.NAME,
                desc: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("brain_item_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.CIRCLE,
                width: 1.7f,
                height: 1.9f,
                isPickupable: true,
                sortOrder: 0,
                element: SimHashes.Creature,
                additionalTags: new List<Tag>()
                {
                    GameTags.IndustrialIngredient,
                    ModAssets.Tags.SpaceBrain
                });

            prefab.AddComponent<FlyingBrain>();

            return prefab;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public void OnPrefabInit(GameObject inst)
        {
            //inst.AddOrGet<CanRecycler>();
        }

        public void OnSpawn(GameObject inst)
        {
            
        }
    }
}
