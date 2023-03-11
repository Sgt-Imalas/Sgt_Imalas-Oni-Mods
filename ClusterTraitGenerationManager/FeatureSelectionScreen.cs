using ClusterTraitGenerationManager.SettinPrefabComps;
using ClusterTraitGenerationManager.TemplateComponents;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static SandboxSettings;

namespace ClusterTraitGenerationManager
{
    internal class FeatureSelectionScreen : KModalScreen
    {
        KButton closeButton;
        GridLayouter galleryGridLayouter;

        GameObject PlanetoidCategoryPrefab;
        GameObject PlanetoidEntryPrefab;

        private Dictionary<StarmapItemCategory, MultiToggle> categoryToggles = new Dictionary<StarmapItemCategory, MultiToggle>();
        Dictionary<StarmapItem, MultiToggle> planetoidGridButtons = new Dictionary<StarmapItem, MultiToggle>();

        GameObject RandomPlanetoidEntryPrefab;

        public RectTransform galleryGridContent;
        public RectTransform categoryListContent;

        private LocText galleryHeaderLabel;



        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.canBackoutWithRightClick = true;
            this.SetHasFocus(true);
            if (LockerNavigator.Instance.kleiInventoryScreen.TryGetComponent<KleiInventoryScreen>(out var cmp))
            {
                PlanetoidCategoryPrefab = cmp.categoryRowPrefab;
                PlanetoidEntryPrefab = cmp.gridItemPrefab;
            }
            //UIUtils.ListAllChildrenPath(this.transform);

            galleryGridContent = transform.Find("Panel/Content/ColumnItemGallery/LayoutBreaker/Content/Categories/ScrollRect/GridContent").rectTransform();
            categoryListContent = transform.Find("Panel/Content/ColumnCategorySelection/LayoutBreaker/Content/Categories/ScrollRect/ContentContainer/Content").rectTransform();
            galleryHeaderLabel = transform.Find("Panel/Content/ColumnItemGallery/LayoutBreaker/Header/Label").GetComponent<LocText>();

            foreach (Transform child in galleryGridContent.transform)
            {
                //SgtLogger.log("ToDelete1: " + child.ToString());
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child2 in categoryListContent.transform)
            {
                //SgtLogger.log("ToDelet2e: "+child2.ToString());
                GameObject.Destroy(child2.gameObject);
            }

            ///Details
            selectionHeaderLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Header/Label").GetComponent<LocText>();
            UIUtils.FindAndDestroy(transform, "Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/KleiPermitDioramaVis");
            UIUtils.FindAndDestroy(transform, "Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection");



            //selectionNameLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/NameLabel").GetComponent<LocText>();
            //selectionDescriptionLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/DescriptionLabel").GetComponent<LocText>();
            //selectionFacadeForLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/FacadeForLabel").GetComponent<LocText>();
            //selectionRarityDetailsLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/RarityDetailsLabel").GetComponent<LocText>();
            //selectionOwnedCount = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/OwnedCountLabel").GetComponent<LocText>();


            //var info = selectScreen.transform.Find("Layout/DestinationInfo/Content/InfoColumn/Horiz/Section - SelectedAsteroidDetails"); 
            var SliderPrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_SliderSetting");
            var CyclePrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_CycleSetting");
            var SeedPrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_SeedSetting");
            var CheckboxPrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_Checkbox");

            var infoInsert = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content");

            //var PerPlanetSettings = Util.KInstantiateUI(infoInsert.gameObject, infoInsert.gameObject, true);
            //var NormalSettings = Util.KInstantiateUI(infoInsert.gameObject, infoInsert.gameObject, true);


            SgtLogger.l("CYCLEPREFAB");
            UIUtils.ListAllChildrenWithComponents(CyclePrefab);
            SgtLogger.l("SLIDER");
            UIUtils.ListAllChildrenWithComponents(SliderPrefab);
            SgtLogger.l("SEED");
            UIUtils.ListAllChildrenWithComponents(SeedPrefab);
            SgtLogger.l("CHECK");
            UIUtils.ListAllChildrenWithComponents(CheckboxPrefab);

            //var Slider = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            //var Seed = Util.KInstantiateUI(SeedPrefab.gameObject, infoInsert.gameObject, true);



            #region individualConfig
            ///PlanetEnabledCheckbox, ListItem 0
            var Check = Util.KInstantiateUI(CheckboxPrefab.gameObject, infoInsert.gameObject, true);
            UIUtils.TryChangeText(Check.transform, "Label", "Enabled");
            var PlanetEnabled = Check.AddComponent<CheckBoxHandler>();
            PlanetEnabled.SetAction(() => DoAndRefreshView(
                () =>
                {
                    CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
                    this.RefreshDetails();
                }
                )
            );
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(Check, PlanetEnabled));

            ///Sliders for inner and outer ring

            var planetMinRing = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var planetMinRingHandler = planetMinRing.AddComponent<SliderHandler>();
            planetMinRingHandler.SetupSlider(0, SelectedPlanet.minRing, CustomCluster.Rings, true, "Minimum Ring: ", (value) => { SelectedPlanet.SetInnerRing((int)value); this.RefreshDetails(); }, true);
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(planetMinRing, planetMinRingHandler));

            var planetMaxRing = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var planetMaxRingHandler = planetMaxRing.AddComponent<SliderHandler>();
            planetMaxRingHandler.SetupSlider(0, SelectedPlanet.maxRing, CustomCluster.Rings, true, "Maximum Ring: ", (value) => { SelectedPlanet.SetOuterRing((int)value); this.RefreshDetails(); }, true);
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(planetMaxRing, planetMaxRingHandler));

            ///Slider for buffer
            ///
            var planetBuffer = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var planetBufferHandler = planetBuffer.AddComponent<SliderHandler>();
            planetBufferHandler.SetupSlider(0, SelectedPlanet.buffer, CustomCluster.Rings, true, "Buffer Distance: ", (value) => { SelectedPlanet.SetBuffer((int)value); this.RefreshDetails(); }, true);
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(planetBuffer, planetBufferHandler));


            #endregion

            #region globalClusterConfig
            ///Global Rings

            var GlobalRingSlider = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var globalRingHandler = GlobalRingSlider.AddComponent<SliderHandler>();
            globalRingHandler.SetupSlider(ringMin, CustomCluster.Rings, ringMax, true, "Map Size: ", (value) => { CustomCluster.SetRings((int)value); this.RefreshDetails(); });

            GlobalClusterSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(GlobalRingSlider, globalRingHandler));



            var Buttons = Util.KInstantiateUI(selectScreen.transform.Find("Layout/Buttons").gameObject, infoInsert.gameObject, true);

            UIUtils.ListAllChildren(Buttons.transform);

            UIUtils.AddActionToButton(Buttons.transform, "BackButton", () => Show(false));
            UIUtils.AddActionToButton(Buttons.transform, "LaunchButton",
                () =>
                {
                    AddCustomCluster();
                    CGSMClusterManager.selectScreen.LaunchClicked();
                }
            );
            ///Global Config
            SettingsButtonText = UIUtils.TryFindComponent<LocText>(Buttons.transform, "CustomizeButton/Label");
            UIUtils.AddActionToButton(Buttons.transform, "CustomizeButton", () => ToggleGameSettings());
            ToggleGameSettings();


            #endregion

            base.ConsumeMouseScroll = true;
            galleryGridLayouter = new GridLayouter
            {
                minCellSize = 64f,
                maxCellSize = 96f,
                targetGridLayout = galleryGridContent.GetComponent<GridLayoutGroup>()
            };
            UIUtils.FindAndDestroy(infoInsert, "KleiPermitDioramaVis");
            //UIUtils.ListAllChildren(infoInsert);
        }
        bool showGameSettings = true;

        LocText SettingsButtonText = null;
        void ToggleGameSettings()
        {
            showGameSettings = !showGameSettings;
            foreach (var setting in GlobalClusterSettings)
            {
                setting.Key.SetActive(showGameSettings);
            }
            foreach (var planetConfig in customPlanetoidSettings)
            {
                planetConfig.Key.SetActive(!showGameSettings);
            }
            SettingsButtonText.text = showGameSettings ? "Hide Cluster Config" : "Show Cluster Config";
        }

        List<KeyValuePair<GameObject, ICustomPlanetoidSetting>> GlobalClusterSettings = new List<KeyValuePair<GameObject, ICustomPlanetoidSetting>>();
        List<KeyValuePair<GameObject, ICustomPlanetoidSetting>> customPlanetoidSettings = new List<KeyValuePair<GameObject, ICustomPlanetoidSetting>>();
        public void DoAndRefreshView(System.Action action)
        {
            action.Invoke();
            this.RefreshGallery();
            this.RefreshDetails();
        }

        public override float GetSortKey() => 20f;

        public override void OnActivate() => this.OnShow(true);
        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                Show(show: false);
            }

            base.OnKeyDown(e);
        }



        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        private void RefreshGallery()
        {
            //SgtLogger.warning(planetoidGridButtons.Count.ToString(),"CUND");
            var activePlanets = CGSMClusterManager.GetActivePlanetsCluster();
            //Debug.Log(activePlanets.Count + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            //foreach(var planet in activePlanets )
            //{
            //    SgtLogger.l(planet, "PLANET");
            //}

            foreach (KeyValuePair<StarmapItem, MultiToggle> galleryGridButton in planetoidGridButtons)
            {
                StarmapItem key = CustomCluster.DataSetterFor(galleryGridButton.Key);
                MultiToggle multiToggle1 = galleryGridButton.Value;

                //SgtLogger.log(SelectedCategory.ToString()+" == ? "+ key.category, "CATEGORY");
                ///Is in category?
                multiToggle1.gameObject.SetActive(key.category == this.SelectedCategory);
                bool PlanetIsInList = activePlanets.Contains(key.id);

                ///Enabled?
                multiToggle1.ChangeState(key.Equals(SelectedPlanet) ? 1 : 0);

                HierarchyReferences component = multiToggle1.gameObject.GetComponent<HierarchyReferences>();
                LocText reference1 = component.GetReference<LocText>("OwnedCountLabel");
                Image reference2 = component.GetReference<Image>("IsUnownedOverlay");
                reference2.gameObject.SetActive(!PlanetIsInList);
                galleryGridButton.Value.gameObject.SetActive(galleryGridButton.Key.category == this.SelectedCategory);

                SgtLogger.l(key.InstancesToSpawn.ToString(), "COUNT");
                reference1.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", key.InstancesToSpawn.ToString("0.0"));
                reference1.gameObject.SetActive(PlanetIsInList);
            }
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!show)
                return;
            this.galleryGridLayouter.RequestGridResize();
            this.PopulateCategories();
            this.PopulateGallery();
            this.SelectCategory(StarmapItemCategory.Starter);
        }

        public override void OnCmpEnable()
        {
            base.OnCmpEnable();
            KleiItemsStatusRefresher.AddOrGetListener((Component)this).OnRefreshUI((System.Action)(() =>
            {
                this.RefreshCategories();
                this.RefreshGallery();
                this.RefreshDetails();
            }));
        }
        public void RefreshView()
        {
            this.RefreshCategories();
            this.RefreshGallery();
            this.RefreshDetails();
        }


        /// <summary>
        /// Details
        /// </summary>
        private LocText selectionHeaderLabel;
        private void RefreshDetails()
        {

            string name = SelectedPlanet.DisplayName;
            string description = SelectedPlanet.DisplayDescription;

            this.selectionHeaderLabel.SetText(name);
            //this.selectionNameLabel.SetText(name);
            //this.selectionDescriptionLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(description));
            //this.selectionDescriptionLabel.SetText(description);
            //selectionDescriptionLabel.staticLayout = true;
            //this.selectionFacadeForLabel.gameObject.SetActive(selectedPermit.planetSprite !=null));
            //this.selectionFacadeForLabel.SetText(presentationInfo.facadeFor);
            //string text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", selectedPermit.category.ToString());

            //this.selectionRarityDetailsLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));            
            //this.selectionRarityDetailsLabel.SetText(text);

            //this.selectionOwnedCount.gameObject.SetActive(true);
            //if (ownedCount > 0)
            //    this.selectionOwnedCount.SetText(global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT.Replace("{OwnedCount}", ownedCount.ToString()));
            //else
            //    this.selectionOwnedCount.SetText(KleiItemsUI.WrapWithColor((string)global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));

            bool isPoi = SelectedPlanet.category == StarmapItemCategory.POI;

            StarmapItem current;
            bool IsPartOfCluster = isPoi ? CustomCluster.HasPOI(SelectedPlanet, out current) : CustomCluster.HasPlanet(SelectedPlanet, out current);


            customPlanetoidSettings[0].Value.HandleData(IsPartOfCluster); ///PlanetToggle
            customPlanetoidSettings[1].Value.HandleData((float)current.minRing); ///inner ring
            customPlanetoidSettings[1].Value.ToggleInteractable(IsPartOfCluster);
            customPlanetoidSettings[2].Value.HandleData((float)current.maxRing); ///outer ring
            customPlanetoidSettings[2].Value.ToggleInteractable(IsPartOfCluster);

            customPlanetoidSettings[3].Key.SetActive(!isPoi && !showGameSettings);///buffer ring, only on planets
            if (!isPoi)
            {
                customPlanetoidSettings[3].Value.HandleData((float)current.buffer);
                customPlanetoidSettings[3].Value.ToggleInteractable(IsPartOfCluster);
            }


        }

        #region buttonRecycling

        private List<GameObject> recycledGalleryGridButtons = new List<GameObject>();
        private GameObject GetAvailableGridButton()
        {
            if (this.recycledGalleryGridButtons.Count == 0)
                return Util.KInstantiateUI(this.PlanetoidEntryPrefab, this.galleryGridContent.gameObject, true);

            GameObject galleryGridButton = this.recycledGalleryGridButtons[0];
            this.recycledGalleryGridButtons.RemoveAt(0);
            return galleryGridButton;
        }
        private void RecycleGalleryGridButton(GameObject button)
        {
            button.GetComponent<MultiToggle>().onClick = (System.Action)null;
            this.recycledGalleryGridButtons.Add(button);
        }
        #endregion

        public void PopulateGallery()
        {
            foreach (KeyValuePair<StarmapItem, MultiToggle> galleryGridButton in this.planetoidGridButtons)
                this.RecycleGalleryGridButton(galleryGridButton.Value.gameObject);
            this.planetoidGridButtons.Clear();

            this.galleryGridLayouter.ImmediateSizeGridToScreenResolution();

            foreach (var Planet in PlanetoidDict())
            {
                this.AddItemToGallery(Planet.Value);
            }
            foreach (StarmapItemCategory category in (StarmapItemCategory[])Enum.GetValues(typeof(StarmapItemCategory)))
            {
                AddPermitCategory(category);

                //var RandomPlanet = new StarmapItem("Random", category, Assets.GetSprite("unknown"));

                //AddItemToGallery(RandomPlanet);
            }
            ;
        }

        private void AddItemToGallery(StarmapItem planet)
        {
            if (planetoidGridButtons.ContainsKey(planet))
            {
                SgtLogger.l("wasthereallready");
                return;
            }

            // PermitPresentationInfo presentationInfo = permit.GetPermitPresentationInfo();
            GameObject availableGridButton = this.GetAvailableGridButton();
            HierarchyReferences component1 = availableGridButton.GetComponent<HierarchyReferences>();
            Image reference1 = component1.GetReference<Image>("Icon");
            LocText reference2 = component1.GetReference<LocText>("OwnedCountLabel");
            Image reference3 = component1.GetReference<Image>("IsUnownedOverlay");
            MultiToggle component2 = availableGridButton.GetComponent<MultiToggle>();

            reference1.sprite = planet.planetSprite;

            int ownedCount = 1;
            reference2.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString("0.0"));
            reference2.gameObject.SetActive(ownedCount > 0);
            reference3.gameObject.SetActive(ownedCount <= 0);
            component2.onEnter += new System.Action(this.OnMouseOverToggle);
            component2.onClick = (System.Action)(() => this.SelectItem(planet));

            planetoidGridButtons[planet] = component2;
            //this.SetItemClickUISound(planet, component2);
            KleiItemsUI.ConfigureTooltipOn(availableGridButton, default(Option<string>));
            availableGridButton.SetActive(true);
        }
        public void SelectItem(StarmapItem planet)
        {
            SelectedPlanet = planet;
            //CGSMClusterManager.TogglePlanetoid(planet);
            ///Select Planet
            this.RefreshGallery();
            this.RefreshDetails();
            //AddCustomCluster();
        }

        private StarmapItem _selectedPlanet;
        StarmapItem SelectedPlanet
        {
            get { return CustomCluster.DataSetterFor(_selectedPlanet); }
            set { _selectedPlanet = value; }
        }

        StarmapItemCategory SelectedCategory = StarmapItemCategory.Starter;
        private void RefreshCategories()
        {
            foreach (KeyValuePair<StarmapItemCategory, MultiToggle> categoryToggle in this.categoryToggles)
            {
                categoryToggle.Value.ChangeState(categoryToggle.Key == this.SelectedCategory ? 1 : 0);
            }
        }
        public void SelectCategory(StarmapItemCategory category)
        {
            this.SelectedCategory = category;
            this.galleryHeaderLabel.SetText("Planet"); //TODO: set Planet Type header 
            this.SelectDefaultCategoryItem();
            this.RefreshView();
        }
        private void SelectDefaultCategoryItem()
        {
            foreach (var galleryGridButton in this.planetoidGridButtons)
            {
                if (galleryGridButton.Key.category == this.SelectedCategory)
                {
                    this.SelectItem(galleryGridButton.Key);
                    return;
                }
            }
            this.SelectItem(default);
        }

        public void PopulateCategories()
        {
            foreach (KeyValuePair<StarmapItemCategory, MultiToggle> categoryToggle in this.categoryToggles)
                UnityEngine.Object.Destroy((UnityEngine.Object)categoryToggle.Value.gameObject);
            this.categoryToggles.Clear();
        }
        private void AddPermitCategory(StarmapItemCategory StarmapItemCategory)
        {
            GameObject gameObject = Util.KInstantiateUI(this.PlanetoidCategoryPrefab, this.categoryListContent.gameObject, true);
            HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
            component1.GetReference<LocText>("Label").SetText(StarmapItemCategory.ToString());
            component1.GetReference<Image>("Icon").sprite = Assets.GetSprite("unknown"); /// better icons
            MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
            component2.onEnter += new System.Action(this.OnMouseOverToggle);
            component2.onClick = (System.Action)(() => this.SelectCategory(StarmapItemCategory));
            this.categoryToggles.Add(StarmapItemCategory, component2);
            this.SetCatogoryClickUISound(StarmapItemCategory, component2);
        }

        private void OnMouseOverToggle() => KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover"));
        private void SetCatogoryClickUISound(StarmapItemCategory category, MultiToggle toggle)
        {
            if (!this.categoryToggles.ContainsKey(category))
            {
                toggle.states[1].on_click_override_sound_path = "";
                toggle.states[0].on_click_override_sound_path = "";
            }
            else
            {
                toggle.states[1].on_click_override_sound_path = "General_Category_Click";
                toggle.states[0].on_click_override_sound_path = "General_Category_Click";
            }
        }
        //private void SetItemClickUISound(PermitResource permit, MultiToggle toggle)
        //{
        //    string facadeItemSoundName = KleiInventoryScreen.GetFacadeItemSoundName(permit);
        //    toggle.states[1].on_click_override_sound_path = facadeItemSoundName + "_Click";
        //    toggle.states[1].sound_parameter_name = "Unlocked";
        //    toggle.states[1].sound_parameter_value = permit.IsUnlocked() ? 1f : 0.0f;
        //    toggle.states[1].has_sound_parameter = true;
        //    toggle.states[0].on_click_override_sound_path = facadeItemSoundName + "_Click";
        //    toggle.states[0].sound_parameter_name = "Unlocked";
        //    toggle.states[0].sound_parameter_value = permit.IsUnlocked() ? 1f : 0.0f;
        //    toggle.states[0].has_sound_parameter = true;
        //}
    }
}
