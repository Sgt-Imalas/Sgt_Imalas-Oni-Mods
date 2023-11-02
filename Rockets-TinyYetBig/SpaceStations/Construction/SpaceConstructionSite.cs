using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    public class SpaceConstructionSite : ClusterGridEntity
    {
        [Serialize]
        private string m_name;
        // private string clusterAnimSymbolSwapTarget;
        // private string clusterAnimSymbolSwapSymbol;

        public override bool SpaceOutInSameHex() => false;

        public override EntityLayer Layer => EntityLayer.POI;

        public override string Name => this.m_name;
        public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
        {
            new ClusterGridEntity.AnimConfig()
            {
                animFile = Assets.GetAnim("harvestable_space_poi_kanim"),
                initialAnim = "cloud",
                //symbolSwapTarget = this.clusterAnimSymbolSwapTarget,
               // symbolSwapSymbol = this.clusterAnimSymbolSwapSymbol
            }
        };


        public void SetItemName(string newName)
        {
            this.m_name = newName;

            this.name = newName;

            if (TryGetComponent<KSelectable>(out var selectable))
            {
                selectable.SetName(this.name);
                selectable.entityName = this.name;
            }

            if (TryGetComponent<CharacterOverlay>(out var component))
            {
                NameDisplayScreen.Instance.UpdateName(component.gameObject);
            }

        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        public override bool IsVisible => true;

        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
        public void Init(AxialI location) => this.Location = location;
        public override bool ShowName() => false;// true;

    }
}
