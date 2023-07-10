using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EdiblesManager;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Meteors
{
    internal class GhostlyTacoConfig : IEntityConfig
    {
        public static string ID = "ICT_GhostlyTaco";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            GameObject prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: STRINGS.ITEMS.ICT_GHOSTTACO.NAME,
                desc: STRINGS.ITEMS.ICT_GHOSTTACO.DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("taco_food_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 1.1f,
                height: 0.6f,
                isPickupable: true,
                sortOrder: 0,
                element: SimHashes.Creature
                );

            prefab.AddOrGet<GhostFade>();
            return prefab;
        }

        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}

