using BlueprintsV2.BlueprintData;
using UtilLibs;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.FILEHIERARCHY.SCROLLAREA.CONTENT;
using static BlueprintsV2.STRINGS.UI.DIALOGUE;

namespace BlueprintsV2.UnityUI.Components
{
	public class FileHierarchyEntry : KMonoBehaviour
	{
		public Blueprint blueprint;

		public System.Action<bool> OnDialogueToggled;
		public System.Action OnEntryClicked;
		public System.Action<string> OnRenamed, OnMoved;
		FButton button, deleteButton, renameButton, moveButton;
		LocText Label;
		public System.Action<Blueprint> OnSelectBlueprint, OnDeleted;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Label = transform.Find("Label").gameObject.GetComponent<LocText>();
			button = gameObject.AddComponent<FButton>();
			renameButton = transform.Find("RenameButton").gameObject.AddComponent<FButton>();
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
				button.OnClick += SelectBlueprint;
			}
		}

		private void SelectBlueprint()
		{
			if (OnSelectBlueprint != null)
				OnSelectBlueprint(blueprint);
			//ModAssets.SelectedBlueprint = blueprint;
		}

		void OpenFolderChangeDialogue()
		{
			SetDialogueState(true);
			var ChangeFolderAction = (string result) =>
			{
				SetDialogueState(false);
				if (result == blueprint.Folder)
					return;

				blueprint.SetFolder(result);
				if (OnMoved != null)
					OnMoved(result);
			};
			DialogUtil.CreateTextInputDialog(MOVETOFOLDER_TITLE, blueprint.Folder, null, true, ChangeFolderAction, () => SetDialogueState(false), ModAssets.ParentScreen, true, false);
		}
		void SetDialogueState(bool state)
		{
			if (OnDialogueToggled != null)
				OnDialogueToggled(state);
		}

		void OpenRenameDialogue()
		{
			SetDialogueState(true);
			var RenameAction = (string result) =>
			{
				SetDialogueState(false);
				if (result == blueprint.FriendlyName)
					return;

				blueprint.Rename(result);
				Label.SetText(blueprint.FriendlyName);
				if (OnRenamed != null)
					OnRenamed(result);
			};
			DialogUtil.CreateTextInputDialog(RENAMEBLUEPRINT_TITLE, blueprint.FriendlyName, null, false, RenameAction, () => SetDialogueState(false), ModAssets.ParentScreen, true, false);
		}
		void ConfirmDelete()
		{
			SetDialogueState(true);
			var OnDeleteAction = () =>
			{
				SetDialogueState(false);
				DeleteBlueprint();
			};
			DialogUtil.CreateConfirmDialog(CONFIRMDELETE.TITLE, string.Format(CONFIRMDELETE.TEXT, blueprint?.FriendlyName), on_confirm: OnDeleteAction, on_cancel: () => SetDialogueState(false));
		}

		void DeleteBlueprint()
		{
			if (OnDeleted != null)
				OnDeleted(blueprint);
		}

	}
}
