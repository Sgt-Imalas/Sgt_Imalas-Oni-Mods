using BathTub.Duck.Floating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BathTub.Duck
{
    internal class RubberDuckieConfig : IEntityConfig
    {
        public const string ID = "BT_RubberDuckie";

        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public GameObject CreatePrefab()
        {
            GameObject looseEntity = EntityTemplates.CreateLooseEntity(
                  id: ID,
                  name: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.BT_RUBBERDUCKIE.NAME,
                  desc: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.BT_RUBBERDUCKIE.DESC,
                  mass: 1f,
                  unitMass: false,
                  anim: Assets.GetAnim("rubber_ducky_kanim"),
                  initialAnim: "object",
                  sceneLayer: Grid.SceneLayer.Ore,
                  collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                  width: 0.7f,
                  height: 0.7f,
                  isPickupable: true,
                  element: SimHashes.Polypropylene,
                  additionalTags: new List<Tag>()
                  {
                      GameTags.IndustrialProduct
                  });

            looseEntity.AddOrGet<OccupyArea>();
            looseEntity.AddOrGet<Floater>();
            DecorProvider decorProvider = looseEntity.AddOrGet<DecorProvider>();
            decorProvider.SetValues(TUNING.DECOR.BONUS.TIER3);
            decorProvider.overrideName = STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.BT_RUBBERDUCKIE.NAME;

            return looseEntity;
        }


        public void OnPrefabInit(GameObject inst)
        {
        }
        public void OnSpawn(GameObject inst) { }
    }
}
