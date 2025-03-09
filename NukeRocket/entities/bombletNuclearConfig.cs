using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExplosiveMaterials.entities
{
    public class BombletNuclearConfig : IEntityConfig
    {
        public const string ID = "nuclearBomblet";
        public const string NAME = "Nuclear Bomblet";
        public const string DESC = "I wonder what this red button does";
        public static ComplexRecipe recipe;
        public string[] GetDlcIds() => null;

        public GameObject CreatePrefab()
        {
            GameObject looseEntity = EntityTemplates.CreateLooseEntity(
                  id: ID,
                  name: NAME,
                  desc: DESC,
                  mass: 10f,
                  unitMass: true,
                  anim: Assets.GetAnim("bomblet_nuclear_kanim"),
                  initialAnim: "object",
                  sceneLayer: Grid.SceneLayer.Front,
                  collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                  width: 0.6f,
                  height: 1.0f,
                  isPickupable: true,
                  element: SimHashes.EnrichedUranium,
                  additionalTags: new List<Tag>()
                  {
                      ModAssets.Tags.SplitOnRail,
                      ModAssets.Tags.NuclearFuel,
                      ModAssets.Tags.BuildableExplosive,
                      GameTags.IndustrialIngredient
                  });
            var bomb = looseEntity.AddOrGet<ExplosiveBomblet>();

            looseEntity.AddOrGet<EntitySplitter>().maxStackSize = 50f;

            //RadiationEmitter radiationEmitter = looseEntity.AddComponent<RadiationEmitter>();
            //radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
            //radiationEmitter.radiusProportionalToRads = true;
            //radiationEmitter.emitRads = 200;
            //radiationEmitter.emissionOffset = new Vector3(0.0f, 0.0f, 0.0f);
            return looseEntity;
        }
        

        public void OnPrefabInit(GameObject inst)
        {
            RadiationEmitter radiationEmitter = inst.AddOrGet<RadiationEmitter>();
            radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
            radiationEmitter.radiusProportionalToRads = false;
            radiationEmitter.emitRadiusX = (short)4;
            radiationEmitter.emitRadiusY = radiationEmitter.emitRadiusX;
            radiationEmitter.emitRads = 200;
            radiationEmitter.emissionOffset = new Vector3(0.0f, 0.0f, 0.0f);
            radiationEmitter.emitRate = 0f;
        }
        public void OnSpawn(GameObject inst) { }
    }
}
