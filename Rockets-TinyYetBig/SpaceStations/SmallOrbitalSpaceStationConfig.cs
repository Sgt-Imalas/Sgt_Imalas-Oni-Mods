using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SmallOrbitalSpaceStationConfig : IEntityConfig, IListableOption
    {
        public const string ID = "RTB_SpaceStationOrbitalSmall";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            var entity = EntityTemplates.CreateEntity(
                   id: ID,
                   name: "TestSpaceStationOrbitalSmall"
                   //desc: "its a TestSpaceStationOrbitalSmall",
                   //mass: 600,
                   //unitMass: true,
                   //collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                   //isPickupable: false,
                   //anim: Assets.GetAnim("gravitas_space_poi_kanim"),
                   //initialAnim: "station_1",
                   //sceneLayer: Grid.SceneLayer.Creatures,
                   //element: SimHashes.Steel,
                  // additionalTags: new List<Tag>()
                   //{
                  //    GameTags.IgnoreMaterialCategory,
                   //   GameTags.Experimental
                   //}
        );
            entity.AddOrGet<CharacterOverlay>().shouldShowName = true;
            entity.AddOrGetDef<AlertStateManager.Def>();
            entity.AddOrGet<Notifier>();

            ClusterDestinationSelector destinationSelector = entity.AddOrGet<ClusterDestinationSelector>();
            destinationSelector.assignable = false;
            destinationSelector.shouldPointTowardsPath = false;
            destinationSelector.requireAsteroidDestination = false;
            var traveler = entity.AddOrGet<ClusterTraveler>();
            traveler.stopAndNotifyWhenPathChanges = false;
            entity.AddOrGet<SpaceStation>();

            return entity;
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
