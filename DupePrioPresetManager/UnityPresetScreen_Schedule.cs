using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static DupePrioPresetManager.STRINGS.UI.PRESETWINDOWDUPEPRIOS;
using static STRINGS.UI.FRONTEND;

namespace DupePrioPresetManager
{
    internal class UnityPresetScreen_Schedule : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityPresetScreen_Schedule Instance = null;


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

        public bool CurrentlyActive = false;
        private bool HoveringPrio = false;

        ///Preset
        ScheduleSettingsPreset CurrentlySelected;
        ///Referenced Stats to apply presets to.

        Dictionary<ScheduleSettingsPreset, GameObject> Presets = new Dictionary<ScheduleSettingsPreset, GameObject>();
        //List<GameObject> InformationObjects = new List<GameObject>();

        Dictionary<int, Tuple<FButton, LocText, Image>> ScheduleBlocks = new Dictionary<int, Tuple<FButton, LocText, Image>>();
        LocText TitleHolder = null;
        Image IsActiveAsDefaultSchedule = null;
        FButton IsActiveAsDefaultScheduleBtn = null;
        Image IsActiveAsDefaultScheduleBG = null;

        string RefName;

        public static void ShowWindow(Schedule toLoadFrom, System.Action onClose, string refName = "")
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<UnityPresetScreen_Schedule>();
                Instance.Init();
            }
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.LoadAllPresets();
            Instance.RefName = refName;
            Instance.LoadTemporalPreset(toLoadFrom);
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;
        }

        private bool init;
        private System.Action OnCloseAction;
        private Schedule referencedSchedule;

        public void LoadTemporalPreset(Schedule toGenerateFrom)
        {
            referencedSchedule = toGenerateFrom;
            ScheduleSettingsPreset tempStats = ScheduleSettingsPreset.CreateFromSchedule(toGenerateFrom, RefName);
            SetAsCurrent(tempStats);
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.MouseRight))
            {
                //if (!HoveringPrio)
                //{
                //    this.Show(false);
                //}
            }
            if (e.TryConsume(Action.Escape))
            {
                this.Show(false);
            }
            if (e.TryConsume(Action.DebugToggleClusterFX))
            {
                Searchbar.ExternalStartEditing();
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

        public static void GenerateAllDefaultPresets()
        {
            List<string> ExistingSchedules = ScheduleManager.Instance.GetSchedules().Select(s => s.name).ToList();
            var dbAllGroups = Db.Get().ScheduleGroups.allGroups;
            var ToGenerates = new List<ScheduleSettingsPreset>();
            int omitCounter = 0;
            foreach (var Preset in LoadPresets())
            {
                if (!Preset.InDefaultList)
                    continue;
                if (ExistingSchedules.Contains(Preset.ConfigName))
                {
                    omitCounter++;
                    continue;
                }
                ToGenerates.Add(Preset);
            }
            if (ToGenerates.Count == 0)
            {
                KMod.Manager.Dialog(GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay),
               SCHEDULESTRINGS.GENERATEALL,
               SCHEDULESTRINGS.ALLGENERATED,
               SAVESCREEN.CANCELNAME,
                () => { });
                return;
            }

            var generateAction = () =>
            {
                foreach (var Preset in ToGenerates)
                {
                    Preset.ApplyPreset(ScheduleManager.Instance.AddSchedule(dbAllGroups, Preset.ConfigName, false));
                }
            };

            string Text = string.Format(SCHEDULESTRINGS.GENERATEALLCONFIRM, ToGenerates.Count);
            if (omitCounter > 0)
            {
                Text += string.Format(SCHEDULESTRINGS.OMITNUMBER, omitCounter);
            }

            KMod.Manager.Dialog(Global.Instance.globalCanvas,
               SCHEDULESTRINGS.GENERATEALL,
                Text,
               SAVESCREEN.CONFIRMNAME,
               generateAction,
               SAVESCREEN.CANCELNAME
               , () => { }
               );
        }
        public static List<ScheduleSettingsPreset> LoadPresets()
        {
            List<ScheduleSettingsPreset> minionStatConfigs = new List<ScheduleSettingsPreset>();
            var files = new DirectoryInfo(ModAssets.ScheduleTemplatePath).GetFiles();


            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                try
                {
                    var preset = ScheduleSettingsPreset.ReadFromFile(File);
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
            minionStatConfigs = minionStatConfigs.OrderBy(entry => entry.ConfigName).ToList();
            return minionStatConfigs;
        }

        private bool AddUiElementForPreset(ScheduleSettingsPreset config)
        {
            if (!Presets.ContainsKey(config))
            {
                var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);
                //PresetHolder.transform.Find("TraitImage").gameObject.SetActive(false);
                var img = PresetHolder.transform.Find("TraitImage").GetComponent<Image>();
                InDefaultListImage(img, config.InDefaultList);

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

        void DeletePreset(ScheduleSettingsPreset config)
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

        void SetAsCurrent(ScheduleSettingsPreset config)
        {
            CurrentlySelected = config;
            RebuildInformationPanel();
        }

        void RebuildInformationPanel()
        {
            if (CurrentlySelected == null)
                return;

            var dbGetter = Db.Get().ScheduleGroups;

            foreach(var entry in ScheduleBlocks)
            {
                entry.Value.first.transform.parent.gameObject.SetActive(false);
            }

            for (int i = 0; i < CurrentlySelected.ScheduleGroups.Count; ++i)
            {
                var schedule = dbGetter.TryGet(CurrentlySelected.ScheduleGroups[i]);
                if (dbGetter.TryGet(CurrentlySelected.ScheduleGroups[i]) == null)
                {
                    SgtLogger.warning("unknown schedule type found, defaulting to worktime");
                    schedule = dbGetter.Worktime;
                }

                var scheduleblock = GetScheduleBlock(i);

                FButton bt = ScheduleBlocks[i].first;
                bt.transform.parent.gameObject.SetActive(true);

                bt.disabledColor = schedule.uiColor;
                bt.hoverColor = schedule.uiColor;
                bt.normalColor = schedule.uiColor;
                bt.SetInteractable(Presets.ContainsKey(CurrentlySelected));

                ScheduleBlocks[i].second.text = 1 + i + ". " + schedule.Name;
                ScheduleBlocks[i].third.color = schedule.uiColor;
            }

            TitleHolder.text = CurrentlySelected.ConfigName;
            IsActiveAsDefaultScheduleBtn.SetInteractable(Presets.ContainsKey(CurrentlySelected));
            GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected));

            ToggleActiveInDefaults(false);
        }

        void ToggleActiveInDefaults(bool DoToggle = true)
        {
            if (DoToggle)
                CurrentlySelected.ToggleActiveInDefault();

            if (Presets.ContainsKey(CurrentlySelected))
            {
                var img = Presets[CurrentlySelected].transform.Find("TraitImage").GetComponent<Image>();
                InDefaultListImage(img, CurrentlySelected.InDefaultList);
            }
            IsActiveAsDefaultSchedule.gameObject.SetActive(CurrentlySelected.InDefaultList);
            IsActiveAsDefaultScheduleBG.color = CurrentlySelected.InDefaultList ? Color.green : Color.red;
            //SetAllowedSprite(!CurrentlySelected.ForbiddenTags.Contains(id.ToTag()), image);
        }
        void InDefaultListImage(Image img, bool defaultList)
        {
            img.sprite = Assets.GetSprite(defaultList ? "check" : "cancel");
            img.color = defaultList ? Color.green : Color.red;
            UIUtils.AddSimpleTooltipToObject(img.transform, defaultList ? SCHEDULESTRINGS.DEFAULTYES : SCHEDULESTRINGS.DEFAULTNO, true, onBottom: true);

        }

        void ChangeValue(int index, Image image, bool increase)
        {
            CurrentlySelected.DeltaBlock(index, increase);
            RebuildInformationPanel();
            //SetAllowedSprite(!CurrentlySelected.ForbiddenTags.Contains(id.ToTag()), image);
        }

        private void Init()
        {
            //UIUtils.ListAllChildrenPath(this.transform);
            UIUtils.TryChangeText(transform, "Title", TITLESCHEDULES);


            GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrent").FindOrAddComponent<FButton>();
            CloseButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").FindOrAddComponent<FButton>();
            ApplyButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/ApplyPresetButton").FindOrAddComponent<FButton>();

            OpenPresetFolder = transform.Find("HorizontalLayout/ObjectList/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            OpenPresetFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.ScheduleTemplatePath) { UseShellExecute = true });

            Searchbar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
            Searchbar.OnValueChanged.AddListener(ApplyFilter);
            Searchbar.Text = string.Empty;


            ClearSearchBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

            ApplyButton.OnClick += () =>
            {
                CurrentlySelected.ApplyPreset(referencedSchedule);
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
                                    if (this.CurrentlyActive && Presets[CurrentlySelected] != null)
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


            var Name = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(Name.transform, "Label", "\"" + CurrentlySelected.ConfigName + "\"");
            TitleHolder = Name.transform.Find("Label").GetComponent<LocText>();

            var IsEnabledHolder = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            if (IsEnabledHolder.transform.Find("AddThisTraitButton/image").TryGetComponent<Image>(out var fg))
            {
                fg.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
                fg.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
                fg.sprite = Assets.GetSprite("overview_jobs_icon_checkmark");
                fg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
                IsActiveAsDefaultSchedule = fg;
            }
            if (IsEnabledHolder.transform.Find("Label/TraitImage").TryGetComponent<Image>(out var image))
            {
                image.gameObject.SetActive(false);
            }
            UIUtils.TryChangeText(IsEnabledHolder.transform, "Label", SCHEDULESTRINGS.MARKEDASDEFAULT);
            UIUtils.AddSimpleTooltipToObject(IsEnabledHolder.transform.Find("Label"), SCHEDULESTRINGS.MARKEDASDEFAULTTOOLTIP, true, onBottom: true);

            if (IsEnabledHolder.transform.Find("AddThisTraitButton").TryGetComponent<Image>(out var bg))
            {
                IsActiveAsDefaultScheduleBG = bg;
            }
            var bt = IsEnabledHolder.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>();
            bt.OnClick += () => ToggleActiveInDefaults();
            IsActiveAsDefaultScheduleBtn = bt;

            var spacer = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);

            init = true;
        }
        public Tuple<FButton, LocText, Image> GetScheduleBlock(int index)
        {
            if (!ScheduleBlocks.ContainsKey(index))
            {
                var ScheduleBlockHolder = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                if (ScheduleBlockHolder.transform.Find("Label/TraitImage").TryGetComponent<Image>(out var traitimg))
                {
                    traitimg.gameObject.SetActive(false);
                }

                UIUtils.TryChangeText(ScheduleBlockHolder.transform, "Label", "1");
                //UIUtils.AddSimpleTooltipToObject(ConsumableAllowedItem.transform.Find("Label"), descHolder.description, true);

                ScheduleBlockHolder.transform.Find("AddThisTraitButton/image").TryGetComponent<Image>(out var SunMoonImg);
                if (index % 24 < 5)
                {
                    SunMoonImg.sprite = Assets.GetSprite("schedule_early");
                    SunMoonImg.color = Color.white;
                }
                else if (index % 24 > 20)
                {
                    SunMoonImg.color = Color.white;
                    SunMoonImg.sprite = Assets.GetSprite("schedule_night");
                }
                else
                {
                    SunMoonImg.gameObject.SetActive(false);
                }

                ScheduleBlockHolder.transform.Find("AddThisTraitButton").TryGetComponent<Image>(out var prioimage);
                ScheduleBlockHolder.transform.Find("Label").TryGetComponent<LocText>(out var Description);

                var BlockChangeBTN = ScheduleBlockHolder.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>();
                BlockChangeBTN.allowRightClick = true;
                BlockChangeBTN.OnClick += () => ChangeValue(index, prioimage, true);
                BlockChangeBTN.OnRightClick += () => ChangeValue(index, prioimage, false);
                BlockChangeBTN.OnPointerEnterAction += () => this.HoveringPrio = true;
                BlockChangeBTN.OnPointerExitAction += () => this.HoveringPrio = false;

                ScheduleBlocks[index] = new Tuple<FButton, LocText, Image>(BlockChangeBTN, Description, prioimage);
            }
            return ScheduleBlocks[index];
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
