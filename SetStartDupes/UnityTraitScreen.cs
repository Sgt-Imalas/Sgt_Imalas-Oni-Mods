using Database;
using Epic.OnlineServices.Sessions;
using FMOD;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using rail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static SetStartDupes.DupeTraitManager;
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
        public Image ToReplaceColour;

        public GameObject PresetListContainer;
        public GameObject PresetListPrefab;

        public FButton ClearSearchBar;
        public FInputField2 Searchbar;

        public bool CurrentlyActive;

        ///Referenced Stats to apply stuff to.
        MinionStartingStats ReferencedStats = null;
        OpenedFrom openedFrom;
        NextType TraitCategory;


        Dictionary<Trait, GameObject> TraitContainers = new Dictionary<Trait, GameObject>();
        Dictionary<SkillGroup, GameObject> DupeInterestContainers = new Dictionary<SkillGroup, GameObject>();

        SkillGroup CurrentGroup = null;
        Trait CurrentTrait = null;

        public enum OpenedFrom
        {
            Interest,
            Trait,
            undefined
        }


        public static void ShowWindow(MinionStartingStats startingStats, System.Action onClose, SkillGroup currentGroup = null, Trait currentTrait = null)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitsWindowPrefab, UnityPresetScreen.parentScreen, true);
                Instance = screen.AddOrGet<UnityTraitScreen>();
                Instance.Init();
            }
            Instance.ReferencedStats = startingStats;
            Instance.SetOpenedType(currentGroup, currentTrait);
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
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
        private void SetOpenedType(SkillGroup currentGroup = null, Trait currentTrait = null)
        {
            var type = currentTrait != null ? OpenedFrom.Trait : currentGroup != null ? OpenedFrom.Interest : OpenedFrom.undefined;

            CurrentGroup = currentGroup;
            CurrentTrait = currentTrait;

            List<string> allowedTraits = new List<string>();

            if (currentGroup != null)
            {
                ToReplaceName.text = GetSkillgroupName(currentGroup);
                ToReplaceColour.color = ModAssets.Colors.grey;
            }
            if (currentTrait != null)
            {
                var next = ModAssets.GetTraitListOfTrait(currentTrait.Id, out var list);
                TraitCategory = next;
                ToReplaceName.text = GetTraitName(currentTrait);
                ToReplaceColour.color = ModAssets.GetColourFromType(next);
                allowedTraits = GetAllowedTraits();
            }


            foreach (var go in TraitContainers)
            {
                go.Value.SetActive(type == OpenedFrom.Trait && allowedTraits.Contains(go.Key.Id));
            }
            foreach (var go in DupeInterestContainers.Values)
            {
                go.SetActive(type == OpenedFrom.Interest);
            }
            openedFrom = type;
        }

        string GetSkillgroupName(SkillGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + group.Id.ToUpperInvariant() + ".NAME");
        }
        string GetTraitName(Trait trait)
        {
            return trait.Name;
        }

        private void AddUIContainer(SkillGroup group)
        {
            if (group != null)
                AddUiContainer(
                GetSkillgroupName(group)
                + " (" + Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME") + ")",
                Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".DESC"),
                skillGroup: group);
        }
        private void AddUIContainer(Trait trait2, NextType next)
        {
            if (trait2 != null)
                AddUiContainer(
                GetTraitName(trait2),
                trait2.GetTooltip(),
                trait: trait2,
                traitType: next);

        }

        private void AddUiContainer(string name, string description, Trait trait = null, NextType traitType = default, SkillGroup skillGroup = null)
        {
            UIUtils.ListAllChildren(PresetListPrefab.transform);
            if (trait != null && !TraitContainers.ContainsKey(trait) || skillGroup != null && !DupeInterestContainers.ContainsKey(skillGroup))
            {
                var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);

                UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
                if (description != null && description.Length > 0)
                {
                    UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description);
                }

                if (trait != null)
                {
                    PresetHolder.transform.Find("SwitchIn").FindOrAddComponent<FButton>().OnClick += () => ChoseThis(trait);
                    PresetHolder.transform.Find("Background").FindOrAddComponent<Image>().color = ModAssets.GetColourFromType(traitType);
                    TraitContainers[trait] = PresetHolder;
                }
                else if (skillGroup != null)
                {
                    PresetHolder.transform.Find("SwitchIn").FindOrAddComponent<FButton>().OnClick += () => ChoseThis(skillGroup);
                    DupeInterestContainers[skillGroup] = PresetHolder;
                }
            }
        }

        private void ChoseThis(Trait trait)
        {
            switch (TraitCategory)
            {
                case NextType.geneShufflerTrait:
                case NextType.posTrait:
                case NextType.negTrait:
                case NextType.needTrait:
                    if (ReferencedStats.Traits.Contains(CurrentTrait))
                        ReferencedStats.Traits.Remove(CurrentTrait);
                    ReferencedStats.Traits.Add(trait);
                    break;
                case NextType.stress:
                    ReferencedStats.stressTrait = trait;
                    break;
                case NextType.joy:
                    ReferencedStats.joyTrait = trait;
                    break;

            }

            if (OnCloseAction != null)
                this.OnCloseAction.Invoke();
            this.Show(false);
        }
        private void ChoseThis(SkillGroup group)
        {
            //TODO
            if (OnCloseAction != null)
                this.OnCloseAction.Invoke();
            this.Show(false);
        }

        void ApplyColorToTraitContainer(GameObject container, string traitID)
        {
            var type = ModAssets.GetTraitListOfTrait(traitID, out var list);
            container.FindOrAddComponent<Image>().color = ModAssets.GetColourFromType(type);
        }

        public string SkillGroup(SkillGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME");
        }

        private void Init()
        {
            if (init) { return; }
            SgtLogger.l("Initializing TraitWindow");
            //UIUtils.ListAllChildren(this.transform);


            ToReplaceName = transform.Find("ToReplace/CurrentlyActive/Label").FindComponent<LocText>();
            ToReplaceColour = transform.Find("ToReplace/CurrentlyActive/Background").FindComponent<Image>();

            Searchbar = transform.Find("SearchBar/Input").FindOrAddComponent<FInputField2>();
            Searchbar.OnValueChanged.AddListener(ApplyFilter);
            Searchbar.Text = string.Empty;


            ClearSearchBar = transform.Find("SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

            UIUtils.AddSimpleTooltipToObject(ClearSearchBar.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);

            PresetListContainer = transform.Find("ScrollArea/Content").gameObject;
            PresetListPrefab = transform.Find("ScrollArea/Content/PresetEntryPrefab").gameObject;
            InitAllContainers();

            init = true;
        }

        private void InitAllContainers()
        {
            var traitsDb = Db.Get().traits;
            var interests = Db.Get().SkillGroups.resources;
            foreach (var type in (NextType[])Enum.GetValues(typeof(NextType)))
            {
                var TraitsOfCategory = ModAssets.TryGetTraitsOfCategory(type);
                foreach (var item in TraitsOfCategory)
                {
                    AddUIContainer(traitsDb.TryGet(item.id), type);
                }
            }
            foreach (var item in interests)
            {
                AddUIContainer(item);
            }
        }


        public void ApplyFilter(string filterstring = "")
        {
            if (openedFrom == OpenedFrom.Interest)
            {
                foreach (var go in DupeInterestContainers)
                {
                    go.Value.SetActive(filterstring == string.Empty ? true : go.Key.Name.ToLowerInvariant().Contains(filterstring.ToLowerInvariant()));
                }
            }
            else
            {
                var allowedTraits = GetAllowedTraits();
                foreach (var go in TraitContainers)
                {
                    bool Contained = allowedTraits.Contains(go.Key.Id);
                    go.Value.SetActive(filterstring == string.Empty ? Contained : Contained && go.Key.Name.ToLowerInvariant().Contains(filterstring.ToLowerInvariant()));
                }
            }

        }

        List<string> GetAllowedTraits()
        {
            var allowedTraits = ModAssets.TryGetTraitsOfCategory(TraitCategory).Select(t => t.id).ToList();
            var finalTraits = new List<string>();
            var forbiddenTraits = ReferencedStats.Traits.Count>0 ? ReferencedStats.Traits.Select(allowedTraits => allowedTraits.Id).ToList() : new List<string>();

            foreach (string existing in allowedTraits)
            {
                if (!forbiddenTraits.Contains(existing))
                {
                    finalTraits.Add(existing);
                }
            }
            return finalTraits;
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
