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
using System.Data.SqlClient;
using static ModProfileManager_Addon.STRINGS.UI.PRESETOVERVIEW.FILEHIERARCHY;
using System.Collections;
using static ModProfileManager_Addon.STRINGS.UI.PRESETOVERVIEW.FILEHIERARCHY.BUTTONS;
using static ModProfileManager_Addon.STRINGS.UI.PRESETOVERVIEW.MODENTRYVIEW.BUTTONS;

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

        public FButton NewPreset, ImportPreset, ApplyPreset, EnableAllFromPreset;


        //ProfileListing
        public FInputField2 ModProfileSearchbar;
        public FButton ClearModProfileSearchBar;
        public FButton OpenPresetFolder;

        public GameObject HierarchyContainer;
        public FileHierarchyEntry HierarchyEntryPrefab;
        public FolderHierarchyEntry HierarchyFolderPrefab;
        //public Dictionary<SaveGameModList, FolderHierarchyEntry> FolderEntries = new();
        public Dictionary<ModPresetEntry, FileHierarchyEntry> ModPresetEntries = new();

        //ModView
        public FInputField2 ModEntrySearchbar;
        public Dictionary<string, ModScreenEntry> ModEntryEntries = new();
        public Dictionary<string, string> ModFilterStrings = new();
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

            CloseBtn = transform.Find("TopBar/CloseButton").gameObject.AddOrGet<FButton>();
            CloseBtn.OnClick += () => Show(false);
            //blueprint files

            ModProfileSearchbar = transform.Find("FileHierarchy/SearchBar/Input").FindOrAddComponent<FInputField2>();
            ModProfileSearchbar.OnValueChanged.AddListener(ApplyPresetsFilter);
            ModProfileSearchbar.Text = string.Empty;

            OpenPresetFolder = transform.Find("FileHierarchy/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            OpenPresetFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.ModPacksPath) { UseShellExecute = true });
            UIUtils.AddSimpleTooltipToObject(OpenPresetFolder.gameObject, FOLDERBUTTON.TOOLTIP);

            ClearModProfileSearchBar = transform.Find("FileHierarchy/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearModProfileSearchBar.OnClick += () => ModProfileSearchbar.Text = string.Empty;
            HierarchyContainer = transform.Find("FileHierarchy/ScrollArea/Content").gameObject;

            var hierarchyEntryGO = transform.Find("FileHierarchy/ScrollArea/Content/BlueprintEntryPrefab").gameObject;
            hierarchyEntryGO.SetActive(false);
            HierarchyEntryPrefab = hierarchyEntryGO.AddOrGet<FileHierarchyEntry>();

            var hierarchyFolderGO = transform.Find("FileHierarchy/ScrollArea/Content/FolderPrefab").gameObject;
            hierarchyFolderGO.SetActive(false);
            HierarchyFolderPrefab = hierarchyFolderGO.AddOrGet<FolderHierarchyEntry>();

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

            var savePresetButtonGO = transform.Find("FileHierarchy/Buttons/SaveButton").gameObject;
            NewPreset = savePresetButtonGO.AddComponent<FButton>();
            NewPreset.OnClick += CreateNewNameDialog;
            UIUtils.AddSimpleTooltipToObject(NewPreset.gameObject, SAVEBUTTON.TOOLTIP);

            var importButtonGO = transform.Find("FileHierarchy/Buttons/ImportButton").gameObject;
            ImportPreset = importButtonGO.AddComponent<FButton>();
            Action<string> onConfirmImport = (importString) =>
            {
                ModAssets.ImportPresetFromImportString(importString);
                UpdatePresetButtons();
            };
            ImportPreset.OnClick += () => DialogUtil.CreateTextInputDialog(IMPORT_POPUP.TITLE, parent: FrontEndManager.Instance.gameObject, frontEnd: true, onConfirm: onConfirmImport, maxCharCount: 0, fillerText: UIUtils.ColorText(IMPORT_POPUP.FILLER, Color.grey), high: true, undoStripping: true);
            UIUtils.AddSimpleTooltipToObject(ImportPreset.gameObject, IMPORTBUTTON.TOOLTIP);


            var applyPresetButtonGO = transform.Find("ModEntryView/Buttons/ApplyPreset").gameObject;
            ApplyPreset = applyPresetButtonGO.AddComponent<FButton>();
            ApplyPreset.OnClick += ApplyCurrentPreset;
            UIUtils.AddSimpleTooltipToObject(ApplyPreset.gameObject, APPLYPRESET.TOOLTIP);

            var enableAllButtonGO = transform.Find("ModEntryView/Buttons/ActivateAll").gameObject;
            EnableAllFromPreset = enableAllButtonGO.AddComponent<FButton>();
            EnableAllFromPreset.OnClick += EnableAll;
            UIUtils.AddSimpleTooltipToObject(EnableAllFromPreset.gameObject, ACTIVATEALL.TOOLTIP);

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
            DialogUtil.CreateTextInputDialog(CREATE_POPUP.TITLE, "", null, false, NameAction, null, FrontEndManager.Instance.gameObject, false, false, true);

        }
        void ApplyCurrentPreset() => ApplyCurrentPreset(false);
        void EnableAll() => ApplyCurrentPreset(true);

        void ApplyCurrentPreset(bool dontDisableActives)
        {
            ModAssets.SyncMods(dontDisableActives);
            var mm = Global.Instance.modManager;
            mm.events.Add(new Event() { event_type = EventType.RestartRequested });

            mm.RestartDialog((string)STRINGS.UI.PRESET_APPLIED_TITLE, global::STRINGS.UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.MESSAGE, new System.Action(() => this.Show(false)), true, this.gameObject);
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
            TMP.AddOrUpdateEntryToModList(ModAssets.TMP_PRESET, currentMods, true);

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

            HashSet<string> activeMods = new();
            List<Label> presetMods = new();

            if (ModAssets.SelectedModPack != null)
            {
                presetMods = ModAssets.SelectedModPack.GetActiveMods();
                activeMods = new(ModAssets.SelectedModPack.GetActiveMods().Select(e => e.defaultStaticID));
            }

            foreach (var mod in mods)
            {
                ModAssets.RegisterModMapping(mod);
                if (mod.status == KMod.Mod.Status.NotInstalled || mod.status == KMod.Mod.Status.UninstallPending)
                {
                    RemoveUIMod(mod.label);
                    continue;
                }
                if (mod.HasOnlyTranslationContent() || mod.contentCompatability != ModContentCompatability.OK)
                {
                    continue;
                }
                ModScreenEntry modEntry = AddOrGetModEntry(mod, default);

                bool enabled = ModAssets.SelectedModPack == null ? mod.IsEnabledForActiveDlc() : activeMods.Contains(mod.label.defaultStaticID);
                MPM_POptionDataEntry data = null;


                bool hasPlib = ModAssets.SelectedModPack != null && ModAssets.SelectedModPack.GetActivePlibConfig().TryGetValue(mod.staticID, out data);

                string dataString = null;
                if (data != null)
                    dataString = data.ModConfigData.ToString();
                modEntry.gameObject.SetActive(ShowModByStaticID(mod.label.defaultStaticID));
                modEntry.Refresh(enabled, hasPlib, dataString);
                if (activeMods.Contains(mod.label.defaultStaticID))
                    activeMods.Remove(mod.label.defaultStaticID);
            }
            for (int i = presetMods.Count - 1; i > 0; i--)
            {
                Label potentiallyMissing = presetMods[i];
                if (activeMods.Contains(potentiallyMissing.defaultStaticID))
                {
                    ModScreenEntry modEntryMissing = AddOrGetModEntry(null, potentiallyMissing);

                    bool enabled = true;
                    bool hasPlib = false;
                    modEntryMissing.gameObject.SetActive(ShowModByStaticID(potentiallyMissing.defaultStaticID));
                    modEntryMissing.Refresh(enabled, hasPlib, string.Empty);
                }
                if (ModEntryEntries.TryGetValue(potentiallyMissing.defaultStaticID, out var e))
                {
                    e.transform.SetAsFirstSibling();
                }
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
                bpEntry.OnDeleted = DeletePresetCallback;
                //bpEntry.OnApplyPreset = OnSelectBlueprint;

                ModPresetEntries[entry] = bpEntry;
            }
            return ModPresetEntries[entry];
        }

        private void DeletePresetCallback(ModPresetEntry obj)
        {
            if (ModPresetEntries.TryGetValue(obj, out var go))
            { 
                ModPresetEntries.Remove(obj);
                go.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(go.gameObject);
            }
            CreateTempPreset();
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
        public void RemoveUIEntryForMod(KMod.Label label)
        {
            if (CurrentlyActive)
                StartCoroutine(RemoveUICoroutine(label));
        }
        private IEnumerator RemoveUICoroutine(KMod.Label label)
        {
            yield return null;
            scroll.OnBuild();
            RemoveUIMod(label);
            scroll.Rebuild();
            RebuildModsScreen();
        }
        private void RemoveUIMod(Label label)
        {
            if (ModEntryEntries.TryGetValue(label.defaultStaticID, out var UIEntry))
            {
                ModEntryEntries.Remove(label.defaultStaticID);
                UnityEngine.Object.Destroy(UIEntry.gameObject);
            }
        }
        private ModScreenEntry AddOrGetModEntry(KMod.Mod mod, KMod.Label label)
        {
            if (mod != null)
                label = mod.label;

            if (!ModEntryEntries.ContainsKey(label.defaultStaticID))
            {

                var elementEntry = Util.KInstantiateUI<ModScreenEntry>(ModEntryPrefab.gameObject, ModEntrysContainer);
                elementEntry.TargetMod = mod;
                elementEntry.MissingLabel = label;
                ModEntryEntries[label.defaultStaticID] = elementEntry;
                ModFilterStrings[label.defaultStaticID] = label.title;
            }
            return ModEntryEntries[label.defaultStaticID];
        }

        string ModsFilterString = string.Empty;
        public void ApplyModsFilter(string filterstring = "")
        {
            ModsFilterString = filterstring;
            if (filterstring.Length == 0)
            {
                RebuildModsScreen();
                return;
            }
            scroll.OnBuild();
            foreach (var go in ModEntryEntries)
            {
                go.Value.gameObject.SetActive(ShowModByStaticID(go.Key));
            }
            scroll.Rebuild();
        }
        public bool ShowModByStaticID(string staticModID)
        {
            if (ModFilterStrings.TryGetValue(staticModID, out var titleString))
            {
                return ShowInFilter(ModsFilterString, new string[] { staticModID, titleString });
            }
            else
            {
                return ShowInFilter(ModsFilterString, staticModID);
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
                go.Value.gameObject.SetActive(ShowInFilter(filterstring, new string[] { go.Key.Path, go.Key.ModList.ModlistPath }));
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
            if (CurrentlyActive)
            {
                RebuildModsScreen();
            }
        }

        internal static void RefreshOnBpAdded()
        {
            if (Instance != null && Instance.CurrentlyActive)
            {
                Instance.ClearUIState();
            }
        }

        internal static void NewModInstalled(KMod.Mod mod)
        {
            if (Instance != null)
            {
                Instance.RemoveUIEntryForMod(mod.label);
            }
        }

        internal static void ModUninstalled(KMod.Mod mod)
        {
            if (Instance != null)
            {
                Instance.RemoveUIEntryForMod(mod.label);
            }
        }
    }
}

