using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI;

namespace BlueprintsV2.BlueprintsV2.UnityUI
{
	internal class TextNoteSideScreen : SideScreenContent
	{
		TextNote Target = null;

		FButton ClearTitle, ClearText;
		FInputField2 TitleInput, TextInput;
		FColorPickerArray ColorPicker;
		public override bool IsValidForTarget(GameObject target) => !target.IsNullOrDestroyed() && target.TryGetComponent<TextNote>(out _);
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}
		public override string GetTitle() => NOTETOOLSTATECONTAINER.TITLE.SIDESCREENTEXT;
		void Init()
		{
			transform.Find("Title").gameObject.SetActive(false);
			transform.Find("Buttons").gameObject.SetActive(false);

			ColorPicker = transform.Find("ColorPicker").gameObject.AddOrGet<FColorPickerArray>();
			ColorPicker.OnColorChange += SetColor;

			TitleInput = transform.Find("NoteTitleInput/Input").gameObject.AddOrGet<FInputField2>();
			TitleInput.Text = string.Empty;
			TitleInput.OnValueChanged.AddListener(SetTitle);

			TextInput = transform.Find("NoteTextInput/Input").gameObject.AddOrGet<FInputField2>();
			TextInput.Text = string.Empty;
			TextInput.OnValueChanged.AddListener(SetText);

			ClearTitle = transform.Find("NoteTitleInput/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearTitle.OnClick += () => TitleInput.Text = string.Empty;
			
			ClearText = transform.Find("NoteTextInput/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearText.OnClick += () => TextInput.Text = string.Empty;
		}
		public override void SetTarget(GameObject target)
		{
			base.SetTarget(target);

			if (!target.TryGetComponent<TextNote>(out var note)) return;

			Target = note;
			var data = note.GetNoteData();
			TextInput.SetTextFromData(data.Text != STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TEXT ? data.Text : string.Empty);
			TitleInput.SetTextFromData(data.Title != STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TITLE ? data.Title : string.Empty);
			ColorPicker.SetSelected(data.SymbolTint);
		}
		void SetTitle(string val)
		{
			val = val.Any() ? val : STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TITLE;
			Target?.UpdateInfo(val);
		}
		void SetText(string val)
		{
			val = val.Any() ? val : STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEXTNOTE_EMPTY.TEXT;

			Target?.UpdateInfo(text: val);
		}
		void SetColor(Color color)  => Target?.UpdateInfo(tint: color);
		public override void ClearTarget()
		{
			base.ClearTarget();
			Target = null;
		}
	}
}
