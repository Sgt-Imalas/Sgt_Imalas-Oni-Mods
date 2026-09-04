using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static STRINGS.UI;

namespace BlueprintsV2.UnityUI
{
	internal class BlueprintRenamingScreen : FScreen
	{
		class BlueprintNameOption : FButton
		{
			public LocText Text;
			public string Name;
			public System.Action<string> OnClicked;

			bool spawned = false;

			public void Init(string name, System.Action<string> onClicked)
			{
				Name = name;
				OnClicked = onClicked;
				if (spawned)
					Text?.SetText(name);
				Name = name;
			}

			public override void OnSpawn()
			{
				base.OnSpawn();
				Text = transform.Find("Label").gameObject.GetComponent<LocText>();
				if (!Name.IsNullOrWhiteSpace())
					Text.SetText(Name);
				OnClick += OnClickedInternal;
				spawned = true;
			}
			void OnClickedInternal()
			{
				OnClicked?.Invoke(Name);
			}
		}


		public static BlueprintRenamingScreen Instance = null;


		public LocText TitleText;
		public FButton CloseBtn;

		public FInputField2 NameInput;
		public FButton ClearNameInput;
		public FButton PasteClipboardToInput;

		public FButton ConfirmBtn, CancelBtn;

		System.Action<string> _onConfirm;
		System.Action _onCancel;

		private Image _dropDownIcon;
		private FButton _dropDownBtn;
		private GameObject _dropDownGO;
		private GameObject _dropDownContainer;
		private BlueprintNameOption _dropDownEntryPrefab;
		private Dictionary<string, BlueprintNameOption> _dropDownEntries = [];


		private bool init, spawned, _allowEmpty;
		private string _cachedTitle;
		private Sprite _dropdownOpen, _dropdownClose;
		private HashSet<string> _currentSelectableNames = [];
		public static void DestroyInstance() { Instance = null; }
		public bool DropdownMode => _currentSelectableNames.Any();

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
			SgtLogger.l("Initializing BlueprintNamingScreen");

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
			NameInput.OnSelect.AddListener(OnStartedTyping);
			NameInput.Text = string.Empty;

			transform.Find("Body/SearchBar/Input/TextArea/Placeholder").gameObject.GetComponent<LocText>().SetText(FILE_NAME_DIALOG.ENTER_TEXT);

			ConfirmBtn.transform.Find("Text").gameObject.GetComponent<LocText>().SetText(CONFIRMDIALOG.OK);
			CancelBtn.transform.Find("Text").gameObject.GetComponent<LocText>().SetText(CONFIRMDIALOG.CANCEL);

			//dropdown:
			_dropDownIcon = transform.Find("Body/SearchBar/OpenDropdown/Image").gameObject.GetComponent<Image>();
			_dropDownBtn = transform.Find("Body/SearchBar/OpenDropdown").gameObject.AddOrGet<FButton>();
			_dropDownBtn.OnClick += ToggleDropDown;

			_dropDownGO = transform.Find("Body/DropDownArea").gameObject;
			_dropDownContainer = transform.Find("Body/DropDownArea/Content").gameObject;
			_dropDownEntryPrefab = transform.Find("Body/DropDownArea/Content/EntryPrefab").gameObject.AddOrGet<BlueprintNameOption>();
			_dropDownEntryPrefab.gameObject.SetActive(false);

			_dropdownClose = Assets.GetSprite("icon_TrendArrows_Up_1");
			_dropdownOpen = Assets.GetSprite("icon_TrendArrows_Down_1");


			init = true;
			InitialHookupRefresh();
		}
		void ToggleDropDown()
		{
			bool setActive = !_dropDownGO.activeSelf;
			_dropDownGO.SetActive(setActive);
			RefreshDropdownIcon();
			if (setActive)
				RefreshOptions();

		}
		void RefreshOptions()
		{
			if (DropdownMode)
				FilterSelectableOptions(NameInput.Text.Trim());
		}

		void GetNameFromClipboard()
		{
			if (IO_Utils.TryGetStringFromClipboard(out string clipboardText))
			{
				NameInput.Text = clipboardText.Trim();
			}
		}
		void OnCloseClicked() => OnCancelClicked();

		public void OnNameInputChanged(string filterstring = "")
		{
			filterstring = filterstring.Trim();
			ConfirmBtn.SetInteractable(_allowEmpty || filterstring.Any());
			if(DropdownMode)
				FilterSelectableOptions(filterstring);
		}
		void OnStartedTyping(string _)
		{
			if (DropdownMode)
			{
				_dropDownGO.SetActive(true);
				RefreshOptions();
				RefreshDropdownIcon();
			}
		}
		void OnConfirmClicked()
		{
			string text = NameInput.Text.Trim();
			if (!text.Any() && !_allowEmpty)
				return;

			if (_onConfirm != null)
			{
				_onConfirm.Invoke(text);
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
		void RefreshDropDownEntries(string[] selectableOptions)
		{
			if (selectableOptions == null || selectableOptions.Length == 0)
			{
				_dropDownGO.SetActive(false);
				_dropDownBtn.gameObject.SetActive(false);
				_currentSelectableNames.Clear();
				return;
			}
			_dropDownGO.SetActive(true);
			foreach (var entry in _dropDownEntries.Values)
			{
				entry.gameObject.SetActive(false);
			}
			foreach (var name in selectableOptions.StableSort())
			{
				var entry = AddOrGetDropDownEntry(name);
				entry.transform.SetAsLastSibling();
				entry.gameObject.SetActive(true);
			}
			_currentSelectableNames = selectableOptions.ToHashSet();
			SetDropdownSize(_currentSelectableNames.Count);
			_dropDownGO.SetActive(false);
			RefreshDropdownIcon();

		}

		void FilterSelectableOptions(string filterString = "")
		{
			foreach (var entry in _dropDownEntries.Values)
			{
				entry.gameObject.SetActive(false);
			}
			int count = 0;
			foreach (var name in _currentSelectableNames)
			{
				bool matchesFilter = string.IsNullOrWhiteSpace(filterString) || name.ToLowerInvariant().Contains(filterString.ToLowerInvariant());

				var entry = AddOrGetDropDownEntry(name);
				entry.gameObject.SetActive(matchesFilter);
				if(matchesFilter)
					count++;
			}
			SetDropdownSize(count);
		}
		void SetDropdownSize(int count)
		{
			float height = Mathf.Clamp(count * (25+2) + 4, 10, 150);
			_dropDownGO.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		BlueprintNameOption AddOrGetDropDownEntry(string name)
		{
			if (_dropDownEntries.TryGetValue(name, out BlueprintNameOption entry))
				return entry;
			var newEntry = Util.KInstantiateUI<BlueprintNameOption>(_dropDownEntryPrefab.gameObject, _dropDownContainer);
			newEntry.gameObject.SetActive(true);
			newEntry.Init(name, OnDropDownEntryClicked);
			_dropDownEntries[name] = newEntry;
			return newEntry;
		}
		void OnDropDownEntryClicked(string name)
		{
			NameInput.Text = name;
			_dropDownGO.SetActive(false);
			RefreshDropdownIcon();
		}
		void RefreshDropdownIcon()
		{
			_dropDownIcon.sprite = _dropDownGO.activeSelf ? _dropdownClose : _dropdownOpen;
		}

		private void Refresh(string title, System.Action<string> onConfirm, System.Action onCancel, string startString = "", bool allowEmpty = false, string[] selectableOptions = null)
		{
			_onConfirm = onConfirm;
			_onCancel = onCancel;
			if (spawned)
				TitleText.SetText(title);
			else
				_cachedTitle = title;
			startString = startString.Trim();
			NameInput.Text = startString;
			ConfirmBtn.SetInteractable(allowEmpty || startString.Any());
			_allowEmpty = allowEmpty;
			RefreshDropDownEntries(selectableOptions);
			this.Activate();
		}
		public static void OpenNamingDialogue(string title, System.Action<string> onConfirm, System.Action onCancel, string startString = "", bool allowEmpty = false, string[] selectableOptions = null)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.RenamingScreenGO, ModAssets.ParentScreen);
				Instance = screen.AddOrGet<BlueprintRenamingScreen>();
				Instance.gameObject.SetActive(true);
				Instance.Init();
			}
			Instance.mouseOver = true;
			Instance.Refresh(title, onConfirm, onCancel, startString, allowEmpty, selectableOptions);
			Instance.transform.SetAsLastSibling();
			//Instance.NameInput.ExternalStartEditing();

			//KScreenManager.Instance.RefreshStack();
		}


		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				//SgtLogger.l("BlueprintRenamingScreen consume esc.");
				OnCancelClicked();
			}
			else if (e.TryConsume(Action.DialogSubmit) && NameInput.Text.Trim().Any())
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

