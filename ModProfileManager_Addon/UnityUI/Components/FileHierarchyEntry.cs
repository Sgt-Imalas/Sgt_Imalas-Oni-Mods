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
using UnityEngine.EventSystems;
using static ModProfileManager_Addon.STRINGS.UI.PRESETOVERVIEW.FILEHIERARCHY;

namespace ModProfileManager_Addon.UnityUI.Components
{
    public class FileHierarchyEntry:KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ModPresetEntry ModProfile;


        public System.Action<bool> OnDialogueToggled;
        public System.Action RefreshUI;
        FButton deleteButton, renameButton, exportButton
            ;
        FToggleButton selectButton; 
        LocText Label;
        public System.Action<ModPresetEntry> OnApplyPreset;
        ToolTip selectButtonTT;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Label = transform.Find("Label").gameObject.GetComponent<LocText>();
            selectButton = gameObject.AddComponent<FToggleButton>();
            renameButton  = transform.Find("RenameButton").gameObject.AddComponent<FButton>();
            deleteButton = transform.Find("DeleteButton").gameObject.AddComponent<FButton>();
            exportButton = transform.Find("ExportButton").gameObject.AddComponent<FButton>();

            selectButtonTT =
            UIUtils.AddSimpleTooltipToObject(selectButton.transform, PRESETHIERARCHYENTRY.TOOLTIP_SELECT);
            UIUtils.AddSimpleTooltipToObject(renameButton.transform, PRESETHIERARCHYENTRY.TOOLTIP_RENAME);
            UIUtils.AddSimpleTooltipToObject(deleteButton.transform, PRESETHIERARCHYENTRY.TOOLTIP_DELETE);
            UIUtils.AddSimpleTooltipToObject(exportButton.transform, PRESETHIERARCHYENTRY.TOOLTIP_EXPORT);

            if (ModProfile != null)
            {
                if(ModProfile.ModList.SavePoints.Count == 1)
                    Label.SetText(ModProfile.ModList.ModlistPath);
                else
                    Label.SetText(ModProfile.ModList.ModlistPath + ": " + ModProfile.Path);
                bool isCloneEntry = ModProfile.Clone;
                deleteButton.OnClick += ConfirmDelete;
                deleteButton.SetInteractable(!isCloneEntry);

                renameButton.OnClick += OpenRenameDialogue;
                renameButton.SetInteractable(!isCloneEntry);

                selectButton.OnClick += EditPreset;
                //button.OnClick += ApplyPreset;
                UpdateSelected();

                exportButton.SetInteractable(!isCloneEntry);
                exportButton.OnClick += ExportToString;

                deleteButton.gameObject.SetActive(false);
                renameButton.gameObject.SetActive(false);
                exportButton.gameObject.SetActive(false);

                if (isCloneEntry)
                    selectButtonTT.SetSimpleTooltip(PRESETHIERARCHYENTRY.TOOLTIP_CLONE);
            }
        }

        private void ExportToString()
        {
            if(ModProfile!=null && ModProfile.ModList != null)
            {
                ModProfile.ExportToClipboard();
            }
        }

        public void UpdateSelected()
        {
            bool selected = (ModAssets.SelectedModPack.Path == ModProfile.Path && ModAssets.SelectedModPack.ModList.ModlistPath == ModProfile.ModList.ModlistPath);

            selectButton.ChangeSelection(selected);
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
                result = SanitationUtils.SanitizeName(result);
                SetDialogueState(false);
                ModAssets.HandleRenaming(ModProfile, result);
                Label.SetText(result);
                if (RefreshUI != null)
                    RefreshUI();
            };
            DialogUtil.CreateTextInputDialog(RENAME_POPUP.TITLE, FriendlyName,null, false, RenameAction, () => SetDialogueState(false), FrontEndManager.Instance.gameObject, false, false, true);
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

        public void OnPointerExit(PointerEventData eventData)
        {
            deleteButton.gameObject.SetActive(false);
            renameButton.gameObject.SetActive(false);
            exportButton.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            deleteButton.gameObject.SetActive(true);
            exportButton.gameObject.SetActive(true);
            renameButton.gameObject.SetActive(true);
        }
    }
}
