using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using BlueprintsV2.Tools;
using BlueprintsV2.UnityUI.Components;
using rail;
using STRINGS;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.MATERIALSWITCH;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.MATERIALSWITCH.BUTTONS;
using static BlueprintsV2.STRINGS.UI.DIALOGUE;
using static STRINGS.UI;

namespace BlueprintsV2.UnityUI
{
	internal class BlueprintRenamingScreen : FScreen
	{
		public static BlueprintRenamingScreen Instance = null;


		public LocText TitleText;
		public FButton CloseBtn;

		public FInputField2 NameInput;
		public FButton ClearNameInput;
		public FButton PasteClipboardToInput;

		public FButton ConfirmBtn, CancelBtn;

		System.Action<string> _onConfirm;
		System.Action _onCancel;

		private bool init, spawned, _allowEmpty;
		private string _cachedTitle;
		public static void DestroyInstance() { Instance = null; }

		public override float GetSortKey()
		{
			return 400;
		}
		public override void OnSpawn()
		{
			spawned = true;
			base.OnSpawn();
			if (_cachedTitle.Any())
			{
				TitleText.SetText(_cachedTitle);
				_cachedTitle = string.Empty;
			}
		}

		private void Init()
		{
			if (init) { return; }
			SgtLogger.l("Initializing BlueprintWindow");

			CloseBtn = transform.Find("Header/Close").gameObject.AddOrGet<FButton>();
			CloseBtn.OnClick += OnCloseClicked;


			ClearNameInput = transform.Find("Body/SearchBar/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearNameInput.OnClick += () => NameInput.Text = string.Empty;

			PasteClipboardToInput = transform.Find("Body/SearchBar/PasteButton").gameObject.AddOrGet<FButton>();
			PasteClipboardToInput.OnClick += GetNameFromClipboard;

			ConfirmBtn = transform.Find("Body/Buttons/Confirm").gameObject.AddOrGet<FButton>();
			ConfirmBtn.OnClick += OnConfirmClicked;
			CancelBtn = transform.Find("Body/Buttons/Cancel").gameObject.AddOrGet<FButton>();
			CancelBtn.OnClick += OnCancelClicked;

			TitleText = transform.Find("Header/Label").gameObject.GetComponent<LocText>();

			NameInput = transform.Find("Body/SearchBar/Input").gameObject.AddOrGet<FInputField2>();
			NameInput.OnValueChanged.AddListener(OnNameInputChanged);
			NameInput.Text = string.Empty;

			transform.Find("Body/SearchBar/Input/TextArea/Placeholder").gameObject.GetComponent<LocText>().SetText(FILE_NAME_DIALOG.ENTER_TEXT);

			ConfirmBtn.transform.Find("Text").gameObject.GetComponent<LocText>().SetText(CONFIRMDIALOG.OK);
			CancelBtn.transform.Find("Text").gameObject.GetComponent<LocText>().SetText(CONFIRMDIALOG.CANCEL);

			init = true;
			InitialHookupRefresh();
		}

		void GetNameFromClipboard()
		{
			if (IO_Utils.TryGetStringFromClipboard(out string clipboardText))
			{
				NameInput.Text = clipboardText;
			}
		}
		void OnCloseClicked() => OnCancelClicked();

		public void OnNameInputChanged(string filterstring = "")
		{
			ConfirmBtn.SetInteractable(_allowEmpty ? true : filterstring.Any());
		}

		void OnConfirmClicked()
		{
			if (!NameInput.Text.Any() && !_allowEmpty)
				return;

			if (_onConfirm != null)
			{
				_onConfirm.Invoke(NameInput.Text);
			}
			Deactivate();
		}
		void OnCancelClicked()
		{
			if (_onCancel != null)
			{
				_onCancel.Invoke();
			}
			Deactivate();
		}


		public static void OpenNamingDialogue(string title, System.Action<string> onConfirm, System.Action onCancel, string startString = "",bool allowEmpty = false)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.RenamingScreenGO, ModAssets.ParentScreen);
				Instance = screen.AddOrGet<BlueprintRenamingScreen>();
				Instance.gameObject.SetActive(true);
				Instance.Init();
			}
			Instance.mouseOver = true;
			Instance.Refresh(title, onConfirm, onCancel, startString, allowEmpty);
			Instance.transform.SetAsLastSibling();
			//Instance.NameInput.ExternalStartEditing();

			//KScreenManager.Instance.RefreshStack();
		}

		private void Refresh(string title, System.Action<string> onConfirm, System.Action onCancel, string startString = "", bool allowEmpty = false)
		{
			_onConfirm = onConfirm;
			_onCancel = onCancel;
			if(spawned)
				TitleText.SetText(title);
			else
				_cachedTitle = title;

			NameInput.Text = startString;
			ConfirmBtn.SetInteractable(allowEmpty ? true : startString.Any());
			_allowEmpty = allowEmpty;
			this.Activate();
		}


		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				//SgtLogger.l("BlueprintRenamingScreen consume esc.");
				OnCancelClicked();
			}
			else if (e.TryConsume(Action.DialogSubmit) && NameInput.Text.Any())
				this.OnConfirmClicked();
			e.Consumed = true;
			//base.OnKeyDown(e);
		}
		public override void OnKeyUp(KButtonEvent e)
		{
			e.Consumed = true;
		}
		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!init)
			{
				Init();
			}
			this.isActive = show;

			CameraController.Instance.DisableUserCameraControl = show;
			Instance.ConsumeMouseScroll = show;
			Instance.isEditing = show;
		}

	}
}

