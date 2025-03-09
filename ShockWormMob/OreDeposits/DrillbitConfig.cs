using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EdiblesManager;
using UnityEngine;

namespace ShockWormMob.OreDeposits
{
    public class DrillbitConfig : IEntityConfig
    {
        public const string ID = "CraftableDrillBit";
        public const string NAME = "Mining Drillbit";
        public const string DESC = "This drillbit is used in the mining of ore veins.\nA mining drill will slowly consume drillbits when mining";
        public const string DESC_RECIPE = "Use drillbits to mine resources from ore veins.";
        public GameObject CreatePrefab()
        {
            GameObject looseEntity = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: NAME,
                desc: DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("kit_electrician_kanim"), //insert custom anim
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.8f,
                height: 0.4f,
                isPickupable: true,
                additionalTags: new List<Tag>
                {
                    GameTags.IndustrialProduct,
                    Miner.DrillbitMaterial
                });
            looseEntity.AddOrGet<EntitySplitter>();

            return looseEntity;
        }

        public string[] GetDlcIds() => null;

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
