using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatelites.Behaviours
{
    public class SatelliteGridEntity : ClusterGridEntity
    {
        [SerializeField]
        public string clusterAnimName;
        [SerializeField]
        public StringKey nameKey;
       // private string clusterAnimSymbolSwapTarget;
       // private string clusterAnimSymbolSwapSymbol;

        public override bool SpaceOutInSameHex() => true;

        public override string Name => (string)Strings.Get(this.nameKey);
        public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
        {
            new ClusterGridEntity.AnimConfig()
            {
                animFile = Assets.GetAnim((HashedString) this.clusterAnimName),
                initialAnim = "object",
                //symbolSwapTarget = this.clusterAnimSymbolSwapTarget,
               // symbolSwapSymbol = this.clusterAnimSymbolSwapSymbol
            }
        };
        public override bool IsVisible => true;

        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
        public override EntityLayer Layer => EntityLayer.POI;
        public void Init(AxialI location) => this.Location = location;

    }
}
