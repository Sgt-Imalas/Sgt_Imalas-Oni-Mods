using Rockets_TinyYetBig.Content.ModDb.RocketBlueprintData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;

namespace Rockets_TinyYetBig.Content.Scripts.UI.UIComponents
{
	internal class RocketBlueprintEntry : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public RocketBlueprint blueprint;

		public System.Action<bool> OnDialogueToggled;
		public System.Action OnEntryClicked;
		public System.Action<string> OnRenamed, OnMoved;
		FButton deleteButton, exportButton
			;
		FToggleButton button;
		LocText Label;
		public System.Action<RocketBlueprint> OnSelectBlueprint, OnDeleted
			;
		public ToolTip Description;

		List<GameObject> HoverShowButtons = [];

		bool spawned = false, init = false;
		public void SetSelected(bool enabled)
		{
			if (button != null)
				button.SetIsSelected(enabled);
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
			OnPointerExit(null);
		}

		void Init()
		{
			if (init)
				return;
			init = true;
			Description = UIUtils.AddSimpleTooltipToObject(this.gameObject, string.Empty, false,onBottom:false);

			Label = transform.Find("Label").gameObject.GetComponent<LocText>();
			button = gameObject.AddComponent<FToggleButton>();
			//renameButton = transform.Find("RenameButton").gameObject.AddComponent<FButton>();
			deleteButton = transform.Find("DeleteButton").gameObject.AddComponent<FButton>();
			//moveButton = transform.Find("MoveFolderButton").gameObject.AddComponent<FButton>();
			//exportButton = transform.Find("ExportButton").gameObject.AddComponent<FButton>();
			//retakeButton = transform.Find("RetakeButton").gameObject.AddComponent<FButton>();
			//infoButton = transform.Find("InfoButton").gameObject.AddComponent<FButton>();

			HoverShowButtons = [
				deleteButton.gameObject, 
				//renameButton?.gameObject, 
				//exportButton?.gameObject, 
				//moveButton?.gameObject, 
				//exportButton?.gameObject, 
				//retakeButton?.gameObject
				//, infoButton?.gameObject
				];

			//UIUtils.AddSimpleTooltipToObject(moveButton.transform, BLUEPRINTENTRY.TOOLTIP_MOVE);
			//UIUtils.AddSimpleTooltipToObject(renameButton.transform, BLUEPRINTENTRY.TOOLTIP_RENAME);
			//UIUtils.AddSimpleTooltipToObject(deleteButton.transform, BLUEPRINTENTRY.TOOLTIP_DELETE);
			//UIUtils.AddSimpleTooltipToObject(exportButton.transform, BLUEPRINTENTRY.TOOLTIP_EXPORT);
			//UIUtils.AddSimpleTooltipToObject(retakeButton.transform, BLUEPRINTENTRY.TOOLTIP_RETAKE);
			////UIUtils.AddSimpleTooltipToObject(infoButton.transform, BLUEPRINTENTRY.TOOLTIP_INFO);
		}

		public void SetBlueprint(RocketBlueprint bp)
		{
			Init();
			blueprint = bp;			
			Label.SetText(blueprint.FriendlyName);

		}
		public void SetInteractable(bool interactable)
		{
			button?.SetInteractable(interactable);
			deleteButton?.SetInteractable(interactable);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			Init();
			spawned = true;
			if (blueprint != null)
			{
				Label.SetText(blueprint.FriendlyName);
				deleteButton.OnClick += ConfirmDelete;
				//renameButton.OnClick += OpenRenameDialogue;
				//moveButton.OnClick += OpenFolderChangeDialogue;
				button.OnClick += SelectBlueprint;
				//exportButton.OnClick += ExportBlueprintToClipboard;
				//retakeButton.OnClick += RetakeBlueprint;
				//infoButton.OnClick += ShowBlueprintInfoScreen;


				Description?.SetSimpleTooltip(blueprint.GetDescription());
			}
		}

		//private void ExportBlueprintToClipboard()
		//{
		//	if (blueprint != null)
		//	{
		//		SetDialogueState(true);
		//		ModAssets.ExportToClipboard(blueprint);
		//		DialogUtil.CreateConfirmDialog(BASE64_EXPORTED.TITLE, BASE64_EXPORTED.TEXT, on_confirm: () => SetDialogueState(false));
		//	}
		//}
		//void ShowBlueprintInfoScreen()
		//{
		//	if (blueprint == null || OnInfoClicked == null)
		//		return;
		//	OnInfoClicked(blueprint);
		//}

		private void SelectBlueprint()
		{
			if (OnSelectBlueprint != null)
				OnSelectBlueprint(blueprint);
			//ModAssets.SelectedBlueprint = blueprint;
		}

		void SetDialogueState(bool state)
		{
			if (OnDialogueToggled != null)
				OnDialogueToggled(state);
		}

		void ConfirmDelete()
		{
			SetDialogueState(true);
			var deleteAction = () =>
			{
				SetDialogueState(false);
				DeleteBlueprint();
			};
			DialogUtil.CreateConfirmDialog(
				global::STRINGS.UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.HEADER.Replace("{OutfitName}", blueprint.FriendlyName),
				global::STRINGS.UI.FRONTEND.LOADSCREEN.CONFIRMDELETE.Replace("{0}", blueprint.FriendlyName),
				  on_confirm: deleteAction,
				  on_cancel: () => SetDialogueState(false), lockCamera: true);
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

		internal void RefreshTooltip(string TT)
		{

			Description?.SetSimpleTooltip(TT);
		}
	}
}
