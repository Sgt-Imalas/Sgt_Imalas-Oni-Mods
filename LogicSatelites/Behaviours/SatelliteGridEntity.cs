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
        protected override void OnSpawn()
        {
            base.OnSpawn();
            SetSatelliteName(ModAssets.GetSatelliteNameRandom());
        }

        public void SetSatelliteName(string newName)
        {
            this.m_name = newName;
            this.name = "Satellite: " + newName;
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
            ModAssets.Satellites.Add(this);
            ModAssets.RedoAdjacencyMatrix();
        }

        protected override void OnCleanUp()
        {
            ModAssets.Satellites.Remove(this);
            ModAssets.RedoAdjacencyMatrix();
            base.OnCleanUp();
        }
        public override bool IsVisible => true;

        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
        public void Init(AxialI location) => this.Location = location;

    }
}
