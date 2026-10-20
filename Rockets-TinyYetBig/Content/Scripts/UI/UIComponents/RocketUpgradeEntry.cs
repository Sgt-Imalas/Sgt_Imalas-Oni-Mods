using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.ModDb.RocketBlueprintData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static SaveUpgradeWarning;

namespace Rockets_TinyYetBig.Content.Scripts.UI.UIComponents
{
	internal class RocketUpgradeEntry : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{

		FButton deleteButton;
		LocText Label;
		private ToolTip Description, DeconstructTooltip;
		private RocketModuleUpgrade _upgrade;
		Image deconstructImage;

		List<GameObject> HoverShowButtons = [];

		bool spawned = false, init = false;
		Sprite ic_deconstruct, ic_cancelDeconstruct;

		public System.Func<bool> DeconstructPending = null;
		public System.Action ToggleDeconstruct = null;

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
			Description = UIUtils.AddSimpleTooltipToObject(this.gameObject, string.Empty, false, onBottom: false);

			Label = transform.Find("PartLabel").gameObject.GetComponent<LocText>();
			deleteButton = transform.Find("DeleteButton").gameObject.AddComponent<FButton>();
			deleteButton.OnClick += OnDeleteButtonPressed;

			deconstructImage = deleteButton.transform.Find("Image").gameObject.GetComponent<Image>();
			DeconstructTooltip = UIUtils.AddSimpleTooltipToObject(deleteButton.gameObject, global::STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.LABEL);
		
			ic_deconstruct = Assets.GetSprite("action_deconstruct");			
			ic_cancelDeconstruct = Assets.GetSprite("action_cancel");

			HoverShowButtons = [deleteButton.gameObject];
		}
		void OnDeleteButtonPressed()
		{
			if (ToggleDeconstruct != null)
				ToggleDeconstruct();
			SetDeconstructButtonState(false);
		}

		public void SetUpgrade(RocketModuleUpgrade upgrade)
		{
			Init();
			_upgrade = upgrade;
			Label.SetText(upgrade.GetName());

			var def = Assets.GetBuildingDef(upgrade.ID);
			if (def != null)
				transform.Find("Image").GetComponent<Image>().sprite = def.GetUISprite();
		}

		void SetDeconstructButtonState(bool pending)
		{
			if (DeconstructPending != null)
				pending = DeconstructPending();

			DeconstructTooltip.SetSimpleTooltip( pending ? global::STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.LABEL_CANCEL : global::STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.LABEL);
			deconstructImage.sprite = pending ? ic_cancelDeconstruct : ic_deconstruct;
		}

		public void RefreshDeconstructState(bool pending) => SetDeconstructButtonState(pending);

		public override void OnSpawn()
		{
			base.OnSpawn();
			Init();
			spawned = true;
			if (_upgrade != null)
			{
				Label.SetText(_upgrade.GetName());
				//deleteButton.OnClick += ConfirmDelete;
				Description?.SetSimpleTooltip(_upgrade.GetDescription());
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

	
		//void ConfirmDelete()
		//{
		//	SetDialogueState(true);
		//	var deleteAction = () =>
		//	{
		//		SetDialogueState(false);
		//		DeleteBlueprint();
		//	};
		//	DialogUtil.CreateConfirmDialog(
		//		global::STRINGS.UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.HEADER.Replace("{OutfitName}", blueprint.FriendlyName),
		//		global::STRINGS.UI.FRONTEND.LOADSCREEN.CONFIRMDELETE.Replace("{0}", blueprint.FriendlyName),
		//		  on_confirm: deleteAction,
		//		  on_cancel: () => SetDialogueState(false), lockCamera: true);
		//}

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
