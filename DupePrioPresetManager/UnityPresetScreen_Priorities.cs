using Database;
using Epic.OnlineServices.Sessions;
using FMOD;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static ResearchTypes;
using static SandboxSettings;
using static STRINGS.DUPLICANTS;
using static STRINGS.DUPLICANTS.CHORES;
using static STRINGS.UI.DETAILTABS.PERSONALITY.RESUME;
using static DupePrioPresetManager.STRINGS.UI.PRESETWINDOWDUPEPRIOS;
using Satsuma;
using static Operational;

namespace DupePrioPresetManager
{
    internal class UnityPresetScreen_Priorities : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityPresetScreen_Priorities Instance = null;


        public FButton GeneratePresetButton;
        public FButton CloseButton;
        public FButton ApplyButton;

        public GameObject InfoHeaderPrefab;
        public GameObject InfoRowPrefab;
        public GameObject InfoSpacer;
        public GameObject InfoScreenContainer;

        public GameObject PresetListContainer;
        public GameObject PresetListPrefab;

        public FButton OpenPresetFolder;
        public FButton ClearSearchBar;
        public FInputField2 Searchbar;

        public bool CurrentlyActive=false;
        private bool HoveringPrio = false;

        ///Preset
        MinionPrioPreset CurrentlySelected;
        ///Referenced Stats to apply presets to.
        IPersonalPriorityManager ReferencedPriorityManager = null;


        Dictionary<MinionPrioPreset, GameObject> Presets = new Dictionary<MinionPrioPreset, GameObject>();
        List<GameObject> InformationObjects = new List<GameObject>();

        string RefName;

        public static void ShowWindow(IPersonalPriorityManager priorityManager, System.Action onClose, string refName = "")
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<UnityPresetScreen_Priorities>();
                Instance.Init();
            }
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.LoadAllPresets();
            Instance.RefName = refName;
            Instance.LoadTemporalPreset(priorityManager);
            Instance.ReferencedPriorityManager = priorityManager;
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;
        }

        private bool init;
        private System.Action OnCloseAction;

        public void LoadTemporalPreset(IPersonalPriorityManager toGenerateFrom)
        {
            MinionPrioPreset tempStats = MinionPrioPreset.CreateFromPriorityManager(toGenerateFrom, RefName);
            SetAsCurrent(tempStats);
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.MouseRight))
            {
                if (!HoveringPrio)
                {
                    this.Show(false);
                }
            }
            if (e.TryConsume(Action.Escape))
            {
                this.Show(false);
            }
            base.OnKeyDown(e);
        }

        void LoadAllPresets()
        {
            foreach (var existing in Presets.Values)
            {
                Destroy(existing.gameObject);
            }
            Presets.Clear();
            foreach (var loadedPreset in LoadPresets())
            {
                AddUiElementForPreset(loadedPreset);
            }
        }

        List<MinionPrioPreset> LoadPresets()
        {
            List<MinionPrioPreset> minionStatConfigs = new List<MinionPrioPreset>();
            var files = new DirectoryInfo(ModAssets.DupeTemplatePath).GetFiles();


            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                try
                {
                    var preset = MinionPrioPreset.ReadFromFile(File);
                    if (preset != null)
                    {
                        minionStatConfigs.Add(preset);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load priority preset from: " + File.FullName + ",\nError: " + e);
                }
            }
            return minionStatConfigs;
        }

        private bool AddUiElementForPreset(MinionPrioPreset config)
        {
            if (!Presets.ContainsKey(config))
            {
                var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);
                PresetHolder.transform.Find("TraitImage").gameObject.SetActive(false);

                UIUtils.TryChangeText(PresetHolder.transform, "Label", config.ConfigName);
                PresetHolder.transform.Find("RenameButton").FindOrAddComponent<FButton>().OnClick +=
                    () => config.OpenPopUpToChangeName(
                        () =>
                            {
                                UIUtils.TryChangeText(PresetHolder.transform, "Label", config.ConfigName);
                                RebuildInformationPanel();
                            }
                        );

                PresetHolder.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>().OnClick += () => SetAsCurrent(config);
                PresetHolder.transform.Find("DeleteButton").FindOrAddComponent<FButton>().OnClick += () => DeletePreset(config);

                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("RenameButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.RENAMEPRESETTOOLTIP);
                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("DeleteButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.DELETEPRESETTOOLTIP);
                Presets[config] = PresetHolder;
                return true;
            }
            return false;
        }

        void DeletePreset(MinionPrioPreset config)
        {
            System.Action Delete = () =>
            {
                if (Presets.ContainsKey(config))
                {
                    Destroy(Presets[config]);
                    Presets.Remove(config);
                    config.DeleteFile();
                }
            };
            System.Action nothing = () =>
            { };

            KMod.Manager.Dialog(Global.Instance.globalCanvas,
           string.Format(STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.TITLE, config.ConfigName),
           string.Format(STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.DESC, config.ConfigName),
           STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.YES,
           Delete,
           STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.CANCEL
           , nothing
           );
        }

        void SetAsCurrent(MinionPrioPreset config)
        {
            CurrentlySelected = config;
            RebuildInformationPanel();
        }
        void RebuildInformationPanel()
        {
            SgtLogger.l("rebuilding UI start");
            for (int i = InformationObjects.Count - 1; i >= 0; i--)
            {
                Destroy(InformationObjects[i]);
            }
            if (CurrentlySelected == null)
                return;

            var Name = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(Name.transform, "Label", "\"" + CurrentlySelected.ConfigName + "\"");
            InformationObjects.Add(Name);

            var dbChoreGroups = Db.Get().ChoreGroups;
            foreach (var priority in CurrentlySelected.ChoreGroupPriorities)
            {
                if (dbChoreGroups.TryGet(priority.Key) != null)
                {
                    ChoreGroup choreGroup = dbChoreGroups.TryGet(priority.Key);

                    var choreGroupPriorityItem = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                    UIUtils.TryChangeText(choreGroupPriorityItem.transform, "Label", ChoreGroupName(choreGroup));
                    UIUtils.AddSimpleTooltipToObject(choreGroupPriorityItem.transform.Find("Label"), choreGroup.description, true);
                    if(choreGroupPriorityItem.transform.Find("Label/TraitImage").TryGetComponent<Image>(out var image))
                    {
                        image.sprite = Assets.GetSprite(choreGroup.sprite);
                    }

                    if (choreGroupPriorityItem.transform.Find("AddThisTraitButton/image").TryGetComponent<Image>(out var prioimage))
                    {
                        prioimage.sprite = GetPriorityInfo(priority.Value).sprite;
                        prioimage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                    }
                    UIUtils.AddSimpleTooltipToObject(choreGroupPriorityItem.transform.Find("AddThisTraitButton"), GetPriorityStr(priority.Value), true);

                    var PrioChangeBtn = choreGroupPriorityItem.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>();
                    PrioChangeBtn.allowRightClick = true;
                    PrioChangeBtn.OnClick += () => { CurrentlySelected.ChangeValue(choreGroup, 1); RebuildInformationPanel(); };
                    PrioChangeBtn.OnRightClick += () => { CurrentlySelected.ChangeValue(choreGroup, -1); RebuildInformationPanel(); };
                    PrioChangeBtn.OnPointerEnterAction += () => this.HoveringPrio = true;
                    PrioChangeBtn.OnPointerExitAction += () => this.HoveringPrio = false;
                    PrioChangeBtn.SetInteractable(Presets.ContainsKey(CurrentlySelected));

                    InformationObjects.Add(choreGroupPriorityItem);
                }
            }
            GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected));
        }

        private LocString GetPriorityStr(int priority)
        {
            priority = Mathf.Clamp(priority, 0, 5);
            LocString priorityStr = (LocString)null;
            foreach (JobsTableScreen.PriorityInfo priorityInfo in JobsTableScreen.priorityInfo)
            {
                if (priorityInfo.priority == priority)
                    priorityStr = priorityInfo.name;
            }
            return priorityStr;
        }
        private JobsTableScreen.PriorityInfo GetPriorityInfo(int priority)
        {
            JobsTableScreen.PriorityInfo priorityInfo = new JobsTableScreen.PriorityInfo();
            for (int index = 0; index < JobsTableScreen.priorityInfo.Count; ++index)
            {
                if (JobsTableScreen.priorityInfo[index].priority == priority)
                {
                    priorityInfo = JobsTableScreen.priorityInfo[index];
                    break;
                }
            }
            return priorityInfo;
        }


        public string ChoreGroupName(ChoreGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + group.Id.ToUpperInvariant() + ".NAME");
        }
        public string ChoreGroupTooltip(ChoreGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + group.Id.ToUpperInvariant() + ".DESC");
        }

        private void Init()
        {
            SgtLogger.l("Initializing PresetWindow");
            UIUtils.ListAllChildrenPath(transform);

            GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrent").FindOrAddComponent<FButton>();
            CloseButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").FindOrAddComponent<FButton>();
            ApplyButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/ApplyPresetButton").FindOrAddComponent<FButton>();

            OpenPresetFolder = transform.Find("HorizontalLayout/ObjectList/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            OpenPresetFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.DupeTemplatePath) { UseShellExecute = true });


            Searchbar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
            Searchbar.OnValueChanged.AddListener(ApplyFilter);
            Searchbar.Text = string.Empty;


            ClearSearchBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

            ApplyButton.OnClick += () =>
            {
                CurrentlySelected.ApplyPreset(ReferencedPriorityManager);
                this.OnCloseAction.Invoke();
                this.Show(false);
            };
            ///OpenFolder

            CloseButton.OnClick += () => this.Show(false);
            GeneratePresetButton.OnClick += () =>
            {
                bool added = AddUiElementForPreset(CurrentlySelected);
                if (added)
                {
                    CurrentlySelected.WriteToFile();
                    CurrentlySelected.OpenPopUpToChangeName(
                            () =>
                                {
                                    if(this.CurrentlyActive && Presets[CurrentlySelected] != null)
                                    {
                                        UIUtils.TryChangeText(Presets[CurrentlySelected].transform, "Label", CurrentlySelected.ConfigName);
                                        RebuildInformationPanel();
                                    }
                                }
                            );
                    RebuildInformationPanel();
                }
            };


            UIUtils.AddSimpleTooltipToObject(GeneratePresetButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.GENERATEFROMCURRENT.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(CloseButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(ApplyButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.APPLYPRESETBUTTON.TOOLTIP);

            UIUtils.AddSimpleTooltipToObject(ClearSearchBar.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);
            UIUtils.AddSimpleTooltipToObject(OpenPresetFolder.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.OPENFOLDERTOOLTIP);

            InfoHeaderPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/HeaderPrefab").gameObject;
            InfoRowPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ListViewEntryPrefab").gameObject;
            InfoSpacer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ItemPrefab").gameObject;
            UIUtils.FindAndDestroy(InfoSpacer.transform, "Label");
            InfoScreenContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content").gameObject;
            PresetListContainer = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content").gameObject;
            PresetListPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content/PresetEntryPrefab").gameObject;


            init = true;
        }

        public void ApplyFilter(string filterstring = "")
        {
            foreach (var go in Presets)
            {
                go.Value.SetActive(filterstring == string.Empty ? true : go.Key.ConfigName.ToLowerInvariant().Contains(filterstring.ToLowerInvariant()));
            }
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!init)
            {
                Init();
            }

            if (show)
            {
                CurrentlyActive = show;
            }
            else
            {
                DeactivateStatusWithDelay(600);
            }
        }
        async Task DeactivateStatusWithDelay(int ms)
        {
            await Task.Delay(ms);
            CurrentlyActive = false;
        }
    }
}
