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

            AsteroidSize.SetActive(!isPoi);
            AsteroidTraits.SetActive(!isPoi);
            AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.PlanetDimensions.x, current.PlanetDimensions.y);


        }
        public void UpdateUI()
        {
            if(lastSelected!=null)
                UpdateForSelected(lastSelected);

            UiRefresh.Invoke();
        }
        public System.Action UiRefresh;


        private bool init;
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

            ClusterSize = transform.Find("ClusterSize/Slider").FindOrAddComponent<FSlider>();
            //UIUtils.FindAndDisable(ClusterSize.transform, "Descriptor/Input");
            ClusterSize.SetWholeNumbers(true);
            ClusterSize.AttachOutputField(transform.Find("ClusterSize/Descriptor/Output").GetComponent<LocText>());
            ClusterSize.OnChange += (value) =>
            {
                CustomCluster.SetRings((int)value);
                UpdateUI();
            };



            AsteroidSize = transform.Find("AsteroidSizeInfo").gameObject;
            AsteroidSizeLabel = transform.Find("AsteroidSizeInfo/Info").GetComponent<LocText>();
            AsteroidTraits = transform.Find("AsteroidTraits").gameObject;

            AddTraitButton = transform.Find("AsteroidTraits/AddTraitButton").FindOrAddComponent<FButton>();

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

            UIUtils.AddSimpleTooltipToObject(ResetAllButton.transform, STRINGS.UI.CUSTOMCLUSTERUI.RESET.DESC, true);


            init = true;
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
