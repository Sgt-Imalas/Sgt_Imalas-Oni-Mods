using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    public class SpaceConstructionSite:ClusterGridEntity
    {
        [Serialize]
        public string clusterAnimName;
        [Serialize]
        private string m_name;
        // private string clusterAnimSymbolSwapTarget;
        // private string clusterAnimSymbolSwapSymbol;

        public override bool SpaceOutInSameHex() => true;

        public override EntityLayer Layer => EntityLayer.Payload;

        public override string Name => this.m_name;
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
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
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
    }
}
