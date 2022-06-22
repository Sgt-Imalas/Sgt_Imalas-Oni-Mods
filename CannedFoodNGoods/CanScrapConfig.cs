using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CannedFoods
{
    class CanScrapConfig : IEntityConfig
    {
        public const string ID = "CF_CanScrap";
        public const string NAME = "Can Scraps";
        public const string DESC = "You don't want to know where this can has been.\n\nDecreases Decor\n\nCan be recycled at the Rock Crusher or the Refinery.";
        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_EXPANSION1_ONLY;
        }

        public GameObject CreatePrefab()
        {
            GameObject looseEntity = EntityTemplates.CreateLooseEntity(
                  id: ID,
                  name: NAME,
                  desc: DESC, 
                  mass: 1f,
                  unitMass: false,
                  anim: Assets.GetAnim("can_scrap_kanim"),
                  initialAnim: "object",
                  sceneLayer: Grid.SceneLayer.Front,
                  collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                  width: 0.64f,
                  height: 0.7f,
                  isPickupable: true,
                  element: SimHashes.Copper,
                  additionalTags: new List<Tag>()
                  {
                      GameTags.IndustrialIngredient

                  });

            looseEntity.AddOrGet<EntitySplitter>();
            return looseEntity;
        }


        public void OnPrefabInit(GameObject inst)
        {
        }
        public void OnSpawn(GameObject inst) { }
    }
}
