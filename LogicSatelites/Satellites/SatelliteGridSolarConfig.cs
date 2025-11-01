using LogicSatellites.Behaviours;
using LogicSatellites.Satellites;
using System.Collections.Generic;
using UnityEngine;
using static LogicSatellites.Behaviours.ModAssets;
using static LogicSatellites.STRINGS.ITEMS;

namespace LogicSatellites.Entities
{
	public class SatelliteGridSolarConfig : IEntityConfig,IHasDlcRestrictions
	{
		public string[] GetAnyRequiredDlcIds()
		{
			return null;
		}

		public const string ID = "LS_SatelliteGridSolar";

		public string[] GetDlcIds() => null;
		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];
		public string[] GetForbiddenDlcIds() => null;

		public GameObject CreatePrefab()
		{
			var looseEntity = EntityTemplates.CreateLooseEntity(
				   id: ID,
				   name: LS_SATELLITEGRID.TITLE,
				   desc: LS_SATELLITEGRID.DESC,
				   mass: 600,
				   unitMass: true,
				   collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				   isPickupable: false,
				   anim: Assets.GetAnim("space_satellite_kanim"),
				   initialAnim: "idle_loop",
				   sceneLayer: Grid.SceneLayer.Creatures,
				   element: SimHashes.Steel,
				   additionalTags: new List<Tag>()
				   {
					  GameTags.IgnoreMaterialCategory,
					  GameTags.Experimental
				   });
			looseEntity.AddOrGet<CharacterOverlay>().shouldShowName = true;
			ClusterDestinationSelector destinationSelector = looseEntity.AddOrGet<ClusterDestinationSelector>();
			destinationSelector.assignable = false;
			destinationSelector.shouldPointTowardsPath = true;
			destinationSelector.requireAsteroidDestination = false;
			var traveler = looseEntity.AddOrGet<ClusterTraveler>();
			traveler.stopAndNotifyWhenPathChanges = false;
			var entity = looseEntity.AddOrGet<SatelliteGridEntity>();
			entity.satelliteType = (int)SatType.SolarLens;
			entity.clusterAnimName = "space_satellite_kanim";
			entity.enabled = true;

			return looseEntity;
		}

		public void OnPrefabInit(GameObject go)
		{
			var entity = go.AddOrGet<SolarLens>();
		}

		public void OnSpawn(GameObject go)
		{
		}
	}
}