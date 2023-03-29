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
using static STRINGS.UI.BUILDCATEGORIES;
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
        private ToolTip AsteroidSizeTooltip;


        private FInputField2 PlanetSizeWidth;
        private FInputField2 PlanetSizeHeight;

        private FCycle PlanetSizeCycle;
        private FCycle PlanetRazioCycle;

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

            if (SelectedPlanet == null) return;
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

            UpdateSizeLabels(current);
            //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
            PlanetSizeCycle.Value = current.CurrentSizePreset.ToString();


            foreach (var traitContainer in Traits.Values)
            {
                traitContainer.SetActive(false);
            }
            foreach (var activeTrait in lastSelected.CurrentTraits)
            {
                Traits[activeTrait].SetActive(true);
            }

        }

        string Warning3 = "EC1802";
        string Warning2 = ("ff8102");
        string Warning1 = ("F6D42A");
         
        public void UpdateSizeLabels(StarmapItem current)
        {
            PlanetSizeWidth.EditTextFromData(current.CustomPlanetDimensions.X.ToString());
            PlanetSizeHeight.EditTextFromData(current.CustomPlanetDimensions.Y.ToString());
            PercentageLargerThanTerra(current, out var Percentage);
            if (Percentage > 200)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZEINFO.LABEL,Warning3);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZEINFO.SIZEWARNING,Percentage), Warning3));
            }
            else if (Percentage > 100)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZEINFO.LABEL, Warning2);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZEINFO.SIZEWARNING, Percentage), Warning2));
            }
            else if (Percentage > 33)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZEINFO.LABEL, Warning1);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZEINFO.SIZEWARNING, Percentage), Warning1));
            }
            else
            {
                AsteroidSizeLabel.text = ASTEROIDSIZEINFO.LABEL;
                AsteroidSizeTooltip.SetSimpleTooltip(ASTEROIDSIZEINFO.TOOLTIP);
            }
        }
        void PercentageLargerThanTerra(StarmapItem current, out int dimensions)
        {
            float TerraArea = 240 * 380;
            float CustomSize = current.CustomPlanetDimensions.X * current.CustomPlanetDimensions.Y;

            dimensions = Mathf.RoundToInt((CustomSize / TerraArea) * 100f);
            dimensions -= 100;
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
            NumberToGenerate.OnChange += (value) =>
            {
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
            MinimumDistance.OnChange += (value) =>
            {
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
            MaximumDistance.OnChange += (value) =>
            {
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
            BufferDistance.OnChange += (value) =>
            {
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


            AsteroidSize = transform.Find("AsteroidSizeCycle").gameObject;
            AsteroidSizeLabel = AsteroidSize.transform.Find("Label").GetComponent<LocText>();
            AsteroidSizeTooltip = UIUtils.AddSimpleTooltipToObject(AsteroidSize.transform.Find("Label"), STRINGS.UI.CGM.INDIVIDUALSETTINGS.ASTEROIDSIZEINFO.TOOLTIP);
            UIUtils.TryChangeText(AsteroidSize.transform, "Info/WidthLabel/Label", ASTEROIDSIZEINFO.WIDTH);
            UIUtils.TryChangeText(AsteroidSize.transform, "Info/HeightLabel/Label", ASTEROIDSIZEINFO.HEIGHT);

            PlanetSizeWidth = AsteroidSize.transform.Find("Info/WidthLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeWidth.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, false));

            PlanetSizeHeight = AsteroidSize.transform.Find("Info/HeightLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeHeight.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, true));

            PlanetSizeCycle = transform.Find("AsteroidSizeCycle/SizeCycle").gameObject.AddOrGet<FCycle>();
            PlanetSizeCycle.Initialize(
                PlanetSizeCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());



            PlanetSizeCycle.Options = new List<FCycle.Option>()
            {
                //new FCycle.Option(WorldSizePresets.Tiny.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE0, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE0TOOLTIP),
                //new FCycle.Option(WorldSizePresets.Smaller.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE1, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE1TOOLTIP),
                //new FCycle.Option(WorldSizePresets.Small.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE2, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE2TOOLTIP),
                //new FCycle.Option(WorldSizePresets.SlightlySmaller.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE3, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE3TOOLTIP),

                new FCycle.Option(WorldSizePresets.Normal.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.SIZE0, ASTEROIDSIZEINFO.SIZESELECTOR.SIZE0TOOLTIP),
                new FCycle.Option(WorldSizePresets.SlightlyLarger.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.SIZE1, ASTEROIDSIZEINFO.SIZESELECTOR.SIZE1TOOLTIP),
                new FCycle.Option(WorldSizePresets.Large.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.SIZE2, ASTEROIDSIZEINFO.SIZESELECTOR.SIZE2TOOLTIP),
                new FCycle.Option(WorldSizePresets.Huge.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.SIZE3, ASTEROIDSIZEINFO.SIZESELECTOR.SIZE3TOOLTIP),
                new FCycle.Option(WorldSizePresets.Massive.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.SIZE4, ASTEROIDSIZEINFO.SIZESELECTOR.SIZE4TOOLTIP),
                new FCycle.Option(WorldSizePresets.Enormous.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.SIZE5, ASTEROIDSIZEINFO.SIZESELECTOR.SIZE5TOOLTIP),
            };

            PlanetSizeCycle.OnChange += () =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                    {
                        WorldSizePresets setTo = Enum.TryParse<WorldSizePresets>(PlanetSizeCycle.Value, out var result) ? result : WorldSizePresets.Normal;
                        current.SetPlanetSizeToPreset(setTo);
                        UpdateSizeLabels(current);
                        //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                    }
                }
            };

            PlanetRazioCycle = transform.Find("AsteroidSizeCycle/RazioCycle").gameObject.AddOrGet<FCycle>();
            PlanetRazioCycle.Initialize(
                PlanetRazioCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            PlanetRazioCycle.Options = new List<FCycle.Option>()
            {
                //new FCycle.Option(WorldSizePresets.Tiny.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE0, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE0TOOLTIP),
                //new FCycle.Option(WorldSizePresets.Smaller.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE1, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE1TOOLTIP),
                //new FCycle.Option(WorldSizePresets.Small.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE2, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE2TOOLTIP),
                //new FCycle.Option(WorldSizePresets.SlightlySmaller.ToString(), ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE3, ASTEROIDSIZEINFO.SIZESELECTOR.NEGSIZE3TOOLTIP),
                
                new FCycle.Option(WorldRatioPresets.LotWider.ToString(), ASTEROIDSIZEINFO.RATIOSELECTOR.WIDE3, ASTEROIDSIZEINFO.RATIOSELECTOR.WIDE3TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Wider.ToString(), ASTEROIDSIZEINFO.RATIOSELECTOR.WIDE2, ASTEROIDSIZEINFO.RATIOSELECTOR.WIDE2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyWider.ToString(), ASTEROIDSIZEINFO.RATIOSELECTOR.WIDE1, ASTEROIDSIZEINFO.RATIOSELECTOR.WIDE1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Normal.ToString(), ASTEROIDSIZEINFO.RATIOSELECTOR.NORMAL, ASTEROIDSIZEINFO.RATIOSELECTOR.NORMALTOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyTaller.ToString(), ASTEROIDSIZEINFO.RATIOSELECTOR.HEIGHT1, ASTEROIDSIZEINFO.RATIOSELECTOR.HEIGHT1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Taller.ToString(), ASTEROIDSIZEINFO.RATIOSELECTOR.HEIGHT2, ASTEROIDSIZEINFO.RATIOSELECTOR.HEIGHT2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.LotTaller.ToString(), ASTEROIDSIZEINFO.RATIOSELECTOR.HEIGHT3, ASTEROIDSIZEINFO.RATIOSELECTOR.HEIGHT3TOOLTIP),
            };
            PlanetRazioCycle.Value = WorldRatioPresets.Normal.ToString();

            PlanetRazioCycle.OnChange += () =>
            {
                if (lastSelected != null)
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                    {
                        WorldRatioPresets setTo = Enum.TryParse<WorldRatioPresets>(PlanetRazioCycle.Value, out var result) ? result : WorldRatioPresets.Normal;
                        current.SetPlanetRatioToPreset(setTo);
                        UpdateSizeLabels(current);
                        //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                    }
                }
            };

            AsteroidTraits = transform.Find("AsteroidTraits").gameObject;
            ActiveTraitsContainer = transform.Find("AsteroidTraits/ListView/Content").gameObject;
            TraitPrefab = transform.Find("AsteroidTraits/ListView/Content/ListViewEntryPrefab").gameObject;

            AddTraitButton = transform.Find("AsteroidTraits/AddTraitButton").FindOrAddComponent<FButton>();

            AddTraitButton.OnClick += () =>
            {
                TraitSelectorScreen.InitializeView(lastSelected, () => UpdateUI());
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
            //SgtLogger.Assert("AsteroidSizeLabel", AsteroidSizeLabel);
            SgtLogger.Assert("AsteroidTraits", AsteroidTraits);
            SgtLogger.Assert("ActiveTraitsContainer", ActiveTraitsContainer);
            SgtLogger.Assert("TraitPrefab", TraitPrefab);

            InitializeTraitContainer();

            init = true;
        }

        void TryApplyingCoordinates(string msg, bool Height)
        {
            if (int.TryParse(msg, out var size))
            {
                if (CustomCluster.HasStarmapItem(lastSelected.id, out var current))
                {
                    if (size == (Height ? current.CustomPlanetDimensions.Y : current.CustomPlanetDimensions.X))
                        return;

                    current.ApplyCustomDimension(size, Height);
                    UpdateSizeLabels(current);
                    //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                }
            }

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
                var combined = UIUtils.ColorText(name.ToString(), kvp.Value.colorHex);

                string associatedIcon = kvp.Value.filePath.Substring(kvp.Value.filePath.LastIndexOf("/") + 1);

                var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.sprite = Assets.GetSprite(associatedIcon);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);

                if (kvp.Key.Contains(SpritePatch.randomTraitsTraitIcon))
                {
                    combined = UIUtils.RainbowColorText(name.ToString());
                }

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);
                //
                RemoveButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(lastSelected.id, out var item))
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
