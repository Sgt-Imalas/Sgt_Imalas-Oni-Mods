using BlueprintsV2.BlueprintData;
using BlueprintsV2.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.FILEHIERARCHY.SCROLLAREA.CONTENT;
using static BlueprintsV2.STRINGS.UI.DIALOGUE;

namespace BlueprintsV2.UnityUI.Components
{
	public class FileHierarchyEntry : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public Blueprint blueprint;

		public System.Action<bool> OnDialogueToggled;
		public System.Action OnEntryClicked;
		public System.Action<string> OnRenamed, OnMoved;
		FButton deleteButton, renameButton, moveButton, exportButton, retakeButton, infoButton;
		FToggleButton button;
		LocText Label;
		public System.Action<Blueprint> OnSelectBlueprint, OnDeleted, OnInfoClicked;
		public ToolTip Description;

		List<GameObject> HoverShowButtons = [];

		public void SetSelected(bool enabled)
		{
			if (button != null)
				button.SetIsSelected(enabled);
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Description = UIUtils.AddSimpleTooltipToObject(this.gameObject, string.Empty, true, 250);

			Label = transform.Find("Label").gameObject.GetComponent<LocText>();
			button = gameObject.AddComponent<FToggleButton>();
			renameButton = transform.Find("RenameButton").gameObject.AddComponent<FButton>();
			deleteButton = transform.Find("DeleteButton").gameObject.AddComponent<FButton>();
			moveButton = transform.Find("MoveFolderButton").gameObject.AddComponent<FButton>();
			exportButton = transform.Find("ExportButton").gameObject.AddComponent<FButton>();
			retakeButton = transform.Find("RetakeButton").gameObject.AddComponent<FButton>();
			infoButton = transform.Find("InfoButton").gameObject.AddComponent<FButton>();

			HoverShowButtons = [deleteButton.gameObject, renameButton?.gameObject, exportButton?.gameObject, moveButton?.gameObject, exportButton?.gameObject, retakeButton?.gameObject, infoButton?.gameObject];

			UIUtils.AddSimpleTooltipToObject(moveButton.transform, BLUEPRINTENTRY.TOOLTIP_MOVE);
			UIUtils.AddSimpleTooltipToObject(renameButton.transform, BLUEPRINTENTRY.TOOLTIP_RENAME);
			UIUtils.AddSimpleTooltipToObject(deleteButton.transform, BLUEPRINTENTRY.TOOLTIP_DELETE);
			UIUtils.AddSimpleTooltipToObject(exportButton.transform, BLUEPRINTENTRY.TOOLTIP_EXPORT);
			UIUtils.AddSimpleTooltipToObject(retakeButton.transform, BLUEPRINTENTRY.TOOLTIP_RETAKE);
			UIUtils.AddSimpleTooltipToObject(infoButton.transform, BLUEPRINTENTRY.TOOLTIP_INFO);

			if (blueprint != null)
			{
				Label.SetText(blueprint.FriendlyName);

				deleteButton.OnClick += ConfirmDelete;
				renameButton.OnClick += OpenRenameDialogue;
				moveButton.OnClick += OpenFolderChangeDialogue;
				button.OnClick += SelectBlueprint;
				exportButton.OnClick += ExportBlueprintToClipboard;
				retakeButton.OnClick += RetakeBlueprint;
				infoButton.OnClick += ShowBlueprintInfoScreen;
			}
			OnPointerExit(null);
		}

		private void ExportBlueprintToClipboard()
		{
			if (blueprint != null)
			{
				SetDialogueState(true);
				ModAssets.ExportToClipboard(blueprint);
				DialogUtil.CreateConfirmDialog(BASE64_EXPORTED.TITLE, BASE64_EXPORTED.TEXT, on_confirm: () => SetDialogueState(false));
			}
		}
		void ShowBlueprintInfoScreen()
		{
			if (blueprint == null|| OnInfoClicked == null)
				return;
			OnInfoClicked(blueprint);
		}

		private void RetakeBlueprint()
		{
			if (blueprint == null)
				return;
			SgtLogger.l("ReTake BP");
			BlueprintSelectionScreen.Instance.Show(false);
			UseBlueprintTool.Instance.DeactivateTool();
			CreateBlueprintTool.ReTakeBlueprint(blueprint);
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
		public void OnPointerExit(PointerEventData eventData)
		{
			foreach (var buttonGO in HoverShowButtons)
			{
				buttonGO.SetActive(false);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			foreach (var buttonGO in HoverShowButtons)
			{
				buttonGO.SetActive(true);
			}
		}

		internal void RefreshTooltip()
		{
			if (blueprint == null)
				return;

			Description?.SetSimpleTooltip(blueprint.UserDescription);
		}
	}
}
