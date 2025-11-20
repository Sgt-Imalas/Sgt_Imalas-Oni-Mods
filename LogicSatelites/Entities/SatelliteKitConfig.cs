using LogicSatellites.Behaviours;
using System.Collections.Generic;
using UnityEngine;
using static LogicSatellites.Behaviours.ModAssets;
using static LogicSatellites.STRINGS.ITEMS;

namespace LogicSatellites.Entities
{
	class SatelliteKitConfig : IEntityConfig,IHasDlcRestrictions
	{
		public string[] GetAnyRequiredDlcIds()
		{
			return null;
		}

		public static string ID = "LS_ClusterSatelliteLogic";
		public const float MASS = 600f;

		public string[] GetDlcIds() => null;
		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];
		public GameObject CreatePrefab()
		{
			GameObject looseEntity = EntityTemplates.CreateLooseEntity(
				   id: ID,
				   name: SATELLITE.TITLE,
				   desc: SATELLITE.DESC,
				   mass: MASS,
				   unitMass: true,
				   anim: Assets.GetAnim("space_satellite_kanim"),
				   initialAnim: "object",
				   sceneLayer: Grid.SceneLayer.Ore,
				   collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				   element: SimHashes.Steel,
				   isPickupable: true,
				   additionalTags: new List<Tag>()
				   {
					   Tags.LS_Satellite,
					  GameTags.IndustrialIngredient
				   });

			looseEntity.AddOrGet<SatelliteTypeHolder>();
			looseEntity.AddOrGet<EntitySplitter>();
			return looseEntity;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}
		public void OnSpawn(GameObject inst) { }

		public string[] GetForbiddenDlcIds() => null;
	}
}
