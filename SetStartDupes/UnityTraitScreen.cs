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
    internal class UnityTraitScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityTraitScreen Instance = null;

        public LocText ToReplaceName;

        public GameObject PresetListContainer;
        public GameObject PresetListPrefab;

        public FButton ClearSearchBar;
        public FInputField2 Searchbar;

        public bool CurrentlyActive;

        ///Referenced Stats to apply stuff to.
        MinionStartingStats ReferencedStats = null;
        OpenedFrom openedFrom;


        Dictionary<Trait, GameObject> Presets = new Dictionary<Trait, GameObject>();
        List<GameObject> InformationObjects = new List<GameObject>();

        public static GameObject parentScreen = null;

        public enum OpenedFrom
        {
            Interest,
            Trait
        }
       

        public int PointsPerInterests(int numberOfInterests)
        {
            int pointsPer = 0;
            if(numberOfInterests > 0)
            {
                if(numberOfInterests==1)
                    pointsPer = 7;
                else if (numberOfInterests == 2)
                    pointsPer = 3;
                else
                    pointsPer = 1;
            }
            return pointsPer;
        }



        public static void ShowWindow(MinionStartingStats startingStats, OpenedFrom from, System.Action onClose)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitsWindowPrefab, parentScreen, true);
                Instance = screen.AddOrGet<UnityTraitScreen>();
                Instance.Init();
            }
            Instance.openedFrom = from;
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.ReferencedStats = startingStats;
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;
        }

        private bool init;
        private System.Action OnCloseAction;


        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                this.Show(false);
            }

            base.OnKeyDown(e);
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

                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("RenameButton"), SCROLLAREA.CONTENT.PRESETENTRYPREFAB.RENAMEPRESETTOOLTIP);
                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("DeleteButton"), SCROLLAREA.CONTENT.PRESETENTRYPREFAB.DELETEPRESETTOOLTIP);
                Presets[config] = PresetHolder;
            }
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
            UIUtils.ListAllChildren(this.transform);


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


            UIUtils.AddSimpleTooltipToObject(GeneratePresetButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.GENERATEFROMCURRENT.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(CloseButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(ApplyButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.APPLYPRESETBUTTON.TOOLTIP);

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
