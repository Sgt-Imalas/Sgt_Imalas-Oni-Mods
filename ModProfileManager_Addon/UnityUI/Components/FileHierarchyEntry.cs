using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.UIcmp;
using UtilLibs;
using static ModProfileManager_Addon.STRINGS.UI.PRESETOVERVIEW;
using UtilLibs.UI.FUI;
using ModProfileManager_Addon.ModProfileData;

namespace ModProfileManager_Addon.UnityUI.Components
{
    public class FileHierarchyEntry:KMonoBehaviour
    {
        public ModPresetEntry ModProfile;


        public System.Action<bool> OnDialogueToggled;
        public System.Action RefreshUI;
        FButton deleteButton, renameButton
            //, editButton
            ;
        FToggleButton editButton; 
        LocText Label;
        public System.Action<ModPresetEntry> OnApplyPreset;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Label = transform.Find("Label").gameObject.GetComponent<LocText>();
            editButton = gameObject.AddComponent<FToggleButton>();
            renameButton  = transform.Find("RenameButton").gameObject.AddComponent<FButton>();
            deleteButton = transform.Find("DeleteButton").gameObject.AddComponent<FButton>();
            //editButton = transform.Find("EditButton").gameObject.AddComponent<FButton>();

            UIUtils.AddSimpleTooltipToObject(editButton.transform, PRESETHIERARCHYENTRY.TOOLTIP_EDIT);
            UIUtils.AddSimpleTooltipToObject(renameButton.transform, PRESETHIERARCHYENTRY.TOOLTIP_RENAME);
            UIUtils.AddSimpleTooltipToObject(deleteButton.transform, PRESETHIERARCHYENTRY.TOOLTIP_DELETE);

            if (ModProfile != null)
            {
                if(ModProfile.ModList.SavePoints.Count == 1)
                    Label.SetText(ModProfile.ModList.ModlistPath);
                else
                    Label.SetText(ModProfile.ModList.ModlistPath + ": " + ModProfile.Path);

                deleteButton.OnClick += ConfirmDelete;
                renameButton.OnClick += OpenRenameDialogue;
                editButton.OnClick += EditPreset;
                //button.OnClick += ApplyPreset;
                UpdateSelected();
            }
        }
        public void UpdateSelected()
        {
            editButton.ChangeSelection(ModAssets.SelectedModPack == ModProfile);
        }
        //private void ApplyPreset()
        //{
        //    if (ModProfile!=null && ModProfile.ModList.TryGetModListEntry(ModProfile.Path, out var modsState))
        //    {
        //        ModAssets.SyncMods(modsState);

        //        if(ModProfile.ModList.TryGetPlibOptionsEntry(ModProfile.Path, out var plibOptions))
        //        {
        //            SaveGameModList.WritePlibOptions(plibOptions);
        //        }
        //    }
        //    OnApplyPreset(ModProfile);
        //    //ModAssets.SelectedBlueprint = blueprint;
        //}

        void EditPreset()
        {
            ModAssets.SelectedModPack = ModProfile;
            if (RefreshUI != null)
                RefreshUI();
        }
        void SetDialogueState(bool state)
        {
            if(OnDialogueToggled!=null)
                OnDialogueToggled(state);
        }
        string FriendlyName => ModProfile.ModList.SavePoints.Count == 1 ? ModProfile.ModList.ModlistPath : ModProfile.ModList.ModlistPath + ModProfile.Path;
        void OpenRenameDialogue()
        {
            SetDialogueState(true);
            var RenameAction = (string result) =>
            {
                SetDialogueState(false);
                ModAssets.HandleRenaming(ModProfile, result);
                Label.SetText(result);
                if (RefreshUI != null)
                    RefreshUI();
            };
            DialogUtil.CreateTextInputDialog(RENAME_POPUP.TITLE, FriendlyName, false, RenameAction, () => SetDialogueState(false), FrontEndManager.Instance.gameObject, false, false, true);
        }
        void ConfirmDelete()
        {
            SetDialogueState(true);
            var OnDeleteAction = () =>
            {
                SetDialogueState(false);
                DeletePreset();
            };
            DialogUtil.CreateConfirmDialog(DELETE_POPUP.TITLE, string.Format(DELETE_POPUP.TEXT, FriendlyName),on_confirm: OnDeleteAction, on_cancel: () => SetDialogueState(false),frontend:true);
        }

        void DeletePreset()
        {
            ModAssets.HandleDeletion(ModProfile);
            if (RefreshUI != null)
                RefreshUI();
        }
    }
}
