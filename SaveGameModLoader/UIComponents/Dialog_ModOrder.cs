using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeterHan.PLib.UI.PUIDelegates;
using static SaveGameModLoader.STRINGS.UI.FRONTEND.MODTAGS;
using UnityEngine;
using UtilLibs;
using static SaveGameModLoader.STRINGS.UI.FRONTEND;

namespace SaveGameModLoader.UIComponents
{
    internal class Dialog_ModOrder
    {
        const string DialogOption_Ok = "ok";
        const string DialogOption_Cancel = "cancel";
        const string DialogOption_Clear = "clear";

        int CurrentModIndex = -1;
        string ModTitle = "";
        int TargetModPos = -1;

        int MaxCount = -1;
        GameObject sourceGO;
        private PDialog _pDialog = null;
        private PPanel _dialogBody = null;
        private KScreen _componentScreen = null;
        const int SpacingInPixels = 4;

        public static Dialog_ModOrder Instance;

        public static void ShowIndexDialog(int modIndex, string title,  GameObject source)
        {
            Close();
            Instance = new Dialog_ModOrder(modIndex,  title,source);
            Instance.CreateAndShow(source);
        }
        public static void Close()
        {
            Instance?._componentScreen?.Deactivate();
        }

        private Dialog_ModOrder(int modIndex, string title, GameObject source)
        {
            if(title.Count () > 40) 
            {
                title = title.Substring(0, 38) + "...";
            }

            ModTitle = title;
            CurrentModIndex = modIndex;
            TargetModPos = modIndex;
            sourceGO = source;
        }
        private void OnDialogClosed(string option)
        {
            if (option != DialogOption_Ok)
            {
                return;
            }
        }
        public void InsertMod(int TargetPosition)
        {
            var manager = Global.Instance.modManager;
            if (TargetPosition == 0)
                TargetPosition--;

            if(CurrentModIndex!=TargetPosition) 
                manager.Reinsert(CurrentModIndex, TargetPosition, TargetPosition >= manager.mods.Count,this);
        }

        private void CreateAndShow(object obj)
        {
            var dialog = new PDialog("EditModOrder")
            {
                Title = MODORDER.WINDOWTITLE,
                DialogClosed = OnDialogClosed,
                Size = new Vector2 { x = 150, y = 160 },
                //MaxSize = new Vector2 { x = 150, y = 160 },
                SortKey = 300.0f,
                
            };
            dialog.AddButton(DialogOption_Cancel, global::STRINGS.UI.CONFIRMDIALOG.CANCEL, null, PUITuning.Colors.ButtonBlueStyle);

            _pDialog = dialog;
            _dialogBody = dialog.Body;
            _dialogBody.Margin = new RectOffset(0, 0, SpacingInPixels, SpacingInPixels);
            _dialogBody.Direction = PanelDirection.Vertical;
            //.AddButton(DialogOption_Ok, global::STRINGS.UI.CONFIRMDIALOG.OK, null, PUITuning.Colors.ButtonPinkStyle)

            RebuildAndShow();
        }

        private void RebuildAndShow()
        {
            _componentScreen?.Deactivate();
            GenerateControlPanel();

            _componentScreen = null;
            var isBuilt = _pDialog.Build().TryGetComponent<KScreen>(out _componentScreen);
            if (isBuilt)
            {
                _componentScreen.Activate();
                var size = _componentScreen.rectTransform().localScale;
                Debug.Log(size);
                var sourcePos = sourceGO.rectTransform().GetPosition();
                //sourcePos.x += (size.x/2f);
                //sourcePos.y -= (size.y/2f);
                _componentScreen.rectTransform().SetPosition(sourcePos);
            }
        }
        private void MoveToTop(GameObject source)
        {
            InsertMod(-1);
            _componentScreen?.Deactivate();
        }
        private void MoveToBottom(GameObject source)
        {
            var manager = Global.Instance.modManager;
            InsertMod(manager.mods.Count-1);
            _componentScreen?.Deactivate();
        }
        private void MoveToIndex(GameObject source)
        {
            InsertMod(TargetModPos);
            _componentScreen?.Deactivate();
        }

        private void OnNumInput(GameObject source,string value)
        {
            if(int.TryParse(value, out var index))
            {
                TargetModPos = index;
            }
        }

        private void GenerateControlPanel()
        {

            var mainDialoguePanel = new PPanel("mainDialoguePanel") { Direction = PanelDirection.Vertical, Spacing = SpacingInPixels };
            _dialogBody.AddChild(mainDialoguePanel);

            var titleLabel = new PLabel("NameLabel") { Text = ModTitle};
            mainDialoguePanel.AddChild(titleLabel);

            var MoveToTopButton = new PButton("MoveToTop") { Margin = new RectOffset(SpacingInPixels, SpacingInPixels, SpacingInPixels, SpacingInPixels), FlexSize = new(1,0) };

            MoveToTopButton.Text = MODORDER.PUTTOSTART;
            MoveToTopButton.ToolTip = MODORDER.PUTTOSTART_TOOLTIP;
            MoveToTopButton.OnClick = MoveToTop;
            mainDialoguePanel.AddChild(MoveToTopButton);


            var MoveToBottomButton = new PButton("MoveToBottom") { Margin = new RectOffset(SpacingInPixels, SpacingInPixels, SpacingInPixels, SpacingInPixels), FlexSize = new(1, 0) };
            MoveToBottomButton.Text = MODORDER.PUTTOEND;
            MoveToBottomButton.ToolTip = MODORDER.PUTTOEND_TOOLTIP;
            MoveToBottomButton.OnClick = MoveToBottom;
            mainDialoguePanel.AddChild(MoveToBottomButton);


            var addRemoveDialogSettingsPanel = new PPanel("AddRemoveDialogSettingsPanel") { Direction = PanelDirection.Horizontal, FlexSize = new(1, 0) };
            mainDialoguePanel.AddChild(addRemoveDialogSettingsPanel);
            

            var MoveToIndexButton = new PButton("MoveToIndex") { Margin = new RectOffset(SpacingInPixels, SpacingInPixels, SpacingInPixels, SpacingInPixels), FlexSize = new(1, 0) };
            MoveToIndexButton.Text = MODORDER.PUTTOINDEX;
            MoveToIndexButton.ToolTip = MODORDER.PUTTOINDEX_TOOLTIP;
            MoveToIndexButton.OnClick = MoveToIndex;
            addRemoveDialogSettingsPanel.AddChild(MoveToIndexButton);

            var txtFilter = new PTextField("ModIndex")
            {
                Text = CurrentModIndex.ToString(),
                MinWidth = 30,
                TextAlignment = TMPro.TextAlignmentOptions.CenterGeoAligned,
                OnTextChanged = OnNumInput,
                Type = PTextField.FieldType.Integer
            };
            addRemoveDialogSettingsPanel.AddChild(txtFilter);
        }

    }
}
