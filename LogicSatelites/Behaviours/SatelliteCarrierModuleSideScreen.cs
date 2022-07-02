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
        private LocText titleText;


        private List<int> refreshHandle = new List<int>();

        public override float GetSortKey() => 21f;
        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Clustercraft>() != null && this.HasSatelliteCarriers(target.GetComponent<Clustercraft>());
        
        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            this.targetCraft = target.GetComponent<Clustercraft>();
            this.Refresh();
        }

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
            UIUtils.ListAllChildren(transform);
            this.titleKey = "STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLE";

            //Transform contents = transform.Find("Contents");

            titleText = transform.Find("TitleBox/Label").GetComponent<LocText>();
            transform.Find("ModuleWidget/Layout/Portrait/Sprite").GetComponent<Image>().sprite = Def.GetUISprite((object)Modules.First().master.gameObject).first;
            button = transform.Find("ModuleWidget/Layout/Info/Buttons/Button")?.GetComponent<KButton>();
            label = transform.Find("ModuleWidget/Layout/Info/Label")?.GetComponent<LocText>();
            buttonText = button.GetComponentInChildren<LocText>(true);

            BuildModules();
        }
        
        protected override void OnSpawn()
        {
            base.OnSpawn();
            button.onClick += OnButtonClick;
            Refresh();
        }
        private void Refresh()
        {
            BuildModules();

            if (buttonText is null || label is null || Modules.Count==0)
            {
                return;
            }
            titleText.SetText(GetTitle());
            
            button.GetComponentInChildren<ToolTip>().SetSimpleTooltip(CanDeploySatellite() ? "Deploys a satellite at the current space hex" : "Retrieves a satellite from the current space hex");
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
            Modules[0].DeploySatellite();

            Refresh();
        }
    }
}
