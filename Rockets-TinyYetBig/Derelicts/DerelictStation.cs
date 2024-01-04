using KSerialization;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Derelicts
{
    internal class DerelictStation : Clustercraft
    {
        [MyCmpReq]
        LoreBearer lorebearer;

        public bool ShowInWorldSelector => lorebearer.BeenClicked;

        int SpaceStationInteriorId = -1;

        public override bool SpaceOutInSameHex() => false;
        public override void OnSpawn()
        {

            if(transform.TryGetComponent<ArtifactPOIClusterGridEntity>(out var old))
            {
                var position = old.Location;
                Destroy(old);
                this.Location = position;
            }//SgtLogger.debuglog("MY WorldID:" + SpaceStationInteriorId);
            if (SpaceStationInteriorId < 0)
            {
                var interiorWorld = SpaceStationManager.Instance.CreateSpaceStationInteriorWorld(gameObject, InteriorTemplate, InteriorSize, false, null, Location,true);
                SpaceStationInteriorId = interiorWorld.id;
                SgtLogger.debuglog("new WorldID:" + SpaceStationInteriorId);
                SgtLogger.debuglog("ADDED NEW SPACE STATION INTERIOR");
            }
            base.OnSpawn();
            ClusterManager.Instance.GetWorld(SpaceStationInteriorId).AddTag(ModAssets.Tags.IsSpaceStation);
            ClusterManager.Instance.GetWorld(SpaceStationInteriorId).AddTag(ModAssets.Tags.IsDerelict);
            this.SetCraftStatus(CraftStatus.InFlight);

            var destinationSelector = gameObject.GetComponent<RocketClusterDestinationSelector>();
            destinationSelector.SetDestination(this.Location);
            
            var m_clusterTraveler = gameObject.GetComponent<ClusterTraveler>();
            m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
            m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
            m_clusterTraveler.onTravelCB = (System.Action)null;
            m_clusterTraveler.validateTravelCB = null;


        }

        public string poiID;

        public string m_Anim;
        public override string Name => poiID;

        public override EntityLayer Layer => EntityLayer.POI;

        public Vector2I InteriorSize;

        public string InteriorTemplate;

        public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
        {
            new AnimConfig
            {
                animFile = Assets.GetAnim("gravitas_space_poi_kanim"),
                initialAnim = (m_Anim.IsNullOrWhiteSpace() ? "station_1" : m_Anim)
            }
        };

        public override Sprite GetUISprite()
        {
            List<ClusterGridEntity.AnimConfig> animConfigs = this.AnimConfigs;
            if (animConfigs.Count > 0)
                return Def.GetUISpriteFromMultiObjectAnim(animConfigs[0].animFile, animConfigs[0].initialAnim, true);
            return null;
        }
        public override bool IsVisible => true;

        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

        }
        

        public void Init(AxialI location)
        {
            base.Location = location;
        }
    }
}
