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

namespace ClusterTraitGenerationManager
{
    internal class FeatureSelectionScreen : KModalScreen
    {
        KButton closeButton;
        GridLayouter galleryGridLayouter;

        GameObject PlanetoidCategoryPrefab;
        GameObject PlanetoidEntryPrefab;

        private Dictionary<PlanetCategory, MultiToggle> categoryToggles = new Dictionary<PlanetCategory, MultiToggle>();
        Dictionary<PlanetoidGridItem, MultiToggle> planetoidGridButtons = new Dictionary<PlanetoidGridItem, MultiToggle>();

        GameObject RandomPlanetoidEntryPrefab;

        public RectTransform galleryGridContent;
        public RectTransform categoryListContent;

        private LocText galleryHeaderLabel;

        public struct PlanetoidGridItem
        {
            public string id;
            public PlanetCategory category;
            public Sprite planetSprite;
            public PlanetoidGridItem(string id, PlanetCategory category, Sprite sprite = null)
            {
                this.id = id;
                this.category = category;
                this.planetSprite = sprite;
            }
        }
        public enum PlanetCategory
        {
            Starter,
            Teleport,
            Outer,
            POI,
            Derelict
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

            if (LockerNavigator.Instance.kleiInventoryScreen.TryGetComponent<KleiInventoryScreen>(out var cmp))
            {
                PlanetoidCategoryPrefab = cmp.categoryRowPrefab;
                PlanetoidEntryPrefab = cmp.gridItemPrefab;
            }
            UIUtils.ListAllChildren(this.transform);

            galleryGridContent = transform.Find("Panel/Content/ColumnItemGallery/LayoutBreaker/Content/Categories/ScrollRect/GridContent").rectTransform();
            categoryListContent = transform.Find("Panel/Content/ColumnCategorySelection/LayoutBreaker/Content/Categories/ScrollRect/ContentContainer/Content").rectTransform();
            galleryHeaderLabel = transform.Find("Panel/Content/ColumnItemGallery/LayoutBreaker/Header/Label").GetComponent<LocText>();


            ///Details
            selectionHeaderLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Header/Label").GetComponent<LocText>();
            permitVis = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/KleiPermitDioramaVis").GetComponent<KleiPermitDioramaVis>();
            selectionNameLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/NameLabel").GetComponent<LocText>();
            selectionDescriptionLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/DescriptionLabel").GetComponent<LocText>();
            selectionFacadeForLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/FacadeForLabel").GetComponent<LocText>();
            selectionRarityDetailsLabel = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/RarityDetailsLabel").GetComponent<LocText>();
            selectionOwnedCount = transform.Find("Panel/Content/ColumnSelectedDetails/LayoutBreaker/Content/Content/DescriptionSection/ScrollRect/Content/OwnedCountLabel").GetComponent<LocText>();

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
            SgtLogger.warning(planetoidGridButtons.Count.ToString(),"CUNT");
            foreach (KeyValuePair<PlanetoidGridItem, MultiToggle> galleryGridButton in this.planetoidGridButtons)
            {
                PlanetoidGridItem key;
                MultiToggle multiToggle1;
                galleryGridButton.Deconstruct<PlanetoidGridItem, MultiToggle>(out key, out multiToggle1);

                SgtLogger.log(SelectedCategory.ToString()+" ==? "+ key.category, "CATEGORY");
                ///Is in category?
                multiToggle1.gameObject.SetActive(key.category == this.SelectedCategory);

                ///Enabled?
                multiToggle1.ChangeState(true ? 1 : 0);
                HierarchyReferences component = multiToggle1.gameObject.GetComponent<HierarchyReferences>();
                LocText reference1 = component.GetReference<LocText>("OwnedCountLabel");
                Image reference2 = component.GetReference<Image>("IsUnownedOverlay");

                galleryGridButton.Value.gameObject.SetActive(galleryGridButton.Key.category == this.SelectedCategory);

                int ownedCount = 1;
                reference1.text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString());
                //reference1.gameObject.SetActive(ownedCount > 0);
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
            this.SelectCategory(PlanetCategory.Starter);
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
        void RefreshView()
        {
            this.RefreshCategories();
            this.RefreshGallery();
            this.RefreshDetails();
        }


        /// <summary>
        /// Details
        /// </summary>
        private LocText selectionHeaderLabel;
        private KleiPermitDioramaVis permitVis;
        private LocText selectionNameLabel;
        private LocText selectionDescriptionLabel;
        private LocText selectionFacadeForLabel;
        private LocText selectionRarityDetailsLabel;
        private LocText selectionOwnedCount;
        private void RefreshDetails()
        {
            PlanetoidGridItem selectedPermit = this.SelectedPlanet;

            //this.permitVis.ConfigureWith(selectedPermit);
            this.selectionHeaderLabel.SetText(selectedPermit.id);
            this.selectionNameLabel.SetText(selectedPermit.id);
            this.selectionDescriptionLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(selectedPermit.id));
            this.selectionDescriptionLabel.SetText(selectedPermit.id);
            //this.selectionFacadeForLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(presentationInfo.facadeFor));
            //this.selectionFacadeForLabel.SetText(presentationInfo.facadeFor);
            //string text = global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", selectedPermit.Rarity.GetLocStringName());
            //this.selectionRarityDetailsLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));
            //this.selectionRarityDetailsLabel.SetText(text);
            this.selectionOwnedCount.gameObject.SetActive(true);
            int ownedCount = 1;
            if (ownedCount > 0)
                this.selectionOwnedCount.SetText(global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT.Replace("{OwnedCount}", ownedCount.ToString()));
            else
                this.selectionOwnedCount.SetText(KleiItemsUI.WrapWithColor((string)global::STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));
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
            foreach (KeyValuePair<PlanetoidGridItem, MultiToggle> galleryGridButton in this.planetoidGridButtons)
                this.RecycleGalleryGridButton(galleryGridButton.Value.gameObject);
            this.planetoidGridButtons.Clear();

            this.galleryGridLayouter.ImmediateSizeGridToScreenResolution();

            foreach (var Planet in Planets)
            {
                this.AddItemToGallery(Planet);
            }
        }

        List<PlanetoidGridItem> Planets = new List<PlanetoidGridItem>();

        private void AddItemToGallery(PlanetoidGridItem planet)
        {
            if (this.planetoidGridButtons.ContainsKey(planet))
                return;

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
            this.planetoidGridButtons.Add(planet, component2);

            //this.SetItemClickUISound(planet, component2);
            KleiItemsUI.ConfigureTooltipOn(availableGridButton, default(Option<string>));
            availableGridButton.SetActive(true);
        }
        public void SelectItem(PlanetoidGridItem planet)
        {
            SelectedPlanet = planet;
            ///Select Planet
            this.RefreshView();
        }

        PlanetoidGridItem SelectedPlanet;
        PlanetCategory SelectedCategory = PlanetCategory.Starter;
        private void RefreshCategories()
        {
            SgtLogger.log(SelectedCategory.ToString(), "CATEGORY");
            foreach (KeyValuePair<PlanetCategory, MultiToggle> categoryToggle in this.categoryToggles)
            {
                categoryToggle.Value.ChangeState(categoryToggle.Key == this.SelectedCategory ? 1 : 0);
            }
        }
        public void SelectCategory(PlanetCategory category)
        {
            this.SelectedCategory = category;
            this.galleryHeaderLabel.SetText("Planet"); //TODO: set Planet Type header 
            this.RefreshView();
        }

        public void PopulateCategories()
        {
            foreach (KeyValuePair<PlanetCategory, MultiToggle> categoryToggle in this.categoryToggles)
                UnityEngine.Object.Destroy((UnityEngine.Object)categoryToggle.Value.gameObject);
            this.categoryToggles.Clear();

            foreach (PlanetCategory category in (PlanetCategory[])Enum.GetValues(typeof(PlanetCategory)))
            {
                AddPermitCategory(category);
                var RandomPlanet = new PlanetoidGridItem("Random", category, Assets.GetSprite("unknown"));
                
                AddItemToGallery(RandomPlanet);
            }
        }
        private void AddPermitCategory(PlanetCategory planetCategory)
        {
            GameObject gameObject = Util.KInstantiateUI(this.PlanetoidCategoryPrefab, this.categoryListContent.gameObject, true);
            HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
            component1.GetReference<LocText>("Label").SetText(planetCategory.ToString());
            component1.GetReference<Image>("Icon").sprite = Assets.GetSprite("unknown"); /// better icons
            MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
            component2.onEnter += new System.Action(this.OnMouseOverToggle);
            component2.onClick = (System.Action)(() => this.SelectCategory(planetCategory));
            this.categoryToggles.Add(planetCategory, component2);
            this.SetCatogoryClickUISound(planetCategory, component2);
        }

        private void OnMouseOverToggle() => KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover"));
        private void SetCatogoryClickUISound(PlanetCategory category, MultiToggle toggle)
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
