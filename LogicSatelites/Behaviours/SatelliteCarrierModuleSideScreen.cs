using PeterHan.PLib.UI;
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
        private Dictionary<ISatelliteCarrier, GameObject> modulePanels = new Dictionary<ISatelliteCarrier, GameObject>();
        [SerializeField]
        private Clustercraft targetCraft;

        [SerializeField]
        public GameObject moduleContentContainer;
        [SerializeField]
        public GameObject modulePanelPrefab;

        private List<int> refreshHandle = new List<int>();

        private LocText Title;
        private Dictionary<ISatelliteCarrier, LocText> buttonLabels = new Dictionary<ISatelliteCarrier, LocText>();
        private Dictionary<ISatelliteCarrier, ToolTip> buttonTooltips1 = new Dictionary<ISatelliteCarrier, ToolTip>();
        private Dictionary<ISatelliteCarrier, ToolTip> buttonTooltips2 = new Dictionary<ISatelliteCarrier, ToolTip>();


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

            RefreshStrings();
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
            buttonLabels.Clear();
            buttonTooltips1.Clear();
            buttonTooltips2.Clear();

            foreach (KeyValuePair<ISatelliteCarrier, GameObject> modulePanel in this.modulePanels)
                Util.KDestroyGameObject(modulePanel.Value.gameObject);
            modulePanels.Clear();
        }

        private void BuildModules()
        {
            ClearModules();
            foreach (Ref<RocketModuleCluster> clusterModule in targetCraft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                if (clusterModule.Get().GetSMI<ISatelliteCarrier>() != null) {
                    GameObject hierarchyReferences = Util.KInstantiateUI(this.modulePanelPrefab, this.moduleContentContainer, true);
                ISatelliteCarrier carrierInstance = clusterModule.Get().GetSMI<ISatelliteCarrier>();
                if (carrierInstance != null)
                {
                    this.modulePanels.Add(carrierInstance, hierarchyReferences);
                    this.RefreshModulePanel(carrierInstance);
                }
                }
            }
        }

        protected override void OnShow(bool show)
        {
            base.OnShow(show);
            this.ConsumeMouseScroll = true;
            RefreshStrings();
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
            this.titleKey = "STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLE";
            Title  = transform.Find("Title/Label").GetComponent<LocText>();
            RefreshStrings();

            //titleText = transform.Find("TitleBox/Label").GetComponent<LocText>();
            //button = transform.Find("ModuleWidget/Layout/Info/Buttons/Button")?.GetComponent<KButton>();
            // label = transform.Find("ModuleWidget/Layout/Info/Label")?.GetComponent<LocText>();
            //buttonText = button.GetComponentInChildren<LocText>(true);

            //BuildModules();
        }
        private void RefreshAll(object data = null) => this.BuildModules();

        private void RefreshModulePanel(ISatelliteCarrier module)
        {
            UIUtils.ListAllChildren(transform);
            //UIUtils.GiveAllChildObjects(transform.gameObject);
            var modulePanel = this.modulePanels[module].transform;
            modulePanel.Find("Layout/Portrait/Sprite").GetComponent<Image>().sprite = Def.GetUISprite((object)module.master.gameObject).first;
            var Button1 = modulePanel.Find("Layout/Info/Buttons/Button").GetComponent<KButton>();
            var Button2 = modulePanel.Find("Layout/Info/Buttons/RepeatButton").GetComponent<KButton>();
            modulePanel.Find("Layout/Info/DropDown").GetComponent<DropDown>().gameObject.SetActive(false);
            //modulePanel.Find("ModuleWidget/Layout/Info/Label").GetComponent<CrewPortrait>().gameObject.SetActive(false);
            modulePanel.Find("Layout/Info/Label").GetComponent<LocText>().SetText(module.master.gameObject.GetProperName());
            Debug.Log(modulePanel.Find("Layout/Info/Buttons/Button/Label"));



            buttonLabels.Add(module, modulePanel.Find("Layout/Info/Buttons/Button/Label").GetComponent<LocText>());// module.HoldingSatellite() ? "Deploy Satellite" : "Retrieve Satellite";
            buttonTooltips1.Add(module, Button1.GetComponentInChildren<ToolTip>());
            buttonTooltips2.Add(module, Button2.GetComponentInChildren<ToolTip>());

            Button1.onClick += () => module.OnButtonClicked();
            Button1.GetComponentInChildren<ToolTip>().SetSimpleTooltip(module.HoldingSatellite() ? "Deploys a satellite at the current space hex" : "Retrieves a satellite from the current space hex");
            Button2.onClick += () => ChangeOperationMode(module);
            //UIUtils.ListAllChildren(Button1);
            UIUtils.GiveAllChildObjects(Button1.gameObject);
            RefreshStrings();
        }
        private void RefreshStrings(ISatelliteCarrier module = null)
        {
            foreach (var v in modulePanels.Keys)
            {
                buttonLabels[v].SetText(v.ModeIsDeployment ? "Deploy Satellite" : "Retrieve Satellite");
                buttonTooltips1[v].SetSimpleTooltip(v.ModeIsDeployment ? "Deploys a satellite at the current space hex" : "Retrieves a satellite from the current space hex");
                buttonTooltips2[v].SetSimpleTooltip("Change the operation the module should perform");
            }
        }
        private void ChangeOperationMode(ISatelliteCarrier module)
        {
            module.ModeIsDeployment = !module.ModeIsDeployment;
            RefreshStrings();
        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Title.SetText(STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLE); 
            RefreshStrings();
        }
    }
}
