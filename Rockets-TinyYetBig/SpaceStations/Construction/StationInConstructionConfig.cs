using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    internal class StationInConstructionConfig : IEntityConfig
    {
        public const string ID = "RTB_StationConstructionSite";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            var looseEntity = EntityTemplates.CreateLooseEntity(
                   id: ID,
                   name: "Station construction site",
                   desc: "tba.", ///TODO STRINGS
                   mass: 1,
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
            var entity = looseEntity.AddOrGet<StationInConstruction>();

            return looseEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

    }
}
