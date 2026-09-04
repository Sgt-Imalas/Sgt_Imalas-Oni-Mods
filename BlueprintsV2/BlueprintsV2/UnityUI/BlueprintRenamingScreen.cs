using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using BlueprintsV2.Tools;
using BlueprintsV2.UnityUI.Components;
using Database;
using rail;
using STRINGS;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
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
using TMPro;

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
			}
			void OnClickedInternal()
			{
				OnClicked?.Invoke(Name);
			}
		}
		class DropDownCheckMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
		{
			public BlueprintRenamingScreen parent;
			public void OnPointerEnter(PointerEventData eventData)
			{
				parent.MouseOverDropdown = true;
			}
			public void OnPointerExit(PointerEventData eventData)
			{
				parent.MouseOverDropdown = false;
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
		public bool MouseOverDropdown = false;

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
			NameInput.inputField.onSelect.AddListener(OnStartedTyping);
			NameInput.Text = string.Empty;

			transform.Find("Body/SearchBar/Input/TextArea/Placeholder").gameObject.GetComponent<LocText>().SetText(FILE_NAME_DIALOG.ENTER_TEXT);

			ConfirmBtn.transform.Find("Text").gameObject.GetComponent<LocText>().SetText(CONFIRMDIALOG.OK);
			CancelBtn.transform.Find("Text").gameObject.GetComponent<LocText>().SetText(CONFIRMDIALOG.CANCEL);

			//dropdown:
			_dropDownIcon = transform.Find("Body/SearchBar/OpenDropdown/Image").gameObject.GetComponent<Image>();
			_dropDownBtn = transform.Find("Body/SearchBar/OpenDropdown").gameObject.AddOrGet<FButton>();
			_dropDownBtn.OnClick += ToggleDropDown;

			_dropDownGO = transform.Find("Body/DropDownArea").gameObject;
			_dropDownGO.AddOrGet<DropDownCheckMouse>().parent = this;
			// 下移60像素 / Move down 60 pixels
			_dropDownGO.transform.localPosition += new Vector3(0, -65, 0);
			
			RectTransform dropRect = _dropDownGO.GetComponent<RectTransform>();
			// 水平方向拉伸：左对齐到 0，右对齐到 1 / Horizontal stretch: left anchor 0, right anchor 1
			dropRect.anchorMin = new Vector2(0, dropRect.anchorMin.y);
			dropRect.anchorMax = new Vector2(1, dropRect.anchorMax.y);
			dropRect.pivot = new Vector2(0.5f, dropRect.pivot.y); // 轴心水平居中 / Pivot centered horizontally
			// 宽度自动匹配父物体，sizeDelta.x = 0 表示无偏移 / Width auto-matches parent, sizeDelta.x = 0 means no offset
			dropRect.sizeDelta = new Vector2(-20, dropRect.sizeDelta.y);
			// 水平位置归零（因为拉伸锚点下，anchoredPosition.x 控制整体偏移） / Reset horizontal position (with stretch anchors, anchoredPosition.x controls overall offset)
			dropRect.anchoredPosition = new Vector2(0, dropRect.anchoredPosition.y);
			
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
			bool willOpen = !_dropDownGO.activeSelf;
			_dropDownGO.SetActive(willOpen);

            
			// 手动点击下拉按钮进入选择模式（非搜索） / Manual dropdown toggle enters selection mode (not search)
			if (willOpen && _currentSelectableNames.Any())
			{
				FilterSelectableOptions(NameInput.Text, true);
			}

			RefreshDropdownIcon();
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

			if (_currentSelectableNames.Any())
			{
				bool hasMatch = _currentSelectableNames.Any(name => 
					name.ToLower().Contains(filterstring.ToLower()));

				if (hasMatch)
				{
					_dropDownGO.SetActive(true);
					FilterSelectableOptions(filterstring); // 显示匹配项（搜索模式） / Show matching items (search mode)
				}
				else
				{
					_dropDownGO.SetActive(false);
				}
				RefreshDropdownIcon();
			}
		}
		void OnStartedTyping(string _)
		{
			if (!_currentSelectableNames.Any())
				return;

			string currentText = NameInput.Text;
			bool excludeCurrent = !string.IsNullOrEmpty(currentText);
			
			// 进入选择模式 / Enter selection mode
			_dropDownGO.SetActive(true);
			FilterSelectableOptions(currentText, excludeCurrent);
			RefreshDropdownIcon();
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

			// 打开输入框就进入选择模式 / Enter selection mode when input is focused
			string currentText = NameInput.Text;
			bool excludeCurrent = !string.IsNullOrEmpty(currentText);
			FilterSelectableOptions(currentText, excludeCurrent);
			
			RefreshDropdownIcon();
		}

		void FilterSelectableOptions(string filterString = "", bool invertMatch = false)
		{
			foreach (var entry in _dropDownEntries.Values)
			{
				entry.gameObject.SetActive(false);
			}
			int count = 0;
			foreach (var name in _currentSelectableNames)
			{
				bool contains = name.ToLower().Contains(filterString.ToLower());
                bool matchesFilter = string.IsNullOrWhiteSpace(filterString) || (invertMatch ? !contains : contains);	
				
				var entry = AddOrGetDropDownEntry(name);
				entry.gameObject.SetActive(matchesFilter);
				if(matchesFilter)
					count++;
			}
			SetDropdownSize(count);
		}
		void SetDropdownSize(int count)
		{
			float height = Mathf.Clamp(count * 32 + 2, 10, 300);
			_dropDownGO.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		BlueprintNameOption AddOrGetDropDownEntry(string name)
		{
			if (_dropDownEntries.TryGetValue(name, out BlueprintNameOption entry))
				return entry;
			var newEntry = Util.KInstantiateUI<BlueprintNameOption>(_dropDownEntryPrefab.gameObject, _dropDownContainer);
			newEntry.gameObject.SetActive(true);
			newEntry.Init(name, OnDropDownEntryClicked);
			
			// 强制设置 RectTransform 锚点为左右拉伸，宽度自动填充父容器 / Force RectTransform anchors to stretch horizontally, width fills parent
			var rect = newEntry.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0, 1);   // 左上角 / Top-left
			rect.anchorMax = new Vector2(1, 1);   // 右上角（宽度拉伸） / Top-right (stretch width)
			rect.pivot = new Vector2(0.5f, 1);    // 顶部中心 / Top-center
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = new Vector2(0, 30);  // 宽度为0由锚点决定，高度固定30 / Width 0 determined by anchors, height fixed 30
			
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

			NameInput.Text = startString;
			ConfirmBtn.SetInteractable(allowEmpty ? true : startString.Any());
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

