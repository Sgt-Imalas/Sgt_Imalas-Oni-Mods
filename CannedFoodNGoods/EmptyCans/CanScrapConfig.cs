using System.Collections.Generic;
using UnityEngine;

namespace CannedFoods.EmptyCans
{
	class CanScrapConfig : IEntityConfig
	{
		public const string ID = "CF_CanScrap";

		public string[] GetDlcIds() => null;

		public GameObject CreatePrefab()
		{
			GameObject looseEntity = EntityTemplates.CreateLooseEntity(
				  id: ID,
				  name: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_CANSCRAP.NAME,
				  desc: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CF_CANSCRAP.DESC,
				  mass: 1f,
				  unitMass: false,
				  anim: Assets.GetAnim("can_scrap_kanim"),
				  initialAnim: "object",
				  sceneLayer: Grid.SceneLayer.Ore,
				  collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				  width: 0.64f,
				  height: 0.7f,
				  isPickupable: true,
				  element: Config.Instance.GetCanElement(),
				  additionalTags: new List<Tag>()
				  {
					  ModAssets.Tags.CanTag,
					  GameTags.IndustrialProduct
				  });

			looseEntity.AddOrGet<EntitySplitter>();

			looseEntity.AddOrGet<OccupyArea>();

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
