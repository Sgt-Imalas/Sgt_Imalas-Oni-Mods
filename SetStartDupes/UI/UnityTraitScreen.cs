using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static SetStartDupes.DupeTraitManager;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW;
using static TUNING.DUPLICANTSTATS;

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


        Dictionary<GameObject, ToolTip> ContainerTooltips = new();

        Dictionary<Trait, GameObject> TraitContainers = new Dictionary<Trait, GameObject>();
        Dictionary<Trait, string> TraitTooltips = new();

        Dictionary<SkillGroup, GameObject> DupeInterestContainers = new Dictionary<SkillGroup, GameObject>();
        Dictionary<SkillGroup, string> InterestTooltips = new();

        SkillGroup CurrentGroup = null;
        Trait CurrentTrait = null;
        DupeTraitManager currentStatManager;
        public enum OpenedFrom
        {
            undefined,
            Interest,
            Trait,
            Editor_Interest,
            Editor_Trait,
            Editor_Effect

        }


        public static void ShowWindow(MinionStartingStats startingStats, System.Action onClose, SkillGroup currentGroup = null, DupeTraitManager DupeTraitManager = null, Trait currentTrait = null, OpenedFrom openedFrom = default)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitsWindowPrefab, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<UnityTraitScreen>();
                Instance.Init();
            }
            Instance.ReferencedStats = startingStats;
            Instance.SetOpenedType(currentGroup, currentTrait, DupeTraitManager, openedFrom);
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
            if (e.TryConsume(Action.DebugToggleClusterFX))
            {
                Searchbar.ExternalStartEditing();
            }

            base.OnKeyDown(e);
        }
        private void SetOpenedType(SkillGroup currentGroup = null, Trait currentTrait = null, DupeTraitManager dupeTraitManager = null, OpenedFrom from = OpenedFrom.undefined)
        {
            var type = currentTrait != null ? OpenedFrom.Trait : currentGroup != null ? OpenedFrom.Interest : from;

            if (from == OpenedFrom.Trait)
                TraitCategory = NextType.allTraits;


            CurrentGroup = currentGroup;
            CurrentTrait = currentTrait;
            currentStatManager = dupeTraitManager;

            List<string> allowedTraits = new List<string>();

            if (currentGroup != null)
            {
                ToReplaceName.text = ModAssets.GetSkillgroupName(currentGroup);
                ToReplaceColour.color = UIUtils.Darken(ModAssets.Colors.grey, 40);
            }
            else if (currentTrait != null)
            {
                var next = ModAssets.GetTraitListOfTrait(currentTrait.Id);
                TraitCategory = next;
                ToReplaceName.text = GetTraitName(currentTrait);
                ToReplaceColour.color = ModAssets.GetColourFromType(next);
                allowedTraits = GetAllowedTraits();
            }
            else
            {
                ToReplaceName.text = global::STRINGS.DUPLICANTS.CONGENITALTRAITS.NONE.NAME;
                ToReplaceColour.color = ModAssets.Colors.grey;
            }
            foreach (var go in TraitContainers)
            {
                go.Value.SetActive(type == OpenedFrom.Trait && allowedTraits.Contains(go.Key.Id));
            }

            List<SkillGroup> forbidden = ReferencedStats.skillAptitudes.Keys.ToList();
            foreach (var go in DupeInterestContainers)
            {
                go.Value.SetActive(type == OpenedFrom.Interest && !forbidden.Contains(go.Key));
            }


            openedFrom = type;
            ApplyFilter();
        }


        string GetTraitName(Trait trait)
        {
            return trait.GetName();
        }

        private void AddUIContainer(SkillGroup group)
        {
            SgtLogger.l("adding ui container for skillgroup " + group.Id);

            string tooltip = ModAssets.GetSkillgroupDescription(group);
            if (group != null && !DupeInterestContainers.ContainsKey(group))
                DupeInterestContainers[group] = AddUiContainer(
                ModAssets.GetSkillgroupName(group),
                tooltip,
                () => ChoseThis(group));

            InterestTooltips[group] = tooltip;
        }

        private void AddUIContainer(Trait trait2, NextType next)
        {
            if (trait2 != null && !TraitContainers.ContainsKey(trait2))
            {
                string traitTooltip = ModAssets.GetTraitTooltip(trait2, trait2.Id);

                TraitContainers[trait2] = AddUiContainer(
                GetTraitName(trait2),
              traitTooltip,
                () => ChoseThis(trait2),
                 ModAssets.GetColourFromType(next));

                TraitTooltips[trait2] = traitTooltip;
            }
        }

        private GameObject AddUiContainer(string name, string description, System.Action onClickAction, Color overrideColor = default)
        {
            var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);

            UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
            if (description != null && description.Length > 0)
            {
                ContainerTooltips[PresetHolder] = UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
            }

            PresetHolder.transform.FindOrAddComponent<FButton>().OnClick += onClickAction;
            if (overrideColor != default)
                PresetHolder.transform.Find("Background").FindOrAddComponent<Image>().color = overrideColor;



            return PresetHolder;
        }


        private void ChoseThis(Trait trait)
        {
            switch (TraitCategory)
            {
                case NextType.geneShufflerTrait:
                case NextType.posTrait:
                case NextType.bionic_boost:
                case NextType.bionic_bug:
                case NextType.negTrait:
                case NextType.needTrait:
                case NextType.allTraits:
                case NextType.RainbowFart:
                    currentStatManager.RemoveTrait(CurrentTrait);
                    currentStatManager.AddTrait(trait);
                    break;
                case NextType.stress:
                    ReferencedStats.stressTrait = trait;
                    break;
                case NextType.joy:
                    ReferencedStats.joyTrait = trait;
                    break;
                case NextType.Beached_LifeGoal:
                    currentStatManager.RemoveLifeGoal();
                    currentStatManager.AddLifeGoal(trait);

                    break;

            }

            if (OnCloseAction != null)
                this.OnCloseAction.Invoke();
            this.Show(false);
        }
        private void ChoseThis(SkillGroup group)
        {

            if (CurrentGroup == null)
            {
                currentStatManager.AddInterest(group);
            }
            else
                currentStatManager.ReplaceInterest(CurrentGroup, group);

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
            PresetListPrefab.SetActive(false);
            transform.Find("ScrollArea/Content/CarePackagePrefab").gameObject.SetActive(false);

            var CloserButton = transform.Find("CloseButton").gameObject;
            CloserButton.FindOrAddComponent<FButton>().OnClick += () => this.Show(false);
            CloserButton.transform.Find("Text").GetComponent<LocText>().text = STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TEXT;

            SgtLogger.Assert("PresetListPrefab was null", PresetListPrefab);
            SgtLogger.Assert("PresetListContainer was null", PresetListContainer);
            SgtLogger.l("initializing ListEntries");

            transform.Find("ScrollArea/Content/DupeSkinPartPrefab").gameObject.SetActive(false);
            //hairEntry.gameObject.SetActive(false);
            InitAllContainers();


            init = true;
        }

        private void InitAllContainers()
        {
            var traitsDb = Db.Get().traits;
            var interests = new List<SkillGroup>(Db.Get().SkillGroups.resources);
            interests.RemoveAll(interest => interest.choreGroupID == null);
            var sortedInterests = interests.OrderBy(interest =>  interest.Name);


            foreach (var type in (NextType[])Enum.GetValues(typeof(NextType)))
            {
                if (type == NextType.allTraits) continue;

                var TraitsOfCategory = ModAssets.TryGetTraitsOfCategory(type, null);

                var sortedTraits = TraitsOfCategory.OrderBy(traitval => traitsDb.TryGet(traitval.id)?.GetName());

                foreach (var item in sortedTraits)
                {
                    if (ModAssets.TraitAllowedInCurrentDLC(item))
                        AddUIContainer(traitsDb.TryGet(item.id), type);
                    else
                        SgtLogger.l(item.id, "Filtered, not active dlc");

                }
            }
            foreach (var item in sortedInterests)
            {
                AddUIContainer(item);
            }
        }


        public void ApplyFilter(string filterstring = "")
        {
            if (openedFrom == OpenedFrom.Interest)
            {
                List<SkillGroup> forbidden = ReferencedStats.skillAptitudes.Keys.ToList();
                foreach (var go in DupeInterestContainers)
                {
                    bool isForbidden = !forbidden.Contains(go.Key);
                    go.Value.SetActive(filterstring == string.Empty ? isForbidden : isForbidden && ShowInFilter(filterstring, [go.Key.Id,go.Key.Name]));
                    RefreshInterestTooltip(go.Key);
                }
            }
            else
            {
                var allowedTraits = GetAllowedTraits();
                foreach (var kvp in TraitContainers)
                {
                    string traitId = kvp.Key.Id;
                    bool Contained = allowedTraits.Contains(traitId);
                    kvp.Value.SetActive(filterstring == string.Empty ? Contained : Contained && ShowInFilter(filterstring, [ kvp.Key.Id, kvp.Key.GetName(), kvp.Key.description ]));
                    RefreshTraitTooltip(kvp.Key);
                }
            }

        }
        Color exclusivityWarningHex = UIUtils.rgb(255, 88, 63);
        void RefreshInterestTooltip(SkillGroup skillGroup)
        {
            bool hasConflicts = false;
            var tooltipString = InterestTooltips[skillGroup];
            List<string> conflictingItems = new List<string>();
            var skillgroupId = skillGroup.IdHash;

            if (ReferencedStats.Traits.Count > 0)
            {
                foreach (var existingTrait in ReferencedStats.Traits)
                {
					string existingTraitId = existingTrait.Id;
                    ModAssets.GetTraitListOfTrait(existingTraitId, out var list);
                    if (list == null)
                        continue;
                    var traitVal = list.Find(checkTraitVal => checkTraitVal.id == existingTraitId);
                    if (traitVal.mutuallyExclusiveAptitudes != null && traitVal.mutuallyExclusiveAptitudes.Any() && traitVal.mutuallyExclusiveAptitudes.Contains(skillgroupId))
                    {
                        conflictingItems.Add(ModAssets.GetTraitName(existingTrait));
                        hasConflicts = true;
                    }
                }
            }
            if (DupeInterestContainers.TryGetValue(skillGroup, out var container) && ContainerTooltips.TryGetValue(container, out var toolTip))
            {
                if (hasConflicts)
                {
                    tooltipString += "\n\n" + UIUtils.EmboldenText(UIUtils.ColorText(STRINGS.EXCLUSIVITY_RULE_CONFLICTING, exclusivityWarningHex));
                    foreach (var conflict in conflictingItems)
                    {
                        tooltipString += "\n" + UIUtils.EmboldenText(UIUtils.ColorText("    • " + conflict, exclusivityWarningHex));
                    }
                }
                toolTip.SetSimpleTooltip(tooltipString);
            }
        }

        void RefreshTraitTooltip(Trait trait)
        {
            string traitId = trait.Id;
            bool hasConflicts = false;

            var tooltipString = TraitTooltips[trait];
            List<string> conflictingItems = new List<string>();

            if (ReferencedStats.Traits.Count > 0)
            {
                foreach (var existingTrait in ReferencedStats.Traits)
				{
					if (CurrentTrait.Id == existingTrait.Id)
					{
						continue; //skip the trait we are currently replacing

					}
					string existingTraitId = existingTrait.Id;
                    ModAssets.GetTraitListOfTrait(existingTraitId, out var list);
                    if (list == null)
                        continue;
                    var traitVal = list.Find(checkTraitVal => checkTraitVal.id == existingTraitId);
                    if (traitVal.mutuallyExclusiveTraits != null && traitVal.mutuallyExclusiveTraits.Any() && traitVal.mutuallyExclusiveTraits.Contains(traitId))
                    {
                        conflictingItems.Add(ModAssets.GetTraitName(existingTrait));
                        hasConflicts = true;
                    }
                }
            }
            ModAssets.GetTraitListOfTrait(traitId, out var ownTraitList);
            if (ownTraitList != null)
            {
                TraitVal traitVal = ownTraitList.Find(checkTraitVal => checkTraitVal.id == traitId);
                var mutualExclusiveAptitudes = traitVal.mutuallyExclusiveAptitudes;
                if (ReferencedStats.skillAptitudes != null && mutualExclusiveAptitudes != null && mutualExclusiveAptitudes.Any())
                {
                    foreach (KeyValuePair<SkillGroup, float> skillAptitude in ReferencedStats.skillAptitudes)
                    {
                        if (skillAptitude.Value == 0)
                            continue;
                        if (mutualExclusiveAptitudes.Contains(skillAptitude.Key.IdHash))
                        {
                            conflictingItems.Add(ModAssets.GetSkillgroupName(skillAptitude.Key));
                            hasConflicts = true;
                        }
                    }
                }

            }

            if (TraitContainers.TryGetValue(trait, out var container) && ContainerTooltips.TryGetValue(container, out var toolTip))
            {

                if (hasConflicts)
                {
                    tooltipString += "\n\n" + UIUtils.EmboldenText(UIUtils.ColorText(STRINGS.EXCLUSIVITY_RULE_CONFLICTING, exclusivityWarningHex));
                    foreach(var conflict in conflictingItems)
                    {
                        tooltipString += "\n" + UIUtils.EmboldenText(UIUtils.ColorText("    • "+ conflict,exclusivityWarningHex));
                    }
                }

                toolTip.SetSimpleTooltip(tooltipString);
            }

        }

        bool ShowInFilter(string filtertext, string stringsToInclude)
        {
            return ShowInFilter(filtertext, new string[] { stringsToInclude });
        }

        bool ShowInFilter(string filtertext, string[] stringsToInclude)
        {
            bool show = false;
            filtertext = filtertext.ToLowerInvariant();

            foreach (var text in stringsToInclude)
            {
                if (text != null && text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
                {
                    show = true;
                    break;
                }
            }
            return show;
        }


        List<string> GetAllowedTraits()
        {
            var allowedTraitVals = ModAssets.TryGetTraitsOfCategory(TraitCategory, ReferencedStats.personality.model);
            var forbiddenTraits = ReferencedStats.Traits.Count > 0 ? ReferencedStats.Traits.Select(allowedTraits => allowedTraits.Id).ToList() : new List<string>();
            var allowedTraits = allowedTraitVals.Select(t => t.id).ToList();
            var finalTraits = new List<string>();

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
