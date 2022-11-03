using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStation : Clustercraft
    {
        [Serialize]
        private string m_name = "SpaceStation";

        [Serialize]
        public int SpaceStationInteriorId = -1;

        public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
        {
            new ClusterGridEntity.AnimConfig()
            {
                animFile = Assets.GetAnim((HashedString) "gravitas_space_poi_kanim"),
                initialAnim =  "station_1"
            }
        };

        public override string Name => this.m_name;
        public override bool IsVisible => true;
        public override EntityLayer Layer => EntityLayer.Craft;
        public override bool SpaceOutInSameHex() => true;
        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
        public override Sprite GetUISprite() => Assets.GetSprite("rocket_landing"); //Def.GetUISprite((object)this.gameObject).first;

        protected override void OnSpawn()
        {
            Debug.Log("MY WorldID:" + SpaceStationInteriorId);
            if (SpaceStationInteriorId < 0)
            {
                var interiorWorld = SpaceStationManager.Instance.CreateSpaceStationInteriorWorld(gameObject, "interiors/OrbitalSpaceStation", new Vector2I(27, 37), null);
                SpaceStationInteriorId = interiorWorld.id;
                Debug.Log("new WorldID:" + SpaceStationInteriorId);
                Debug.Log("ADDED NEW SPACE STATION INTERIOR");
            }
            ClusterManager.Instance.GetWorld(SpaceStationInteriorId).AddTag(ModAssets.Tags.IsSpaceStation);
            Debug.Log(Location.Q + ","+Location.R + " RASDKANMSDKAO");
            base.OnSpawn();
            this.SetCraftStatus(CraftStatus.InFlight);
            Debug.Log(Location.Q + "," + Location.R + " RASDKANMSDKAO");
            Debug.Log(" viss"+ IsVisible);
            var destination = gameObject.GetComponent<RocketClusterDestinationSelector>();
            destination.SetDestination(this.Location);
        }
        public new void Sim4000ms(float dt)
        {

            Debug.Log(Location.Q + "," + Location.R + " Status: "+ Status);
        }

    }
}
