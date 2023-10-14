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
    internal class UnityCrewPresetScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityCrewPresetScreen Instance = null;


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
        MinionCrewPreset CurrentlySelected;
        ///Referenced Stats to apply presets to.
        CharacterSelectionController ReferencedCrewController = null;


        Dictionary<MinionCrewPreset, GameObject> Presets = new Dictionary<MinionCrewPreset, GameObject>();
        List<GameObject> InformationObjects = new List<GameObject>();



        public static void ShowWindow(CharacterSelectionController controller, System.Action onClose)
        {
            SgtLogger.Assert("CharController was null", controller);

            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<UnityCrewPresetScreen>();
                Instance.Init();
            }
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.LoadAllPresets();
            Instance.LoadTemporalPreset(controller);

            Instance.ReferencedCrewController = controller;
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;

            //Instance.CurrentlySelected.ApplyCrewPreset(controller);
        }

        private bool init;
        private System.Action OnCloseAction;

        public void LoadTemporalPreset(CharacterSelectionController toGenerateFrom)
        {
            var tempStats = MinionCrewPreset.CreateCrewPreset(toGenerateFrom);
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

        List<MinionCrewPreset> LoadPresets()
        {
            List<MinionCrewPreset> minionStatConfigs = new List<MinionCrewPreset>();
            var files = new DirectoryInfo(ModAssets.DupeGroupTemplatePath).GetFiles();


            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                try
                {   
                    var preset = MinionCrewPreset.ReadFromFile(File);
                    if (preset != null)
                    {
                        minionStatConfigs.Add(preset);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.warning("Couln't load crew preset from: " + File.FullName + ", Error: " + e);
                }
            }
            return minionStatConfigs;
        }

        private bool AddUiElementForPreset(MinionCrewPreset config)
        {
            if (!Presets.ContainsKey(config))
            {
                var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);
                PresetHolder.transform.Find("TraitImage").gameObject.SetActive(false);

                UIUtils.TryChangeText(PresetHolder.transform, "Label", config.CrewName);
                PresetHolder.transform.Find("RenameButton").FindOrAddComponent<FButton>().OnClick +=
                    () => config.OpenPopUpToChangeName(
                        () =>
                            {
                                UIUtils.TryChangeText(PresetHolder.transform, "Label", config.CrewName);
                                RebuildInformationPanel();
                            }
                        );

                PresetHolder.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>().OnClick += () => SetAsCurrent(config);
                PresetHolder.transform.Find("DeleteButton").FindOrAddComponent<FButton>().OnClick += () => DeletePreset(config);

                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("RenameButton"), SCROLLAREA.CONTENT.PRESETENTRYPREFAB.RENAMEPRESETTOOLTIP);
                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("DeleteButton"), SCROLLAREA.CONTENT.PRESETENTRYPREFAB.DELETEPRESETTOOLTIP);
                Presets[config] = PresetHolder;
                return true;
            }
            return false;
        }

        void DeletePreset(MinionCrewPreset config)
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
           string.Format(DELETEWINDOW.TITLE, config.CrewName),
           string.Format(DELETEWINDOW.DESC, config.CrewName),
           DELETEWINDOW.YES,
           Delete,
           DELETEWINDOW.CANCEL
           , nothing
           );
        }

        void SetAsCurrent(MinionCrewPreset config)
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
            UIUtils.TryChangeText(Name.transform, "Label", "\"" + CurrentlySelected.CrewName + "\"");
            InformationObjects.Add(Name);

            var spacer4 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            InformationObjects.Add(spacer4);

            foreach(var mate in CurrentlySelected.Crewmates)
            {

                var aptitude = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                UIUtils.TryChangeText(aptitude.transform, "Label", mate.second.ConfigName) ;
                InformationObjects.Add(aptitude);
            }
            //var aptitudeHeader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(aptitudeHeader.transform, "Label", global::STRINGS.UI.CHARACTERCONTAINER_APTITUDES_TITLE + ":");
            //InformationObjects.Add(aptitudeHeader);

            //foreach (var skill in CurrentlySelected.skillAptitudes)
            //{
            //    //if (skill.Value < 1)
            //    //    continue;

            //    var aptitude = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            //    UIUtils.TryChangeText(aptitude.transform, "Label", SkillGroupName(skill.Key));
            //    UIUtils.AddSimpleTooltipToObject(aptitude.transform, SkillGroupDesc(skill.Key), true);
            //    InformationObjects.Add(aptitude);

            //}

            //var spacer3 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            //InformationObjects.Add(spacer3);

            //var traitHeader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(traitHeader.transform, "Label", global::STRINGS.UI.CHARACTERCONTAINER_TRAITS_TITLE + ":");
            //InformationObjects.Add(traitHeader);


            //var traits = Db.Get().traits;
            //foreach (var trait in CurrentlySelected.Traits)
            //{
            //    if (trait == MinionConfig.MINION_BASE_TRAIT_ID)
            //        continue;

            //    var traitcon = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            //    UIUtils.TryChangeText(traitcon.transform, "Label", traits.TryGet(trait).Name);
            //    UIUtils.AddSimpleTooltipToObject(traitcon.transform, ModAssets.GetTraitTooltip(traits.TryGet(trait)), true);
            //    InformationObjects.Add(traitcon);
            //    ApplyColorToTraitContainer(traitcon, trait);
            //}

            //var spacer2 = Util.KInstantiateUI(InfoSpacer, InfoScreenContainer, true);
            //InformationObjects.Add(spacer2);

            //var joyheader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(joyheader.transform, "Label", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_JOYTRAIT, string.Empty));
            //InformationObjects.Add(joyheader);


            //var joy = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(joy.transform, "Label", traits.TryGet(CurrentlySelected.joyTrait).Name);
            //UIUtils.AddSimpleTooltipToObject(joy.transform, ModAssets.GetTraitTooltip(traits.TryGet(CurrentlySelected.joyTrait)), true);
            //InformationObjects.Add(joy);
            //ApplyColorToTraitContainer(joy, CurrentlySelected.joyTrait);

            //var stressheader = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(stressheader.transform, "Label", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_STRESSTRAIT, string.Empty));
            //InformationObjects.Add(stressheader);

            //var stress = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(stress.transform, "Label", traits.TryGet(CurrentlySelected.stressTrait).Name);
            //InformationObjects.Add(stress);
            //UIUtils.AddSimpleTooltipToObject(stress.transform, ModAssets.GetTraitTooltip(traits.TryGet(CurrentlySelected.stressTrait)), true);
            //ApplyColorToTraitContainer(stress, CurrentlySelected.stressTrait);


            GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected));
        }
        //public string SkillGroupName(string groupID)
        //{
        //    if (groupID == null)
        //        return "";
        //    else
        //    {

        //        var skillGroup = Db.Get().SkillGroups.TryGet(groupID);
        //        string relevantSkillID = skillGroup.relevantAttributes.First().Id;

        //        return string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, ModAssets.GetChoreGroupNameForSkillgroup(skillGroup), SkillGroup(skillGroup), SkillLevel(relevantSkillID));
        //    }
        //}
        //public string SkillGroupDesc(string groupID)
        //{
        //    if (groupID == null)
        //        return "";
        //    else
        //    {
        //        var skillGroup = Db.Get().SkillGroups.TryGet(groupID);
        //        return ModAssets.GetSkillgroupDescription(skillGroup);
        //    }
        //}
        void ApplyColorToTraitContainer(GameObject container, string traitID)
        {

            var type = ModAssets.GetTraitListOfTrait(traitID, out var list);
            container.FindOrAddComponent<Image>().color = ModAssets.GetColourFromType(type);
        }


        private void Init()
        {
            UIUtils.TryChangeText(transform, "Title", TITLECREW);
            SgtLogger.l("Initializing CrewPresetWindow");
            GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrentButton").FindOrAddComponent<FButton>();
            CloseButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").FindOrAddComponent<FButton>();
            ApplyButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/ApplyPresetButton").FindOrAddComponent<FButton>();

            OpenPresetFolder = transform.Find("HorizontalLayout/ObjectList/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            OpenPresetFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.DupeGroupTemplatePath) { UseShellExecute = true });


            Searchbar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
            Searchbar.OnValueChanged.AddListener(ApplyFilter);
            Searchbar.Text = string.Empty;


            ClearSearchBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

            ApplyButton.OnClick += () =>
            {
                CurrentlySelected.ApplyCrewPreset(ReferencedCrewController);
                if(OnCloseAction!= null)
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
                                    UIUtils.TryChangeText(Presets[CurrentlySelected].transform, "Label", CurrentlySelected.CrewName);
                                    RebuildInformationPanel();
                                }
                            );
                    RebuildInformationPanel();
                }
            };


            UIUtils.AddSimpleTooltipToObject(GeneratePresetButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.GENERATEFROMCURRENT.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(CloseButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(ApplyButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.APPLYPRESETBUTTON.TOOLTIPCREW);

            UIUtils.AddSimpleTooltipToObject(ClearSearchBar.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);
            UIUtils.AddSimpleTooltipToObject(OpenPresetFolder.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.OPENFOLDERTOOLTIP);

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
                go.Value.SetActive(filterstring == string.Empty ? true : go.Key.CrewName.ToLowerInvariant().Contains(filterstring.ToLowerInvariant()));
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
