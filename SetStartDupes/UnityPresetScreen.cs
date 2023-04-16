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
        new bool ConsumeMouseScroll = true; // do not remove!!!!
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

        public static GameObject parentScreen = null;

        public static void ShowWindow(MinionStartingStats startingStats,System.Action onClose)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, parentScreen, true);
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
            MinionStatConfig tempStats = MinionStatConfig.CreateFromStartingStats(toGenerateFrom, ModAssets.DupeTemplateName);
            SetAsCurrent(tempStats);
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
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

        private void AddUiElementForPreset(MinionStatConfig config)
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
                Presets[config] = PresetHolder;
            }
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
            UIUtils.TryChangeText(aptitudeHeader.transform, "Label", "Interests:"); //TODO LOC
            InformationObjects.Add(aptitudeHeader);

            foreach (var skill in CurrentlySelected.skillAptitudes)
            {
                //if (skill.Value < 1)
                //    continue;

                var aptitude = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                UIUtils.TryChangeText(aptitude.transform, "Label", SkillGroupName(skill.Key));
                InformationObjects.Add(aptitude);

            }

            var spacer3 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            InformationObjects.Add(spacer3);

            var traitHeader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(traitHeader.transform, "Label", "Traits:");//TODO LOC
            InformationObjects.Add(traitHeader);


            var traits = Db.Get().traits;
            foreach (var trait in CurrentlySelected.Traits)
            {
                if (trait == MinionConfig.MINION_BASE_TRAIT_ID)
                    continue;

                var traitcon = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                UIUtils.TryChangeText(traitcon.transform, "Label", traits.TryGet(trait).Name);
                UIUtils.AddSimpleTooltipToObject(traitcon.transform, traits.TryGet(trait).GetTooltip(), true);
                InformationObjects.Add(traitcon);
                ApplyColorToTraitContainer(traitcon, trait);
            }

            var spacer2 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            InformationObjects.Add(spacer2);

            var joyheader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(joyheader.transform, "Label", "Overjoyed Response:");//TODO LOC
            InformationObjects.Add(joyheader);


            var joy = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(joy.transform, "Label", traits.TryGet(CurrentlySelected.joyTrait).Name);
            UIUtils.AddSimpleTooltipToObject(joy.transform, traits.TryGet(CurrentlySelected.joyTrait).GetTooltip(), true);
            InformationObjects.Add(joy);
            ApplyColorToTraitContainer(joy, CurrentlySelected.joyTrait);

            var stressheader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(stressheader.transform, "Label", "Stress Trait:");//TODO LOC
            InformationObjects.Add(stressheader);

            var stress = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            UIUtils.TryChangeText(stress.transform, "Label", traits.TryGet(CurrentlySelected.stressTrait).Name);
            InformationObjects.Add(stress);
            UIUtils.AddSimpleTooltipToObject(stress.transform, traits.TryGet(CurrentlySelected.stressTrait).GetTooltip(), true);
            ApplyColorToTraitContainer(stress, CurrentlySelected.stressTrait);


            GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected));
        }
        public string SkillGroupName(string groupID)
        {
            if (groupID == null)
                return "";
            else
            {
                Strings.TryGet("STRINGS.DUPLICANTS.SKILLGROUPS." + groupID.ToUpperInvariant() + ".NAME", out var attribute);

                var skillGroup = Db.Get().SkillGroups.TryGet(groupID);
                string relevantSkillID = skillGroup.relevantAttributes.First().Id;

                return string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, attribute, SkillGroup(skillGroup), SkillLevel(relevantSkillID));
            }
        }
        void ApplyColorToTraitContainer(GameObject container, string traitID)
        {

            var type = DupeTraitManager.GetTraitListOfTrait(traitID, out var list);
            container.FindOrAddComponent<Image>().color = ModAssets.GetColourFromType(type);
        }

        public string SkillGroup(SkillGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME");
        }
        string SkillLevel(string skillID)
        {
            return CurrentlySelected.StartingLevels.Find((skill) => skill.Key == skillID).Value.ToString();
        }




        private void Init()
        {
            SgtLogger.l("Initializing PresetWindow");
            GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrentButton").FindOrAddComponent<FButton>();
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
                AddUiElementForPreset(CurrentlySelected);
                CurrentlySelected.WriteToFile();
                RebuildInformationPanel();
            };

            InfoHeaderPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/HeaderPrefab").gameObject; ;
            InfoRowPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ItemPrefab").gameObject;
            InfoSpacer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/Spacer").gameObject;

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
            CurrentlyActive = show;
        }
    }
}
