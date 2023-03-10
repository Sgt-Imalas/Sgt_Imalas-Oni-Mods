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
            this.canBackoutWithRightClick= true;
            this.SetHasFocus( true);
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

            // var Slider = Util.KInstantiateUI(SliderPrefab.gameObject, infoInsert.gameObject, true);
            // var Cycle = Util.KInstantiateUI(CyclePrefab.gameObject, infoInsert.gameObject, true);
            // var Seed = Util.KInstantiateUI(SeedPrefab.gameObject, infoInsert.gameObject, true);


            ///PlanetEnabledCheckbox
            var Check = Util.KInstantiateUI(CheckboxPrefab.gameObject, infoInsert.gameObject, true);
            UIUtils.TryChangeText(Check.transform, "Label", "Enabled");
            var PlanetEnabled = Check.AddComponent<CheckBoxHandler>();
            PlanetEnabled.SetAction(() => ToggleCurrentSelected());
            customPlanetoidSettings.Add(PlanetEnabled);


            var Buttons = Util.KInstantiateUI(selectScreen.transform.Find("Layout/Buttons").gameObject, infoInsert.gameObject, true);

            //UIUtils.ListAllChildren(Buttons.transform);
            
            UIUtils.AddActionToButton(Buttons.transform, "BackButton", () => Show(false));
            UIUtils.AddActionToButton(Buttons.transform, "LaunchButton", () => {
                AddCustomCluster();
                CGSMClusterManager.selectScreen.LaunchClicked(); 
            }
            );

            UIUtils.FindAndDisable(Buttons.transform, "CustomizeButton");
            //UIUtils.AddActionToButton(Buttons.transform, "CustomizeButton", () => OpenCustomSettingsAbove());

            

            //closeButton.onClick += delegate
            //{
            //    Show(show: false);
            //};
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
        List<ICustomPlanetoidSetting> customPlanetoidSettings = new List<ICustomPlanetoidSetting>();

        public void ToggleCurrentSelected()
        {
            CGSMClusterManager.TogglePlanetoid(SelectedPlanet);
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
                StarmapItem key = galleryGridButton.Key;
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

                int ownedCount = 1;
                reference1.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString());
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
        private LocText selectionNameLabel;
        private LocText selectionDescriptionLabel;
        private LocText selectionFacadeForLabel;
        private LocText selectionRarityDetailsLabel;
        private LocText selectionOwnedCount;
        private void RefreshDetails()
        {
            StarmapItem selectedPermit = this.SelectedPlanet;
            
            StringEntry name;
            StringEntry description;

            if (selectedPermit.world == null)
            {
                name = new StringEntry("Random Planet");
            }
            else
            {
                Strings.TryGet(selectedPermit.world.name, out name);
                Strings.TryGet(selectedPermit.world.description, out description);
            }
            this.selectionHeaderLabel.SetText(name);
            //this.selectionNameLabel.SetText(name);
            //this.selectionDescriptionLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(description));
            //this.selectionDescriptionLabel.SetText(description);
            //selectionDescriptionLabel.staticLayout = true;
            //this.selectionFacadeForLabel.gameObject.SetActive(selectedPermit.planetSprite !=null));
            //this.selectionFacadeForLabel.SetText(presentationInfo.facadeFor);
            string text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", selectedPermit.category.ToString());

            //this.selectionRarityDetailsLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));            
            //this.selectionRarityDetailsLabel.SetText(text);

            //this.selectionOwnedCount.gameObject.SetActive(true);
            int ownedCount = 1;
            //if (ownedCount > 0)
            //    this.selectionOwnedCount.SetText(global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT.Replace("{OwnedCount}", ownedCount.ToString()));
            //else
            //    this.selectionOwnedCount.SetText(KleiItemsUI.WrapWithColor((string)global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));

            customPlanetoidSettings[0].HandleData(CustomCluster.HasPlanet(selectedPermit));
            
        
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
            reference2.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString());
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

        StarmapItem SelectedPlanet;
        StarmapItemCategory SelectedCategory = StarmapItemCategory.Starter;
        private void RefreshCategories()
        {
            SgtLogger.log(SelectedCategory.ToString(), "CATEGORY");
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
