using Beached_ModAPI;
using Database;
using Epic.OnlineServices.Sessions;
using FMOD;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using rail;
using SetStartDupes.DuplicityEditing;
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
    internal class UnityDuplicitySelectionScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityDuplicitySelectionScreen Instance = null;

        public LocText ToReplaceName;
        public Image ToReplaceColour;

        public GameObject PresetListContainer;
        public GameObject PresetListPrefab;
        public GameObject BodypartPrefab;

        public FButton ClearSearchBar;
        public FInputField2 Searchbar;

        public bool CurrentlyActive;

        OpenedFrom openedFrom;


        Dictionary<Trait, GameObject> TraitContainers = new Dictionary<Trait, GameObject>();
        Dictionary<SkillGroup, GameObject> DupeInterestContainers = new Dictionary<SkillGroup, GameObject>();
        Dictionary<Effect, GameObject> EffectContainers = new Dictionary<Effect, GameObject>();
        Dictionary<string, Dictionary<HashedString, GameObject>> BodypartContainers = new();

        Dictionary<OpenedFrom, List<GameObject>> CategoryEntries = new();

        public enum OpenedFrom
        {
            Undefined = 0,
            Interest,
            Trait,
            Effect,
            Bodypart,

        }


        public static void ShowWindow(OpenedFrom from, System.Action<object> OnSelect, System.Action onClose, string bodyPartGroup = null)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitsWindowPrefab, DuplicityMainScreen.Instance.gameObject, true);
                Instance = screen.AddOrGet<UnityDuplicitySelectionScreen>();
                Instance.Init();
            }
            Instance.OnSelect = OnSelect;
            Instance.SetOpenedType(from);
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;

        }

        private bool init;
        private System.Action OnCloseAction;
        private System.Action<object> OnSelect;


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
        private void SetOpenedType(OpenedFrom from = OpenedFrom.Undefined)
        {
            openedFrom = from;

            foreach(var cat in CategoryEntries)
            {
                bool enable = cat.Key == openedFrom;

                if (!enable)
                {
                    foreach(var entry in cat.Value)
                    {
                        entry.SetActive(false);
                    }
                }
            }

            ApplyFilter();
        }


        string GetTraitName(Trait trait)
        {
            return trait.Name;
        }

        private GameObject AddUIContainer(SkillGroup group)
        {
            if (group != null && !DupeInterestContainers.ContainsKey(group))
            {
                DupeInterestContainers[group] = AddUiContainer(
                ModAssets.GetSkillgroupName(group),
                ModAssets.GetSkillgroupDescription(group),
                () => SelectItem(group.Id));
                return DupeInterestContainers[group];
            }
            return null;
        }

        private GameObject AddUIContainer(Trait trait2, NextType next)
        {
            if (trait2 != null && !TraitContainers.ContainsKey(trait2))
            {
                TraitContainers[trait2] = AddUiContainer(
                GetTraitName(trait2),
                ModAssets.GetTraitTooltip(trait2, trait2.Id),
                () => SelectItem(trait2.Id),
                 ModAssets.GetColourFromType(next));
                return TraitContainers[trait2];
            }
            return null;

        }



        private GameObject AddUiContainer(string name, string description, System.Action onClickAction, Color overrideColor = default, GameObject prefabOverride = null, Sprite placeImage = null)
        {
            if (prefabOverride == null)
                prefabOverride = PresetListPrefab;

            var PresetHolder = Util.KInstantiateUI(prefabOverride, PresetListContainer, true);

            UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
            if (description != null && description.Length > 0)
            {
                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
            }
            if (placeImage != null)
            {
                var image = PresetHolder.transform.Find("Image").FindOrAddComponent<Image>();
                image.sprite = placeImage;
                UnityEngine.Rect rect = image.sprite.rect;
                if (rect.width > rect.height)
                {
                    var size = (rect.height / rect.width) * 55;
                    image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
                else
                {
                    var size = (rect.width / rect.height) * 55;
                    image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }
            }

            PresetHolder.transform.FindOrAddComponent<FButton>().OnClick += onClickAction;
            if (overrideColor != default)
                PresetHolder.transform.Find("Background").FindOrAddComponent<Image>().color = overrideColor;

            return PresetHolder;
        }

        private void SelectItem(object obj)
        {
            OnSelect(obj);
            if (OnCloseAction != null)
                this.OnCloseAction.Invoke();
            this.Show(false);
        }

        void ApplyColorToTraitContainer(GameObject container, string traitID)
        {
            var type = ModAssets.GetTraitListOfTrait(traitID);
            container.FindOrAddComponent<Image>().color = ModAssets.GetColourFromType(type);
        }

        public string SkillGroup(SkillGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME");
        }

        private void Init()
        {
            if (init) { return; }
            SgtLogger.l("Initializing DupeEditorSelectionWindow");

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
            //UIUtils.ListAllChildren(CloserButton.transform);
            CloserButton.FindOrAddComponent<FButton>().OnClick += () => this.Show(false);
            CloserButton.transform.Find("Text").GetComponent<LocText>().text = STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TEXT;

            SgtLogger.Assert("PresetListPrefab was null", PresetListPrefab);
            SgtLogger.Assert("PresetListContainer was null", PresetListContainer);
            SgtLogger.l("initializing ListEntries");

            BodypartPrefab = transform.Find("ScrollArea/Content/DupeSkinPartPrefab").gameObject;
            BodypartPrefab.SetActive(false);
            //hairEntry.gameObject.SetActive(false);
            InitAllContainers();


            init = true;
        }

        private void InitAllContainers()
        {
            CategoryEntries.Add(OpenedFrom.Interest, new());
            CategoryEntries.Add(OpenedFrom.Trait, new());
            CategoryEntries.Add(OpenedFrom.Effect, new());
            CategoryEntries.Add(OpenedFrom.Bodypart, new());


            var traitsDb = Db.Get().traits;
            var interests = Db.Get().SkillGroups.resources;
            foreach (var type in (NextType[])Enum.GetValues(typeof(NextType)))
            {
                if (type == NextType.allTraits) continue;

                var TraitsOfCategory = ModAssets.TryGetTraitsOfCategory(type);
                foreach (var item in TraitsOfCategory)
                {
                    if (ModAssets.TraitAllowedInCurrentDLC(item))
                        CategoryEntries[OpenedFrom.Trait].Add( AddUIContainer(traitsDb.TryGet(item.id), type));
                    else
                        SgtLogger.l(item.id, "Filtered, not active dlc");

                }
            }
            foreach (var item in interests)
            {
                CategoryEntries[OpenedFrom.Interest].Add(AddUIContainer(item));
            }
            
            //foreach (var effect in Db.Get().effects.resources)
            foreach (var effect in DuplicityEditingPatches.VanillaDupeEffects)
            {
                if(AccessorySlotHelper.IsCritterTrait(effect.Id)) continue;

                if (effect != null && !EffectContainers.ContainsKey(effect))
                {
                    EffectContainers[effect] = AddUiContainer(
                    effect.Id,
                    "",
                    () => SelectItem(effect.Id));
                    CategoryEntries[OpenedFrom.Effect].Add(EffectContainers[effect]);
                }
            }
            foreach(var slot in AccessorySlotHelper.GetAllChangeableSlot())
            {
                BodypartContainers[slot.Id] = new Dictionary<HashedString, GameObject>();
                foreach (var accessory in slot.accessories)
                {
                    if (!BodypartContainers[slot.Id].ContainsKey(accessory.IdHash))
                    {
                        BodypartContainers[slot.Id].Add(accessory.IdHash,
                            AddUiContainer(
                                accessory.Id,
                                "",
                                () => SgtLogger.l(""),
                                prefabOverride: BodypartPrefab,
                                placeImage: AccessorySlotHelper.GetSpriteFrom(accessory.symbol)
                            ));
                        CategoryEntries[OpenedFrom.Bodypart].Add(BodypartContainers[slot.Id][accessory.Id]);
                    }
                }
            }
        }


        public void ApplyFilter(string filterstring = "")
        {
            if (openedFrom == OpenedFrom.Interest)
            {
                List<SkillGroup> forbidden = new List<SkillGroup>();// ReferencedStats.skillAptitudes.Keys.ToList();
                foreach (var go in DupeInterestContainers)
                {
                    bool isForbidden = !forbidden.Contains(go.Key);
                    go.Value.SetActive(filterstring == string.Empty ? isForbidden : isForbidden && ShowInFilter(filterstring, go.Key.Name));
                }
            }
            else if( openedFrom == OpenedFrom.Trait)
            {
                var allowedTraits = GetAllowedTraits();
                foreach (var go in TraitContainers)
                {
                    bool Contained = allowedTraits.Contains(go.Key.Id);
                    go.Value.SetActive(filterstring == string.Empty ? Contained : Contained && ShowInFilter(filterstring, new string[] { go.Key.Name, go.Key.description }));
                }
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
                if (text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
                {
                    show = true;
                    break;
                }
            }
            return show;
        }


        List<string> GetAllowedTraits()
        {
            var allowedTraits = ModAssets.TryGetTraitsOfCategory(NextType.allTraits, overrideShowAll:true).Select(t => t.id).ToList();
            var finalTraits = new List<string>();
            var forbiddenTraits = 
                //ReferencedStats.Traits.Count > 0 ? ReferencedStats.Traits.Select(allowedTraits => allowedTraits.Id).ToList() : 
                new List<string>();

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
