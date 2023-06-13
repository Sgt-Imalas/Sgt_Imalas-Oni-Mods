using Database;
using Epic.OnlineServices.Sessions;
using FMOD;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using rail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static LogicGate.LogicGateDescriptions;
using static ResearchTypes;
using static SandboxSettings;
using static SetStartDupes.DupeTraitManager;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.OBJECTLIST;
using static STRINGS.DUPLICANTS;
using static STRINGS.DUPLICANTS.CHORES;
using static STRINGS.ELEMENTS;
using static STRINGS.UI.DETAILTABS.PERSONALITY.RESUME;
using static STRINGS.UI.STARMAP;

namespace SetStartDupes
{
    internal class UnityCarePackageScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityCarePackageScreen Instance = null;

        public LocText ToReplaceName;
        public Image ToReplaceColour;

        public GameObject PresetListContainer;
        public GameObject PresetListPrefab;

        public FButton ClearSearchBar;
        public FInputField2 Searchbar;

        public bool CurrentlyActive;

        ///Referenced carepackage to apply stuff to.
        CarePackageContainer ReferencedContainer = null;


        Dictionary<CarePackageInfo, GameObject> CarePackageContainers = new Dictionary<CarePackageInfo, GameObject>();
        Dictionary<CarePackageInfo, GameObject> CurrentlyAllowedCarePackages = new Dictionary<CarePackageInfo, GameObject>();
        Dictionary<CarePackageInfo, searchableCarePackageContainer> SearchableCarePackageInfo = new Dictionary<CarePackageInfo, searchableCarePackageContainer>();


        Trait CurrentCarePackage = null;

        public static void ShowWindow(CarePackageContainer currentContainer, System.Action onClose, List<CarePackageInfo> allowedCarePackages)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitsWindowPrefab, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<UnityCarePackageScreen>();
                Instance.Init();
            }
            Instance.ReferencedContainer = currentContainer;
            Instance.SetOpenedType(allowedCarePackages);
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;
        }

        private bool init;
        private System.Action OnCloseAction;


        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                this.Show(false);
            }
            if (e.TryConsume(Action.DebugToggleClusterFX))
            {
                Searchbar.ExternalStartEditing();
            }

            base.OnKeyDown(e);
        }
        private void SetOpenedType(List<CarePackageInfo> currentlyAllowed)
        {

            ToReplaceName.text = GetSpawnableQuantity(ReferencedContainer.info.id,ReferencedContainer.info.quantity);

            CurrentlyAllowedCarePackages.Clear();
            foreach (var go in CarePackageContainers)
            {
                go.Value.SetActive(false);
            }
            foreach (var activeInfo in currentlyAllowed)
            {
                CurrentlyAllowedCarePackages.Add(activeInfo, CarePackageContainers[activeInfo]);
            }
            ApplyFilter();
        }


        private void ChoseThis(CarePackageInfo packageConfig)
        {
            ReferencedContainer.controller.RemoveLast();

            ReferencedContainer.info = packageConfig;
            if (ReferencedContainer.animController != null)
            {
                UnityEngine.Object.Destroy(ReferencedContainer.animController.gameObject);
                ReferencedContainer.animController = null;
            }

            ReferencedContainer.carePackageInstanceData = new CarePackageContainer.CarePackageInstanceData();
            ReferencedContainer.carePackageInstanceData.info = ReferencedContainer.info;
            ReferencedContainer.carePackageInstanceData.facadeID = ReferencedContainer.info.facadeID != "SELECTRANDOM" ? ReferencedContainer.info.facadeID : Db.GetEquippableFacades().resources.FindAll((Predicate<EquippableFacadeResource>)(match => match.DefID == ReferencedContainer.info.id)).GetRandom<EquippableFacadeResource>().Id;
            ReferencedContainer.ClearEntryIcons(); 
            ReferencedContainer.SetAnimator(); 
            ReferencedContainer.SetInfoText();
            ReferencedContainer.selectButton.ClearOnClick();
            ReferencedContainer.selectButton.onClick += (System.Action)(() => ReferencedContainer.SelectDeliverable());

            this.Show(false);
        }

        private void Init()
        {
            if (init) { return; }
            SgtLogger.l("Initializing CarePackageWindow");
            //UIUtils.ListAllChildren(this.transform);


            ToReplaceName = transform.Find("ToReplace/CurrentlyActive/Label").FindComponent<LocText>();
            ToReplaceColour = transform.Find("ToReplace/CurrentlyActive/Background").FindComponent<Image>();

            Searchbar = transform.Find("SearchBar/Input").FindOrAddComponent<FInputField2>();
            Searchbar.OnValueChanged.AddListener(ApplyFilter);
            Searchbar.Text = string.Empty;


            ClearSearchBar = transform.Find("SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

            UIUtils.AddSimpleTooltipToObject(ClearSearchBar.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);

            PresetListContainer = transform.Find("ScrollArea/Content").gameObject;
            PresetListPrefab = transform.Find("ScrollArea/Content/CarePackagePrefab").gameObject;


            var CloserButton = transform.Find("CloseButton").gameObject;
            UIUtils.ListAllChildren(CloserButton.transform);
            CloserButton.FindOrAddComponent<FButton>().OnClick += () => this.Show(false);
            InitAllContainers();

            init = true;
        }

        private void InitAllContainers()
        {
            foreach (CarePackageInfo carePackage in Immigration.Instance.carePackages)
            {
                AddUiContainer(carePackage);
            }
        }
        private void AddUiContainer(CarePackageInfo info)
        {
            if (info != null && !CarePackageContainers.ContainsKey(info))
            {
                var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);


                string name = GetSpawnableName(info.id);
                string description = GetSpawnableDescription(info.id);
                string amountDesc = GetSpawnableQuantity(info.id, info.quantity);


                UIUtils.TryChangeText(PresetHolder.transform, "Label", amountDesc);

                if (description != null && description.Length > 0)
                {
                    UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
                }

                PresetHolder.transform.Find("SwitchIn").FindOrAddComponent<FButton>().OnClick += () => ChoseThis(info);
                PresetHolder.transform.Find("CarePackageSprite").TryGetComponent<Image>(out var image);
                ApplyCarePackageSprite(info, image);

                CarePackageContainers[info] = PresetHolder;
                SearchableCarePackageInfo[info] = new searchableCarePackageContainer(name, description, amountDesc);

            }
        }

        public struct searchableCarePackageContainer
        {
            public string nameString;
            public string descriptionString;
            public string quantityString;
            public searchableCarePackageContainer(string nameString, string descriptionString, string quantityString)
            {
                this.nameString = nameString;
                this.descriptionString = descriptionString;
                this.quantityString = quantityString;
            }
            public override string ToString()
            {
                return nameString + " " + descriptionString + " " + quantityString;
            }
        }

        public void ApplyFilter(string filterstring = "")
        {
            foreach (KeyValuePair<CarePackageInfo, GameObject> packageInfo in CurrentlyAllowedCarePackages)
            {

                if (CarePackageAllowed(packageInfo.Key))
                {
                    var searchinfo = SearchableCarePackageInfo[packageInfo.Key];

                    packageInfo.Value.SetActive(filterstring == string.Empty 
                        ? true : ShowInFilter(filterstring, new string[] { searchinfo.nameString, searchinfo.descriptionString, searchinfo.quantityString }));
                }
            }
        }
        bool CarePackageAllowed(CarePackageInfo info) => info.requirement == null ? true : info.requirement.Invoke();

        private void ApplyCarePackageSprite(CarePackageInfo CarePackage, Image image)
        {
            GameObject prefab = Assets.GetPrefab(CarePackage.id.ToTag());
            if (prefab == null)
            {
                image.sprite = Def.GetUISpriteFromMultiObjectAnim(ElementLoader.GetElement(CarePackage.id.ToTag()).substance.anim);
                image.color = (Color)ElementLoader.GetElement(CarePackage.id.ToTag()).substance.uiColour;
                return;
            }


            string facadeID = CarePackage.facadeID!=null?CarePackage.facadeID:string.Empty;
            facadeID = !facadeID.Contains("SELECTRANDOM")
                ? CarePackage.facadeID
                : Db.GetEquippableFacades().resources
                    .FindAll(match => match.DefID == CarePackage.id)
                    .GetRandom().Id;

            
            var prefabSprite =
                facadeID.IsNullOrWhiteSpace()
                ? Def.GetUISprite(prefab)
                : Def.GetUISprite(prefab.PrefabID(), facadeID);

            image.sprite = prefabSprite.first;
            image.color = prefabSprite.second;

            UnityEngine.Rect rect = image.sprite.rect;
            if (rect.width > rect.height)
            {
                var size = (rect.height / rect.width) * 30f;
                image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            }
            else
            {
                var size = (rect.width / rect.height) * 30f;
                image.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (2 + (30 - size) / 2), size);
            }



        }
        private string GetSpawnableName(string CarePackageID)
        {
            GameObject prefab = Assets.GetPrefab((Tag)CarePackageID);
            if (prefab == null)
            {
                Element elementByName = ElementLoader.FindElementByName(CarePackageID);
                return elementByName != null ? elementByName.substance.name : "";
            }
            return prefab.GetProperName();
        }
        private string GetSpawnableQuantity(string CarePackageID, float CarePackageQuantity)
        {
            if (ElementLoader.GetElement(CarePackageID.ToTag()) != null)
                return string.Format((string)global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_QUANTITY, GameUtil.GetFormattedMass(CarePackageQuantity), Assets.GetPrefab((Tag)CarePackageID).GetProperName());
            return EdiblesManager.GetFoodInfo(CarePackageID) != null ? string.Format((string)global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_QUANTITY, GameUtil.GetFormattedCaloriesForItem((Tag)CarePackageID, CarePackageQuantity), Assets.GetPrefab((Tag)CarePackageID).GetProperName()) : string.Format((string)global::STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT, Assets.GetPrefab((Tag)CarePackageID).GetProperName(), CarePackageQuantity.ToString());
        }

        private string GetSpawnableDescription(string CarePackageID)
        {
            Element element = ElementLoader.GetElement(CarePackageID.ToTag());
            if (element != null)
                return element.Description();
            GameObject prefab = Assets.GetPrefab((Tag)CarePackageID);
            if (prefab == null)
                return "";
            InfoDescription component = prefab.GetComponent<InfoDescription>();
            return component != null ? component.description : prefab.GetProperName();
        }



        bool ShowInFilter(string filtertext, string stringsToInclude)
        {
            return ShowInFilter(filtertext, new string[] { stringsToInclude });
        }

        bool ShowInFilter(string filtertext, string[] stringsToInclude)
        {
            bool show = false;
            filtertext = filtertext.ToLowerInvariant();

            foreach (var text in stringsToInclude)
            {
                if (text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
                {
                    show = true;
                    break;
                }
            }
            return show;
        }


        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!init)
            {
                Init();
            }
            CurrentlyActive = show;
        }
    }
}
