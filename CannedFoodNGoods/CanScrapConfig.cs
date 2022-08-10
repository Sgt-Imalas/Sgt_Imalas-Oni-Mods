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
       
        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public GameObject CreatePrefab()
        {
            var canElement = Config.Instance.GetCanElement();
            GameObject looseEntity = EntityTemplates.CreateLooseEntity(
                  id: ID,
                  name: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_CANSCRAP.NAME,
                  desc: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_CANSCRAP.DESC, 
                  mass: 1f,
                  unitMass: false,
                  anim: Assets.GetAnim("can_scrap_kanim"),
                  initialAnim: "object",
                  sceneLayer: Grid.SceneLayer.Front,
                  collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                  width: 0.64f,
                  height: 0.7f,
                  isPickupable: true,
                  element: canElement,
                  additionalTags: new List<Tag>()
                  {
                      GameTags.IndustrialIngredient
                  });

            looseEntity.AddOrGet<EntitySplitter>();


            looseEntity.AddOrGet<OccupyArea>().OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(0, 0);

            DecorProvider decorProvider = looseEntity.AddOrGet<DecorProvider>();
            decorProvider.SetValues(TUNING.DECOR.PENALTY.TIER5);
            decorProvider.overrideName = STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_CANSCRAP.NAME;

            return looseEntity;
        }


        public void OnPrefabInit(GameObject inst)
        {
        }
        public void OnSpawn(GameObject inst) { }
    }
}
