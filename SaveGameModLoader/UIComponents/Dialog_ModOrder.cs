//using PeterHan.PLib.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static PeterHan.PLib.UI.PUIDelegates;
//using static SaveGameModLoader.STRINGS.UI.FRONTEND.MODTAGS;
//using UnityEngine;
//using UtilLibs;

//namespace SaveGameModLoader.UIComponents
//{
//    internal class Dialog_ModOrder
//    {
//        const string DialogOption_Ok = "ok";
//        const string DialogOption_Cancel = "cancel";
//        const string DialogOption_Clear = "clear";

//        int CurrentModID = -1;
//        GameObject sourceGO;

//        public static void ShowFilterDialog(int modIndex, GameObject source)
//        {
//            var instance = new Dialog_ModOrder(modIndex, source);
//            instance.CreateAndShow(null);
//        }

//        private Dialog_ModOrder(int modIndex, GameObject source)
//        {
//            CurrentModID = modIndex; 
//            sourceGO = source;
//        }
//        private void OnDialogClosed(string option)
//        {
//            if (option != DialogOption_Ok)
//            {
//                return;
//            }
//        }

//        private void CreateAndShow(object obj)
//        {
//            var dialog = new PDialog("EditModOrder")
//            {
//                Title = "",
//                DialogClosed = OnDialogClosed,
//                Size = new Vector2 { x = 100, y = 100 },
//                MaxSize = new Vector2 { x = 100, y = 100 },
//                SortKey = 300.0f
//            }

//            .AddButton(DialogOption_Ok, global::STRINGS.UI.CONFIRMDIALOG.OK, null, PUITuning.Colors.ButtonPinkStyle)
//            .AddButton(DialogOption_Cancel, global::STRINGS.UI.CONFIRMDIALOG.CANCEL, null, PUITuning.Colors.ButtonBlueStyle);

//            RebuildAndShow(showFirstTime: true);
//        }

//        private void RebuildAndShow(bool showFirstTime = false, bool rebuildData = false)
//        {
//            AllCheckbox = null;
//            if (!showFirstTime)
//            {
//                _componentScreen?.Deactivate();
//            }
//            if (showFirstTime || rebuildData)
//            {
//                GenerateInitialData();
//            }
//            _pDialog.Title = targetsMod ? TAGEDITWINDOW.TITLE_MOD : TAGEDITWINDOW.TITLE_SELECTOR;
//            GenerateControlPanel();

//            _componentScreen = null;
//            var isBuilt = _pDialog.Build().TryGetComponent<KScreen>(out _componentScreen);
//            if (isBuilt)
//            {
//                _componentScreen.Activate();
//            }
//        }

//        private void GenerateControlPanel()
//        {
//            var addRemoveDialogSettingsPanel = new PPanel("AddRemoveDialogSettingsPanel") { Direction = PanelDirection.Horizontal, Spacing = SpacingInPixels };

//            addRemoveDialogSettingsPanel.AddChild(new PLabel("FilterFlagsLabel") { Text = TAGEDITWINDOW.FILTERTAGS });

//            var txtFilter = new PTextField("TextFilterFlags")
//            {
//                Text = FilterText,
//                MinWidth = 80,
//                TextAlignment = TMPro.TextAlignmentOptions.CenterGeoAligned,
//                OnTextChanged = OnTextChanged_Filter
                
//            };
//            addRemoveDialogSettingsPanel.AddChild(txtFilter);

//            //var btnRefresh = new PButton("BtnFilterFlags") { Margin = new RectOffset(LeftOffset, RightOffset, TopOffset, BottomOffset) };
//            //btnRefresh.Text = TAGEDITWINDOW.FILTERBTN;
//            //btnRefresh.OnClick = OnClick_Refresh;
//            //addRemoveDialogSettingsPanel.AddChild(btnRefresh);

//            _dialogBodyChild.AddChild(addRemoveDialogSettingsPanel);

//            var addNewTagPanel = new PPanel("AddNewTagPanel") { Direction = PanelDirection.Horizontal, Spacing = SpacingInPixels };

//            addNewTagPanel.AddChild(new PLabel("AddTagLabel") { Text = TAGEDITWINDOW.ADDNEWTAGLABEL });

//            var txt_AddTag = new PTextField("AddTagInputText")
//            {
//                Text = NewFlagText,
//                MinWidth = 386,
//                TextAlignment = TMPro.TextAlignmentOptions.CenterGeoAligned,
//                OnTextChanged = OnTextChanged_AddNew
//            };
//            addNewTagPanel.AddChild(txt_AddTag);

//            var AddTagBtn = new PButton("AddTagBtn") { Margin = new RectOffset(LeftOffset, RightOffset, TopOffset, BottomOffset) };
//            AddTagBtn.Text = TAGEDITWINDOW.ADDNEWTAGBTN;
//            AddTagBtn.OnClick = OnConfirm_AddFilterTag;
//            addNewTagPanel.AddChild(AddTagBtn);

//            _dialogBodyChild.AddChild(addNewTagPanel);

//            var pageButtonsPanel = new PPanel("EntriesPanel")
//            {
//                Direction = PanelDirection.Horizontal,
//                Spacing = SpacingInPixels,
//                FlexSize = Vector2.right
//            };

//            _dialogBodyChild.AddChild(pageButtonsPanel);
//        }

//    }
//}
