using Klei.AI;
using KMod;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM.INDIVIDUALSETTINGS;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM.INDIVIDUALSETTINGS.ASTEROIDTRAITS;
using static STRINGS.UI.UISIDESCREENS.AUTOPLUMBERSIDESCREEN;

namespace ClusterTraitGenerationManager
{
    internal class SelectedItemSettings : FScreen
    {
        private FToggle2 StarmapItemEnabled;

        private FSlider NumberToGenerate;

        private FSlider MinimumDistance;

        private FSlider MaximumDistance;

        private FSlider BufferDistance;

        private FSlider ClusterSize;

        private GameObject AsteroidSize;
        private LocText AsteroidSizeLabel;
        private GameObject AsteroidTraits;
        private GameObject ActiveTraitsContainer;
        private GameObject TraitPrefab;


        public FButton AddTraitButton;


        public FButton ResetButton;
        public FButton ResetAllButton;
        public FButton ReturnButton;
        public FButton GenerateClusterButton;

        public System.Action OnClose;

        private StarmapItem lastSelected;
        public void UpdateForSelected(StarmapItem SelectedPlanet)
        {
            if (!init)
            {
                Init();
            }

            if (SelectedPlanet==null) return;
            lastSelected = SelectedPlanet;
            bool isPoi = SelectedPlanet.category == StarmapItemCategory.POI;

            bool IsPartOfCluster = CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current);

            StarmapItemEnabled.SetOn(IsPartOfCluster);

            bool canGenerateMultiple = current.MaxNumberOfInstances > 1;
            NumberToGenerate.transform.parent.gameObject.SetActive(canGenerateMultiple);///Amount, only on poi / random planets
            if (canGenerateMultiple)
            {
                NumberToGenerate.SetWholeNumbers(!isPoi);
                NumberToGenerate.SetMinMaxCurrent(0, current.MaxNumberOfInstances, current.InstancesToSpawn);
                NumberToGenerate.SetInteractable(IsPartOfCluster);
            }

            MinimumDistance.SetMinMaxCurrent(0, CustomCluster.Rings, current.minRing);
            MinimumDistance.SetInteractable(IsPartOfCluster);

            MaximumDistance.SetMinMaxCurrent(0, CustomCluster.Rings, current.maxRing);
            MaximumDistance.SetInteractable(IsPartOfCluster);

            BufferDistance.SetMinMaxCurrent(0, CustomCluster.Rings, SelectedPlanet.buffer);
            BufferDistance.transform.parent.gameObject.SetActive(!isPoi);
            BufferDistance.SetInteractable(IsPartOfCluster);

            ClusterSize.SetMinMaxCurrent(ringMin, ringMax, CustomCluster.Rings);

            AddTraitButton.SetInteractable(IsPartOfCluster);

            AsteroidSize.SetActive(!isPoi);
            AsteroidTraits.SetActive(!isPoi);
            AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.PlanetDimensions.x, current.PlanetDimensions.y);
           
            foreach (var traitContainer in Traits.Values)
            {
                traitContainer.SetActive(false);
            }
            foreach (var activeTrait in lastSelected.CurrentTraits)
            {
                Traits[activeTrait].SetActive(true);
            }

        }
        public void UpdateUI()
        {
            

            if (lastSelected != null)
            {
                UpdateForSelected(lastSelected);                
            }

            UiRefresh.Invoke();
        }
        public System.Action UiRefresh;


        private bool init;


        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                SgtLogger.l("CONSUMING1");
                if (TraitSelectorScreen.Instance != null ? !TraitSelectorScreen.Instance.IsCurrentlyActive : true)
                    OnClose.Invoke();
            }

            base.OnKeyDown(e);
        }

        private void Init()
        {
            StarmapItemEnabled = transform.Find("StarmapItemEnabled").FindOrAddComponent<FToggle2>();
            StarmapItemEnabled.SetCheckmark("Background/Checkmark");
            StarmapItemEnabled.OnClick += () =>
                {
                    if (lastSelected != null)
                    {
                        CGSMClusterManager.TogglePlanetoid(lastSelected);
                        UpdateUI();
                    }
                };

            UIUtils.AddSimpleTooltipToObject(StarmapItemEnabled.transform, STRINGS.UI.CGM.INDIVIDUALSETTINGS.STARMAPITEMENABLED.TOOLTIP);

            NumberToGenerate = transform.Find("AmountSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(NumberToGenerate.transform, "Descriptor/Input");
            //UIUtils.ListAllChildrenWithComponents(NumberToGenerate.transform);
            NumberToGenerate.SetWholeNumbers(true);
            NumberToGenerate.AttachOutputField(transform.Find("AmountSlider/Descriptor/Output").GetComponent<LocText>());
            NumberToGenerate.OnChange += (value) =>{
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetSpawnNumber(value);
                    UpdateUI();
                }
            };
            UIUtils.AddSimpleTooltipToObject(NumberToGenerate.transform.parent.Find("Descriptor"), STRINGS.UI.CGM.INDIVIDUALSETTINGS.AMOUNTSLIDER.DESCRIPTOR.TOOLTIP);

            MinimumDistance = transform.Find("MinDistanceSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(MinimumDistance.transform, "Descriptor/Input");
            MinimumDistance.SetWholeNumbers(true);
            MinimumDistance.AttachOutputField(transform.Find("MinDistanceSlider/Descriptor/Output").GetComponent<LocText>());
            MinimumDistance.OnChange += (value) => {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetInnerRing((int)value);
                    UpdateUI();
                }
            };
            UIUtils.AddSimpleTooltipToObject(MinimumDistance.transform.parent.Find("Descriptor"), STRINGS.UI.CGM.INDIVIDUALSETTINGS.MINDISTANCESLIDER.DESCRIPTOR.TOOLTIP);


            MaximumDistance = transform.Find("MaxDistanceSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(MaximumDistance.transform, "Descriptor/Input");
            MaximumDistance.SetWholeNumbers(true);
            MaximumDistance.AttachOutputField(transform.Find("MaxDistanceSlider/Descriptor/Output").GetComponent<LocText>());
            MaximumDistance.OnChange += (value) => {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetOuterRing((int)value);
                    UpdateUI();
                }
            };
            UIUtils.AddSimpleTooltipToObject(MaximumDistance.transform.parent.Find("Descriptor"), STRINGS.UI.CGM.INDIVIDUALSETTINGS.MAXDISTANCESLIDER.DESCRIPTOR.TOOLTIP);


            BufferDistance = transform.Find("BufferSlider/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(BufferDistance.transform, "Descriptor/Input");
            BufferDistance.SetWholeNumbers(true);
            BufferDistance.AttachOutputField(transform.Find("BufferSlider/Descriptor/Output").GetComponent<LocText>());
            BufferDistance.OnChange += (value) => {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                        current.SetBuffer((int)value);
                }
            };
            UIUtils.AddSimpleTooltipToObject(BufferDistance.transform.parent.Find("Descriptor"), STRINGS.UI.CGM.INDIVIDUALSETTINGS.BUFFERSLIDER.DESCRIPTOR.TOOLTIP);

            ClusterSize = transform.Find("ClusterSize/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(ClusterSize.transform, "Descriptor/Input");
            ClusterSize.SetWholeNumbers(true);
            ClusterSize.AttachOutputField(transform.Find("ClusterSize/Descriptor/Output").GetComponent<LocText>());
            ClusterSize.OnChange += (value) =>
            {
                CustomCluster.SetRings((int)value);
                UpdateUI();
            };
            UIUtils.AddSimpleTooltipToObject(ClusterSize.transform.parent.Find("Descriptor"), STRINGS.UI.CGM.INDIVIDUALSETTINGS.CLUSTERSIZE.DESCRIPTOR.TOOLTIP);


            AsteroidSize = transform.Find("AsteroidSizeInfo").gameObject;
            AsteroidSizeLabel = transform.Find("AsteroidSizeInfo/Info").GetComponent<LocText>();
            UIUtils.AddSimpleTooltipToObject(AsteroidSizeLabel.transform.parent, STRINGS.UI.CGM.INDIVIDUALSETTINGS.ASTEROIDSIZEINFO.TOOLTIP);

            AsteroidTraits = transform.Find("AsteroidTraits").gameObject;
            ActiveTraitsContainer = transform.Find("AsteroidTraits/ListView/Content").gameObject;
            TraitPrefab = transform.Find("AsteroidTraits/ListView/Content/ListViewEntryPrefab").gameObject;

            AddTraitButton = transform.Find("AsteroidTraits/AddTraitButton").FindOrAddComponent<FButton>();

            AddTraitButton.OnClick += () =>
            {
                TraitSelectorScreen.InitializeView(lastSelected, ()=>UpdateUI());
            };



            ReturnButton = transform.Find("Buttons/ReturnButton").FindOrAddComponent<FButton>();
            ReturnButton.OnClick += OnClose;

            GenerateClusterButton = transform.Find("Buttons/GenerateClusterButton").FindOrAddComponent<FButton>();
            GenerateClusterButton.OnClick += () => CGSMClusterManager.InitializeGeneration();

            ResetButton = transform.Find("Buttons/ResetSelectionButton").FindOrAddComponent<FButton>();
            ResetButton.OnClick += () =>
            {
                CGSMClusterManager.ResetPlanetFromPreset(lastSelected.id);
                UpdateUI();
            }; 

            ResetAllButton = transform.Find("Buttons/ResetClusterButton").FindOrAddComponent<FButton>();
            ResetAllButton.OnClick += () =>
            {
                CGSMClusterManager.ResetToLastPreset();
                UpdateUI();
            };

            UIUtils.AddSimpleTooltipToObject(ResetAllButton.transform, STRINGS.UI.CGM.INDIVIDUALSETTINGS.BUTTONS.RESETCLUSTERBUTTON.TOOLTIP, true);
            UIUtils.AddSimpleTooltipToObject(ResetButton.transform, STRINGS.UI.CGM.INDIVIDUALSETTINGS.BUTTONS.RESETSELECTIONBUTTON.TOOLTIP, true);

            SgtLogger.Assert("AsteroidSize", AsteroidSize);
            SgtLogger.Assert("AsteroidSizeLabel", AsteroidSizeLabel);
            SgtLogger.Assert("AsteroidTraits", AsteroidTraits);
            SgtLogger.Assert("ActiveTraitsContainer", ActiveTraitsContainer);
            SgtLogger.Assert("TraitPrefab", TraitPrefab);

            InitializeTraitContainer();

            init = true;
        }

        Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();
        void InitializeTraitContainer()
        {
            foreach (var kvp in SettingsCache.worldTraits)
            {
                var TraitHolder = Util.KInstantiateUI(TraitPrefab, ActiveTraitsContainer, true);
                //UIUtils.ListAllChildrenWithComponents(TraitHolder.transform);
                var RemoveButton = TraitHolder.transform.Find("DeleteButton").gameObject.FindOrAddComponent<FButton>();
                Strings.TryGet(kvp.Value.name, out var name);
                Strings.TryGet(kvp.Value.description, out var description);
                var combined = "<color=#" + kvp.Value.colorHex + ">" + name.ToString() + "</color>";

                string associatedIcon = kvp.Value.filePath.Substring(kvp.Value.filePath.LastIndexOf("/") + 1);

                var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.sprite = Assets.GetSprite(associatedIcon);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);
                //
                RemoveButton.OnClick += () =>
                {
                    if(CustomCluster.HasStarmapItem(lastSelected.id, out var item))
                    {
                        item.RemoveWorldTrait(kvp.Value);
                    }
                    UpdateUI();
                };
                Traits[kvp.Value.filePath] = TraitHolder;
            }
            UpdateUI();
        }


        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!init)
            {
                Init();
            }
        }
    }
}
