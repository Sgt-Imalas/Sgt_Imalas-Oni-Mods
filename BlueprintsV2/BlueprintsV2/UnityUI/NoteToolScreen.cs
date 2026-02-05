using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.Tools;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using BlueprintsV2.Tools;
using BlueprintsV2.UnityUI;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI;
using static BlueprintsV2.STRINGS.UI.TOOLS;
using static BlueprintsV2.STRINGS.UI.USEBLUEPRINTSTATECONTAINER.INFOITEMSCONTAINER;
using static KTabMenuHeader;

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
			ColorPicker = transform.Find("ColorPicker").gameObject.AddOrGet<FColorPickerArray>();
			TextMode = transform.Find("Buttons/TextToggle").gameObject.AddOrGet<FToggleButton>();
			TextMode.OnClick += () => SelectMode(true);
			ElementMode = transform.Find("Buttons/ElementToggle").gameObject.AddOrGet<FToggleButton>();
			ElementMode.OnClick += () => SelectMode(false);
			TitleInput = transform.Find("NoteTitleInput/Input").gameObject.AddOrGet<FInputField2>();
			//TitleInput.OnValueChanged.AddListener(ApplyBlueprintFilter);
			TitleInput.Text = string.Empty;

			TextInput = transform.Find("NoteTextInput/Input").gameObject.AddOrGet<FInputField2>();
			//TitleInput.OnValueChanged.AddListener(ApplyBlueprintFilter);
			TextInput.Text = string.Empty;

			ClearTitle = transform.Find("NoteTitleInput/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearTitle.OnClick += () => TitleInput.Text = string.Empty;

			ClearText = transform.Find("NoteTextInput/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearText.OnClick += () => TextInput.Text = string.Empty;

			NoteModeGOs.Add(transform.Find("NoteTitleInput").gameObject);
			NoteModeGOs.Add(transform.Find("NoteTextInput").gameObject);
			NoteModeGOs.Add(transform.Find("ColorPickerLabel").gameObject);
			NoteModeGOs.Add(transform.Find("ColorPicker").gameObject);
		}

		internal void ApplyTextNoteInfo(TextNote info)
		{
			string title = TitleInput.Text.Any() ? TitleInput.Text : STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TITLE;
			string text = TextInput.Text.Any() ? TextInput.Text : STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TEXT;
			info.SetInfo(title, text, ColorPicker.SelectedColor, true);
		}
		public override void OnKeyDown(KButtonEvent e)
		{
			base.OnKeyDown(e);
			if (e.TryConsume(Action.CameraHome))
				e.Consumed = true;
		}
		public override void OnKeyUp(KButtonEvent e)
		{
			base.OnKeyUp(e);
			if (e.TryConsume(Action.CameraHome))
				e.Consumed = true;
		}
	}
}
