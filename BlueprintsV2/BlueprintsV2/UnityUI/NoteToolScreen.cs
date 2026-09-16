using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;

namespace BlueprintsV2.BlueprintsV2.UnityUI
{
	internal class NoteToolScreen : KScreen
	{

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members

		public static NoteToolScreen Instance = null;
		FButton ClearTitle, ClearText;
		FToggleButton TextMode, ElementMode;
		FInputField2 TitleInput, TextInput;
		FColorPickerArray ColorPicker;
		FItemPickerArray SymbolPicker;

		List<GameObject> NoteModeGOs = [];
		public bool IsTextMode = true;

		public static void DestroyInstance() { Instance = null; }

		public static void ShowScreen(bool show)
		{
			if (Instance == null)
			{
				GameObject baseContent = ToolMenu.Instance.toolParameterMenu.content;
				//GameObject baseWidgetContainer = ToolMenu.Instance.toolParameterMenu.widgetContainer;

				Instance = Util.KInstantiateUI<NoteToolScreen>(ModAssets.NoteToolStateScreenGO, baseContent.transform.parent.gameObject);
				Instance.gameObject.SetActive(true);
			}
			Instance.gameObject.SetActive(show);
			if (show)
			{
				///Reactivate with a frame delay to get the content size fitter resize to reach the outer container
				Instance.RefreshMode();
				//Instance.StartCoroutine(Instance.RefreshSize());
			}
		}
		void SelectMode(bool textMode)
		{
			IsTextMode = textMode;
			RefreshMode();
		}
		void RefreshMode()
		{
			foreach (var go in NoteModeGOs)
				go.SetActive(IsTextMode);
			TextMode.SetIsSelected(IsTextMode);
			ElementMode.SetIsSelected(!IsTextMode);
			if (IsTextMode)
				RefreshClearButtons(null);

			CreateNoteTool.SetElementSelectorVisibility(!IsTextMode);
			this.SetHasFocus(true);
		}
		public override float GetSortKey()
		{
			return MODAL_SCREEN_SORT_KEY;
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}

		void Init()
		{
			SymbolPicker = transform.Find("SymbolPicker").gameObject.AddOrGet<FItemPickerArray>();
			SymbolPicker.Init(TextNote.SymbolMap, ModAssets.IconSort);

			ColorPicker = transform.Find("ColorPicker").gameObject.AddOrGet<FColorPickerArray>();
			TextMode = transform.Find("Buttons/TextToggle").gameObject.AddOrGet<FToggleButton>();
			TextMode.OnClick += () => SelectMode(true);
			ElementMode = transform.Find("Buttons/ElementToggle").gameObject.AddOrGet<FToggleButton>();
			ElementMode.OnClick += () => SelectMode(false);
			TitleInput = transform.Find("NoteTitleInput/Input").gameObject.AddOrGet<FInputField2>();
			//TitleInput.OnValueChanged.AddListener(ApplyBlueprintFilter);
			TitleInput.AddListener( RefreshClearButtons);
			TitleInput.ClearInputTextWithoutEvent();

			TextInput = transform.Find("NoteTextInput/Input").gameObject.AddOrGet<FInputField2>();
			//TitleInput.OnValueChanged.AddListener(ApplyBlueprintFilter);
			TextInput.AddListener(RefreshClearButtons);
			TextInput.ClearInputTextWithoutEvent();

			ClearTitle = transform.Find("NoteTitleInput/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearTitle.OnClick += () => TitleInput.Text = string.Empty;

			ClearText = transform.Find("NoteTextInput/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearText.OnClick += () => TextInput.Text = string.Empty;

			NoteModeGOs.Add(transform.Find("NoteTitleInput").gameObject);
			NoteModeGOs.Add(transform.Find("NoteTextInput").gameObject);
			NoteModeGOs.Add(transform.Find("ColorPickerLabel").gameObject);
			NoteModeGOs.Add(transform.Find("ColorPicker").gameObject);
			NoteModeGOs.Add(transform.Find("SymbolPickerLabel").gameObject);
			NoteModeGOs.Add(transform.Find("SymbolPicker").gameObject);
		}


		void RefreshClearButtons(string _)
		{
			ClearTitle.SetInteractable(TitleInput.Text.Any());
			ClearText.SetInteractable(TextInput.Text.Any());
		}

		internal void GetTextNoteInfo(out string title, out string text, out string symbol, out Color color)
		{
			title = TitleInput.Text.Any() ? TitleInput.Text : STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TITLE;
			text = TextInput.Text.Any() ? TextInput.Text : STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TEXT;
			color = ColorPicker.SelectedColor;
			symbol = SymbolPicker.SelectedEntry;
		}		
	}
}
