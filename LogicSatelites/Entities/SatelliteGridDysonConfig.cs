using LogicSatellites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LogicSatellites.Behaviours.ModAssets;
using static LogicSatellites.STRINGS.ITEMS;

namespace LogicSatellites.Entities
{
    public class SatelliteGridDysonConfig : IEntityConfig, IListableOption
    {
        public const string ID = "LS_SatelliteGridDyson";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

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
            entity.satelliteType = (int)SatType.DysonComponent;
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
            return "Dyson Sphere Component";
        }
    }
}