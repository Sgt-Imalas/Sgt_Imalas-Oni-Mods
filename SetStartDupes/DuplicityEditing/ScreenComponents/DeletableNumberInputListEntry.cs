using System;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
	internal class DeletableNumberInputListEntry : KMonoBehaviour
	{
		FInputField2 inputField;
		public Action<string> OnInputChanged;
		public System.Action OnDeleteClicked;
		public string Text, Tooltip, PlaceholderText = "";
		LocText label, placeholder;
		FButton delete;
		public bool WholeNumbers = true;

		public override void OnPrefabInit()
		{
			base.OnSpawn();
			inputField = transform.Find("Input").FindOrAddComponent<FInputField2>();
			inputField.inputField.onEndEdit.AddListener(InputListener);
			inputField.SetTextFromData("0");



			label = transform.Find("Label").GetComponent<LocText>();
			label.SetText(Text);

			if (Tooltip != null && Tooltip.Length > 0)
			{
				UIUtils.AddSimpleTooltipToObject(this.gameObject, Tooltip);
			}


			placeholder = transform.Find("Input/TextArea/Placeholder").GetComponent<LocText>();
			placeholder.SetText(PlaceholderText);

			delete = transform.Find("DeleteBtn").FindOrAddComponent<FButton>();
			delete.OnClick += OnDeleteClicked;
		}
		void InputListener(string text)
		{
			if (OnInputChanged != null)
				OnInputChanged(text);
		}
		public void SetInputFieldVisibility(bool visibility)
		{
			inputField?.gameObject.SetActive(visibility);
		}
		public void SetInputFieldValue(string value)
		{
			inputField?.SetTextFromData(value);
		}
	}
}
