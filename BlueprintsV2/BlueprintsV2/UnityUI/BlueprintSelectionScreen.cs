using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;
using UtilLibs;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using UnityEngine.UI;
using static Database.MonumentPartResource;
using TemplateClasses;
using BlueprintsV2.BlueprintsV2.BlueprintData;
using UtilLibs.UI.FUI;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR;

namespace BlueprintsV2.BlueprintsV2.UnityUI
{
    internal class BlueprintSelectionScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static BlueprintSelectionScreen Instance = null;


        //Main Areas
        public GameObject BlueprintsList;
        public GameObject BlueprintsElements;
        public GameObject ReplaceBlueprintElements;
        public FButton CloseBtn;

        //BlueprintList
        public FInputField2 BlueprintSearchbar;
        public FButton ClearBlueprintSearchbar;
        public FButton FolderUpBtn;
        public GameObject HierarchyContainer;
        public FileHierarchyEntry HierarchyEntryPrefab;
        public FolderHierarchyEntry HierarchyFolderPrefab;
        public Dictionary<BlueprintFolder, FolderHierarchyEntry> FolderEntries = new();
        public Dictionary<Blueprint, FileHierarchyEntry> BlueprintEntries = new();

        //MaterialList
        public Dictionary<BlueprintSelectedMaterial, BlueprintElementEntry> ElementEntries = new();
        public GameObject ElementEntryContainer;
        public GameObject SevereErrorGO, ErrorGO;
        public ToolTip SevereErrorTooltip, ErrorTooltip;
        public BlueprintElementEntry ElementEntryPrefab;
        public FButton ClearOverrides, PlaceBlueprint;
        public LocText MaterialHeaderTitle;


        //ReplacementList
        public FInputField2 ReplacementElementSearchbar;
        public Dictionary<Tag, ReplaceElementEntry> ReplacementElementEntries = new();
        public FButton ClearReplacementElementSearchbar;
        public GameObject ReplacementElementsContainer;
        public ReplaceElementEntry ReplacementElementsPrefab;
        public LocText ToReplaceName;

        System.Action onCloseAction;

        public bool CurrentlyActive;
        public bool DialogueCurrentlyOpen;
        Blueprint TargetBlueprint;

        private void Init()
        {
            if (init) { return; }
            SgtLogger.l("Initializing BlueprintWindow");

            BlueprintsList = transform.Find("FileHierarchy").gameObject;
            BlueprintsElements = transform.Find("MaterialSwitch").gameObject;
            ReplaceBlueprintElements = transform.Find("MaterialReplacer").gameObject;



            CloseBtn = transform.Find("CloseButton").gameObject.AddOrGet<FButton>();
            CloseBtn.OnClick += () => Show(false);
            //blueprint files

            BlueprintSearchbar = transform.Find("FileHierarchy/SearchBar/Input").FindOrAddComponent<FInputField2>();
            BlueprintSearchbar.OnValueChanged.AddListener(ApplyBlueprintFilter);
            BlueprintSearchbar.Text = string.Empty;

            ClearBlueprintSearchbar = transform.Find("FileHierarchy/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearBlueprintSearchbar.OnClick += () => BlueprintSearchbar.Text = string.Empty;

            FolderUpBtn = transform.Find("FileHierarchy/ScrollArea/Content/FolderUp").FindOrAddComponent<FButton>();
            FolderUpBtn.gameObject.SetActive(true);
            FolderUpBtn.OnClick += ()=>SelectFolder(null);

            HierarchyContainer = transform.Find("FileHierarchy/ScrollArea/Content").gameObject;

            ClearOverrides = transform.Find("MaterialSwitch/Buttons/ResetButton").FindOrAddComponent<FButton>();
            ClearOverrides.OnClick += OnClearOverrides;

            PlaceBlueprint = transform.Find("MaterialSwitch/Buttons/PlaceBPbtn").FindOrAddComponent<FButton>();
            PlaceBlueprint.OnClick += OnPlaceBlueprint;

            var hierarchyEntryGO = transform.Find("FileHierarchy/ScrollArea/Content/BlueprintEntryPrefab").gameObject;
            hierarchyEntryGO.SetActive(false);
            HierarchyEntryPrefab = hierarchyEntryGO.AddOrGet<FileHierarchyEntry>();


            var hierarchyFolderGO = transform.Find("FileHierarchy/ScrollArea/Content/FolderPrefab").gameObject;
            hierarchyFolderGO.SetActive(false);
            HierarchyFolderPrefab = hierarchyFolderGO.AddOrGet<FolderHierarchyEntry>();

            ElementEntryContainer = transform.Find("MaterialSwitch/ScrollArea/Content").gameObject;
            MaterialHeaderTitle = transform.Find("MaterialSwitch/MaterialsHeader/Label").gameObject.AddOrGet<LocText>();

            SevereErrorGO = transform.Find("MaterialSwitch/MaterialsHeader/WarningSevere").gameObject;
            SevereErrorTooltip = UIUtils.AddSimpleTooltipToObject(SevereErrorGO.transform, string.Empty);
            SevereErrorGO.SetActive(false);


            ErrorGO = transform.Find("MaterialSwitch/MaterialsHeader/Warning").gameObject;
            ErrorTooltip = UIUtils.AddSimpleTooltipToObject(ErrorGO.transform, string.Empty);
            ErrorGO.SetActive(false);

            var ElementEntryPrefabGo = transform.Find("MaterialSwitch/ScrollArea/Content/PresetEntryPrefab").gameObject;
            ElementEntryPrefabGo.SetActive(false);
            ElementEntryPrefab = ElementEntryPrefabGo.AddOrGet<BlueprintElementEntry>();


            ReplacementElementSearchbar = transform.Find("MaterialReplacer/SearchBar/Input").FindOrAddComponent<FInputField2>();
            ReplacementElementSearchbar.OnValueChanged.AddListener(ApplyBlueprintFilter);
            ReplacementElementSearchbar.Text = string.Empty;

            ClearReplacementElementSearchbar = transform.Find("MaterialReplacer/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearReplacementElementSearchbar.OnClick += () => ReplacementElementSearchbar.Text = string.Empty;
            ReplacementElementsContainer = transform.Find("MaterialReplacer/ScrollArea/Content").gameObject;
            ToReplaceName = transform.Find("MaterialReplacer/ToReplace/CurrentlyActive/Label").gameObject.GetComponent<LocText>();

            var ReplaceElementEntryGo = transform.Find("MaterialReplacer/ScrollArea/Content/CarePackagePrefab").gameObject;
            ReplaceElementEntryGo.SetActive(false);
            ReplacementElementsPrefab = ReplaceElementEntryGo.AddComponent<ReplaceElementEntry>();

            init = true;
        }

        private void OnClearOverrides()
        {
            ModAssets.ClearReplacementTags();
            SetMaterialState();
        }

        public static void ShowWindow(System.Action OnClose)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.BlueprintSelectionScreen, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<BlueprintSelectionScreen>();
                Instance.Init();
            }
            Instance.onCloseAction = OnClose;
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.ClearUIState();
            ModAssets.SelectedBlueprint = null;
        }
        private void ClearSearchbars()
        {
            BlueprintSearchbar.Text = string.Empty;
            ReplacementElementSearchbar.Text = string.Empty;
        }

        private bool init;

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                if(!DialogueCurrentlyOpen)
                    this.Show(false);
            }
            if (e.TryConsume(Action.DebugToggleClusterFX))
            {
                BlueprintSearchbar.ExternalStartEditing();
            }

            base.OnKeyDown(e);
        }

        public void ClearUIState()
        {
            ClearSearchbars();
            UpdateBlueprintButtons();
            ToReplaceTag = null;
            SetMaterialState();
        }
        void SetMaterialState()
        {
            ShowReplacementItems(false);
            if (TargetBlueprint == null)
            {
                BlueprintsElements.SetActive(false);
            }
            else
            {

                foreach (var prev in ElementEntries)
                {
                    prev.Value.gameObject.SetActive(false);
                }

                var blueprintMaterials = TargetBlueprint.BlueprintCost.OrderByDescending(kvp => kvp.Value).ToList();

                BlueprintsElements.SetActive(true);
                MaterialHeaderTitle.SetText(string.Format(MATERIALSWITCH.MATERIALSHEADER.LABEL, TargetBlueprint.FriendlyName));
                foreach (var kvp in blueprintMaterials)
                {
                    var selectedAndCategory = kvp.Key;

                    Tag replacementTag = null;
                    var uiEntry = AddOrGetBlueprintElementEntry(kvp.Key);
                    uiEntry.gameObject.SetActive(true);
                    uiEntry.SetReplacementTag();
                    uiEntry.SetTotalAmount(kvp.Value);
                    uiEntry.transform.SetAsLastSibling();

                }
            }
        }
        void UpdateBlueprintButtons()
        {
            foreach(var kvp in BlueprintEntries)
            {
                if(kvp.Value!=null)
                    kvp.Value.gameObject.SetActive(false);
                
            }
            foreach (var kvp in FolderEntries)
            {
                if (kvp.Value != null)
                    kvp.Value.gameObject.SetActive(false);
            }

            var targetFolder = ModAssets.SelectedFolder;
            bool root = targetFolder == null;
            FolderUpBtn.SetInteractable(!root);
            SgtLogger.l("rebuilding folders");
            if (root)
            {
                targetFolder = ModAssets.BlueprintFileHandling.RootFolder;

                var folders = ModAssets.BlueprintFileHandling.BlueprintFolders.OrderBy (f => f.Name);
                foreach (var folder in folders)
                {
                    var uiEntry = AddOrGetFolderEntry(folder);
                    uiEntry.transform.SetAsLastSibling();
                    uiEntry.gameObject.SetActive(true);
                }

                SgtLogger.l(ModAssets.BlueprintFileHandling.BlueprintFolders.Count + "", "folder count");
            }
            else
                SgtLogger.l("not root");

            SgtLogger.l(targetFolder.BlueprintCount + "", "count");
            var bps = targetFolder.Blueprints.OrderBy(bp => bp.FriendlyName);
            foreach (var bp in bps)
            {
                var uiEntry = AddOrGetBlueprintEntry(bp);
                uiEntry.transform.SetAsLastSibling();
                uiEntry.gameObject.SetActive(true);

            }
            this.ConsumeMouseScroll = true;
            LockCam();
        }

        public void LockCam()
        {
            Task.Run(() =>
            {
                Task.Delay(25);
                if(this.isActive)
                {
                    CameraController.Instance.DisableUserCameraControl = true;
                }
            });
        }

        public void SelectFolder(BlueprintFolder folder)
        {
            ModAssets.SelectedFolder = folder;
            UpdateBlueprintButtons();
        }
        public void ApplyReplacementMaterialUI(BlueprintSelectedMaterial original)
        {
            if (ElementEntries.TryGetValue(original, out var UIcmp))
            {
                UIcmp.SetReplacementTag();
            }
        }
        private BlueprintElementEntry AddOrGetBlueprintElementEntry(BlueprintSelectedMaterial elementTag)
        {
            if (!ElementEntries.ContainsKey(elementTag))
            {
                var BPelementEntry = Util.KInstantiateUI<BlueprintElementEntry>(ElementEntryPrefab.gameObject, ElementEntryContainer);
                BPelementEntry.SelectedAndCategory = elementTag;
                //folderEntry.Name = folder.Name;
                BPelementEntry.OnEntryClicked = StartSelectingReplacementTag;
                ElementEntries[elementTag] = BPelementEntry;
            }
            return ElementEntries[elementTag];
        }
        private FolderHierarchyEntry AddOrGetFolderEntry(BlueprintFolder folder)
        {
            if (!FolderEntries.ContainsKey(folder))
            {
                var folderEntry = Util.KInstantiateUI<FolderHierarchyEntry>(HierarchyFolderPrefab.gameObject, HierarchyContainer);                
                folderEntry.folder = folder;
                //folderEntry.Name = folder.Name;
                folderEntry.OnEntryClicked += ()=>SelectFolder(folder);
                FolderEntries[folder] = folderEntry;
            }
            return FolderEntries[folder];
        }
        
        private FileHierarchyEntry AddOrGetBlueprintEntry(Blueprint blueprint)
        {
            if (!BlueprintEntries.ContainsKey(blueprint))
            {
                var bpEntry = Util.KInstantiateUI<FileHierarchyEntry>(HierarchyEntryPrefab.gameObject, HierarchyContainer);
                bpEntry.blueprint = blueprint;
                //folderEntry.Name = folder.Name;
                //folderEntry.OnSelectFolder = OnSelectFolder(folder);
                bpEntry.OnRenamed = (_)=>UpdateBlueprintButtons();
                bpEntry.OnMoved = (_)=> OnBlueprintMoved();
                bpEntry.OnDeleted = OnBlueprintDeleted;
                bpEntry.OnDialogueToggled = DialogueOpen;
                bpEntry.OnSelectBlueprint = OnSelectBlueprint;

                BlueprintEntries[blueprint] = bpEntry;
            }
            return BlueprintEntries[blueprint];
        }
         
        void OnPlaceBlueprint()
        {
            ModAssets.SelectedBlueprint = TargetBlueprint;
            Show(false);
        }

        void OnSelectBlueprint(Blueprint bp)
        {
            TargetBlueprint = bp;
            SetMaterialState();
        }

        private void DialogueOpen(bool obj)
        {
            DialogueCurrentlyOpen = obj;
        }

        void OnBlueprintMoved()
        {
            if(ModAssets.SelectedFolder !=null && !ModAssets.SelectedFolder.HasBlueprints)
                SelectFolder(null);
            else
                UpdateBlueprintButtons();
        }
        void OnBlueprintDeleted(Blueprint bp)
        {

            if (bp != null)
            {
                if(BlueprintEntries.TryGetValue(bp, out var uientry))
                {
                    UnityEngine.Object.Destroy(uientry.gameObject);
                    BlueprintEntries.Remove(bp);
                }
                ModAssets.BlueprintFileHandling.DeleteBlueprint(bp);
            }

            TargetBlueprint = null;
            ClearUIState();
        }

        public static bool HasReplacementCandidates(Tag original) => MaterialSelector.GetValidMaterials(original).Count() > 1;

        BlueprintSelectedMaterial ToReplaceTag = null;
        List<GameObject> PreviouslyActiveMaterialReplacementButtons = new();
        private void SetReplacementMaterials(BlueprintSelectedMaterial materialTypeTag, float amount)
        {
            ToReplaceTag = materialTypeTag;
            ToReplaceName.SetText(ToReplaceTag.CategoryTag.Name);
            foreach (var prev in PreviouslyActiveMaterialReplacementButtons)
            {
                prev.SetActive(false);
            }
            PreviouslyActiveMaterialReplacementButtons.Clear();

            var replacementTags = ModAssets.GetValidMaterials(materialTypeTag.CategoryTag);

            foreach (var replacementTag in replacementTags)
            {
                var btn = AddOrGetReplaceMaterialContainer(replacementTag);
                if (btn == null)
                {
                    SgtLogger.logError(replacementTag + " go was null!");
                    continue;
                }
                PreviouslyActiveMaterialReplacementButtons.Add(btn.gameObject);
                btn.gameObject.SetActive(true);
                btn.Refresh(TargetBlueprint, amount, ToReplaceTag.SelectedTag);
            }
        }
        private ReplaceElementEntry AddOrGetReplaceMaterialContainer(Tag material)
        {
            if (!ReplacementElementEntries.ContainsKey(material))
            {
                var elementEntry = Util.KInstantiateUI<ReplaceElementEntry>(ReplacementElementsPrefab.gameObject, ReplacementElementsContainer);
                elementEntry.targetTag = material;
                elementEntry.OnSelectElement = OnSelectReplacementTag;
                ReplacementElementEntries[material] = elementEntry;
            }
            return ReplacementElementEntries[material];
        }
        private void OnSelectReplacementTag(Tag replacement)
        {
            ModAssets.AddOrSetReplacementTag(ToReplaceTag, replacement);
            TargetBlueprint.CacheCost();
            ApplyReplacementMaterialUI(ToReplaceTag);
            ToReplaceTag = null;
            ShowReplacementItems(false);
        }

        public void StartSelectingReplacementTag(BlueprintSelectedMaterial materialToReplace, float amount)
        {
            ShowReplacementItems(true);
            SetReplacementMaterials(materialToReplace, amount);
        }

        void ShowReplacementItems(bool show)
        {
            ReplaceBlueprintElements.SetActive(show);
        }

        private GameObject AddUiContainer(GameObject prefab, GameObject parent, string name, string description, System.Action onClickAction, Color overrideColor = default, Sprite placeImage = null)
        {

            var PresetHolder = Util.KInstantiateUI(prefab, parent, true);

            UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
            if (description != null && description.Length > 0)
            {
                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
            }
            if (placeImage != null)
            {
                var image = PresetHolder.transform.Find("Image").FindOrAddComponent<Image>();
                image.sprite = placeImage;
                UnityEngine.Rect rect = image.sprite.rect;
                if (rect.width > rect.height)
                {
                    var size = (rect.height / rect.width) * 55;
                    image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
                else
                {
                    var size = (rect.width / rect.height) * 55;
                    image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }
            }

            PresetHolder.transform.FindOrAddComponent<FButton>().OnClick += onClickAction;
            if (overrideColor != default)
                PresetHolder.transform.Find("Background").FindOrAddComponent<Image>().color = overrideColor;

            return PresetHolder;
        }


        public void ApplyElementsFilter(string filterstring = "")
        {
            foreach (var go in ReplacementElementEntries)
            {
                go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Key.name));
            }
        }

        public void ApplyBlueprintFilter(string filterstring = "")
        {
            foreach (var go in BlueprintEntries)
            {
                go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Key.FriendlyName));
            }
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
                if (text != null && text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
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
            if (!show && onCloseAction != null)
                onCloseAction();
        }

        internal static void RefreshOnBpAdded()
        {
            if(Instance !=null && Instance.CurrentlyActive)
            {
                Instance.ClearUIState();
            }
        }
    }
}

