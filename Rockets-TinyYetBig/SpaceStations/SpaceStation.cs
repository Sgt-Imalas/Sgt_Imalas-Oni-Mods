using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStation : ClusterGridEntity
    {
        [Serialize]
        private string m_name;


        [Serialize]
        public int SpaceStationInteriorId = -1;

        public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
        {
            new ClusterGridEntity.AnimConfig()
            {
                animFile = Assets.GetAnim((HashedString) "rocket01_kanim"),
                initialAnim = "idle_loop"
            }
        };

        public override string Name => this.m_name;
        public override bool IsVisible => true;
        public override EntityLayer Layer => EntityLayer.Craft;
        public override bool SpaceOutInSameHex() => true;
        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;


        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (SpaceStationInteriorId < 0)
            {
                var interiorWorld = SpaceStationManager.Instance.CreateSpaceStationInteriorWorld(gameObject, "interiors/OrbitalSpaceStation", new Vector2I(40, 40), null);
                SpaceStationInteriorId = interiorWorld.GetMyWorldId();
            }
        }

    }
}
