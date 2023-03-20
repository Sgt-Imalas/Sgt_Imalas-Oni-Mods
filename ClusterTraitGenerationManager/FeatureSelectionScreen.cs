using ClusterTraitGenerationManager.SettinPrefabComps;
using ClusterTraitGenerationManager.TemplateComponents;
using Database;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
 using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using static BestFit;
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
        private LocText categoryHeaderLabel;


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
#if DEBUG
            //UIUtils.ListAllChildrenPath(this.transform);
#endif

            OnResize();
            galleryGridContent = transform.Find("Panel/Content/ColumnItemGallery/LayoutBreaker/Content/Categories/ScrollRect/GridContent").rectTransform();
            categoryListContent = transform.Find("Panel/Content/ColumnCategorySelection/LayoutBreaker/Content/Categories/ScrollRect/ContentContainer/Content").rectTransform();
            galleryHeaderLabel = transform.Find("Panel/Content/ColumnItemGallery/LayoutBreaker/Header/Label").GetComponent<LocText>();
            categoryHeaderLabel = transform.Find("Panel/Content/ColumnCategorySelection/LayoutBreaker/Header/Label").GetComponent<LocText>();
            


            //var r = transform.Find("Panel/Content").rectTransform();
            //var LE = gameObject.AddOrGet<LayoutElement>();
            //LE.preferredHeight= UnityEngine.Screen.currentResolution.height;
            //LE.minHeight = 300f;
            //LE.preferredWidth= UnityEngine.Screen.currentResolution.width;
            //LE.minWidth = 600f;

            //var lb = transform.Find("Panel/Content").GetComponent<HorizontalLayoutGroup>();
            //lb.childControlWidth = true ; 
            //transform.Find("Panel/Content").rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 900);
            //UtilMethods.ListAllPropertyValues(lb);
            //UIUtils.ListAllChildrenWithComponents(transform);

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
            //UIUtils.FindAndDestroy(transform, "Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/KleiPermitDioramaVis");
            UIUtils.FindAndDestroy(transform, "Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection");



            //selectionNameLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/NameLabel").GetComponent<LocText>();
            //selectionDescriptionLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/DescriptionLabel").GetComponent<LocText>();
            //selectionFacadeForLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/FacadeForLabel").GetComponent<LocText>();
            //selectionRarityDetailsLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/RarityDetailsLabel").GetComponent<LocText>();
            //selectionOwnedCount = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/OwnedCountLabel").GetComponent<LocText>();


            var infoInsert = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content");
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
        bool init=false;
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
            SettingsButtonText.text = showGameSettings ? STRINGS.UI.CUSTOMCLUSTERUI.CUSTOMCLUSTERCONFIG.HIDE : STRINGS.UI.CUSTOMCLUSTERUI.CUSTOMCLUSTERCONFIG.SHOW;
            RefreshDetails();
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
                var key = CGSMClusterManager.PlanetoidDict()[galleryGridButton.Key.id];
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

                reference1.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", key.InstancesToSpawn.ToString("0.0"));
                reference1.gameObject.SetActive(PlanetIsInList);
            }
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!show)
                return;
            this.PopulateCategories();
            this.PopulateGallery();
            this.SelectCategory(StarmapItemCategory.Starter);
            this.CreateUI();

            this.galleryGridLayouter.RequestGridResize();

            OnResize();
            //RefreshWithDelay(() => OnResize(true),300);
            ScreenResize.Instance.OnResize += ()=>OnResize();
        }

        private void CreateUI()
        {

            var infoInsert = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content");
            //var info = selectScreen.transform.Find("Layout/DestinationInfo/Content/InfoColumn/Horiz/Section - SelectedAsteroidDetails"); 
            var SliderPrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_SliderSetting");
            var CyclePrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_CycleSetting");
            var SeedPrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_SeedSetting");
            var CheckboxPrefab = selectScreen.customSettings.transform.Find("Customization/Content/ScrollRect/ScrollContent/Columns/Prefab_Checkbox");


            //var PerPlanetSettings = Util.KInstantiateUI(infoInsert.gameObject, infoInsert.gameObject, true);
            //var NormalSettings = Util.KInstantiateUI(infoInsert.gameObject, infoInsert.gameObject, true);

#if DEBUG
            //SgtLogger.l("CYCLEPREFAB");
            //UIUtils.ListAllChildrenWithComponents(CyclePrefab);
            //SgtLogger.l("SLIDER");
            //UIUtils.ListAllChildrenWithComponents(SliderPrefab);
            //SgtLogger.l("SEED");
            //UIUtils.ListAllChildrenWithComponents(SeedPrefab);
            //SgtLogger.l("CHECK");
            //UIUtils.ListAllChildrenWithComponents(CheckboxPrefab);
#endif
            //var Slider = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            //var Seed = Util.KInstantiateUI(SeedPrefab.gameObject, infoInsert.gameObject, true);

            var unityScreen = Util.KInstantiateUI(ModAssets.CustomPlanetSideScreen, infoInsert.gameObject, true);
            UIUtils.ListAllChildren(unityScreen.transform);
            UIUtils.ListAllChildrenPath(unityScreen.transform);
            //UIUtils.ListAllChildrenWithComponents(unityScreen.transform);

            return;

            #region individualConfig
            ///PlanetEnabledCheckbox, ListItem 0
            var Check = Util.KInstantiateUI(CheckboxPrefab.gameObject, infoInsert.gameObject, true);
            UIUtils.TryChangeText(Check.transform, "Label", STRINGS.UI.CUSTOMCLUSTERUI.ENABLED.NAME);
            UIUtils.AddSimpleTooltipToObject(Check.transform.Find("Label"), STRINGS.UI.CUSTOMCLUSTERUI.ENABLED.DESC, true);
            var PlanetEnabled = Check.AddComponent<CheckBoxHandler>();
            PlanetEnabled.SetAction(() => DoAndRefreshView(
                () =>
                {
                    if (SelectedPlanet != null)
                    {
                        CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
                        this.RefreshView();
                    }
                }
                )
            );
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(Check, PlanetEnabled)); ///custom index 0, enabled

            var NumberCounter = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var NumberCounterHandler = NumberCounter.AddComponent<SliderHandler>();
            NumberCounterHandler.SetupSlider(0, SelectedPlanet.InstancesToSpawn, SelectedPlanet.MaxNumberOfInstances, false, STRINGS.UI.CUSTOMCLUSTERUI.NUMBERS.NAME, STRINGS.UI.CUSTOMCLUSTERUI.NUMBERS.DESC,
                (value) => {
                    if (SelectedPlanet != null)
                    {
                        if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                            current.SetSpawnNumber(value);
                        else
                            SelectedPlanet.SetSpawnNumber(value);
                        RefreshGallery();
                    }
                });
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(NumberCounter, NumberCounterHandler)); /// custom index 1, number

            ///Sliders for inner and outer ring

            var planetMinRing = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var planetMinRingHandler = planetMinRing.AddComponent<SliderHandler>();
            planetMinRingHandler.SetupSlider(0, SelectedPlanet.minRing, CustomCluster.Rings, true, STRINGS.UI.CUSTOMCLUSTERUI.MINRINGS.NAME, STRINGS.UI.CUSTOMCLUSTERUI.MINRINGS.DESC,
                (value) => {
                    if (SelectedPlanet != null)
                    {
                        if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                            current.SetInnerRing((int)value);
                        else
                            SelectedPlanet.SetInnerRing((int)value);
                        //this.RefreshDetails();
                    }
                }, true);
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(planetMinRing, planetMinRingHandler)); ///custom index 2, min

            var planetMaxRing = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var planetMaxRingHandler = planetMaxRing.AddComponent<SliderHandler>();
            planetMaxRingHandler.SetupSlider(0, SelectedPlanet.maxRing, CustomCluster.Rings, true, STRINGS.UI.CUSTOMCLUSTERUI.MAXRINGS.NAME, STRINGS.UI.CUSTOMCLUSTERUI.MAXRINGS.DESC, (value) =>
            {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                        current.SetOuterRing((int)value);
                    else
                        SelectedPlanet.SetOuterRing((int)value);
                    //    this.RefreshDetails(); 
                }
            }, true);
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(planetMaxRing, planetMaxRingHandler)); ///Custom index 3, max

            ///Slider for buffer
            ///
            var planetBuffer = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var planetBufferHandler = planetBuffer.AddComponent<SliderHandler>();
            planetBufferHandler.SetupSlider(0, SelectedPlanet.buffer, CustomCluster.Rings, true, STRINGS.UI.CUSTOMCLUSTERUI.BUFFER.NAME, STRINGS.UI.CUSTOMCLUSTERUI.BUFFER.DESC, (value) => {
                if (SelectedPlanet != null)
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
                        current.SetBuffer((int)value);
                    else
                        SelectedPlanet.SetBuffer((int)value);
                    // this.RefreshDetails();
                }
            }, true);
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(planetBuffer, planetBufferHandler)); ///custom index 4, buffer


            var planetDimensionTmp = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var planetDimensionTmpHandler = planetDimensionTmp.AddComponent<SliderReusedAsInfo>();
            planetDimensionTmpHandler.SetupInfo(STRINGS.UI.CUSTOMCLUSTERUI.PLANETSIZE.NAME, STRINGS.UI.CUSTOMCLUSTERUI.PLANETSIZE.DESC, SelectedPlanet.PlanetDimensions);
            customPlanetoidSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(planetDimensionTmp, planetDimensionTmpHandler)); ///custom index 5, Size

            #endregion

            #region globalClusterConfig
            ///Global Rings

            var GlobalRingSlider = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            var globalRingHandler = GlobalRingSlider.AddComponent<SliderHandler>();
            globalRingHandler.SetupSlider(ringMin, CustomCluster.Rings, ringMax, true, STRINGS.UI.CUSTOMCLUSTERUI.MAPSIZE.NAME, STRINGS.UI.CUSTOMCLUSTERUI.MAPSIZE.DESC, (value) => { CustomCluster.SetRings((int)value); this.RefreshDetails(); });

            GlobalClusterSettings.Add(new KeyValuePair<GameObject, ICustomPlanetoidSetting>(GlobalRingSlider, globalRingHandler));



            var Buttons = Util.KInstantiateUI(selectScreen.transform.Find("Layout/Buttons").gameObject, infoInsert.gameObject, true);
#if DEBUG
           // SgtLogger.l("BUTTONS");
           // UIUtils.ListAllChildrenWithComponents(Buttons.transform);
#endif
            UIUtils.AddActionToButton(Buttons.transform, "BackButton", () => Show(false));
            UIUtils.AddActionToButton(Buttons.transform, "LaunchButton",
                () =>
                {
                    //AddCustomCluster();
                    CGSMClusterManager.InitializeGeneration();
                }
            );
            ///Global Config

            var resetButton = UIUtils.TryInsertNamedCopy(Buttons.transform, "BackButton", "ResetButton");
            resetButton.SetSiblingIndex(1);
            UIUtils.TryChangeText(resetButton, "Label", STRINGS.UI.CUSTOMCLUSTERUI.RESET.NAME);
            UIUtils.AddSimpleTooltipToObject(resetButton, STRINGS.UI.CUSTOMCLUSTERUI.RESET.DESC, true);
            UIUtils.AddActionToButton(resetButton, "", () => { CGSMClusterManager.ResetToLastPreset(); RefreshView(); });

            var BackButtonLE = UIUtils.TryFindComponent<LayoutElement>(Buttons.transform, "BackButton");
            var LaunchButtonLE = UIUtils.TryFindComponent<LayoutElement>(Buttons.transform, "LaunchButton");
            var ResetButtonLE = UIUtils.TryFindComponent<LayoutElement>(Buttons.transform, "ResetButton");
            var CustomizeButtonLE = UIUtils.TryFindComponent<LayoutElement>(Buttons.transform, "CustomizeButton");

            BackButtonLE.preferredWidth = 150;
            BackButtonLE.minWidth = 50;

            LaunchButtonLE.preferredWidth = 150;
            LaunchButtonLE.minWidth = 50;

            ResetButtonLE.preferredWidth = 150;
            ResetButtonLE.minWidth = 50;

            CustomizeButtonLE.preferredWidth = 150;
            CustomizeButtonLE.minWidth = 50;

            Buttons.GetComponent<LayoutElement>().preferredWidth = 300;
            Buttons.GetComponent<LayoutElement>().minWidth = 100;

            //var v = resetButton.GetComponent<LayoutElement>();
            //foreach (var p in v.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            //{
            //    Console.WriteLine(p+": " +p.GetValue(v, null));
            //}
            //var s = Buttons.GetComponent<LayoutElement>();
            //foreach (var p in s.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            //{
            //    Console.WriteLine(p + ": " + p.GetValue(s, null));
            //}


            SettingsButtonText = UIUtils.TryFindComponent<LocText>(Buttons.transform, "CustomizeButton/Label");
            UIUtils.AddActionToButton(Buttons.transform, "CustomizeButton", () => ToggleGameSettings());
            ToggleGameSettings();


            #endregion

            init = true;
            RefreshView();
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
            if (!init || SelectedPlanet == null)
                return;

            string name = SelectedPlanet.DisplayName;
            string description = SelectedPlanet.DisplayDescription;

            this.selectionHeaderLabel.SetText(name);
            bool isPoi = SelectedPlanet.category == StarmapItemCategory.POI;

            StarmapItem current;
            bool IsPartOfCluster = CustomCluster.HasStarmapItem(SelectedPlanet.id, out current);      
     
            customPlanetoidSettings[0].Value.HandleData(IsPartOfCluster); ///PlanetToggle

            customPlanetoidSettings[1].Key.SetActive(current.MaxNumberOfInstances > 1 && !showGameSettings);///Amount, only on poi / random planets
            if (current.MaxNumberOfInstances > 1)
            {
                customPlanetoidSettings[1].Value.HandleData(new float[] { (float)current.InstancesToSpawn, current.MaxNumberOfInstances });
                customPlanetoidSettings[1].Value.ToggleInteractable(IsPartOfCluster);
            }


            customPlanetoidSettings[2].Value.HandleData((float)current.minRing); ///inner ring
            customPlanetoidSettings[2].Value.ToggleInteractable(IsPartOfCluster);

            customPlanetoidSettings[3].Value.HandleData((float)current.maxRing); ///outer ring
            customPlanetoidSettings[3].Value.ToggleInteractable(IsPartOfCluster);

            customPlanetoidSettings[4].Key.SetActive(!isPoi && !showGameSettings);///buffer ring, only on planets
            customPlanetoidSettings[5].Key.SetActive(!isPoi && !showGameSettings);///size, only on planets
            if (!isPoi)
            {
                customPlanetoidSettings[4].Value.HandleData((float)current.buffer);
                customPlanetoidSettings[4].Value.ToggleInteractable(IsPartOfCluster);
                customPlanetoidSettings[5].Value.HandleData(current.PlanetDimensions);
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
            };
        }

        private void Update()
        { 
            this.galleryGridLayouter.CheckIfShouldResizeGrid();
            //var r = transform.Find("Panel/Content").rectTransform();
            //this.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height*0.9f);v
            //OnResize();
        }

        public void OnResize()
        {
            var rectMain = this.rectTransform();
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / (rectMain.lossyScale.x)));
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / (rectMain.lossyScale.y)));

            var rect = this.transform.Find("Panel/Content").rectTransform();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / ( rectMain.lossyScale.x)));
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / (rectMain.lossyScale.y)));
        }
        static async Task RefreshWithDelay(System.Action task,int ms)
        {
            await Task.Delay(ms);
            task.Invoke();
        }

        private void AddItemToGallery(StarmapItem planet)
        {
            if (planetoidGridButtons.ContainsKey(planet))
            {
                SgtLogger.warning(planet.id + " was already in the gallery");
                return;
            }

            // PermitPresentationInfo presentationInfo = permit.GetPermitPresentationInfo();
            GameObject availableGridButton = this.GetAvailableGridButton();
            HierarchyReferences component1 = availableGridButton.GetComponent<HierarchyReferences>();
            Image reference1 = component1.GetReference<Image>("Icon");
            LocText reference2 = component1.GetReference<LocText>("OwnedCountLabel");
            Image reference3 = component1.GetReference<Image>("IsUnownedOverlay");
            
            //UIUtils.ListAllChildren(reference3.transform);

            reference3.color = new Color(0.066f, 0.066f, 0.066f, 0.4f);
            Image lockImage= reference3.transform.Find("IsUnownedOverlayIcon").GetComponent<Image>();
            lockImage.sprite = Assets.GetSprite(SpritePatch.noneSelected);
            lockImage.rectTransform().sizeDelta = new Vector2(70,70);
            //UtilMethods.ListAllPropertyValues(reference3);

            MultiToggle component2 = availableGridButton.GetComponent<MultiToggle>();

            reference1.sprite = planet.planetSprite;

            int ownedCount = 1;
            reference2.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString("0.0"));
            reference2.gameObject.SetActive(ownedCount > 0);
            reference3.gameObject.SetActive(ownedCount <= 0);
            component2.onEnter += new System.Action(this.OnMouseOverToggle);
            component2.onClick = (System.Action)(() => this.SelectItem(planet));
            component2.onDoubleClick = (Func<bool>)(() =>
            {
                this.SelectItem(planet);
                if(SelectedPlanet!= null)
                {
                    CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
                    this.RefreshView();
                    return true;
                }
                else
                    return false;
            }
            );
            planetoidGridButtons[planet] = component2;
            //this.SetItemClickUISound(planet, component2);
            var tooltip = new Option<string>(planet.DisplayDescription);

            UIUtils.AddSimpleTooltipToObject(availableGridButton.transform, planet.DisplayName + "\n\n" + planet.DisplayDescription, true, 300);


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

        private StarmapItem _selectedPlanet = null;// new StarmapItem("none", StarmapItemCategory.Starter,null);
        StarmapItem SelectedPlanet
        {
            get { return _selectedPlanet; }
            set {
                _selectedPlanet = value;
                this.RefreshView();
            }
        }

        StarmapItemCategory SelectedCategory = StarmapItemCategory.Starter;
        private void RefreshCategories()
        {
            foreach (KeyValuePair<StarmapItemCategory, MultiToggle> categoryToggle in this.categoryToggles)
            {
                categoryToggle.Value.ChangeState(categoryToggle.Key == this.SelectedCategory ? 1 : 0);

                HierarchyReferences component1 = categoryToggle.Value.gameObject.GetComponent<HierarchyReferences>();
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
                        PlanetSprite = CustomCluster.OuterPlanets.Count>0 ? CustomCluster.OuterPlanets.First().Value.planetSprite : Assets.GetSprite("unknown");
                        break;
                    case StarmapItemCategory.POI:
                        PlanetSprite = CustomCluster.POIs.Count > 0 ? CustomCluster.POIs.First().Value.planetSprite : Assets.GetSprite("unknown");
                        break;
                }
                component1.GetReference<Image>("Icon").sprite = PlanetSprite;

            }
        }
        public void SelectCategory(StarmapItemCategory category)
        {
            this.SelectedCategory = category;
            this.galleryHeaderLabel.SetText(STRINGS.UI.CUSTOMCLUSTERUI.NAMEITEMS); //TODO: set Planet Type header 
            this.categoryHeaderLabel.SetText(STRINGS.UI.CUSTOMCLUSTERUI.NAMECATEGORIES); 
            this.SelectDefaultCategoryItem();
            this.RefreshView();
        }
        private void SelectDefaultCategoryItem()
        {
            foreach (var galleryGridButton in this.planetoidGridButtons)
            {
                if (galleryGridButton.Key.category == this.SelectedCategory && CustomCluster.HasStarmapItem(galleryGridButton.Key.id,out var i))
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
            string categoryName = string.Empty; //CATEGORYENUM

            switch (StarmapItemCategory)
            {
                case StarmapItemCategory.Starter:
                    categoryName = STRINGS.UI.CUSTOMCLUSTERUI.CATEGORYENUM.START;
                    break;
                case StarmapItemCategory.Warp:
                    categoryName = STRINGS.UI.CUSTOMCLUSTERUI.CATEGORYENUM.WARP;
                    break;
                case StarmapItemCategory.Outer:
                    categoryName = STRINGS.UI.CUSTOMCLUSTERUI.CATEGORYENUM.OUTER;
                    break;
                case StarmapItemCategory.POI:
                    categoryName = STRINGS.UI.CUSTOMCLUSTERUI.CATEGORYENUM.POI;
                    break;
            }

            component1.GetReference<LocText>("Label").SetText(categoryName);
            
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
