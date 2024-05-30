//using Database;
//using Klei.AI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UtilLibs.UIcmp;
//using UtilLibs;
//using BlueprintsV2.BlueprintsV2.UnityUI.Components;

//namespace BlueprintsV2.BlueprintsV2.UnityUI
//{
//    internal class BlueprintSelectionScreen : FScreen
//    {
//#pragma warning disable IDE0051 // Remove unused private members
//        new bool ConsumeMouseScroll = true; // do not remove!!!!
//#pragma warning restore IDE0051 // Remove unused private members
//        public static BlueprintSelectionScreen Instance = null;


//        //Main Areas
//        public GameObject BlueprintsList;
//        public GameObject BlueprintsElements;
//        public GameObject ReplaceBlueprintElements;
//        public FButton CloseBtn;

//        //BlueprintList
//        public FInputField2 BlueprintSearchbar;
//        public FButton ClearBlueprintSearchbar;
//        public FileHierarchyEntry FolderUpBtn;
//        public GameObject HierarchyContainer;
//        public FileHierarchyEntry HierarchyEntryPrefab;
//        public FileHierarchyEntry HierarchyFolderPrefab;
//        public Dictionary<string, GameObject> BlueprintEntriesByPath = new();

//        //MaterialList
//        public Dictionary<Tag, BlueprintElementEntry> ElementEntries = new();
//        public BlueprintElementEntry ElementEntryPrefab;
//        public GameObject ElementEntryContainer;
//        public GameObject SevereErrorGO, ErrorGO;
//        public ToolTip SevereErrorTooltip, ErrorTooltip;

//        //ReplacementList
//        public FInputField2 ReplacementElementSearchbar;
//        public Dictionary<SimHashes, ReplaceElementEntry> ReplacementElementEntries = new();
//        public FButton ClearReplacementElementSearchbar;
//        public GameObject ReplacementElementsContainer;
//        public GameObject ReplacementElementsPrefab;
//        public LocText ToReplaceName;


//        public bool CurrentlyActive;

//        private void Init()
//        {
//            if (init) { return; }
//            SgtLogger.l("Initializing BlueprintWindow");

//            BlueprintsList = transform.Find("FileHierarchy").gameObject;
//            BlueprintsElements = transform.Find("MaterialSwitch").gameObject;
//            ReplaceBlueprintElements = transform.Find("MaterialReplacer").gameObject;

//            CloseBtn = transform.Find("CloseButton").gameObject.AddOrGet<FButton>();
//            CloseBtn.OnClick += () => Show(false);
//            //blueprint files

//            BlueprintSearchbar = transform.Find("FileHierarchy/SearchBar/Input").FindOrAddComponent<FInputField2>();
//            BlueprintSearchbar.OnValueChanged.AddListener(ApplyBlueprintFilter);
//            BlueprintSearchbar.Text = string.Empty;

//            ClearBlueprintSearchbar = transform.Find("FileHierarchy/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
//            ClearBlueprintSearchbar.OnClick += () => BlueprintSearchbar.Text = string.Empty;

//            FolderUpBtn = transform.Find("FileHierarchy/ScrollArea/Content/FolderUp").FindOrAddComponent<FileHierarchyEntry>();
//            FolderUpBtn.Type = FileHierarchyEntry.HierarchyEntryType.goUp;


//            InitAllContainers();

//            init = true;
//        }

//        private void InitAllContainers()
//        {

//        }

//        public static void ShowWindow()
//        {
//            if (Instance == null)
//            {
//                var screen = Util.KInstantiateUI(ModAssets.BlueprintSelectionScreen, GameScreenManager.Instance.transform.Find("ScreenSpaceOverlayCanvas/MiddleCenter - InFrontOfEverything").gameObject, true);
//                Instance = screen.AddOrGet<BlueprintSelectionScreen>();
//                Instance.Init();
//            }
//            Instance.Show(true);
//            Instance.ConsumeMouseScroll = true;
//            Instance.transform.SetAsLastSibling();
//            Instance.ClearSearchbars();

//        }

//        private void ClearSearchbars()
//        {
//            BlueprintSearchbar.Text = string.Empty;
//            ReplacementElementSearchbar.Text = string.Empty;
//        }

//        private bool init;

//        public override void OnKeyDown(KButtonEvent e)
//        {
//            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
//            {
//                this.Show(false);
//            }
//            if (e.TryConsume(Action.DebugToggleClusterFX))
//            {
//                BlueprintSearchbar.ExternalStartEditing();
//            }

//            base.OnKeyDown(e);
//        }


//        private GameObject AddUiContainer(string name, string description, System.Action onClickAction, Color overrideColor = default, GameObject prefabOverride = null, Sprite placeImage = null)
//        {
//            if (prefabOverride == null)
//                prefabOverride = PresetListPrefab;

//            var PresetHolder = Util.KInstantiateUI(prefabOverride, PresetListContainer, true);

//            UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
//            if (description != null && description.Length > 0)
//            {
//                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
//            }
//            if (placeImage != null)
//            {
//                var image = PresetHolder.transform.Find("Image").FindOrAddComponent<Image>();
//                image.sprite = placeImage;
//                UnityEngine.Rect rect = image.sprite.rect;
//                if (rect.width > rect.height)
//                {
//                    var size = (rect.height / rect.width) * 55;
//                    image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
//                }
//                else
//                {
//                    var size = (rect.width / rect.height) * 55;
//                    image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
//                }
//            }

//            PresetHolder.transform.FindOrAddComponent<FButton>().OnClick += onClickAction;
//            if (overrideColor != default)
//                PresetHolder.transform.Find("Background").FindOrAddComponent<Image>().color = overrideColor;

//            return PresetHolder;
//        }




//        public void ApplyBlueprintFilter(string filterstring = "")
//        {
//            var forbidden = DuplicityMainScreen.Instance.CurrentInterestIDs();// new List<SkillGroup>();// ReferencedStats.skillAptitudes.Keys.ToList();
//            foreach (var go in DupeInterestContainers)
//            {
//                bool notYetAdded = !forbidden.Contains(go.Key.Id);
//                go.Value.SetActive(filterstring == string.Empty ? notYetAdded : notYetAdded && ShowInFilter(filterstring, go.Key.Name));
//            }
//        }

//        bool ShowInFilter(string filtertext, string stringsToInclude)
//        {
//            return ShowInFilter(filtertext, new string[] { stringsToInclude });
//        }

//        bool ShowInFilter(string filtertext, string[] stringsToInclude)
//        {
//            bool show = false;
//            filtertext = filtertext.ToLowerInvariant();

//            foreach (var text in stringsToInclude)
//            {
//                if (text != null && text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
//                {
//                    show = true;
//                    break;
//                }
//            }
//            return show;
//        }


//        public override void OnShow(bool show)
//        {
//            base.OnShow(show);
//            if (!init)
//            {
//                Init();
//            }
//            CurrentlyActive = show;
//        }
//    }
//}

