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
using static DupePrioPresetManager.STRINGS.UI.PRESETWINDOWDUPEPRIOS;
using Satsuma;
using static Operational;
using System.Security.Policy;
using static BestFit;

namespace DupePrioPresetManager
{
    internal class UnityPresetScreen_Consumables : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityPresetScreen_Consumables Instance = null;


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
        MinionConsumableSettingsPreset CurrentlySelected;
        ///Referenced Stats to apply presets to.
        Action<HashSet<Tag>> ApplyingAction = null;

        Dictionary<MinionConsumableSettingsPreset, GameObject> Presets = new Dictionary<MinionConsumableSettingsPreset, GameObject>();
        //List<GameObject> InformationObjects = new List<GameObject>();

        Dictionary<string, Tuple<FButton, Image>> Consumables = new Dictionary<string, Tuple<FButton, Image>>();
        LocText TitleHolder = null;

        string RefName;

        public static void ShowWindow(HashSet<Tag> currentlyForbidden, Action<HashSet<Tag>> ApplyingAction, System.Action onClose, string refName = "")
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, ModAssets.ParentScreen, true);
                Instance = screen.AddOrGet<UnityPresetScreen_Consumables>();
                Instance.Init();
            }
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
            Instance.LoadAllPresets();
            Instance.RefName = refName;
            Instance.LoadTemporalPreset(currentlyForbidden);
            Instance.ApplyingAction = ApplyingAction;
            Instance.OnCloseAction = onClose;
            Instance.Searchbar.Text = string.Empty;
        }

        private bool init;
        private System.Action OnCloseAction;

        public void LoadTemporalPreset(HashSet<Tag> toGenerateFrom)
        {
            MinionConsumableSettingsPreset tempStats = MinionConsumableSettingsPreset.CreateFromPriorityManager(toGenerateFrom, RefName);
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

        List<MinionConsumableSettingsPreset> LoadPresets()
        {
            List<MinionConsumableSettingsPreset> minionStatConfigs = new List<MinionConsumableSettingsPreset>();
            var files = new DirectoryInfo(ModAssets.FoodTemplatePath).GetFiles();


            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                try
                {
                    var preset = MinionConsumableSettingsPreset.ReadFromFile(File);
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
            return minionStatConfigs;
        }

        private bool AddUiElementForPreset(MinionConsumableSettingsPreset config)
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

                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("RenameButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.RENAMEPRESETTOOLTIP);
                UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("DeleteButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.DELETEPRESETTOOLTIP);
                Presets[config] = PresetHolder;
                return true;
            }
            return false;
        }

        void DeletePreset(MinionConsumableSettingsPreset config)
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

        void SetAsCurrent(MinionConsumableSettingsPreset config)
        {
            CurrentlySelected = config;
            RebuildInformationPanel();
        }
        void RebuildInformationPanel()
        {
            //for (int i = InformationObjects.Count - 1; i >= 0; i--)
            //{
            //    Destroy(InformationObjects[i]);
            //}
            if (CurrentlySelected == null)
                return;

            TitleHolder.text = CurrentlySelected.ConfigName;
            foreach (var consumable in Consumables)
            {
                SetAllowedSprite(!CurrentlySelected.ForbiddenTags.Contains(consumable.Key.ToTag()), consumable.Value.second);
                consumable.Value.first.SetInteractable(Presets.ContainsKey(CurrentlySelected));
            }
            GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected));
        }

        void ChangeValue(string id, Image image)
        {
            CurrentlySelected.ChangeValue(id);
            SetAllowedSprite(!CurrentlySelected.ForbiddenTags.Contains(id.ToTag()), image);
        }

        private void SetAllowedSprite(bool allowed, Image image)
        {
            image.gameObject.SetActive(allowed);
        }


        public string ChoreGroupName(ChoreGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + group.Id.ToUpperInvariant() + ".NAME");
        }
        public string ChoreGroupTooltip(ChoreGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + group.Id.ToUpperInvariant() + ".DESC");
        }

        private void Init()
        {

            UIUtils.TryChangeText(transform, "Title", TITLECONSUMABLES);


            GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrent").FindOrAddComponent<FButton>();
            CloseButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").FindOrAddComponent<FButton>();
            ApplyButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/ApplyPresetButton").FindOrAddComponent<FButton>();

            OpenPresetFolder = transform.Find("HorizontalLayout/ObjectList/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            OpenPresetFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.FoodTemplatePath) { UseShellExecute = true });


            Searchbar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
            Searchbar.OnValueChanged.AddListener(ApplyFilter);
            Searchbar.Text = string.Empty;


            ClearSearchBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
            ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

            ApplyButton.OnClick += () =>
            {
                ApplyingAction.Invoke(CurrentlySelected.ForbiddenTags);
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
            ///Filling the items
            var Name = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
            //UIUtils.TryChangeText(Name.transform, "Label", "\"" + CurrentlySelected.ConfigName + "\"");
            TitleHolder = Name.transform.Find("Label").GetComponent<LocText>();

            //InformationObjects.Add(Name);

            foreach (IConsumableUIItem ConsumableItem in MinionConsumableSettingsPreset.ConsumableUIItems)
            {
                var ConsumableAllowedItem = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
                GameObject prefab = Assets.GetPrefab(ConsumableItem.ConsumableId.ToTag());
                prefab.TryGetComponent<KBatchedAnimController>(out var animationController);
                if (animationController.AnimFiles.Length > 0)
                {
                    Sprite fromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(animationController.AnimFiles[0]);
                    if (ConsumableAllowedItem.transform.Find("Label/TraitImage").TryGetComponent<Image>(out var image))
                    {
                        UnityEngine.Rect rect = fromMultiObjectAnim.rect;
                        if (rect.width > rect.height)
                        {
                            var size = (rect.height / rect.width) * 40f;
                            image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                        }
                        else
                        {
                            var size = (rect.width / rect.height) * 40f;
                            image.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (-45 + (40 - size) / 2), size);
                        }

                        image.sprite = fromMultiObjectAnim;
                    }
                }
                UIUtils.TryChangeText(ConsumableAllowedItem.transform, "Label", ConsumableItem.ConsumableName);

                if (prefab.TryGetComponent<InfoDescription>(out var descHolder))
                {
                    UIUtils.AddSimpleTooltipToObject(ConsumableAllowedItem.transform.Find("Label"), descHolder.description, true);
                }


                if (ConsumableAllowedItem.transform.Find("AddThisTraitButton/image").TryGetComponent<Image>(out var prioimage))
                {
                    prioimage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
                    prioimage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
                    prioimage.sprite = Assets.GetSprite("overview_jobs_icon_checkmark");
                    prioimage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                }

                var PrioChangeBtn = ConsumableAllowedItem.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>();
                PrioChangeBtn.OnClick += () => ChangeValue(ConsumableItem.ConsumableId, prioimage);
                PrioChangeBtn.OnPointerEnterAction += () => this.HoveringPrio = true;
                PrioChangeBtn.OnPointerExitAction += () => this.HoveringPrio = false;

                Consumables[ConsumableItem.ConsumableId] = new Tuple<FButton, Image>(PrioChangeBtn, prioimage);

                //InformationObjects.Add(ConsumableAllowedItem);

            }

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
