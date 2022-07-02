using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace LogicSatelites.Behaviours
{
    public class SatelliteCarrierModuleSideScreen : SideScreenContent
    {

        [SerializeField]
        private List<ISatelliteCarrier> Modules = new List<ISatelliteCarrier>();
        [SerializeField]
        private Clustercraft targetCraft;
        [SerializeField]
        private KButton button;
        [SerializeField]
        private LocText label;
        [SerializeField]
        private LocText buttonText;

        private List<int> refreshHandle = new List<int>();

        public override float GetSortKey() => 21f;
        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Clustercraft>() != null && this.HasSatelliteCarriers(target.GetComponent<Clustercraft>());
        
        public override void SetTarget(GameObject target)
        {
            if ((UnityEngine.Object)target != (UnityEngine.Object)null)
            {
                foreach (int id in this.refreshHandle)
                    target.Unsubscribe(id);
                this.refreshHandle.Clear();
            }
            base.SetTarget(target);
            this.targetCraft = target.GetComponent<Clustercraft>();
            if ((UnityEngine.Object)this.targetCraft == (UnityEngine.Object)null && (UnityEngine.Object)target.GetComponent<RocketControlStation>() != (UnityEngine.Object)null)
                this.targetCraft = target.GetMyWorld().GetComponent<Clustercraft>();
            this.refreshHandle.Add(this.targetCraft.gameObject.Subscribe(-1298331547, new System.Action<object>(this.RefreshAll)));
            this.refreshHandle.Add(this.targetCraft.gameObject.Subscribe(1792516731, new System.Action<object>(this.RefreshAll)));
            this.BuildModules();
        }
        private void RefreshAll(object data = null) => this.BuildModules();


        private bool HasSatelliteCarriers(Clustercraft craft)
        {
            foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                if (clusterModule.Get().GetSMI<ISatelliteCarrier>() != null)
                    return true;
            }
            return false;
        }

        private void ClearModules()
        {
            this.Modules.Clear();
        }

        private void BuildModules()
        {
            this.ClearModules();
            foreach (Ref<RocketModuleCluster> clusterModule in targetCraft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                ISatelliteCarrier smi = clusterModule.Get().GetSMI<ISatelliteCarrier>();
                if (smi != null)
                {
                    this.Modules.Add(smi);
                }
            }
        }

        protected override void OnShow(bool show)
        {
            base.OnShow(show);
            this.ConsumeMouseScroll = true;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.titleKey = "STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLE";
            HierarchyReferences component = this.GetComponent<HierarchyReferences>();

             //UIUtils.ListChildrenHR(component);
            component.GetReference<Image>("icon").sprite = Def.GetUISprite((object)Modules.First().master.gameObject).first;
            button = component.GetReference<KButton>("button");
            label = component.GetReference<LocText>("label");
            buttonText = button.GetComponentInChildren<LocText>(true);

            BuildModules();


            //var SubObjects = this.GetComponentsInChildren<UnityEngine.Object>(); //finding the pesky tooltip; maybe usefull l8er
            //foreach (var v in SubObjects)
            //{
            //    Debug.Log(v);
            //}
        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            button.onClick += OnButtonClick;
            Refresh();
        }
        private void Refresh()
        {
            Debug.Log(Modules.Count+ " COAUTNS");
            if (buttonText is null || label is null || Modules.Count==0)
            {
                return;
            }
            button.GetComponentInChildren<ToolTip>().SetSimpleTooltip(CanDeploySatellite() ? "Deploys the satellite at the current space hex" : "Retrieves the satellite from the current space hex");
            buttonText.SetText(CanDeploySatellite() ? "Deploy Satellite" : "Retrieve Satellite");
            label.SetText(String.Format("Holding {0}x Satellite",SatelliteCount()));
            
        }

        private int SatelliteCount()
        {
            int retVal = 0;
            foreach (var carrier in Modules)
            {
                if (carrier.HoldingSatellite())
                    ++retVal;
            }
            return retVal;
        }

        private bool CanDeploySatellite()
        {
            foreach(var carrier in Modules)
            {
                if (carrier.CanDeploySatellite())
                    return true;
            }
            return false;
        }

        private void OnButtonClick()
        {
            Debug.Log(GetTitle());
            Refresh();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Refresh();
        }
    }
}
