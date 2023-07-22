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

namespace ClusterTraitGenerationManager
{
    public class CGM_MainScreen_UnityScreen : KModalScreen
    {
        GridLayouter galleryGridLayouter;

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

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.canBackoutWithRightClick = true;
            base.ConsumeMouseScroll = true;
            this.SetHasFocus(true);


#if DEBUG
            //UIUtils.ListAllChildrenPath(this.transform);
#endif
            OnResize();

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

            galleryGridLayouter = new GridLayouter
            {
                minCellSize = 80f,
                maxCellSize = 160f,
                targetGridLayouts = new List<GridLayoutGroup>() { galleryGridContent.GetComponent<GridLayoutGroup>() }
            };
            UIUtils.ListAllChildrenWithComponents(galleryGridContent.transform);

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

        public override void OnKeyDown(KButtonEvent e)
        {
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

            this.SelectCategory(StarmapItemCategory.Starter);

            this.galleryGridLayouter.RequestGridResize();

            OnResize();
            //RefreshWithDelay(() => OnResize(true),300);
            ScreenResize.Instance.OnResize += () => OnResize();
        }
        public void OnResize()
        {
            var rectMain = this.rectTransform();
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / (rectMain.lossyScale.x)));
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / (rectMain.lossyScale.y)));
        }

        public void RefreshView()
        {
            this.RefreshCategories();
            this.RefreshGallery();
            this.RefreshDetails();
            this.ResetSettingButtonStates();
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
            if (!init || SelectedPlanet == null 
               // || selectedItemSettings == null
                )
                return;

            string name = SelectedPlanet.DisplayName;
            string description = SelectedPlanet.DisplayDescription;

            this.selectionHeaderLabel.SetText(name);

            if (SelectedPlanet != null)
            {
                UpdateForSelected(SelectedPlanet);
            }

        }
        public void ResetSettingButtonStates()
        {
            //FAT TODO!
        }
        
        public void UpdateForSelected(StarmapItem starmapItem)
        {
            //FAT TODO!
        }

        public void SelectItem(StarmapItem planet)
        {
            SelectedPlanet = planet;
            //CGSMClusterManager.TogglePlanetoid(planet);
            ///Select Planet
            this.RefreshGallery();
            this.RefreshDetails();
            UpdateForSelected(planet);
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
            UIUtils.ListAllChildrenWithComponents(transform.Find("Details/Content/ScrollRectContainer/MinMaxDistance/Slider"));
            UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider slider = transform.Find("Details/Content/ScrollRectContainer/MinMaxDistance/Slider").FindOrAddComponent<UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider>();
            slider.SliderBounds = slider.transform.Find("Handle Slide Area").rectTransform();
            slider.MinHandle = slider.transform.Find("Handle Slide Area/HandleMin").rectTransform();
            slider.MaxHandle = slider.transform.Find("Handle Slide Area/Handle").rectTransform();
            slider.MiddleGraphic = slider.transform.Find("Fill Area/Fill").rectTransform();


            FButton button = transform.Find("Details/Footer/Buttons/GenerateClusterButton").FindOrAddComponent<FButton>();
            button.OnClick += () => CGSMClusterManager.InitializeGeneration();
            //FAT TODO!
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
            this.galleryGridLayouter.RequestGridResize();
        }

        public class GalleryItem : KMonoBehaviour
        {
            public LocText ItemNumber;
            public FToggleButton ActiveToggle;
            public GameObject DisabledOverlay;
           
            public void Initialize(StarmapItem planet)
            {
                SgtLogger.l(planet.DisplayName, "Init gallery item for");
                Image itemIconImage = transform.Find("Image").GetComponent<Image>();
                ItemNumber = transform.Find("AmountLabel").GetComponent<LocText>();
                DisabledOverlay = transform.Find("DisabledOverlay").gameObject;
                ActiveToggle = this.gameObject.AddOrGet<FToggleButton>();
                itemIconImage.sprite = planet.planetSprite;

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
                    UpdateForSelected(SelectedPlanet);
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

            categoryItem.transform.Find("Label").GetComponent<LocText>().SetText(categoryName);
            var item = categoryItem.AddOrGet<CategoryItem>();
            item.Initialize(StarmapItemCategory, Assets.GetSprite("unknown"));
            item.ActiveToggle.OnClick += (() => this.SelectCategory(StarmapItemCategory));
            this.categoryToggles.Add(StarmapItemCategory, item);
        }


        #endregion

    }
}
