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
        private Dictionary<ISatelliteCarrier, HierarchyReferences> modulePanels = new Dictionary<ISatelliteCarrier,HierarchyReferences>();
        [SerializeField]
        private Clustercraft targetCraft;

        [SerializeField]
        public GameObject moduleContentContainer;
        [SerializeField]
        public GameObject modulePanelPrefab;

        private List<int> refreshHandle = new List<int>();

        public override float GetSortKey() => 21f;
        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Clustercraft>() != null && this.HasSatelliteCarriers(target.GetComponent<Clustercraft>());
        
        public override void SetTarget(GameObject target)
        {
            if (target != null)
            {
                foreach (int id in this.refreshHandle)
                    target.Unsubscribe(id);
                refreshHandle.Clear();
            }
            base.SetTarget(target);

            GetPrefabStrings();
            targetCraft = target.GetComponent<Clustercraft>();
            if (targetCraft == null && target.GetComponent<RocketControlStation>() != null)
                targetCraft = target.GetMyWorld().GetComponent<Clustercraft>();
            refreshHandle.Add(this.targetCraft.gameObject.Subscribe(-1298331547, new System.Action<object>(this.RefreshAll)));
            refreshHandle.Add(this.targetCraft.gameObject.Subscribe(1792516731, new System.Action<object>(this.RefreshAll)));
            BuildModules();
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
            var Content = transform.Find("ScrollSetup/ScrollRect/Content/ModuleWidget");
            while (Content != null)
            {

            }
            Transform Content = transform.Find("ScrollSetup/ScrollRect/Content"); Util.KDestroyGameObject(
            foreach (KeyValuePair<ISatelliteCarrier, HierarchyReferences> modulePanel in this.modulePanels)
                Util.KDestroyGameObject(modulePanel.Value.gameObject);
            modulePanels.Clear();
        }

        private void BuildModules()
        {
            ClearModules();
            foreach (Ref<RocketModuleCluster> clusterModule in targetCraft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.modulePanelPrefab, this.moduleContentContainer, true);
                ISatelliteCarrier carrierInstance = clusterModule.Get().GetSMI<ISatelliteCarrier>();
                if (carrierInstance != null)
                {
                    this.modulePanels.Add(carrierInstance, hierarchyReferences);
                    this.RefreshModulePanel(carrierInstance);
                }
            }
            Debug.Log(modulePanels.Count);
        }

        protected override void OnShow(bool show)
        {
            base.OnShow(show);
            this.ConsumeMouseScroll = true;
        }
        private void GetPrefabStrings()
        {
            Transform Content = transform.Find("ScrollSetup/ScrollRect/Content");
            moduleContentContainer = Content.gameObject;
            modulePanelPrefab = Content.Find("ModuleWidget").gameObject;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            UIUtils.ListAllChildren(this.transform);
            this.titleKey = "STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLE";

            

            //titleText = transform.Find("TitleBox/Label").GetComponent<LocText>();
            //transform.Find("ModuleWidget/Layout/Portrait/Sprite").GetComponent<Image>().sprite = Def.GetUISprite((object)Modules.First().master.gameObject).first;
            //button = transform.Find("ModuleWidget/Layout/Info/Buttons/Button")?.GetComponent<KButton>();
           // label = transform.Find("ModuleWidget/Layout/Info/Label")?.GetComponent<LocText>();
            //buttonText = button.GetComponentInChildren<LocText>(true);

            //BuildModules();
        }
        private void RefreshAll(object data = null) => this.BuildModules();

        private void RefreshModulePanel(ISatelliteCarrier module)
        {
            HierarchyReferences modulePanel = this.modulePanels[module];
            modulePanel.GetReference<Image>("icon").sprite = Def.GetUISprite((object)module.master.gameObject).first;
            KButton Button1 = modulePanel.GetReference<KButton>("button");
            KButton Button2 = modulePanel.GetReference<KButton>("repeatButton"); 
            modulePanel.GetReference<DropDown>("dropDown").gameObject.SetActive(false);
            modulePanel.GetReference<CrewPortrait>("selectedPortrait").gameObject.SetActive(false);

            modulePanel.GetReference<LocText>("label").SetText(module.master.gameObject.GetProperName());
        }


        //protected override void OnSpawn()
        //{
        //    base.OnSpawn();
        //}
        //private void Refresh()
        //{
        //    return;
        //    BuildModules();

        //    if (buttonText is null || label is null || modulePanels.Count==0)
        //    {
        //        return;
        //    }
        //    titleText.SetText(GetTitle());
            
        //    //button.GetComponentInChildren<ToolTip>().SetSimpleTooltip(CanDeploySatellite() ? "Deploys a satellite at the current space hex" : "Retrieves a satellite from the current space hex");
        //    //buttonText.SetText(CanDeploySatellite() ? "Deploy Satellite" : "Retrieve Satellite");
        //    //label.SetText(String.Format("Holding {0}x Satellite",SatelliteCount()));
            
        //}
        
    }
}
