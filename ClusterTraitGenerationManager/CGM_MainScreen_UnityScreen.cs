using System;
using System.Collections.Generic;
using System.Linq;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders;
using static STRINGS.UI.CODEX;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.FOOTER.BUTTONS;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;
using Microsoft.SqlServer.Server;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.FOOTER;
using Klei.AI;
using System.Threading.Tasks;
using static STRINGS.DUPLICANTS.PERSONALITIES;

namespace ClusterTraitGenerationManager
{
    public class CGM_MainScreen_UnityScreen : KModalScreen
    {
        ////GridLayouter galleryGridLayouter;
        //GridLayoutSizeAdjustment galleryGridLayouter;

        private Dictionary<StarmapItemCategory, CategoryItem> categoryToggles = new Dictionary<StarmapItemCategory, CategoryItem>();
        private Dictionary<StarmapItem, GalleryItem> planetoidGridButtons = new Dictionary<StarmapItem, GalleryItem>();

        GameObject PlanetoidEntryPrefab;
        private GameObject galleryGridContent;

        GameObject PlanetoidCategoryPrefab;
        public GameObject categoryListContent;

        private LocText galleryHeaderLabel;
        private LocText categoryHeaderLabel;
        private LocText selectionHeaderLabel;

        private bool init = false;

        private StarmapItem _selectedPlanet = null;// new StarmapItem("none", StarmapItemCategory.Starter,null);
        StarmapItem SelectedPlanet
        {
            get { return _selectedPlanet; }
            set
            {
                _selectedPlanet = value;
                this.RefreshView();
            }
        }


        StarmapItemCategory SelectedCategory = StarmapItemCategory.Starter;


        Dictionary<string, GameObject> SeasonTypes = new Dictionary<string, GameObject>();

        Dictionary<string, GameObject> ShowerTypes = new Dictionary<string, GameObject>();

        Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();


        private FSlider ClusterSize;


        private LocText StarmapItemEnabledText;
        private FToggle2 StarmapItemEnabled;
        private FSlider NumberToGenerate;
        private UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider MinMaxDistanceSlider;
        private LocText SpawnDistanceText;
        private FSlider BufferDistance;

        private GameObject AsteroidSize;
        private LocText AsteroidSizeLabel;
        private ToolTip AsteroidSizeTooltip;

        private FInputField2 PlanetSizeWidth;
        private FInputField2 PlanetSizeHeight;

        private FCycle PlanetSizeCycle;
        private FCycle PlanetRazioCycle;

        private GameObject MeteorSelector;
        private GameObject ActiveMeteorsContainer;
        private GameObject MeteorPrefab;
        private GameObject ActiveSeasonsContainer;
        private GameObject SeasonPrefab;
        private FButton AddSeasonButton;


        private GameObject AsteroidTraits;
        private GameObject ActiveTraitsContainer;
        private GameObject TraitPrefab;
        private FButton AddTraitButton;


        private static FButton ResetButton;
        private static FButton ResetAllButton;
        private FButton ReturnButton;
        private FButton PresetsButton;
        private FButton SettingsButton;
        private FButton GenerateClusterButton;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.canBackoutWithRightClick = true;
            base.ConsumeMouseScroll = true;
            this.SetHasFocus(true);


#if DEBUG
            //UIUtils.ListAllChildrenPath(this.transform);
#endif

            //Categories
            PlanetoidCategoryPrefab = transform.Find("Categories/Content/Item").gameObject;
            categoryListContent = transform.Find("Categories/Content").gameObject;
            categoryHeaderLabel = transform.Find("Categories/Header/Label").GetComponent<LocText>();

            //Gallery
            galleryGridContent = transform.Find("ItemSelection/Content/StarItemContainer").gameObject;
            PlanetoidEntryPrefab = transform.Find("ItemSelection/Content/StarItemContainer/Item").gameObject;
            galleryHeaderLabel = transform.Find("ItemSelection/Header/Label").GetComponent<LocText>();

            ///Details
            selectionHeaderLabel = transform.Find("Details/Header/Label").GetComponent<LocText>();

            //galleryGridLayouter = galleryGridContent.AddOrGet<GridLayoutSizeAdjustment>();
            //galleryGridLayouter.SetValues(100, 140);
            //galleryGridLayouter = new GridLayouter
            //{
            //    minCellSize = 80f,
            //    maxCellSize = 160f,
            //    targetGridLayouts = new List<GridLayoutGroup>() { galleryGridContent.GetComponent<GridLayoutGroup>() }
            //};

            OnResize();
        }

        public void DoAndRefreshView(System.Action action)
        {
            action.Invoke();
            this.RefreshGallery();
            this.RefreshDetails();
        }
        public override float GetSortKey() => 20f;

        public override void OnActivate() => this.OnShow(true);

        public static bool AllowedToClose() 
        {
            return (
                (TraitSelectorScreen.Instance != null ? !TraitSelectorScreen.Instance.IsCurrentlyActive : true)
                    && (SeasonSelectorScreen.Instance != null ? !SeasonSelectorScreen.Instance.IsCurrentlyActive : true)
                    && (CustomSettingsController.Instance != null ? !CustomSettingsController.Instance.IsCurrentlyActive : true)
                    );
        }

        bool overrideToWholeNumbers = false;
        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.Controller.GetKeyDown(KKeyCode.LeftShift))
            {
                overrideToWholeNumbers = !overrideToWholeNumbers;
                e.Consumed = true;
                UpdateForSelected();
            }

            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                if (AllowedToClose())
                    Show(show: false);
            }

            base.OnKeyDown(e);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }
        public override void OnShow(bool show)
        {
            //SgtLogger.l("SHOWING: " + show);
            //this.isActive = show;
            base.OnShow(show);
            if (!show)
                return;

            if(!init)
            {
                this.PopulateGalleryAndCategories();
                this.InitializeItemSettings();
                init = true;
            }


           //// this.galleryGridLayouter.RequestGridResize();

            OnResize();
            //RefreshWithDelay(() => OnResize(true),300);
            ScreenResize.Instance.OnResize += () => OnResize();
            RefreshView();
            DoWithDelay ( ()=> this.SelectCategory(StarmapItemCategory.Starter),25);
        }
        static async Task DoWithDelay(System.Action task, int ms)
        {
            await Task.Delay(ms);
            task.Invoke();
        }
        public void OnResize()
        {
            var rectMain = this.rectTransform();
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / (rectMain.lossyScale.x)));
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / (rectMain.lossyScale.y)));
            //if(galleryGridLayouter!=null)    
            //    this.galleryGridLayouter.RequestGridResize();
        }

        public void RefreshView()
        {
            this.RefreshCategories();
            this.RefreshGallery();
            this.RefreshDetails();
        }
        private void RefreshCategories()
        {
            foreach (var categoryToggle in this.categoryToggles)
            {
                Sprite PlanetSprite = null;
                switch (categoryToggle.Key)
                {
                    case StarmapItemCategory.Starter:
                        PlanetSprite = CustomCluster.StarterPlanet != null ? CustomCluster.StarterPlanet.planetSprite : Assets.GetSprite("unknown");
                        break;
                    case StarmapItemCategory.Warp:
                        PlanetSprite = CustomCluster.WarpPlanet != null ? CustomCluster.WarpPlanet.planetSprite : Assets.GetSprite("unknown");
                        break;
                    case StarmapItemCategory.Outer:
                        PlanetSprite = CustomCluster.OuterPlanets.Count > 0 ? CustomCluster.OuterPlanets.First().Value.planetSprite : Assets.GetSprite("unknown");
                        break;
                    case StarmapItemCategory.POI:
                        PlanetSprite = CustomCluster.POIs.Count > 0 ? CustomCluster.POIs.First().Value.planetSprite : Assets.GetSprite("unknown");
                        break;
                }
                categoryToggle.Value.Refresh(SelectedCategory, PlanetSprite);
            }
        }
        private void RefreshDetails()
        {
            if (SelectedPlanet == null)
                return;
            UpdateForSelected();
        }
        
        public void UpdateForSelected()
        {

            this.galleryHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(STRINGS.UI.CGM_MAINSCREENEXPORT.ITEMSELECTION.HEADER.LABEL, SelectedCategory));
            bool IsPartOfCluster = CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current);

            bool isRandom = current.id.Contains(CGSMClusterManager.RandomKey);
            bool canGenerateMultiple = current.MaxNumberOfInstances > 1;

            selectionHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL,SelectedPlanet.DisplayName), SelectedCategory));
            StarmapItemEnabledText.SetText(ModAssets.Strings.ApplyCategoryTypeToString(STARMAPITEMENABLED.LABEL, SelectedCategory));

            StarmapItemEnabled.SetOn(IsPartOfCluster);

            NumberToGenerate.transform.parent.gameObject.SetActive(canGenerateMultiple);///Amount, only on poi / random planets
            if (canGenerateMultiple)
            {
                NumberToGenerate.SetWholeNumbers(!current.IsPOI|| overrideToWholeNumbers);
                NumberToGenerate.SetMinMaxCurrent(0, current.MaxNumberOfInstances, current.InstancesToSpawn);
                NumberToGenerate.SetInteractable(IsPartOfCluster);
                current.SetSpawnNumber(NumberToGenerate.Value);
            }


            MinMaxDistanceSlider.SetLimits(0, CustomCluster.Rings);
            MinMaxDistanceSlider.SetValues(current.minRing, current.maxRing, 0, CustomCluster.Rings, true);
            MinMaxDistanceSlider.SetInteractable(IsPartOfCluster);
            SpawnDistanceText.SetText(string.Format(MINMAXDISTANCE.DESCRIPTOR.FORMAT, (int)current.minRing, (int)current.maxRing));

            BufferDistance.SetMinMaxCurrent(0, CustomCluster.Rings, SelectedPlanet.buffer);
            BufferDistance.transform.parent.gameObject.SetActive(!current.IsPOI);
            BufferDistance.SetInteractable(IsPartOfCluster);

            ClusterSize.SetMinMaxCurrent(ringMin, ringMax, CustomCluster.Rings);

            AddTraitButton.SetInteractable(IsPartOfCluster && !isRandom);
            AddSeasonButton.SetInteractable(IsPartOfCluster && !isRandom);

            AsteroidSize.SetActive(!current.IsPOI && !isRandom);
            MeteorSelector.SetActive(!current.IsPOI && !isRandom);
            AsteroidTraits.SetActive(!current.IsPOI && !isRandom);

            UpdateSizeLabels(current);
            PlanetSizeCycle.Value = current.CurrentSizePreset.ToString();
            PlanetRazioCycle.Value = current.CurrentRatioPreset.ToString();

            if (current.IsPOI) return;
            RefreshMeteorLists();
            RefreshTraitList();
        }

        public void SelectItem(StarmapItem planet)
        {
            if(planet!= SelectedPlanet)
            {
                overrideToWholeNumbers = false;
            }

            SelectedPlanet = planet;
            this.RefreshView();
        }

        private void RefreshGallery()
        {
            //SgtLogger.warning(planetoidGridButtons.Count.ToString(),"CUND");
            var activePlanets = CGSMClusterManager.GetActivePlanetsStarmapitems();

            foreach (var galleryGridButton in planetoidGridButtons)
            {
                var logicComponent = galleryGridButton.Value;

                bool PlanetIsInList = activePlanets.Contains(galleryGridButton.Key);
                bool selected = SelectedPlanet == null ? false : SelectedPlanet == galleryGridButton.Key;


                logicComponent.Refresh(galleryGridButton.Key, PlanetIsInList, selected);
                galleryGridButton.Value.gameObject.SetActive(galleryGridButton.Key.category == this.SelectedCategory);
            }
            
            this.galleryHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(STRINGS.UI.CGM_MAINSCREENEXPORT.ITEMSELECTION.HEADER.LABEL, SelectedCategory));
        }


        public void SelectCategory(StarmapItemCategory category)
        {
            this.SelectedCategory = category;
            //this.categoryHeaderLabel.SetText(STRINGS.UI.CUSTOMCLUSTERUI.NAMECATEGORIES);
            this.SelectDefaultCategoryItem();
            this.RefreshView();
        }
        private void SelectDefaultCategoryItem()
        {
            foreach (var galleryGridButton in this.planetoidGridButtons)
            {
                if (galleryGridButton.Key.category == this.SelectedCategory && CustomCluster.HasStarmapItem(galleryGridButton.Key.id, out var i))
                {
                    this.SelectItem(galleryGridButton.Key);
                    return;
                }
            }
            foreach (var galleryGridButton in this.planetoidGridButtons)
            {
                if (galleryGridButton.Key.category == this.SelectedCategory)
                {
                    this.SelectItem(galleryGridButton.Key);
                    return;
                }
            }
            this.SelectItem(null);
        }

        #region initialisation

        public void InitializeItemSettings()
        {

            MinMaxDistanceSlider = transform.Find("Details/Content/ScrollRectContainer/MinMaxDistance/Slider").FindOrAddComponent<UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider>();
            MinMaxDistanceSlider.SliderBounds = MinMaxDistanceSlider.transform.Find("Handle Slide Area").rectTransform();
            MinMaxDistanceSlider.MinHandle = MinMaxDistanceSlider.transform.Find("Handle Slide Area/HandleMin").rectTransform();
            MinMaxDistanceSlider.MaxHandle = MinMaxDistanceSlider.transform.Find("Handle Slide Area/Handle").rectTransform();
            MinMaxDistanceSlider.MiddleGraphic = MinMaxDistanceSlider.transform.Find("Fill Area/Fill").rectTransform();
            MinMaxDistanceSlider.wholeNumbers = true;
            MinMaxDistanceSlider.onValueChanged.AddListener(
                (min, max)=>
                {
                    if (SelectedPlanet!=null && CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                    {
                        current.SetInnerRing(Mathf.RoundToInt(min));
                        current.SetOuterRing(Mathf.RoundToInt(max));
                        SpawnDistanceText.SetText(string.Format(MINMAXDISTANCE.DESCRIPTOR.FORMAT, (int)min, (int)max));
                    }          
                }
                );

            //MinMaxDistanceSlider.SetLimits(0, CustomCluster.Rings);
            //MinMaxDistanceSlider.SetValues(0, 0.001f, 0, CustomCluster.Rings, true);

            SpawnDistanceText = MinMaxDistanceSlider.transform.parent.Find("Descriptor/Output").GetComponent<LocText>();
            UIUtils.AddSimpleTooltipToObject(MinMaxDistanceSlider.transform.parent.Find("Descriptor"), (MINMAXDISTANCE.DESCRIPTOR.TOOLTIP), onBottom: true, alignCenter: true);

            StarmapItemEnabledText = transform.Find("Details/Content/ScrollRectContainer/StarmapItemEnabled/Label").GetComponent<LocText>();
            StarmapItemEnabled = transform.Find("Details/Content/ScrollRectContainer/StarmapItemEnabled").FindOrAddComponent<FToggle2>();
            StarmapItemEnabled.SetCheckmark("Background/Checkmark");
            StarmapItemEnabled.OnClick += () =>
            {
                if (SelectedPlanet != null)
                {
                    CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
                    this.RefreshCategories();
                    this.RefreshGallery();
                    this.RefreshDetails();
                }
            };
            UIUtils.AddSimpleTooltipToObject(StarmapItemEnabled.transform, STARMAPITEMENABLED.TOOLTIP, onBottom: true, alignCenter: true);


            NumberToGenerate = transform.Find("Details/Content/ScrollRectContainer/AmountSlider/Slider").FindOrAddComponent<FSlider>();

            NumberToGenerate.SetWholeNumbers(true);
            NumberToGenerate.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/AmountSlider/Descriptor/Output").GetComponent<LocText>());
            NumberToGenerate.OnChange += (value) =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                        current.SetSpawnNumber(value);
                    this.RefreshGallery();
                }
            };

            UIUtils.AddSimpleTooltipToObject(NumberToGenerate.transform.parent.Find("Descriptor"), (AMOUNTSLIDER.DESCRIPTOR.TOOLTIP), onBottom: true, alignCenter: true);

            BufferDistance = transform.Find("Details/Content/ScrollRectContainer/BufferSlider/Slider").FindOrAddComponent<FSlider>();
            BufferDistance.SetWholeNumbers(true);
            BufferDistance.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/BufferSlider/Descriptor/Output").GetComponent<LocText>());
            BufferDistance.OnChange += (value) =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                        current.SetBuffer((int)value);
                }
            };
            UIUtils.AddSimpleTooltipToObject(BufferDistance.transform.parent.Find("Descriptor"), BUFFERSLIDER.DESCRIPTOR.TOOLTIP);

            ClusterSize = transform.Find("Details/Footer/ClusterSizeSlider/Slider").FindOrAddComponent<FSlider>();
            ClusterSize.SetWholeNumbers(true);
            ClusterSize.AttachOutputField(transform.Find("Details/Footer/ClusterSizeSlider/Descriptor/Output").GetComponent<LocText>());
            ClusterSize.OnChange += (value) =>
            {
                CustomCluster.SetRings((int)value);
                this.RefreshGallery();
                this.RefreshDetails();
            };
            UIUtils.AddSimpleTooltipToObject(ClusterSize.transform.parent.Find("Descriptor"), CLUSTERSIZESLIDER.DESCRIPTOR.TOOLTIP);


            AsteroidSize = transform.Find("Details/Content/ScrollRectContainer/AsteroidSize").gameObject;
            AsteroidSizeLabel = AsteroidSize.transform.Find("Descriptor/Label").GetComponent<LocText>();
            AsteroidSizeTooltip = UIUtils.AddSimpleTooltipToObject(AsteroidSizeLabel.transform.parent, ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);

            PlanetSizeWidth = AsteroidSize.transform.Find("Content/Info/WidthLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeWidth.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, false));

            PlanetSizeHeight = AsteroidSize.transform.Find("Content/Info/HeightLabel/Input").FindOrAddComponent<FInputField2>();
            PlanetSizeHeight.inputField.onEndEdit.AddListener((string sizestring) => TryApplyingCoordinates(sizestring, true));

            PlanetSizeCycle = AsteroidSize.transform.Find("Content/Cycles/SizeCycle").gameObject.AddOrGet<FCycle>();
            PlanetSizeCycle.Initialize(
                PlanetSizeCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetSizeCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            PlanetSizeCycle.Options = new List<FCycle.Option>()
            {
                new FCycle.Option(WorldSizePresets.Tiny.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0TOOLTIP),
                new FCycle.Option(WorldSizePresets.Smaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1TOOLTIP),
                new FCycle.Option(WorldSizePresets.Small.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2TOOLTIP),
                new FCycle.Option(WorldSizePresets.SlightlySmaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3TOOLTIP),

                new FCycle.Option(WorldSizePresets.Normal.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE0, ASTEROIDSIZE.SIZESELECTOR.SIZE0TOOLTIP),
                new FCycle.Option(WorldSizePresets.SlightlyLarger.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE1, ASTEROIDSIZE.SIZESELECTOR.SIZE1TOOLTIP),
                new FCycle.Option(WorldSizePresets.Large.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE2, ASTEROIDSIZE.SIZESELECTOR.SIZE2TOOLTIP),
                new FCycle.Option(WorldSizePresets.Huge.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE3, ASTEROIDSIZE.SIZESELECTOR.SIZE3TOOLTIP),
                new FCycle.Option(WorldSizePresets.Massive.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE4, ASTEROIDSIZE.SIZESELECTOR.SIZE4TOOLTIP),
                new FCycle.Option(WorldSizePresets.Enormous.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE5, ASTEROIDSIZE.SIZESELECTOR.SIZE5TOOLTIP),
            };

            PlanetSizeCycle.OnChange += () =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                    {
                        WorldSizePresets setTo = Enum.TryParse<WorldSizePresets>(PlanetSizeCycle.Value, out var result) ? result : WorldSizePresets.Normal;
                        current.SetPlanetSizeToPreset(setTo);
                        UpdateSizeLabels(current);
                    }
                }
            };

            PlanetRazioCycle = AsteroidSize.transform.Find("Content/Cycles/RazioCycle").gameObject.AddOrGet<FCycle>();
            PlanetRazioCycle.Initialize(
                PlanetRazioCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                PlanetRazioCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            PlanetRazioCycle.Options = new List<FCycle.Option>()
            {
                new FCycle.Option(WorldRatioPresets.LotWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE3, ASTEROIDSIZE.RATIOSELECTOR.WIDE3TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Wider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE2, ASTEROIDSIZE.RATIOSELECTOR.WIDE2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE1, ASTEROIDSIZE.RATIOSELECTOR.WIDE1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Normal.ToString(), ASTEROIDSIZE.RATIOSELECTOR.NORMAL, ASTEROIDSIZE.RATIOSELECTOR.NORMALTOOLTIP),
                new FCycle.Option(WorldRatioPresets.SlightlyTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1TOOLTIP),
                new FCycle.Option(WorldRatioPresets.Taller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2TOOLTIP),
                new FCycle.Option(WorldRatioPresets.LotTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3TOOLTIP),
            };
            PlanetRazioCycle.Value = WorldRatioPresets.Normal.ToString();

            PlanetRazioCycle.OnChange += () =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                    {
                        WorldRatioPresets setTo = Enum.TryParse<WorldRatioPresets>(PlanetRazioCycle.Value, out var result) ? result : WorldRatioPresets.Normal;
                        current.SetPlanetRatioToPreset(setTo);
                        UpdateSizeLabels(current);
                        //AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
                    }
                }
            };

            MeteorSelector = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle").gameObject;
            ActiveMeteorsContainer = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Asteroids/ScrollArea/Content").gameObject;
            MeteorPrefab = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Asteroids/ScrollArea/Content/ListViewEntryPrefab").gameObject;

            ActiveSeasonsContainer = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content").gameObject;
            SeasonPrefab = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content/ListViewEntryPrefab").gameObject;

            AddSeasonButton = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content/AddSeasonButton").FindOrAddComponent<FButton>();
            UIUtils.AddSimpleTooltipToObject(AddSeasonButton.transform, METEORSEASONCYCLE.DESCRIPTOR.TOOLTIP);

            AddSeasonButton.OnClick += () =>
            {
                SeasonSelectorScreen.InitializeView(SelectedPlanet, () => RefreshMeteorLists());
            };

            UIUtils.AddSimpleTooltipToObject(MeteorSelector.transform.Find("Descriptor"), METEORSEASONCYCLE.DESCRIPTOR.TOOLTIP);


            AsteroidTraits = transform.Find("Details/Content/ScrollRectContainer/AsteroidTraits").gameObject;
            ActiveTraitsContainer = AsteroidTraits.transform.Find("Content/TraitContainer/ScrollArea/Content").gameObject;
            TraitPrefab = AsteroidTraits.transform.Find("Content/TraitContainer/ScrollArea/Content/ListViewEntryPrefab").gameObject;

            AddTraitButton = AsteroidTraits.transform.Find("Content/AddSeasonButton").FindOrAddComponent<FButton>();

            AddTraitButton.OnClick += () =>
            {
                TraitSelectorScreen.InitializeView(SelectedPlanet, () => RefreshTraitList());
            };

            var buttons = transform.Find("Details/Footer/Buttons");

            ReturnButton = buttons.Find("ReturnButton").FindOrAddComponent<FButton>();
            ReturnButton.OnClick += ()=>Show(false);

            GenerateClusterButton = buttons.Find("GenerateClusterButton").FindOrAddComponent<FButton>();
            GenerateClusterButton.OnClick += () => CGSMClusterManager.InitializeGeneration();

            ResetButton = buttons.Find("ResetSelectionButton").FindOrAddComponent<FButton>();
            ResetButton.OnClick += () =>
            {
                CGSMClusterManager.ResetPlanetFromPreset(SelectedPlanet.id);
                RefreshView();
            };

            ResetAllButton = buttons.Find("ResetClusterButton").FindOrAddComponent<FButton>();
            ResetAllButton.OnClick += () =>
            {
                CGSMClusterManager.ResetToLastPreset();
                RefreshView();
            };

            PresetsButton = buttons.Find("PresetButton").FindOrAddComponent<FButton>();
            PresetsButton.OnClick += () =>
            {
                CGSMClusterManager.OpenPresetWindow(() => RefreshView());
            };

            SettingsButton = buttons.Find("SettingsButton").FindOrAddComponent<FButton>();
            SettingsButton.OnClick += () =>
            {
                CustomSettingsController.ShowWindow(() => RefreshView());
            };

            UIUtils.AddSimpleTooltipToObject(ResetAllButton.transform, RESETCLUSTERBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(ResetButton.transform, RESETSELECTIONBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(GenerateClusterButton.transform, GENERATECLUSTERBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(ReturnButton.transform, RETURNBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(SettingsButton.transform, SETTINGSBUTTON.TOOLTIP, true, onBottom: true);
            UIUtils.AddSimpleTooltipToObject(PresetsButton.transform, PRESETBUTTON.TOOLTIP, true, onBottom: true);

            InitializeTraitContainer();
            InitializeMeteorShowerContainers();
        }


        void InitializeMeteorShowerContainers()
        {
            ///SeasonContainer
            foreach (var gameplaySeason in Db.Get().GameplaySeasons.resources)
            {
                if (!(gameplaySeason is MeteorShowerSeason) || gameplaySeason.Id.Contains("Fullerene") || gameplaySeason.Id.Contains("TemporalTear") || gameplaySeason.dlcId != DlcManager.EXPANSION1_ID)
                    continue;
                var meteorSeason = gameplaySeason as MeteorShowerSeason;

                var seasonInstanceHolder = Util.KInstantiateUI(SeasonPrefab, ActiveSeasonsContainer, true);


                string name = meteorSeason.Name.Replace("MeteorShowers", string.Empty);
                
                string description = meteorSeason.events.Count == 0 ? METEORSEASONCYCLE.CONTENT.SEASONTYPENOMETEORSTOOLTIP : METEORSEASONCYCLE.CONTENT.SEASONTYPETOOLTIP;

                foreach (var meteorShower in meteorSeason.events)
                {
                    description += "\n • ";
                    description += Assets.GetPrefab((meteorShower as MeteorShowerEvent).clusterMapMeteorShowerID).GetProperName();// Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
                }
                UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform, description);
                var LocTextName = seasonInstanceHolder.transform.Find("Label").GetComponent<LocText>();
                LocTextName.fontSizeMax = LocTextName.fontSize;
                LocTextName.fontSizeMin = LocTextName.fontSize - 7f;
                LocTextName.enableAutoSizing = true;
                UIUtils.TryChangeText(seasonInstanceHolder.transform, "Label", name);
                UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform.Find("Label"), description);


                var RemoveButton = seasonInstanceHolder.transform.Find("DeleteButton").gameObject.FindOrAddComponent<FButton>();
                var SwitchButton = seasonInstanceHolder.transform.Find("SwitchButton").gameObject.FindOrAddComponent<FButton>();
                UIUtils.AddSimpleTooltipToObject(SwitchButton.transform, METEORSEASONCYCLE.SWITCHTOOTHERSEASONTOOLTIP);
                UIUtils.AddSimpleTooltipToObject(RemoveButton.transform, METEORSEASONCYCLE.REMOVESEASONTOOLTIP);


                RemoveButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        item.RemoveMeteorSeason(meteorSeason.Id); //SeasonSelectorScreen.InitializeView(lastSelected, () => UpdateUI());
                    }
                    RefreshMeteorLists();
                };
                SwitchButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        SeasonSelectorScreen.InitializeView(SelectedPlanet, () => RefreshMeteorLists(), meteorSeason.Id);
                    }
                };
                SeasonTypes[gameplaySeason.Id] = seasonInstanceHolder;
            }

            ///Shower Container
            foreach (var gameplayEvent in Db.Get().GameplayEvents.resources)
            {
                if (!(gameplayEvent is MeteorShowerEvent) || gameplayEvent.Id.Contains("Fullerene"))
                    continue;
                var meteorEvent = gameplayEvent as MeteorShowerEvent;
                string ClusterEventID = meteorEvent.clusterMapMeteorShowerID;

                ///for those pesky vanilla meteors without starmap entity
                if (ClusterEventID == null || ClusterEventID == string.Empty)
                {
                    continue;
                }

                var ClusterMapShower = Assets.GetPrefab(ClusterEventID);
                var showerInstanceHolder = Util.KInstantiateUI(MeteorPrefab, ActiveMeteorsContainer, true);


                string name = ClusterMapShower.GetProperName();
                string description = METEORSEASONCYCLE.SHOWERTOOLTIP;
                var icon = showerInstanceHolder.transform.Find("TraitImage").GetComponent<Image>();
                icon.sprite = Def.GetUISprite(Assets.GetPrefab(ClusterEventID)).first;

                var meteortypes = meteorEvent.GetMeteorsInfo();
                foreach (var meteor in meteortypes)
                {
                    description += "\n • ";
                    description += Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
                }
                UIUtils.TryChangeText(showerInstanceHolder.transform, "Label", name);
                UIUtils.AddSimpleTooltipToObject(showerInstanceHolder.transform, description);


                ShowerTypes[gameplayEvent.Id] = showerInstanceHolder;
            }
            RefreshMeteorLists();
        }
        void InitializeTraitContainer()
        {
            foreach (var kvp in ModAssets.AllTraitsWithRandom)
            {
                var TraitHolder = Util.KInstantiateUI(TraitPrefab, ActiveTraitsContainer, true);
                //UIUtils.ListAllChildrenWithComponents(TraitHolder.transform);
                var RemoveButton = TraitHolder.transform.Find("DeleteButton").gameObject.FindOrAddComponent<FButton>();
                Strings.TryGet(kvp.Value.name, out var name);
                Strings.TryGet(kvp.Value.description, out var description);

                var combined = UIUtils.ColorText(name.ToString(), kvp.Value.colorHex);

                string associatedIcon = kvp.Value.filePath.Substring(kvp.Value.filePath.LastIndexOf("/") + 1);

                var icon = TraitHolder.transform.Find("TraitImage").GetComponent<Image>();

                icon.sprite = Assets.GetSprite(associatedIcon);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);

                if (kvp.Key.Contains(SpritePatch.randomTraitsTraitIcon))
                {
                    combined = UIUtils.RainbowColorText(name.ToString());
                }

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);
                
                RemoveButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        item.RemoveWorldTrait(kvp.Value);
                    }
                    RefreshTraitList();
                };
                Traits[kvp.Value.filePath] = TraitHolder;
            }
            RefreshTraitList();
        }

        public void RefreshMeteorLists()
        {
            if (SelectedPlanet == null)
                return;
            foreach (var showerHolder in ShowerTypes.Values)
            {
                showerHolder.SetActive(false);
            }
            foreach (var activeShower in SelectedPlanet.CurrentMeteorShowerTypes)
            {
                if (ShowerTypes.ContainsKey(activeShower.Id))
                    ShowerTypes[activeShower.Id].SetActive(true);
            }

            foreach (var seasonHolder in SeasonTypes.Values)
            {
                seasonHolder.SetActive(false);
            }
            foreach (var activeSeason in SelectedPlanet.CurrentMeteorSeasons)
            {
                if (SeasonTypes.ContainsKey(activeSeason.Id))
                    SeasonTypes[activeSeason.Id].SetActive(true);
            }
        }
        public void RefreshTraitList()
        {
            if (SelectedPlanet == null)
                return;

            foreach (var traitContainer in Traits.Values)
            {
                traitContainer.SetActive(false);
            }
            foreach (var activeTrait in SelectedPlanet.CurrentTraits)
            {
                Traits[activeTrait].SetActive(true);
            }
        }
        public static void SetResetButtonStates()
        {
            if (ResetButton != null)
                ResetButton.SetInteractable(!PresetApplied);
            if (ResetAllButton != null)
                ResetAllButton.SetInteractable(!PresetApplied);
        }

        private static bool _presetApplied = false;
        public static bool PresetApplied
        {
            get
            {
                return _presetApplied;
            }
            set
            {
                _presetApplied = value;
                SetResetButtonStates();
            }
        }

        void TryApplyingCoordinates(string msg, bool Height)
        {
            if (int.TryParse(msg, out var size))
            {
                if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                {
                    if (size == (Height ? current.CustomPlanetDimensions.Y : current.CustomPlanetDimensions.X))
                        return;

                    current.ApplyCustomDimension(size, Height);
                    UpdateSizeLabels(current);
                }
            }
        }


        string Warning3 = "EC1802";
        string Warning2 = "ff8102";
        string Warning1 = "F6D42A";

        public void UpdateSizeLabels(StarmapItem current)
        {
            PlanetSizeWidth.EditTextFromData(current.CustomPlanetDimensions.X.ToString());
            PlanetSizeHeight.EditTextFromData(current.CustomPlanetDimensions.Y.ToString());
            PercentageLargerThanTerra(current, out var Percentage);
            if (Percentage > 200)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning3);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning3));
            }
            else if (Percentage > 100)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning2);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning2));
            }
            else if (Percentage > 33)
            {
                AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning1);
                AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning1));
            }
            else
            {
                AsteroidSizeLabel.text = ASTEROIDSIZE.DESCRIPTOR.LABEL;
                AsteroidSizeTooltip.SetSimpleTooltip(ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);
            }
        }
        void PercentageLargerThanTerra(StarmapItem current, out int dimensions)
        {
            float TerraArea = 240 * 380;
            float CustomSize = current.CustomPlanetDimensions.X * current.CustomPlanetDimensions.Y;

            dimensions = Mathf.RoundToInt((CustomSize / TerraArea) * 100f);
            dimensions -= 100;
        }


        public void PopulateGalleryAndCategories()
        {
            foreach (var galleryGridButton in this.planetoidGridButtons)
                UnityEngine.Object.Destroy(galleryGridButton.Value.gameObject);
            planetoidGridButtons.Clear();

            foreach (var item in this.categoryToggles)
                UnityEngine.Object.Destroy(item.Value.gameObject);

            categoryToggles.Clear();


            foreach (var Planet in PlanetoidDict())
            {
                this.AddItemToGallery(Planet.Value);
            }
            foreach (StarmapItemCategory category in (StarmapItemCategory[])Enum.GetValues(typeof(StarmapItemCategory)))
            {
                AddCategoryItem(category);
            };
            //if (galleryGridLayouter != null)
            //    this.galleryGridLayouter.RequestGridResize();
        }

        public class GalleryItem : KMonoBehaviour
        {
            public LocText ItemNumber;
            public FToggleButton ActiveToggle;
            public GameObject DisabledOverlay;
           
            public void Initialize(StarmapItem planet)
            {
                
                Image itemIconImage = transform.Find("Image").GetComponent<Image>();
                ItemNumber = transform.Find("AmountLabel").GetComponent<LocText>();
                DisabledOverlay = transform.Find("DisabledOverlay").gameObject;
                ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
                itemIconImage.sprite = planet.planetSprite;

                UnityEngine.Rect rect = itemIconImage.sprite.rect;
                if (rect.width > rect.height)
                {
                    var size = (rect.height / rect.width) * 80f;
                    itemIconImage.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (5 + (80 - size) / 2), size);
                }
                else
                {
                    var size = (rect.width / rect.height) * 80f;
                    itemIconImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }



                UIUtils.AddSimpleTooltipToObject(this.transform, planet.DisplayName + "\n\n" + planet.DisplayDescription, true, 300, true);
                Refresh(planet,true);
            }
            public void Refresh(StarmapItem planet, bool inCluster, bool currentlySelected = false)
            {
                float number = planet.InstancesToSpawn;
                bool planetActive = inCluster;//CGSMClusterManager.CustomCluster.HasStarmapItem(planet.Id)

                ActiveToggle.ChangeSelection(currentlySelected);
                DisabledOverlay.SetActive(!planetActive);
                ItemNumber.gameObject.SetActive(planetActive);
                if(planetActive)
                    ItemNumber.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", number.ToString("0.0"));
            }
        }

        private void OnMouseOverToggle() => KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover"));

        public void AddItemToGallery(StarmapItem planet)
        {
            if (planetoidGridButtons.ContainsKey(planet))
            {
                SgtLogger.warning(planet.id + " was already in the gallery");
                return;
            }

            // PermitPresentationInfo presentationInfo = permit.GetPermitPresentationInfo();
            GameObject availableGridButton = Util.KInstantiateUI(PlanetoidEntryPrefab, galleryGridContent);
            var itemLogic = availableGridButton.AddComponent<GalleryItem>();
            itemLogic.Initialize(planet);


            LocText itemNameText = availableGridButton.transform.Find("Label").GetComponent<LocText>();
            itemNameText.SetText(planet.DisplayName);
            UIUtils.TryChangeText(availableGridButton.transform, "Label", planet.DisplayName);


            itemLogic.ActiveToggle.OnClick += () => this.SelectItem(planet);
            itemLogic.ActiveToggle.OnDoubleClick += () =>
            {
                this.SelectItem(planet);
                if (SelectedPlanet != null)
                {
                    CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
                    RefreshView();
                }
            };


            planetoidGridButtons[planet] = itemLogic;
            //this.SetItemClickUISound(planet, component2);
            availableGridButton.SetActive(true);
        }
        public class CategoryItem : KMonoBehaviour
        {
            public Image CategoryIcon;
            public FToggleButton ActiveToggle;
            public StarmapItemCategory Category;

            public void Initialize(StarmapItemCategory category, Sprite newSprite)
            {
                CategoryIcon = transform.Find("Image").GetComponent<Image>();
                Category = category;
                ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
                Refresh(StarmapItemCategory.Starter, newSprite);
            }
            public void Refresh(StarmapItemCategory category,Sprite newSprite)
            {
                ActiveToggle.ChangeSelection(this.Category == category);
                CategoryIcon.sprite = newSprite;
            }
        }

        private void AddCategoryItem(StarmapItemCategory StarmapItemCategory)
        {
            GameObject categoryItem = Util.KInstantiateUI(this.PlanetoidCategoryPrefab, this.categoryListContent, true);
            
            string categoryName = ModAssets.Strings.CategoryEnumToName(StarmapItemCategory); //CATEGORYENUM

           

            categoryItem.transform.Find("Label").GetComponent<LocText>().SetText(categoryName);
            var item = categoryItem.AddOrGet<CategoryItem>();
            item.Initialize(StarmapItemCategory, Assets.GetSprite("unknown"));
            item.ActiveToggle.OnClick += (() => this.SelectCategory(StarmapItemCategory));
            this.categoryToggles.Add(StarmapItemCategory, item);
        }


        #endregion

    }
}
