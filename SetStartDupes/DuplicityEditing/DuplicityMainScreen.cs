using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SetStartDupes.DupeTraitManager;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW;
using UnityEngine;
using UtilLibs.UIcmp;
using UtilLibs;
using UnityEngine.UI;
using SetStartDupes.DuplicityEditing.ScreenComponents;
using System.Security.Principal;
using static STRINGS.UI.UISIDESCREENS.AUTOPLUMBERSIDESCREEN.BUTTONS;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;
using UtilLibs.UI.FUI;
using Epic.OnlineServices.Lobby;

namespace SetStartDupes.DuplicityEditing
{
    internal class DuplicityMainScreen : FScreen
    {
        public enum Tab
        {
            undefined,
            Attributes,
            Appearance,
            Health,
            Skills,
            Effects
        }

#pragma warning disable IDE0051 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members


        public static DuplicityMainScreen Instance = null;

        public bool CurrentlyActive;

        GameObject MinionButtonContainer, MinionButtonPrefab;
        Dictionary<MinionAssignablesProxy, MinionSelectButton> MinionButtons = new Dictionary<MinionAssignablesProxy, MinionSelectButton>();
        Dictionary<Tab, List<GameObject>> CategoryGameObjects = new Dictionary<Tab, List<GameObject>>();
        MinionAssignablesProxy SelectedMinion;
        public DuplicantEditableStats Stats;

        Dictionary<Tab, FToggleButton> Tabs = new Dictionary<Tab, FToggleButton>();
        Tab lastCategory = Tab.undefined;

        //Prefabs:
        NumberInput NumberInputPrefabWide, NumberInputPrefab;
        HeaderMain HeaderMainPrefab;
        HeaderDescriptor HeaderDescriptorPrefab;
        CheckboxInput CheckboxInputPrefab;
        SliderInput SliderInputPrefab;

        GameObject ParentContainer;

        //Attribute-Tab:
        Dictionary<Klei.AI.Attribute, NumberInput> attributeEditors;
        Dictionary<string, DeletableListEntry> TraitEntries = new();
        Dictionary<HashedString, DeletableListEntry> AptitudeEntries = new();
        FButton AddNewTrait, AddNewAptitude;
        GameObject TraitContainer, AptitudeContainer;
        DeletableListEntry TraitPrefab, AptitudePrefab;

        //

        public static void ShowWindow(GameObject SourceDupe, System.Action onClose)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.DuplicityWindowPrefab, GameScreenManager.Instance.transform.Find("ScreenSpaceOverlayCanvas/MiddleCenter - InFrontOfEverything").gameObject, true);
                Instance = screen.AddOrGet<DuplicityMainScreen>();
                Instance.Init();
                Instance.name = "DSS_DuplicityEditor_MainScreen";
            }
            //Instance.SetOpenedType(currentGroup, currentTrait, DupeTraitManager, openedFrom);
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.OnCloseAction = onClose;

            Instance.UpdateMinionButtons(true);
            if (SourceDupe.TryGetComponent<MinionIdentity>(out var identity))
            {
                Instance.TryChangeMinion(identity.assignableProxy.Get());
            }
        }

        private bool init;
        private System.Action OnCloseAction;


        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.MouseRight))
            {
                TryClose();
                //Debug.Log("consumed closing action");
            }

            if (e.TryConsume(Action.Escape))
            {
                TryClose();
                //this.Show(false);
            }
            base.OnKeyDown(e);
        }

        void GenerateMinionEditStats(MinionAssignablesProxy minion)
        {
            SelectedMinion = minion;
            Stats = DuplicantEditableStats.GenerateFromMinion(minion);
            UpdateMinionButtons();
            UpdateCategoryButtons();
        }
        void TryClose()
        {
            if (PendingChanges())
            {
                KMod.Manager.Dialog(Global.Instance.globalCanvas,
                       STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TITLE,
                       STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TEXT,
                STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.APPLYCHANGES,
                       () => ApplyAndClose(),
                STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.DISCARDCHANGES,
                       () => DiscardAndClose(),
                       STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.CANCEL,
                       () => { });
            }
            else
                DiscardAndClose();

        }

        void ApplyAndClose()
        {
            Stats.Apply(SelectedMinion);
            Stats = null;
            Show(false);
        }
        void DiscardAndClose()
        {
            Stats = null;
            Show(false);
        }

        private bool PendingChanges() => Stats != null && Stats.EditsPending;

        private void Init()
        {
            if (init) { return; }
            SgtLogger.l("Initializing Duplicity Dupe editing");
            UIUtils.ListAllChildrenPath(this.transform);
            MinionButtonContainer = transform.Find("Categories/Content/ScrollRectContainer").gameObject;
            MinionButtonPrefab = MinionButtonContainer.transform.Find("Item").gameObject;
            MinionButtonPrefab.SetActive(false);

            InitPrefabs();

            InitTabs();
            init = true;
        }
        private void InitPrefabs()
        {
            NumberInputPrefabWide = transform.Find("Details/Content/ScrollRectContainer/NumberInputPrefabWide").FindOrAddComponent<NumberInput>();
            NumberInputPrefabWide.gameObject.SetActive(false);

            NumberInputPrefab = transform.Find("Details/Content/ScrollRectContainer/NumberInputPrefab").FindOrAddComponent<NumberInput>();
            NumberInputPrefab.gameObject.SetActive(false);

            HeaderMainPrefab = transform.Find("Details/Content/ScrollRectContainer/HeaderMain").FindOrAddComponent<HeaderMain>();
            HeaderMainPrefab.gameObject.SetActive(false);

            HeaderDescriptorPrefab = transform.Find("Details/Content/ScrollRectContainer/HeaderDescriptor").FindOrAddComponent<HeaderDescriptor>();
            HeaderDescriptorPrefab.gameObject.SetActive(false);

            CheckboxInputPrefab = transform.Find("Details/Content/ScrollRectContainer/CheckboxPrefab").FindOrAddComponent<CheckboxInput>();
            CheckboxInputPrefab.gameObject.SetActive(false);

            SliderInputPrefab = transform.Find("Details/Content/ScrollRectContainer/SliderPrefab").FindOrAddComponent<SliderInput>();
            SliderInputPrefab.gameObject.SetActive(false);

            ParentContainer = transform.Find("Details/Content/ScrollRectContainer").gameObject;

            //temp:
            transform.Find("Details/Content/ScrollRectContainer/Appearence").gameObject.SetActive(false);
            transform.Find("Details/Content/ScrollRectContainer/SingleListPrefab").gameObject.SetActive(false);
            transform.Find("Details/Content/ScrollRectContainer/TraitInterestContainer").gameObject.SetActive(false);
            transform.Find("Details/Content/ScrollRectContainer/NewButtonPrefab").gameObject.SetActive(false);


        }
        private void InitTabs()
        {
            Tabs.Add(Tab.Attributes, transform.Find("Details/Header/Buttons/AttributeButton").FindOrAddComponent<FToggleButton>());
            Tabs.Add(Tab.Appearance, transform.Find("Details/Header/Buttons/AppearanceButton").FindOrAddComponent<FToggleButton>());
            Tabs.Add(Tab.Health, transform.Find("Details/Header/Buttons/HealthButton").FindOrAddComponent<FToggleButton>());
            Tabs.Add(Tab.Skills, transform.Find("Details/Header/Buttons/SkillsButton").FindOrAddComponent<FToggleButton>());
            Tabs.Add(Tab.Effects, transform.Find("Details/Header/Buttons/EffectsButton").FindOrAddComponent<FToggleButton>());

            foreach (var tab in Tabs)
            {
                tab.Value.OnClick += () => ShowCategory(tab.Key);
            }

            foreach (var category in Tabs.Keys)
            {
                CategoryGameObjects[category] = new List<GameObject>();
            }
            InitAttributeTab();


        }

        void TryChangeAttribute(string number, Klei.AI.Attribute attribute)
        {
            if (!int.TryParse(number, out var newVal))
            {
                attributeEditors[attribute].SetInputFieldValue("0");
                return;
            }

            if (newVal > 10000) newVal = 10000;

            Stats?.SetAttributeLevel(attribute, newVal);
        }
        void InitAttributeTab()
        {
            var TraitsInterestContainer = transform.Find("Details/Content/ScrollRectContainer/TraitInterestContainer").gameObject;
            TraitsInterestContainer.SetActive(true);

            AptitudeContainer = TraitsInterestContainer.transform.Find("Content/grp/InterestContainer/ScrollArea/Content").gameObject;
            TraitContainer = TraitsInterestContainer.transform.Find("Content/grp/TraitContainer/ScrollArea/Content").gameObject;

            TraitPrefab = TraitContainer.transform.Find("ListViewEntryPrefab").gameObject.AddOrGet<DeletableListEntry>();
            TraitPrefab.gameObject.SetActive(false);

            AptitudePrefab = AptitudeContainer.transform.Find("ListViewEntryPrefab").gameObject.AddOrGet<DeletableListEntry>();
            AptitudePrefab.gameObject.SetActive(false);

            TraitsInterestContainer.SetActive(false);
            TraitsInterestContainer.transform.SetAsFirstSibling();
            CategoryGameObjects[Tab.Attributes].Add(TraitsInterestContainer);

            attributeEditors = new();

            FButton addTraits = TraitsInterestContainer.transform.Find("Content/grp2/AddTraitButton").gameObject.AddOrGet<FButton>();
            addTraits.OnClick += () => UnityDuplicitySelectionScreen.ShowWindow(UnityDuplicitySelectionScreen.OpenedFrom.Trait, (obj) => OnAddTrait((string)obj), () => RebuildTraitsAptitudes());
            FButton addAptitudes = TraitsInterestContainer.transform.Find("Content/grp2/AddInterestButton").gameObject.AddOrGet<FButton>();
            addAptitudes.OnClick += () => UnityDuplicitySelectionScreen.ShowWindow(UnityDuplicitySelectionScreen.OpenedFrom.Interest, (obj) => OnAddAptitude((string)obj), () => RebuildTraitsAptitudes());

            foreach (var attribute in AttributeHelper.GetEditableAttributes())
            {
                var attributeInput = Util.KInstantiateUI<NumberInput>(NumberInputPrefab.gameObject, ParentContainer);
                attributeInput.Text = attribute.Name;
                attributeInput.OnInputChanged += (text) => TryChangeAttribute(text, attribute);
                attributeEditors[attribute] = attributeInput;

                CategoryGameObjects[Tab.Attributes].Add(attributeInput.gameObject);
            }
        }

        private void ShowCategory(Tab key)
        {
            lastCategory = key;
            foreach (var tab in Tabs)
            {
                tab.Value.ChangeSelection(tab.Key == key);
            }

            foreach (var tabtype in (Tab[])Enum.GetValues(typeof(Tab)))
            {
                if (CategoryGameObjects.ContainsKey(tabtype))
                {
                    bool setActive = tabtype == key;

                    foreach (var item in CategoryGameObjects[tabtype])
                    {
                        item.SetActive(setActive);
                    }
                }
            }


            switch (key)
            {
                default:
                    break;

                case Tab.Attributes:
                    RefreshAttributeTab();
                    break;
            }
        }

        private void RefreshAttributeTab()
        {
            SgtLogger.Assert("stats were null", Stats);
            if (Stats == null)
                return;
            RebuildTraitsAptitudes();
            foreach (var attribute in AttributeHelper.GetEditableAttributes())
            {
                attributeEditors[attribute].SetInputFieldValue(Stats.GetAttributeLevel(attribute).ToString());
            }
        }
        private void RebuildTraitsAptitudes()
        {
            foreach (var traitEntry in TraitEntries.Values)
            {
                traitEntry.gameObject.SetActive(false);
            }
            foreach (var traitEntry in AptitudeEntries.Values)
            {
                traitEntry.gameObject.SetActive(false);
            }

            if (Stats.StressTraitId.Length > 0)
            {
                var stress = AddOrGetTraitContainer(Stats.StressTraitId);
                stress.gameObject.SetActive(true);
                stress.transform.SetAsFirstSibling();
            }

            if (Stats.JoyTraitId.Length>0)
            {
                var joy = AddOrGetTraitContainer(Stats.JoyTraitId);
                joy.gameObject.SetActive(true);
                joy.transform.SetAsFirstSibling();
            }

            foreach (var trait in Stats.Traits)
            {
                var traitInfo = AddOrGetTraitContainer(trait);
                traitInfo.gameObject.SetActive(true);
            }
            foreach (var apt in Stats.AptitudeBySkillGroup.Keys)
            {
                var aptitudeInfo = AddOrGetAptitudeContainer(apt);
                aptitudeInfo.gameObject.SetActive(true);
            }
        }
        DeletableListEntry AddOrGetTraitContainer(string traitID)
        {
            var trait = Db.Get().traits.TryGet(traitID);
            if (trait == null)
            {
                SgtLogger.error("trait with the id " + traitID + " not found!");
                return null;
            }

            if (!TraitEntries.ContainsKey(traitID))
            {
                var go = Util.KInstantiateUI(TraitPrefab.gameObject, TraitContainer);
                var entry = go.AddOrGet<DeletableListEntry>();
                entry.Text = trait.Name;
                entry.Tooltip = trait.description;
                entry.backgroundColor = ModAssets.GetColourFromType(ModAssets.GetTraitListOfTrait(traitID));
                entry.OnDeleteClicked = () => OnRemoveTrait(traitID);
                go.SetActive(true);
                TraitEntries[traitID] = entry;
            }

            return TraitEntries[traitID];
        }
        void OnRemoveTrait(string id)
        {
            Stats?.RemoveTrait(id);
            RebuildTraitsAptitudes();
        }
        void OnAddTrait(string id)
        {
            Stats?.AddTrait(id);
            RebuildTraitsAptitudes();
        }


        DeletableListEntry AddOrGetAptitudeContainer(HashedString aptiudeID)
        {
            var aptitude = Db.Get().SkillGroups.TryGet(aptiudeID);
            if (aptitude == null)
            {
                SgtLogger.error("aptitude with the id " + aptitude + " not found!");
                return null;
            }

            if (!AptitudeEntries.ContainsKey(aptiudeID))
            {
                var go = Util.KInstantiateUI(AptitudePrefab.gameObject, AptitudeContainer);
                var entry = go.AddOrGet<DeletableListEntry>();
                entry.Text = ModAssets.GetSkillgroupName(aptitude);
                entry.Tooltip = ModAssets.GetSkillgroupDescription(aptitude);
                entry.OnDeleteClicked = () => OnRemoveAptitude(aptiudeID);
                AptitudeEntries[aptiudeID] = entry;
                go.SetActive(true);
            }

            return AptitudeEntries[aptiudeID];
        }
        void OnRemoveAptitude(HashedString id)
        {
            Stats?.RemoveAptitude(id);
            RebuildTraitsAptitudes();
        }
        void OnAddAptitude(string id)
        {
            Stats?.AddAptitude(id);
            RebuildTraitsAptitudes();
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (!init)
            {
                Init();
            }
            CurrentlyActive = show;

            OnResize();
            if (show)
                ScreenResize.Instance.OnResize += () => OnResize();
            else
                ScreenResize.Instance.OnResize -= () => OnResize();

        }
        public void OnResize()
        {
            var rectMain = this.rectTransform();
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / (rectMain.lossyScale.x)));
            rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / (rectMain.lossyScale.y)));
        }
        private void UpdateCategoryButtons()
        {
            if (lastCategory == Tab.undefined)
                lastCategory = Tab.Attributes;
            ShowCategory(lastCategory);
        }

        private void UpdateMinionButtons(bool refreshPortraits = false)
        {
            HashSet<MinionAssignablesProxy> proxies = new HashSet<MinionAssignablesProxy>();
            HashSet<MinionAssignablesProxy> proxiesToRemove = new HashSet<MinionAssignablesProxy>();

            foreach (MinionIdentity minion in Components.LiveMinionIdentities)
            {
                UpdateMinionButton(refreshPortraits, minion);
                proxies.Add(minion.assignableProxy.Get());
            }
            foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
            {
                foreach (MinionStorage.Info info in minionStorage.GetStoredMinionInfo())
                {
                    if (info.serializedMinion != null)
                    {
                        StoredMinionIdentity storedMinionIdentity = info.serializedMinion.Get<StoredMinionIdentity>();
                        UpdateMinionButton(refreshPortraits, null, storedMinionIdentity);
                        proxies.Add(storedMinionIdentity.assignableProxy.Get());

                    }
                }
            }

            //check all buttons if the dupe still exist, remove from list if yes
            foreach (var entry in MinionButtons.Keys)
            {
                if (!proxies.Contains(entry))
                {
                    proxiesToRemove.Add(entry);
                }
            }
            //remove minion buttons that no longer exist
            foreach (var entry in proxiesToRemove)
            {
                RemoveMinionButton(entry);
            }
        }

        private MinionSelectButton AddOrGetMinionButton(MinionIdentity identity = null, StoredMinionIdentity identityStored = null)
        {
            MinionAssignablesProxy proxy = identity != null ? identity.assignableProxy.Get() : identityStored.assignableProxy.Get();
            if (!MinionButtons.ContainsKey(proxy))
            {
                GameObject button = Util.KInstantiateUI(MinionButtonPrefab, MinionButtonContainer, true);
                var component = button.AddOrGet<MinionSelectButton>();

                var img = button.transform.Find("Image").gameObject;
                img.SetActive(false);
                var minionImage = Util.KInstantiateUI(MinionPortraitHelper.GetCrewPortraitPrefab(), button, true);
                minionImage.TryGetComponent<MinionPortraitHelper>(out var helper);
                //minionImage.TryGetComponent<LayoutElement>(out var layout);
                //layout.preferredHeight = 60;
                //layout.preferredWidth = 60;

                minionImage.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 4, 55);
                minionImage.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 4, 55);

                //minionImage.useLabels = false;
                //minionImage.SetIdentityObject(proxy, false);
                component.Init(helper, () => TryChangeMinion(proxy));

                MinionButtons[proxy] = component;
            }
            return MinionButtons[proxy];
        }
        private void RemoveMinionButton(MinionAssignablesProxy proxy)
        {
            if (MinionButtons.ContainsKey(proxy))
            {
                UnityEngine.Object.Destroy(MinionButtons[proxy].gameObject);
                MinionButtons.Remove(proxy);
            }
        }
        public void TryChangeMinion(MinionAssignablesProxy newMinion)
        {
            foreach (var btn in MinionButtons)
            {
                btn.Value.SetActiveState(btn.Key == SelectedMinion);
            }


            if (newMinion == SelectedMinion)
                return;

            if (PendingChanges())
            {
                KMod.Manager.Dialog(Global.Instance.globalCanvas,
                       STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TITLE,
                       STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.TEXT,
                STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.APPLYCHANGES,
                       () =>
                       {
                           Stats.Apply(SelectedMinion);
                           GenerateMinionEditStats(newMinion);
                       },
                STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.DISCARDCHANGES,
                       () => GenerateMinionEditStats(newMinion),
                STRINGS.UI.DUPEEDITING.CONFIRMATIONDIALOG.CANCEL,
                       () => { });
            }
            else
            {
                GenerateMinionEditStats(newMinion);
            }
        }

        private List<KeyValuePair<string, string>> GetAccessoryIDs(MinionIdentity identity = null, StoredMinionIdentity identityStored = null)
        {

            List<KeyValuePair<string, string>> accessories = new List<KeyValuePair<string, string>>();
            List<ResourceRef<Accessory>> accessoriesOrigin = new();

            if (identityStored != null)
            {
                accessoriesOrigin = identityStored.accessories;
            }
            else if (identity != null && identity.TryGetComponent<Accessorizer>(out var accessorizer))
            {
                accessoriesOrigin = accessorizer.accessories;
            }

            foreach (var accessory in accessoriesOrigin)
            {
                if (accessory.Get() != null)
                {
                    accessories.Add(new KeyValuePair<string, string>(accessory.Get().slot.Id, accessory.Get().Id));
                }
            }
            return accessories;
        }

        private void UpdateMinionButton(bool refreshPortraits, MinionIdentity identity = null, StoredMinionIdentity identityStored = null)
        {
            bool storedMinion = identityStored != null;

            MinionAssignablesProxy proxy = identity != null ? identity.assignableProxy.Get() : identityStored.assignableProxy.Get();

            var button = AddOrGetMinionButton(identity, identityStored);
            if (refreshPortraits)
                button.UpdatePortrait(GetAccessoryIDs(identity, identityStored));

            //button.helper.ForceRefresh(); 
            button.UpdateName(identity, identityStored);
            button.UpdateState(storedMinion, proxy == SelectedMinion);
        }
    }
}
