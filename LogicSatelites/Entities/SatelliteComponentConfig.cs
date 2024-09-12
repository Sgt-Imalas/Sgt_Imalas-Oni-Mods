using System.Collections.Generic;
using UnityEngine;
using static LogicSatellites.STRINGS.ITEMS;

namespace LogicSatellites.Entities
{
	class SatelliteComponentConfig : IEntityConfig
	{
		public const string ID = "LS_ClusterSatellitePart";
		public const float MASS = 30f;
		public static ComplexRecipe recipe;
		public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
		public GameObject CreatePrefab()
		{
			GameObject looseEntity = EntityTemplates.CreateLooseEntity(
				   id: ID,
				   name: LS_CLUSTERSATELLITEPART.TITLE,
				   desc: LS_CLUSTERSATELLITEPART.DESC,
				   mass: MASS,
				   unitMass: true,
				   anim: Assets.GetAnim("satellite_parts_kanim"),
				   initialAnim: "object",
				   sceneLayer: Grid.SceneLayer.Ore,
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
			return looseEntity;
		}


		public void OnPrefabInit(GameObject inst)
		{
		}
		public void OnSpawn(GameObject inst) { }
	}
}
