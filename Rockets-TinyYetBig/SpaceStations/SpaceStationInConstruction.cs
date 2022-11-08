using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    public class SpaceStationInConstruction : ClusterGridEntity
    {
        [Serialize]
        public string clusterAnimName;
        [Serialize]
        private string m_name;
        [Serialize]
        public int SpaceStationTypeWhenDone;
        // private string clusterAnimSymbolSwapTarget;
        // private string clusterAnimSymbolSwapSymbol;

        public override bool SpaceOutInSameHex() => true;

        public override EntityLayer Layer => EntityLayer.Payload;

        public override string Name => this.m_name;

        public enum SpaceStationType
        {
            SmallOrbital = 0,
            LargeOrbital=1,
            EmptySmall = 2,
            EmptyMedium = 3,
            EmptyLarge = 4,
        }

        public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
        {
            new ClusterGridEntity.AnimConfig()
            {
                animFile = Assets.GetAnim((HashedString) this.clusterAnimName),
                initialAnim = "idle_loop",
                //symbolSwapTarget = this.clusterAnimSymbolSwapTarget,
               // symbolSwapSymbol = this.clusterAnimSymbolSwapSymbol
            }
        };
        protected override void OnSpawn()
        {
            base.OnSpawn();
        }
        public void SetSatelliteType(SpaceStationType type)
        {
            SpaceStationTypeWhenDone = (int)type;
        }
        public void SetSpaceStationWIPName()
        {
            this.m_name = SpaceStationTypeWhenDone.ToString();

            this.name = SpaceStationTypeWhenDone.ToString();
            CharacterOverlay component = this.GetComponent<CharacterOverlay>();
            KSelectable selectable = this.GetComponent<KSelectable>();
            if (selectable != null)
            {
                selectable.SetName(this.name);
                selectable.entityName = this.name;
            }
            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                {
                    NameDisplayScreen.Instance.UpdateName(component.gameObject);
                }
            
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        public override bool IsVisible => true;

        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
        public void Init(AxialI location) => this.Location = location;

    }
}
