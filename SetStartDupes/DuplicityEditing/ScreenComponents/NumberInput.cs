using System;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{

	public class NumberInput : KMonoBehaviour
	{
		FInputField2 inputField;
		public Action<string> OnInputChanged;
		public string Text, PlaceholderText = "";
		LocText label, placeholder;
		FButton increase, decrease;
		public bool WholeNumbers = true;

		public override void OnPrefabInit()
		{
			base.OnSpawn();
			inputField = transform.Find("Input").FindOrAddComponent<FInputField2>();
			inputField.inputField.onEndEdit.AddListener(InputListener);
			inputField.SetTextFromData("0");

			label = transform.Find("Label").GetComponent<LocText>();
			label.SetText(Text);

			placeholder = transform.Find("Input/TextArea/Placeholder").GetComponent<LocText>();
			placeholder.SetText(PlaceholderText);

			increase = transform.Find("Plus").FindOrAddComponent<FButton>();
			increase.OnClick += IncreaseClicked;
			decrease = transform.Find("Minus").FindOrAddComponent<FButton>();
			decrease.OnClick += DecreaseClicked;
		}
		void InputListener(string text)
		{
			if (OnInputChanged != null)
				OnInputChanged(text);
		}
		void IncreaseClicked() => ChangeValueByOne(true);
		void DecreaseClicked() => ChangeValueByOne(false);

		void ChangeValueByOne(bool increase)
		{
			if (!float.TryParse(inputField.Text, out float currentValue))
			{
				SgtLogger.error(inputField.Text + " is not a valid number!");
				return;
			}
			currentValue += increase ? 1 : -1;
			if (WholeNumbers)
			{
				SetInputFieldValue(Mathf.RoundToInt(currentValue).ToString());
			}
			else
			{
				SetInputFieldValue(currentValue.ToString());
			}
			OnInputChanged(inputField.Text);

		}

		public void SetInputFieldValue(string value)
		{
			inputField?.SetTextFromData(value);
		}
	}
}
