using PeterHan.PLib.UI;
using SaveGameModLoader.UIComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UtilLibs;
using static ModInfo;
using static Operational;
using static SaveGameModLoader.STRINGS.UI.FRONTEND.MODTAGS;

namespace SaveGameModLoader
{
    internal class Dialog_EditFilterTags
    {
        private class EditFilterTagsDialog_Item
        {
            public string FlagName { get; set; }
            public int IsChecked { get; set; }
        }

        private PDialog _pDialog = null;
        private PPanel _dialogBody = null;
        private PPanelWithClearableChildren _dialogBodyChild = null;
        private KScreen _componentScreen = null;
        private readonly List<EditFilterTagsDialog_Item> _dialogData = new List<EditFilterTagsDialog_Item>();
        private readonly List<EditFilterTagsDialog_Item> _dialogData_Filtered = new List<EditFilterTagsDialog_Item>();
        private readonly Dictionary<string, bool> _TagTargetData = new Dictionary<string, bool>();

        const string DialogOption_Ok = "ok";
        const string DialogOption_Cancel = "cancel";
        const string DialogOption_Clear = "clear";
        const int LeftOffset = 4;
        const int RightOffset = 4;
        const int TopOffset = 4;
        const int BottomOffset = 4;
        const int SpacingInPixels = 4;

        public string NewFlagText { get; set; } = string.Empty;

        public string FilterText { get; set; } = string.Empty;

        public string TargetModId { get; private set; } = string.Empty;
        public bool targetsMod { get; private set; } = true;
        public System.Action OnApply { get; private set; }

        private static Dialog_EditFilterTags instance;
        public static void ShowFilterDialog(string targetModID, System.Action onApply = null, bool targetsSingleMod = true)
        {
            if (instance == null)
            {
                instance = new Dialog_EditFilterTags(targetModID, onApply, targetsSingleMod);
                instance.CreateAndShow(null);
            }
            else
            {
                instance.TargetModId = targetModID;
                instance.OnApply = onApply;
                instance.targetsMod = targetsSingleMod;
                instance.RebuildAndShow(rebuildData: true);
            }

        }

        private Dialog_EditFilterTags(string targetModID, System.Action onApply = null, bool targetsSingleMod = true)
        {
            TargetModId = targetModID;
            targetsMod = targetsSingleMod;
            OnApply = onApply;
        }

        private void CreateAndShow(object obj)
        {
            var dialog = new PDialog("EditFilterTags")
            {
                Title = targetsMod ? TAGEDITWINDOW.TITLE_MOD : TAGEDITWINDOW.TITLE_SELECTOR,
                DialogClosed = OnDialogClosed,
                Size = new Vector2 { x = 700, y = 700 },
                MaxSize = new Vector2 { x = 700, y = 700 },
                SortKey = 300.0f
            }
            .AddButton(DialogOption_Ok, global::STRINGS.UI.CONFIRMDIALOG.OK, null, PUITuning.Colors.ButtonPinkStyle)
            .AddButton(DialogOption_Cancel, global::STRINGS.UI.CONFIRMDIALOG.CANCEL, null, PUITuning.Colors.ButtonBlueStyle);

            _componentScreen = null;
            _pDialog = dialog;
            _dialogBody = dialog.Body;
            _dialogBodyChild = null;
            FilterText = string.Empty;
            NewFlagText = string.Empty;
            _TagTargetData.Clear();
            RebuildAndShow(showFirstTime: true);
        }

        private void RebuildAndShow(bool showFirstTime = false, bool rebuildData = false)
        {
            AllCheckbox = null;
            if (!showFirstTime)
            {
                _componentScreen?.Deactivate();
            }
            if (showFirstTime || rebuildData)
            {
                GenerateInitialData();
            }
            _pDialog.Title = targetsMod ? TAGEDITWINDOW.TITLE_MOD : TAGEDITWINDOW.TITLE_SELECTOR;
            ClearContents();
            GenerateFilteredData();
            GenerateControlPanel();
            GenerateRecordsPanel();

            _componentScreen = null;
            var isBuilt = _pDialog.Build().TryGetComponent<KScreen>(out _componentScreen);
            if (isBuilt)
            {
                _componentScreen.Activate();
            }
        }

        private void ClearContents()
        {
            if (_dialogBodyChild == null)
            {
                _dialogBodyChild = new PPanelWithClearableChildren("EditFilterTagsDialog_RecordsPanel");
                _dialogBody.AddChild(_dialogBodyChild);
            }

            _dialogBodyChild.ClearChildren();
        }

        private void GenerateInitialData()
        {
            _dialogData.Clear();
            _TagTargetData.Clear();

            var checkedFlags = MPM_Config.Instance.GetCheckedTags(TargetModId);
            var configNames_Sorted = new SortedSet<string>(checkedFlags);
            var uncheckedFlags = MPM_Config.Instance.GetUncheckedTags(TargetModId);
            if (targetsMod)
            {
                var mod = Global.Instance.modManager.mods.FirstOrDefault(mod => mod.label.defaultStaticID == TargetModId);
                if (mod == null)
                {
                    SgtLogger.warning("mod " + TargetModId + " not found!");
                    return;
                }
            }

            foreach (var Flag in checkedFlags)
            {
                var rec = new EditFilterTagsDialog_Item()
                {
                    IsChecked = 1,
                    FlagName = Flag
                };
                _dialogData.Add(rec);
                _TagTargetData.Add(Flag, true);
            }

            foreach (var uncheckedFlag in uncheckedFlags)
            {
                var rec = new EditFilterTagsDialog_Item()
                {
                    IsChecked = 0,
                    FlagName = uncheckedFlag
                };
                _dialogData.Add(rec);
                _TagTargetData.Add(uncheckedFlag, false);
            }
            
        }

        private void GenerateFilteredData()
        {
            _dialogData_Filtered.Clear();
            foreach (var entry in _dialogData)
            {
                if (!string.IsNullOrEmpty(FilterText))
                {
                    if ((string.IsNullOrEmpty(entry.FlagName) || !entry.FlagName.ToLowerInvariant().Contains(FilterText.ToLower()))
                        && (string.IsNullOrEmpty(entry.FlagName) || !entry.FlagName.ToLowerInvariant().Contains(FilterText.ToLower()))
                       )
                    {
                        continue;
                    }
                }
                _dialogData_Filtered.Add(entry);
            }


            var sortedList = _dialogData_Filtered.OrderBy(x => x.FlagName).OrderByDescending(y => y.IsChecked).ToList();
            _dialogData_Filtered.Clear();
            _dialogData_Filtered.AddRange(sortedList);

        }

        private void GenerateControlPanel()
        {
            var addRemoveDialogSettingsPanel = new PPanel("AddRemoveDialogSettingsPanel") { Direction = PanelDirection.Horizontal, Spacing = SpacingInPixels };

            addRemoveDialogSettingsPanel.AddChild(new PLabel("FilterFlagsLabel") { Text = TAGEDITWINDOW.FILTERTAGS });

            var txtFilter = new PTextField("TextFilterFlags")
            {
                Text = FilterText,
                MinWidth = 450,
                TextAlignment = TMPro.TextAlignmentOptions.Left,
                OnTextChanged = OnTextChanged_Filter
            };
            addRemoveDialogSettingsPanel.AddChild(txtFilter);

            //var btnRefresh = new PButton("BtnFilterFlags") { Margin = new RectOffset(LeftOffset, RightOffset, TopOffset, BottomOffset) };
            //btnRefresh.Text = TAGEDITWINDOW.FILTERBTN;
            //btnRefresh.OnClick = OnClick_Refresh;
            //addRemoveDialogSettingsPanel.AddChild(btnRefresh);

            _dialogBodyChild.AddChild(addRemoveDialogSettingsPanel);

            var addNewTagPanel = new PPanel("AddNewTagPanel") { Direction = PanelDirection.Horizontal, Spacing = SpacingInPixels };

            addNewTagPanel.AddChild(new PLabel("AddTagLabel") { Text = TAGEDITWINDOW.ADDNEWTAGLABEL });

            var txt_AddTag = new PTextField("AddTagInputText")
            {
                Text = NewFlagText,
                MinWidth = 386,
                TextAlignment = TMPro.TextAlignmentOptions.Left,
                OnTextChanged = OnTextChanged_AddNew
            };
            addNewTagPanel.AddChild(txt_AddTag);

            var AddTagBtn = new PButton("AddTagBtn") { Margin = new RectOffset(LeftOffset, RightOffset, TopOffset, BottomOffset) };
            AddTagBtn.Text = TAGEDITWINDOW.ADDNEWTAGBTN;
            AddTagBtn.OnClick = OnConfirm_AddFilterTag;
            addNewTagPanel.AddChild(AddTagBtn);

            _dialogBodyChild.AddChild(addNewTagPanel);

            var pageButtonsPanel = new PPanel("EntriesPanel")
            {
                Direction = PanelDirection.Horizontal,
                Spacing = SpacingInPixels,
                FlexSize = Vector2.right
            };

            _dialogBodyChild.AddChild(pageButtonsPanel);
        }

        private void GenerateRecordsPanel()
        {
            var scrollBody = new PPanel("ScrollContent")
            {
                Spacing = 10,
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.UpperCenter,
                FlexSize = Vector2.right,

            };

            {//"ALL" item
                var contents = new PGridPanel("Entries") { FlexSize = Vector2.right };

                contents.AddRow(new GridRowSpec());
                contents.AddColumn(new GridColumnSpec(550));

                var lCheckbox = new PCheckBox(name: TAGEDITWINDOW.ALLFILTERS);
                int activeFilters = _dialogData_Filtered.Where(i => _TagTargetData[i.FlagName]).Count();
                int allFilters = _dialogData_Filtered.Count();

                if (activeFilters == 0)
                    lCheckbox.InitialState = 0;
                else if (activeFilters < allFilters)
                    lCheckbox.InitialState = 2;
                else
                    lCheckbox.InitialState = 1;
                lCheckbox.OnRealize += (go) =>
                {
                    AllCheckbox = go;
                };

                lCheckbox.Text = TAGEDITWINDOW.ALLFILTERS;

                lCheckbox.OnChecked = OnChecked_AllItem;
                contents.AddChild(lCheckbox, new GridComponentSpec(0, 0) { Alignment = TextAnchor.MiddleLeft, Margin = new RectOffset(0,0,3,6) });

                //scrollBody.AddChild(contents);
                _dialogBodyChild.AddChild(contents);
            }
            foreach (EditFilterTagsDialog_Item entry in _dialogData_Filtered)
            {
                var contents = new PGridPanel("Entries") { FlexSize = Vector2.right };

                contents.AddRow(new GridRowSpec());
                contents.AddColumn(new GridColumnSpec(520));

                var lCheckbox = new PCheckBox(name: entry.FlagName);
                lCheckbox.InitialState = _TagTargetData[entry.FlagName] ? 1 : 0; // entry.IsChecked;

                lCheckbox.Text = entry.FlagName;

                lCheckbox.OnChecked = OnChecked_RecordItem;
                contents.AddChild(lCheckbox, new GridComponentSpec(0, 0) { Alignment = TextAnchor.MiddleLeft });
                if (!targetsMod)
                {
                    var deleteFlagBtn = new PButton("deleteFlagBtn");
                    deleteFlagBtn.Text = TAGEDITWINDOW.DELETETAG;
                    deleteFlagBtn.OnClick = (_) => OnRemoveFlag(entry.FlagName);
                    contents.AddChild(deleteFlagBtn, new GridComponentSpec(0, 1) { Alignment = TextAnchor.MiddleRight });
                }

                scrollBody.AddChild(contents);
            }

            var scrollPane = new PScrollPane()
            {
                ScrollHorizontal = false,
                ScrollVertical = true,
                Child = scrollBody,
                FlexSize = Vector2.right,
                TrackSize = 15,
                AlwaysShowHorizontal = false,
                AlwaysShowVertical = false
            };
            _dialogBodyChild.AddChild(scrollPane);
        }

        private bool TryGetRecord(string name, out EditFilterTagsDialog_Item record)
        {
            if (!_dialogData_Filtered.Any(x => x.FlagName == name))
            {
                record = null;
                return false;
            }
            record = _dialogData.Where(x => x.FlagName == name).First();
            return true;
        }

        public void ApplyChanges(List<Tuple<string, bool>> modifiedRecords)
        {
            MPM_Config.Instance.SetModTagConfigState(TargetModId, modifiedRecords);
            if (OnApply != null)
                OnApply();
        }

        private void OnDialogClosed(string option)
        {
            if (option != DialogOption_Ok)
            {
                return;
            }
            var addRemoveRecords = new List<Tuple<string, bool>>();
            foreach (var entry in _TagTargetData)
            {
                addRemoveRecords.Add(new Tuple<string, bool>(entry.Key, entry.Value));
            }
            ApplyChanges(addRemoveRecords);
        }

        private void OnChecked_AllItem(GameObject source, int state)
        {
            int newState = 0;
            switch (state)
            {
                case 2:
                    newState = 1;
                    break;
                case 1:
                    newState = 0;
                    break;
                case 0:
                    newState = 1;
                    break;
            }

            PCheckBox.SetCheckState(source, newState);
            foreach (var entry in _dialogData_Filtered)
            {
                entry.IsChecked = newState;
                _TagTargetData[entry.FlagName] = (newState==1);

            }
            RebuildAndShow();
        }
        static GameObject AllCheckbox;
        void RefreshAllCheckbox()
        {
            int activeFilters = _dialogData_Filtered.Where(i => _TagTargetData[i.FlagName]).Count();
            int allFilters = _dialogData_Filtered.Count();

            //if (AllCheckbox == null)
            //{
            //    SgtLogger.l("checkbox gone :(");
            //    return;
            //}
            //else
            //    SgtLogger.l("setting all checkbox to " + allFilters + ", " + activeFilters);

            if (activeFilters == 0)
                PCheckBox.SetCheckState(AllCheckbox, 0);
            else if (activeFilters < allFilters)
                PCheckBox.SetCheckState(AllCheckbox, 2);
            else
                PCheckBox.SetCheckState(AllCheckbox, 1);
        }

        private void OnChecked_RecordItem(GameObject source, int state)
        {
            int newState = (state + 1) % 2;
            PCheckBox.SetCheckState(source, newState);
            var checkButton = source.GetComponentInChildren<MultiToggle>();
            var configName = checkButton.name;
            if (!TryGetRecord(configName, out var record))
            {
                return;
            }
            if (!_TagTargetData.ContainsKey(configName))
            {
                _TagTargetData.Add(configName, record.IsChecked == 1);
            }
            _TagTargetData[configName] = (newState==1);
            RefreshAllCheckbox();
        }
        private void OnTextChanged_AddNew(GameObject source, string text)
        {
            NewFlagText = text;
        }
        private void OnConfirm_AddFilterTag(GameObject source)
        {
            if (NewFlagText == string.Empty)
                return;

            MPM_Config.Instance.AddFilterTag(NewFlagText);
            NewFlagText = string.Empty;
            RebuildAndShow(rebuildData: true);
        }
        private void OnRemoveFlag(string flag)
        {
            MPM_Config.Instance.RemoveFilterTag(flag);
            RebuildAndShow(rebuildData: true);
        }

        private void OnTextChanged_Filter(GameObject source, string text)
        {
            FilterText = text;
            RebuildAndShow();
        }
    }
}
