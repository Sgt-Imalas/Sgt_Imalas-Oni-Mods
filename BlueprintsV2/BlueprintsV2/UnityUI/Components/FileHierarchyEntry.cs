using BlueprintsV2.BlueprintsV2.BlueprintData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.UIcmp;
using UtilLibs;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.FILEHIERARCHY.SCROLLAREA.CONTENT;
using static BlueprintsV2.STRINGS.UI.DIALOGE;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
    public class FileHierarchyEntry:KMonoBehaviour
    {
        public Blueprint blueprint;

        public System.Action OnEntryClicked, OnDeleteClicked;
        public System.Action<string> OnRenameClicked, OnMoveClicked;
        FButton button, deleteButton, renameButton, moveButton;
        LocText Label;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Label = transform.Find("Label").gameObject.GetComponent<LocText>();
            button = gameObject.AddComponent<FButton>();
            renameButton  = transform.Find("RenameButton").gameObject.AddComponent<FButton>();
            deleteButton = transform.Find("DeleteButton").gameObject.AddComponent<FButton>();
            moveButton = transform.Find("MoveFolderButton").gameObject.AddComponent<FButton>();

            UIUtils.AddSimpleTooltipToObject(moveButton.transform, BLUEPRINTENTRY.TOOLTIP_MOVE);
            UIUtils.AddSimpleTooltipToObject(renameButton.transform, BLUEPRINTENTRY.TOOLTIP_RENAME);
            UIUtils.AddSimpleTooltipToObject(deleteButton.transform, BLUEPRINTENTRY.TOOLTIP_DELETE);

            if (blueprint != null)
            {
                Label.SetText(blueprint.FriendlyName);

                deleteButton.OnClick += ConfirmDelete;
                renameButton.OnClick += OpenRenameDialogue;
                moveButton.OnClick += OpenFolderChangeDialogue;

            }
        }
        void OpenFolderChangeDialogue()
        {
            var OnDeleteAction = () =>
            {
                DeleteBlueprint();
                if (OnDeleteClicked != null)
                    OnDeleteClicked();
            };
            DialogUtil.CreateConfirmDialog(CONFIRMDELETE.TITLE, string.Format(CONFIRMDELETE.TEXT, blueprint?.FriendlyName), on_confirm: OnDeleteAction);
        }
        void OpenRenameDialogue()
        {
            var OnDeleteAction = () =>
            {
                DeleteBlueprint();
                if (OnDeleteClicked != null)
                    OnDeleteClicked();
            };
            DialogUtil.CreateConfirmDialog(CONFIRMDELETE.TITLE, string.Format(CONFIRMDELETE.TEXT, blueprint?.FriendlyName), on_confirm: OnDeleteAction);
        }
        void ConfirmDelete()
        {
            var OnDeleteAction = () =>
            {
                DeleteBlueprint();
                if (OnDeleteClicked != null)
                    OnDeleteClicked();
            };
            DialogUtil.CreateConfirmDialog(CONFIRMDELETE.TITLE, string.Format(CONFIRMDELETE.TEXT, blueprint?.FriendlyName),on_confirm: OnDeleteAction);
        }

        void DeleteBlueprint()
        {
            if(blueprint != null)
            {
                ModAssets.BlueprintFileHandling.DeleteBlueprint(blueprint);
            }
            UnityEngine.Object.Destroy(this.gameObject);
        }

    }
}
