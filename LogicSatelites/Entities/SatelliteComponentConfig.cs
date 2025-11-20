using System.Collections.Generic;
using UnityEngine;
using static LogicSatellites.STRINGS.ITEMS;

namespace LogicSatellites.Entities
{
	class SatelliteComponentConfig : IEntityConfig, IHasDlcRestrictions
	{
		public string[] GetAnyRequiredDlcIds()
		{
			return null;
		}

		public static string ID = "LS_ClusterSatellitePart";
		public const float MASS = 30f;
		public static ComplexRecipe recipe;
		public static Tag ComponentTag = TagManager.Create(ID);
		public string[] GetDlcIds() => null;
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

		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public string[] GetForbiddenDlcIds() => null;
	}
}
