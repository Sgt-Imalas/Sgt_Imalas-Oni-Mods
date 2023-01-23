using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rockets_TinyYetBig.ModAssets;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStation : Clustercraft
    {

        [Serialize]
        private string m_name = "Space Station";

        [Serialize]
        public int SpaceStationInteriorId = -1;

        [Serialize]
        public int _currentSpaceStationType = 0;

        public SpaceStationWithStats CurrentSpaceStationType => ModAssets.SpaceStationTypes[_currentSpaceStationType];

        [Serialize]
        public int IsOrbitalSpaceStationWorldId = -1;
        [Serialize]
        public bool IsDeconstructable = true;
        [Serialize]
        public bool BuildableInterior = true;

        public Vector2I InteriorSize = new Vector2I(30, 30);
        public string InteriorTemplate = "emptySpaceStationPrefab";

        public string ClusterAnimName = "space_station_small_kanim";
        //public string IconAnimName = "station_3";

        public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
        {
            new AnimConfig
            {
                animFile = Assets.GetAnim(CurrentSpaceStationType.Kanim),
                initialAnim = "idle_loop"
            }
        };

        public override string Name => this.m_name;
        //public override bool IsVisible => true;
        public override EntityLayer Layer => EntityLayer.Craft;
        public override bool SpaceOutInSameHex() => true;
        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
        //public override Sprite GetUISprite() => Assets.GetSprite("rocket_landing"); //Def.GetUISprite((object)this.gameObject).first;
        public override Sprite GetUISprite()
        {
            return Def.GetUISpriteFromMultiObjectAnim(AnimConfigs[0].animFile);
        }

        //public Sprite GetUISpriteAt(int i) => Def.GetUISpriteFromMultiObjectAnim(AnimConfigs[i].animFile);
        public override void OnSpawn()
        {
            base.OnSpawn();
            //Debug.Log("MY WorldID:" + SpaceStationInteriorId);
            if (SpaceStationInteriorId < 0)
            {
                var interiorWorld = SpaceStationManager.Instance.CreateSpaceStationInteriorWorld(gameObject, "interiors/" + InteriorTemplate, CurrentSpaceStationType.InteriorSize, BuildableInterior, null, Location);
                SpaceStationInteriorId = interiorWorld.id;
                Debug.Log("new WorldID:" + SpaceStationInteriorId);
                Debug.Log("ADDED NEW SPACE STATION INTERIOR");
            }
            ClusterManager.Instance.GetWorld(SpaceStationInteriorId).AddTag(ModAssets.Tags.IsSpaceStation);


            this.SetCraftStatus(CraftStatus.InFlight);
            var destinationSelector = gameObject.GetComponent<RocketClusterDestinationSelector>();
            destinationSelector.SetDestination(this.Location);
            var planet = ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.Location, EntityLayer.Asteroid);
            if (planet != null)
            {
                IsOrbitalSpaceStationWorldId = planet.GetComponent<WorldContainer>().id;
            }

            var m_clusterTraveler = gameObject.GetComponent<ClusterTraveler>();
            m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
            m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
            m_clusterTraveler.onTravelCB = (System.Action)null;
            m_clusterTraveler.validateTravelCB = null;


            this.Subscribe<SpaceStation>(1102426921, NameChangedHandler);
        }
        private static EventSystem.IntraObjectHandler<SpaceStation> NameChangedHandler = new EventSystem.IntraObjectHandler<SpaceStation>((System.Action<SpaceStation, object>)((cmp, data) => cmp.SetStationName(data)));
        public void SetStationName(object newName)
        {
            SetStationName((string)newName);
        }

        public void SetStationName(string newName)
        {
            m_name = newName;
            base.name = "Space Station: " + newName;
            ClusterManager.Instance.Trigger(1943181844, newName);
        }
        private bool CanTravel(bool tryingToLand) => true;
        private float GetSpeed() => 1f;
        public void DestroySpaceStation()
        {
            this.SetExploding();
            SpaceStationManager.Instance.DestroySpaceStationInteriorWorld(this.SpaceStationInteriorId);
            UnityEngine.Object.Destroy(this.gameObject);
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

    }
}
