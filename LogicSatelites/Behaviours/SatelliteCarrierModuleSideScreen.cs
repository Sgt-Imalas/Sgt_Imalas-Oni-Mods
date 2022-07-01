using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatelites.Behaviours
{
    public class SatelliteCarrierModuleSideScreen : SideScreenContent
    {

        [SerializeField]
        private SatelliteCarrierModule targetCarrierModule;
        [SerializeField]
        private KButton button;
        [SerializeField]
        private LocText label;

        public override string GetTitle() => "Satellite Carrier";
        public override float GetSortKey() => 21f;
        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Clustercraft>() != null && this.GetSatelliteCarrierModule(target.GetComponent<Clustercraft>()) != (UnityEngine.Object)null;
        
        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            this.targetCarrierModule = GetSatelliteCarrierModule(target.GetComponent<Clustercraft>());
        }

        private SatelliteCarrierModule GetSatelliteCarrierModule(Clustercraft craft)
        {
            foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                SatelliteCarrierModule component = clusterModule.Get().GetComponent<SatelliteCarrierModule>();
                if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                    return component;
            }
            return (SatelliteCarrierModule)null;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            titleKey = "STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLE";

            Transform contents = transform.Find("Contents");
            button = contents.Find("Button")?.GetComponent<KButton>();
            label = contents.Find("Label")?.GetComponent<LocText>();

        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            button.onClick += OnButtonClick;
            Refresh();
        }
        private void Refresh()
        {
            if (label is null || targetCarrierModule is null)
            {
                return;
            }
            label.SetText(targetCarrierModule.smi.IsHoldingSatellite ? "Deploy Satellite" : "Store Satellite");
        }

        private void OnButtonClick()
        {
            //InitiateDetonation();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Refresh();
        }
    }
}
