using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStationConfig : IEntityConfig, IListableOption
    {
        public const string ID = "RTB_SpaceStationOrbitalSmall";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            var entity = EntityTemplates.CreateEntity(
                   id: ID,
                   name: "TestSpaceStationOrbitalSmall"
             );
            SaveLoadRoot saveLoadRoot = entity.AddOrGet<SaveLoadRoot>();
            saveLoadRoot.DeclareOptionalComponent<WorldInventory>();
            saveLoadRoot.DeclareOptionalComponent<WorldContainer>();
            saveLoadRoot.DeclareOptionalComponent<OrbitalMechanics>();
            entity.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;

            RocketClusterDestinationSelector destinationSelector = entity.AddOrGet<RocketClusterDestinationSelector>();
            destinationSelector.assignable = false;
            destinationSelector.shouldPointTowardsPath = false;
            destinationSelector.requireAsteroidDestination = false;

            var spst = entity.AddOrGet<SpaceStation>();

            entity.AddOrGet<CharacterOverlay>().shouldShowName = true;
            entity.AddOrGetDef<AlertStateManager.Def>();
            entity.AddOrGet<Notifier>();

            var traveler = entity.AddOrGet<ClusterTraveler>();
            traveler.stopAndNotifyWhenPathChanges = false;

            entity.AddOrGet<CraftModuleInterface>();
            entity.AddOrGet<UserNameable>();

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
            return "spacae";
        }
    }
}
