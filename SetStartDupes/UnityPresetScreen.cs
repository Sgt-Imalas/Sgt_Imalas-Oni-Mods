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
using static SetStartDupes.STRINGS.UI.PRESETWINDOW;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.OBJECTLIST;
using static STRINGS.DUPLICANTS;
using static STRINGS.DUPLICANTS.CHORES;
using static STRINGS.UI.DETAILTABS.PERSONALITY.RESUME;

namespace SetStartDupes
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

        public bool CurrentlyActive;

        ///Preset
        MinionStatConfig CurrentlySelected;
        ///Referenced Stats to apply presets to.
        MinionStartingStats ReferencedStats = null;


        Dictionary<MinionStatConfig, GameObject> Presets = new Dictionary<MinionStatConfig, GameObject>();
        List<GameObject> InformationObjects = new List<GameObject>();



        public static void ShowWindow(MinionStartingStats startingStats, System.Action onClose)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<UnityPresetScreen>();
                Instance.Init();
            }
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.LoadAllPresets();
            Instance.LoadTemporalPreset(startingStats);
            Instance.ReferencedStats = startingStats;
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;
        }

        private bool init;
        private System.Action OnCloseAction;

        public void LoadTemporalPreset(MinionStartingStats toGenerateFrom)
        {
            MinionStatConfig tempStats = MinionStatConfig.CreateFromStartingStats(toGenerateFrom);
            SetAsCurrent(tempStats);
        }

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

        List<MinionStatConfig> LoadPresets()
        {
            List<MinionStatConfig> minionStatConfigs = new List<MinionStatConfig>();
            var files = new DirectoryInfo(ModAssets.DupeTemplatePath).GetFiles();


            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                try
                {
                    var preset = MinionStatConfig.ReadFromFile(File);
                    if (preset != null)
                    {
                        minionStatConfigs.Add(preset);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load minion preset from: " + File.FullName + ", Error: " + e);
                }
            }
            return minionStatConfigs;
        }

        private bool AddUiElementForPreset(MinionStatConfig config)
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
                PresetHolder.transform
                    //.Find("AddThisTraitButton")
                    .FindOrAddComponent<FButton>().OnClick += () => SetAsCurrent(config);
                PresetHolder.transform.Find("DeleteButton").FindOrAddComponent<FButton>().OnClick += () => DeletePreset(config);

                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("RenameButton"), SCROLLAREA.CONTENT.PRESETENTRYPREFAB.RENAMEPRESETTOOLTIP);
                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("DeleteButton"), SCROLLAREA.CONTENT.PRESETENTRYPREFAB.DELETEPRESETTOOLTIP);
                Presets[config] = PresetHolder;
                return true;
            }
            return false;
        }

        void DeletePreset(MinionStatConfig config)
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
           string.Format(DELETEWINDOW.TITLE, config.ConfigName),
           string.Format(DELETEWINDOW.DESC, config.ConfigName),
           DELETEWINDOW.YES,
           Delete,
           DELETEWINDOW.CANCEL
           , nothing
           );
        }

        void SetAsCurrent(MinionStatConfig config)
        {
            CurrentlySelected = config;
            RebuildInformationPanel();
        }
        void RebuildInformationPanel()
        {
            for (int i = InformationObjects.Count - 1; i >= 0; i--)
            {
                Destroy(InformationObjects[i]);
            }
            if (CurrentlySelected == null)
                return;

            var Name = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(Name.transform, "Label", "\"" + CurrentlySelected.ConfigName + "\"");
            InformationObjects.Add(Name);

            var spacer4 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            InformationObjects.Add(spacer4);

            var aptitudeHeader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(aptitudeHeader.transform, "Label", global::STRINGS.UI.CHARACTERCONTAINER_APTITUDES_TITLE + ":");
            InformationObjects.Add(aptitudeHeader);

            //SgtLogger.l("redoing aptitude vis");
            foreach (var skill in CurrentlySelected.skillAptitudes)
            {
                //if (skill.Value < 1)
                //    continue;

                var aptitude = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                UIUtils.TryChangeText(aptitude.transform, "Label", CurrentlySelected.SkillGroupName(skill.Key));
                UIUtils.AddSimpleTooltipToObject(aptitude.transform, CurrentlySelected.SkillGroupDesc(skill.Key), true);
                InformationObjects.Add(aptitude);

            }

            var spacer3 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            InformationObjects.Add(spacer3);

            var traitHeader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(traitHeader.transform, "Label", global::STRINGS.UI.CHARACTERCONTAINER_TRAITS_TITLE + ":");
            InformationObjects.Add(traitHeader);


            var traits = Db.Get().traits;
            //SgtLogger.l("redoing trait vis");
            foreach (var trait in CurrentlySelected.Traits)
            {
                if (trait == MinionConfig.MINION_BASE_TRAIT_ID)
                    continue;

                var traitcon = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                UIUtils.TryChangeText(traitcon.transform, "Label", ModAssets.GetTraitName(traits.TryGet(trait)));
                UIUtils.AddSimpleTooltipToObject(traitcon.transform, ModAssets.GetTraitTooltip(traits.TryGet(trait), trait), true);
                InformationObjects.Add(traitcon);
                ApplyColorToTraitContainer(traitcon, trait);
            }

            var spacer2 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            InformationObjects.Add(spacer2);

            var joyheader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(joyheader.transform, "Label", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_JOYTRAIT, string.Empty));
            InformationObjects.Add(joyheader);


            var joy = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(joy.transform, "Label", ModAssets.GetTraitName(traits.TryGet(CurrentlySelected.joyTrait)));
            UIUtils.AddSimpleTooltipToObject(joy.transform, ModAssets.GetTraitTooltip(traits.TryGet(CurrentlySelected.joyTrait), CurrentlySelected.joyTrait), true);
            InformationObjects.Add(joy);
            ApplyColorToTraitContainer(joy, CurrentlySelected.joyTrait);

            var stressheader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(stressheader.transform, "Label", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_STRESSTRAIT, string.Empty));
            InformationObjects.Add(stressheader);

            var stress = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(stress.transform, "Label", ModAssets.GetTraitName(traits.TryGet(CurrentlySelected.stressTrait)));
            InformationObjects.Add(stress);
            UIUtils.AddSimpleTooltipToObject(stress.transform, ModAssets.GetTraitTooltip(traits.TryGet(CurrentlySelected.stressTrait), CurrentlySelected.stressTrait), true);
            ApplyColorToTraitContainer(stress, CurrentlySelected.stressTrait);


            GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected));
        }
        

        void ApplyColorToTraitContainer(GameObject container, string traitID)
        {

            var type = ModAssets.GetTraitListOfTrait(traitID, out var list);
            container.FindOrAddComponent<Image>().color = ModAssets.GetColourFromType(type);
        }



        private void Init()
        {
            SgtLogger.l("Initializing PresetWindow");

            //UIUtils.ListAllChildrenPath(transform);
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
                CurrentlySelected.ApplyPreset(ReferencedStats);
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
                                    UIUtils.TryChangeText(Presets[CurrentlySelected].transform, "Label", CurrentlySelected.ConfigName);
                                    RebuildInformationPanel();
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

            InfoHeaderPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/HeaderPrefab").gameObject; ;
            InfoHeaderPrefab.SetActive(false);
            InfoRowPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ItemPrefab").gameObject;
            InfoRowPrefab.SetActive(false);
            InfoSpacer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/Spacer").gameObject;
            InfoSpacer.SetActive(false);

            InfoScreenContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content").gameObject;

            PresetListContainer = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content").gameObject;
            PresetListPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content/PresetEntryPrefab").gameObject;
            PresetListPrefab.SetActive(false);


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
            CurrentlyActive = show;
        }
    }
}
