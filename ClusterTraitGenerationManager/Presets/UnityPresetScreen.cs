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
using static ClusterTraitGenerationManager.STRINGS.UI;
using Satsuma;
using static Operational;
using System.Security.Policy;
using static BestFit;
using YamlDotNet.Serialization;
using static STRINGS.UI.FRONTEND;
using static ClusterTraitGenerationManager.CGSMClusterManager;

namespace ClusterTraitGenerationManager
{
    internal class UnityPresetScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityPresetScreen Instance = null;


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
        CustomClusterSettingsPreset CurrentlySelected;
        ///Referenced Stats to apply presets to.

        Dictionary<CustomClusterSettingsPreset, GameObject> Presets = new Dictionary<CustomClusterSettingsPreset, GameObject>();
        //List<GameObject> InformationObjects = new List<GameObject>();

        Dictionary<int, Tuple<FButton, LocText, Image>> ScheduleBlocks = new Dictionary<int, Tuple<FButton, LocText, Image>>();
        LocText TitleHolder = null;
        Image IsActiveAsDefaultSchedule  = null;
        FButton IsActiveAsDefaultScheduleBtn = null;
        Image IsActiveAsDefaultScheduleBG  = null;

        string RefName;

        public static void ShowWindow(CustomClusterData toLoadFrom, System.Action onClose, string refName = "")
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetScreen, FrontEndManager.Instance.gameObject, true);
                Instance = screen.AddOrGet<UnityPresetScreen>();
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
        private CustomClusterData referencedCluster;

        public void LoadTemporalPreset(CustomClusterData toGenerateFrom)
        {
            referencedCluster = toGenerateFrom;
            CustomClusterSettingsPreset tempStats = CustomClusterSettingsPreset.CreateFromCluster(toGenerateFrom, RefName);
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

                
        public static List<CustomClusterSettingsPreset> LoadPresets()
        {
            List<CustomClusterSettingsPreset> minionStatConfigs = new List<CustomClusterSettingsPreset>();
            var files = new DirectoryInfo(ModAssets.CustomClusterTemplatesPath).GetFiles();

            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                try
                {
                    var preset = CustomClusterSettingsPreset.ReadFromFile(File);
                    if (preset != null)
                    {
                        minionStatConfigs.Add(preset);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load priority preset from: " + File.FullName + ",\nError: " + e.ToString());
                }
            }
            minionStatConfigs= minionStatConfigs.OrderBy(entry => entry.ConfigName).ToList();
            return minionStatConfigs;
        }

        private bool AddUiElementForPreset(CustomClusterSettingsPreset config)
        {
            if (!Presets.ContainsKey(config))
            {
                var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);
                //PresetHolder.transform.Find("TraitImage").gameObject.SetActive(false);
                var img = PresetHolder.transform.Find("TraitImage").GetComponent<Image>();
                //InDefaultListImage(img, config.InDefaultList);

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


                //UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("RenameButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.RENAMEPRESETTOOLTIP);
                //UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("DeleteButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.DELETEPRESETTOOLTIP);
                Presets[config] = PresetHolder;
                return true;
            }
            return false;
        }

        void DeletePreset(CustomClusterSettingsPreset config)
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

           // KMod.Manager.Dialog(Global.Instance.globalCanvas,
           //string.Format(STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.TITLE, config.ConfigName),
           //string.Format(STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.DESC, config.ConfigName),
           //STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.YES,
           //Delete,
           //STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.CANCEL
           //, nothing
           //);
        }

        void SetAsCurrent(CustomClusterSettingsPreset config)
        {
            CurrentlySelected = config;
            RebuildInformationPanel();
        }

        void RebuildInformationPanel()
        {
            if (CurrentlySelected == null)
                return;

            var dbGetter = Db.Get().ScheduleGroups;

            //for (int i = 0; i<CurrentlySelected.ScheduleGroups.Count; ++i)
            //{
            //    var schedule = dbGetter.TryGet(CurrentlySelected.ScheduleGroups[i]);
            //    if (dbGetter.TryGet(CurrentlySelected.ScheduleGroups[i]) == null)
            //    {
            //        SgtLogger.warning("unknown schedule type found, defaulting to worktime");
            //        schedule = dbGetter.Worktime;
            //    }

            //    FButton bt = ScheduleBlocks[i].first;
            //    var setting = ModAssets.GimmeColorForPreset(schedule.Id);

            //    bt.disabledColor = setting.disabledColor;
            //    bt.hoverColor = setting.hoverColor;
            //    bt.normalColor = setting.inactiveColor;
            //    bt.SetInteractable(Presets.ContainsKey(CurrentlySelected));

            //    ScheduleBlocks[i].second.text = 1+i + ". " + schedule.Name;
            //    ScheduleBlocks[i].third.color = setting.inactiveColor;
            //}

            TitleHolder.text = CurrentlySelected.ConfigName;
            GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected));

        }

        void InDefaultListImage(Image img, bool defaultList)
        {
            img.sprite = Assets.GetSprite(defaultList ? "check" : "cancel");
            img.color = defaultList ? Color.green : Color.red;
            //UIUtils.AddSimpleTooltipToObject(img.transform, defaultList ? SCHEDULESTRINGS.DEFAULTYES : SCHEDULESTRINGS.DEFAULTNO, true, onBottom:true);

        }


        private void Init()
        {
            UIUtils.ListAllChildrenPath(this.transform);
            //UIUtils.TryChangeText(transform, "Title", TITLESCHEDULES);
            int i = 1;
            GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrent").FindOrAddComponent<FButton>();
            CloseButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").FindOrAddComponent<FButton>();
            ApplyButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/ApplyPresetButton").FindOrAddComponent<FButton>();

            OpenPresetFolder = transform.Find("HorizontalLayout/ObjectList/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            OpenPresetFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.CustomClusterTemplatesPath) { UseShellExecute = true });

            Searchbar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
            Searchbar.OnValueChanged.AddListener(ApplyFilter);
            Searchbar.Text = string.Empty;


            ClearSearchBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

            ApplyButton.OnClick += () =>
            {
                CurrentlySelected.ApplyPreset();
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


            //UIUtils.AddSimpleTooltipToObject(GeneratePresetButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.GENERATEFROMCURRENT.TOOLTIP);
            //UIUtils.AddSimpleTooltipToObject(CloseButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TOOLTIP);
            //UIUtils.AddSimpleTooltipToObject(ApplyButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.APPLYPRESETBUTTON.TOOLTIP);

            //UIUtils.AddSimpleTooltipToObject(ClearSearchBar.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);
            //UIUtils.AddSimpleTooltipToObject(OpenPresetFolder.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.OPENFOLDERTOOLTIP);

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


            //UIUtils.TryChangeText(IsEnabledHolder.transform, "Label", SCHEDULESTRINGS.MARKEDASDEFAULT); 
            //UIUtils.AddSimpleTooltipToObject(IsEnabledHolder.transform.Find("Label"), SCHEDULESTRINGS.MARKEDASDEFAULTTOOLTIP, true,onBottom:true);
            var spacer = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);


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
