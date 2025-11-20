using LogicSatellites.Behaviours;
using rail;
using System.Collections.Generic;
using UnityEngine;
using static LogicSatellites.Behaviours.ModAssets;
using static LogicSatellites.STRINGS.ITEMS;

namespace LogicSatellites.Entities
{
	public class SatelliteGridConfig : IEntityConfig, IListableOption,IHasDlcRestrictions
	{
		public const string ID = "LS_SatelliteGrid";
		public string[] GetAnyRequiredDlcIds()
		{
			return null;
		}

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
			destinationSelector.shouldPointTowardsPath = false;
			destinationSelector.requireAsteroidDestination = false;
			var traveler = looseEntity.AddOrGet<ClusterTraveler>();
			traveler.stopAndNotifyWhenPathChanges = false;
			var entity = looseEntity.AddOrGet<SatelliteGridEntity>();
			looseEntity.AddOrGetDef<SatelliteTelescope.Def>();
			entity.satelliteType = (int)SatType.Exploration;
			entity.clusterAnimName = "space_satellite_kanim";
			entity.enabled = true;

			return looseEntity;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}

		public string GetProperName()
		{
			return "Logic Satellite";
		}
	}
}