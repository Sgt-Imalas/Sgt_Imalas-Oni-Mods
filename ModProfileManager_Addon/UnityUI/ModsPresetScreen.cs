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
using ModProfileManager_Addon.UnityUI.Components;
using UnityEngine.UI;
using System.Diagnostics;
using FMOD;
using KMod;
using System.Globalization;
using static KMod.Testing;
using static ModProfileManager_Addon.SaveGameModList;
using ModProfileManager_Addon.ModProfileData;
using ModProfileManager_Addon.UnityUI.FastTrack_VirtualScroll;
using static KInputController;
using static ModProfileManager_Addon.STRINGS.UI.PRESETOVERVIEW.MODENTRYVIEW;
using static ModProfileManager_Addon.STRINGS.UI.PRESETOVERVIEW;

namespace ModProfileManager_Addon.UnityUI
{
    internal class ModsPresetScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static ModsPresetScreen Instance = null;

        VirtualScroll scroll;
        //Main Areas
        public FButton CloseBtn;

        public FButton NewPreset,ApplyPreset;


        //ProfileListing
        public FInputField2 ModProfileSearchbar;
        public FButton ClearModProfileSearchBar;
        public FButton OpenBlueprintFolder;
        //public FButton FolderUpBtn;
        public GameObject HierarchyContainer;
        public FileHierarchyEntry HierarchyEntryPrefab;
        public FolderHierarchyEntry HierarchyFolderPrefab;
        //public Dictionary<SaveGameModList, FolderHierarchyEntry> FolderEntries = new();
        public Dictionary<ModPresetEntry, FileHierarchyEntry> ModPresetEntries = new();

        //ModView
        public FInputField2 ModEntrySearchbar;
        public Dictionary<KMod.Mod, ModScreenEntry> ModEntryEntries = new();
        public FButton ClearModEntrySearchbar;
        public GameObject ModEntrysContainer;
        public ModScreenEntry ModEntryPrefab;

        System.Action onCloseAction;

        public bool CurrentlyActive;
        public bool DialogueCurrentlyOpen;

        private void Init()
        {
            if (init) { return; }
            SgtLogger.l("Initializing ModPresetWindow");


            UIUtils.ListAllChildrenPath(this.transform);
            CloseBtn = transform.Find("TopBar/CloseButton").gameObject.AddOrGet<FButton>();
            CloseBtn.OnClick += () => Show(false);
            //blueprint files

            ModProfileSearchbar = transform.Find("FileHierarchy/SearchBar/Input").FindOrAddComponent<FInputField2>();
            ModProfileSearchbar.OnValueChanged.AddListener(ApplyPresetsFilter);
            ModProfileSearchbar.Text = string.Empty;

            OpenBlueprintFolder = transform.Find("FileHierarchy/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            OpenBlueprintFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.ModPacksPath) { UseShellExecute = true });

            ClearModProfileSearchBar = transform.Find("FileHierarchy/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearModProfileSearchBar.OnClick += () => ModProfileSearchbar.Text = string.Empty;

            //FolderUpBtn = transform.Find("FileHierarchy/ScrollArea/Content/FolderUp").FindOrAddComponent<FButton>();
            //FolderUpBtn.gameObject.SetActive(false);
            //FolderUpBtn.OnClick += () => SelectFolder(null);

            HierarchyContainer = transform.Find("FileHierarchy/ScrollArea/Content").gameObject;

            //ClearOverrides = transform.Find("MaterialSwitch/Buttons/ResetButton").FindOrAddComponent<FButton>();
            //ClearOverrides.OnClick += OnClearOverrides;

            //PlaceBlueprint = transform.Find("MaterialSwitch/Buttons/PlaceBPbtn").FindOrAddComponent<FButton>();
            //PlaceBlueprint.OnClick += OnPlaceBlueprint;

            var hierarchyEntryGO = transform.Find("FileHierarchy/ScrollArea/Content/BlueprintEntryPrefab").gameObject;
            hierarchyEntryGO.SetActive(false);
            HierarchyEntryPrefab = hierarchyEntryGO.AddOrGet<FileHierarchyEntry>();

            var hierarchyFolderGO = transform.Find("FileHierarchy/ScrollArea/Content/FolderPrefab").gameObject;
            hierarchyFolderGO.SetActive(false);
            HierarchyFolderPrefab = hierarchyFolderGO.AddOrGet<FolderHierarchyEntry>();


            //ElementEntryContainer = transform.Find("MaterialSwitch/ScrollArea/Content").gameObject;
            //MaterialHeaderTitle = transform.Find("MaterialSwitch/MaterialsHeader/Label").gameObject.AddOrGet<LocText>();

            //SevereErrorGO = transform.Find("MaterialSwitch/MaterialsHeader/WarningSevere").gameObject;
            //SevereErrorTooltip = UIUtils.AddSimpleTooltipToObject(SevereErrorGO.transform, MATERIALSWITCH.WARNINGSEVERE);
            //SevereErrorGO.SetActive(false);


            //ErrorGO = transform.Find("MaterialSwitch/MaterialsHeader/Warning").gameObject;
            //ErrorTooltip = UIUtils.AddSimpleTooltipToObject(ErrorGO.transform, MATERIALSWITCH.WARNING);
            //ErrorGO.SetActive(false);

            //var ElementEntryPrefabGo = transform.Find("MaterialSwitch/ScrollArea/Content/PresetEntryPrefab").gameObject;
            //ElementEntryPrefabGo.SetActive(false);
            //ElementEntryPrefab = ElementEntryPrefabGo.AddOrGet<BlueprintElementEntry>();


            ModEntrySearchbar = transform.Find("ModEntryView/SearchBar/Input").FindOrAddComponent<FInputField2>();

            ModEntrySearchbar.OnValueChanged.AddListener(ApplyModsFilter);

            ModEntrySearchbar.Text = string.Empty;


            ClearModEntrySearchbar = transform.Find("ModEntryView/SearchBar/DeleteButton").FindOrAddComponent<FButton>();

            ClearModEntrySearchbar.OnClick += () => ModEntrySearchbar.Text = string.Empty;

            ModEntrysContainer = transform.Find("ModEntryView/ScrollArea/Content").gameObject;

            //ToReplaceName = transform.Find("ModEntryView/ToReplace/CurrentlyActive/Label").gameObject.GetComponent<LocText>();
            //NoItems = transform.Find("MaterialSwitch/ScrollArea/Content/NoElementsInBlueprint")?.gameObject;


            var modEntryPrefabGO = transform.Find("ModEntryView/ScrollArea/Content/Item").gameObject;
            modEntryPrefabGO.SetActive(false);
            ModEntryPrefab = modEntryPrefabGO.AddComponent<ModScreenEntry>();

            var savePresetButtonGO = transform.Find("FileHierarchy/SaveButton").gameObject;
            NewPreset = savePresetButtonGO.AddComponent<FButton>();
            NewPreset.OnClick += CreateNewNameDialog;

            var applyPresetButtonGO = transform.Find("ModEntryView/ApplyPreset").gameObject;
            ApplyPreset = applyPresetButtonGO.AddComponent<FButton>();
            ApplyPreset.OnClick += ApplyCurrentPreset;

            scroll = ModEntrysContainer.AddOrGet<VirtualScroll>();
            scroll.freezeLayout = true;
            scroll.Initialize();

            init = true;
        }

        public void CreateNewNameDialog()
        {
            var NameAction = (string result) =>
            {
                CloneSingleEntryFromExisting(ModAssets.SelectedModPack, result);
                UpdatePresetButtons();
            };
            DialogUtil.CreateTextInputDialog(CREATE_POPUP.TITLE, "", false, NameAction, null, FrontEndManager.Instance.gameObject, false, false, true );
            
        }
        void ApplyCurrentPreset()
        {
            ModAssets.SyncMods();
            var mm = Global.Instance.modManager;
            mm.events.Add(new Event() { event_type = EventType.RestartRequested });
            
            mm.RestartDialog((string)STRINGS.UI.PRESET_APPLIED_TITLE,global::STRINGS.UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.MESSAGE, new System.Action(()=>this.Show(false)), true, this.gameObject);
        }

        public static void ShowWindow(System.Action OnClose)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.ModPresetScreen, FrontEndManager.Instance.gameObject, true);
                Instance = screen.AddOrGet<ModsPresetScreen>();
                Instance.Init();
            }
            Instance.CreateTempPreset();
            Instance.onCloseAction = OnClose;
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.ClearUIState();
        }
        private void CreateTempPreset()
        {
            var mods = Global.Instance.modManager.mods;
            List<KMod.Label> currentMods = new();
            foreach (var mod in mods)
            {
                if (mod.status == KMod.Mod.Status.NotInstalled || mod.status == KMod.Mod.Status.UninstallPending || mod.HasOnlyTranslationContent() || mod.contentCompatability != ModContentCompatability.OK)
                {
                    continue;
                }
                if (mod.IsEnabledForActiveDlc())
                {
                    currentMods.Add(mod.label);
                }
            }
            var TMP = new SaveGameModList();
            TMP.IsModPack = true;
            TMP.ModlistPath = ModAssets.TMP_PRESET;
            TMP.ReferencedColonySaveName = ModAssets.TMP_PRESET;
            TMP.AddOrUpdateEntryToModList(ModAssets.TMP_PRESET, currentMods);

            ModAssets.SelectedModPack = new ModPresetEntry(TMP, ModAssets.TMP_PRESET);
        }

        private void ClearSearchbars()
        {
            ModProfileSearchbar.Text = string.Empty;
            ModEntrySearchbar.Text = string.Empty;
        }

        private bool init;

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                if (!DialogueCurrentlyOpen)
                    this.Show(false);
            }
            if (e.TryConsume(Action.DebugToggleClusterFX))
            {
                ModProfileSearchbar.ExternalStartEditing();
            }

            base.OnKeyDown(e);
        }

        public void ClearUIState()
        {
            ClearSearchbars(); 
            RebuildUI();
        }
        public void RebuildUI()
        {
            UpdatePresetButtons();
            RebuildModsScreen();
        }
        void UpdatePresetButtons()
        {
            foreach (var kvp in ModPresetEntries)
            {
                if (kvp.Value != null)
                    kvp.Value.gameObject.SetActive(false);

            }
            var presets = ModAssets.GetAllModPresets();
            foreach (var preset in presets)
            {
                var uiEntry = AddOrGetPresetEntry(preset);
                uiEntry.transform.SetAsLastSibling();
                uiEntry.gameObject.SetActive(true);
                uiEntry.UpdateSelected();
            }
            this.ConsumeMouseScroll = true;
        }
        
        public void RebuildModsScreen()
        {
            scroll.OnBuild();
            var mods = Global.Instance.modManager.mods;

            HashSet<string> activeMods=new();
            if (ModAssets.SelectedModPack != null)
            {
                activeMods = new(ModAssets.SelectedModPack.GetActiveMods().Select(e => e.defaultStaticID));
            }

            foreach(var mod in mods)
            {
                ModAssets.RegisterModMapping(mod);

                if (mod.status == KMod.Mod.Status.NotInstalled || mod.status == KMod.Mod.Status.UninstallPending || mod.HasOnlyTranslationContent() || mod.contentCompatability != ModContentCompatability.OK)
                {
                    continue; 
                }
                ModScreenEntry modEntry = AddOrGetModEntry(mod);

                bool enabled = ModAssets.SelectedModPack == null ? mod.IsEnabledForActiveDlc() : activeMods.Contains(mod.label.defaultStaticID);
                MPM_POptionDataEntry data = null;


                bool hasPlib = ModAssets.SelectedModPack != null && ModAssets.SelectedModPack.GetActivePlibConfig().TryGetValue(mod.staticID, out data);

                string dataString=null;
                if (data != null)
                    dataString = data.ModConfigData.ToString();
                modEntry.transform.SetAsLastSibling();
                modEntry.gameObject.SetActive(true);
                modEntry.Refresh(enabled, hasPlib, dataString);
            }
            scroll.Rebuild();
        }

        //public void SelectFolder(SaveGameModList folder)
        //{
        //    ModAssets.ProfileFolder = folder;
        //    UpdateBlueprintButtons();
        //}
        //private FolderHierarchyEntry AddOrGetFolderEntry(SaveGameModList folder)
        //{
        //    if (!FolderEntries.ContainsKey(folder))
        //    {
        //        var folderEntry = Util.KInstantiateUI<FolderHierarchyEntry>(HierarchyFolderPrefab.gameObject, HierarchyContainer);
        //        folderEntry.folder = folder;
        //        //folderEntry.Name = folder.Name;
        //        folderEntry.OnEntryClicked += () => SelectFolder(folder);
        //        FolderEntries[folder] = folderEntry;
        //    }
        //    return FolderEntries[folder];
        //}

        private FileHierarchyEntry AddOrGetPresetEntry(ModPresetEntry entry)
        {
            if (!ModPresetEntries.ContainsKey(entry))
            {
                var bpEntry = Util.KInstantiateUI<FileHierarchyEntry>(HierarchyEntryPrefab.gameObject, HierarchyContainer);
                bpEntry.ModProfile = entry;
                bpEntry.RefreshUI = RebuildUI;
                bpEntry.OnDialogueToggled = DialogueOpen;
                //bpEntry.OnApplyPreset = OnSelectBlueprint;

                ModPresetEntries[entry] = bpEntry;
            }
            return ModPresetEntries[entry];
        }

        //void OnPlaceBlueprint()
        //{
        //    ModAssets.SelectedBlueprint = TargetBlueprint;
        //    TargetBlueprint = null;
        //    Show(false);
        //}

        //void OnSelectBlueprint(Blueprint bp)
        //{
        //    TargetBlueprint = bp;
        //    SetMaterialState();
        //}

        private void DialogueOpen(bool obj)
        {
            DialogueCurrentlyOpen = obj;
        }


        private ModScreenEntry AddOrGetModEntry(KMod.Mod mod)
        {
            if (!ModEntryEntries.ContainsKey(mod))
            {
                var elementEntry = Util.KInstantiateUI<ModScreenEntry>(ModEntryPrefab.gameObject, ModEntrysContainer);
                elementEntry.TargetMod = mod;
                ModEntryEntries[mod] = elementEntry;
            }
            return ModEntryEntries[mod];
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


        public void ApplyModsFilter(string filterstring = "")
        {
            if (filterstring.Length == 0)
            {
                RebuildModsScreen();
                return;
            }
            foreach (var go in ModEntryEntries)
            {
                scroll.OnBuild();
                go.Value.gameObject.SetActive(ShowInFilter(filterstring, new string[] { go.Key.title, go.Key.description }));
                scroll.Rebuild();
            }

        }

        public void ApplyPresetsFilter(string filterstring = "")
        {
            if (filterstring.Length == 0)
            {
                UpdatePresetButtons();
                return;
            }

            foreach (var go in ModPresetEntries)
            {
                go.Value.gameObject.SetActive( ShowInFilter(filterstring, new string[]{ go.Key.Path, go.Key.ModList.ModlistPath }));
            }
        }

        bool ShowInFilter(string filtertext, string stringsToInclude)
        {
            return ShowInFilter(filtertext, new string[] { stringsToInclude });
        }

        bool ShowInFilter(string filtertext, string[] stringsToInclude)
        {
            filtertext = filtertext.ToLowerInvariant();

            foreach (var text in stringsToInclude)
            {
                if (text != null && text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
                {
                    return true;
                }
            }
            return false;
        }


        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!init)
            {
                Init();
            }
            CurrentlyActive = show;
            if (!show)
            {
                if (onCloseAction != null)
                    onCloseAction();
                
            }
        }

        internal static void RefreshOnBpAdded()
        {
            if (Instance != null && Instance.CurrentlyActive)
            {
                Instance.ClearUIState();
            }
        }
    }
}

