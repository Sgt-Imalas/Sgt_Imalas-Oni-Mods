using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace LogicSatellites.Behaviours
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

        private Dictionary<ISatelliteCarrier, LocText> titleLabels = new Dictionary<ISatelliteCarrier, LocText>();
        private Dictionary<ISatelliteCarrier, LocText> buttonLabels = new Dictionary<ISatelliteCarrier, LocText>();
        private Dictionary<ISatelliteCarrier, ToolTip> buttonTooltips1 = new Dictionary<ISatelliteCarrier, ToolTip>();
        private Dictionary<ISatelliteCarrier, KButton> buttons = new Dictionary<ISatelliteCarrier, KButton>();
        //private Dictionary<ISatelliteCarrier, ToolTip> buttonTooltips2 = new Dictionary<ISatelliteCarrier, ToolTip>();
        private Dictionary<ISatelliteCarrier, KButton> buttonsRepeat = new Dictionary<ISatelliteCarrier, KButton>();


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
            titleLabels.Clear();
            buttonTooltips1.Clear();
            //buttonTooltips2.Clear();
            buttons.Clear();
            buttonsRepeat.Clear();

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
            var modulePanel = this.modulePanels[module].transform;
            modulePanel.Find("Layout/Portrait/Sprite").GetComponent<Image>().sprite = Def.GetUISprite((object)module.master.gameObject).first;
            var Button1 = modulePanel.Find("Layout/Info/Buttons/Button").GetComponent<KButton>();
            //var Button2 = modulePanel.Find("Layout/Info/Buttons/RepeatButton").GetComponent<KButton>();
            modulePanel.Find("Layout/Info/Buttons/RepeatButton").GetComponent<KButton>().gameObject.SetActive(false);
            modulePanel.Find("Layout/Info/DropDown").GetComponent<DropDown>().gameObject.SetActive(false);
            modulePanel.Find("Layout/Info/Label").GetComponent<LocText>().SetText(module.master.gameObject.GetProperName());

            titleLabels.Add(module, modulePanel.Find("Layout/Info/Label").GetComponent<LocText>());
            buttonLabels.Add(module, modulePanel.Find("Layout/Info/Buttons/Button/Label").GetComponent<LocText>());
            buttonTooltips1.Add(module, Button1.GetComponentInChildren<ToolTip>());
            //buttonTooltips2.Add(module, Button2.GetComponentInChildren<ToolTip>());
            buttons.Add(module, Button1);
            //buttonsRepeat.Add(module, Button2);

            Button1.onClick += () => DeployButtonClicked(module);
            module.ModeIsDeployment = module.HoldingSatellite();
            //Button2.onClick += () => ChangeOperationMode(module);
            RefreshStrings();
        }
        private void RefreshStrings(ISatelliteCarrier module = null)
        {
            foreach (var v in modulePanels.Keys)
            {
                titleLabels[v].SetText(v.HoldingSatellite() ? string.Format(STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLELABEL_HASSAT_TRUE, ModAssets.SatelliteConfigurations[0].NAME) : (string)STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLELABEL_HASSAT_FALSE);
                buttons[v].isInteractable = v.HoldingSatellite() ? v.CanDeploySatellite(0) : true;
                buttonLabels[v].SetText(v.ModeIsDeployment ? STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.BUTTONLABEL_HASSAT_TRUE : STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.BUTTONLABEL_HASSAT_FALSE);
                buttonTooltips1[v].SetSimpleTooltip(v.ModeIsDeployment ? STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.BUTTONTOOLTIP_DEPLOY : STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.BUTTONTOOLTIP_RETRIEVE);
                //buttonTooltips2[v].SetSimpleTooltip(STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.BUTTONTOOLTIP_CHANGEMODE);
            }
        }
        private void DeployButtonClicked(ISatelliteCarrier module)
        {
           // module.OnButtonClicked(0);
            module.ModeIsDeployment = module.HoldingSatellite();
            RefreshStrings();
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
