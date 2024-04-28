using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
     
    internal class NumberInput:KMonoBehaviour
    {
        FInputField2 inputField;
        public Action<string> OnInputChanged;
        public string Text, PlaceholderText = "";
        LocText label, placeholder;

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
        }
        void InputListener(string text)
        {
            if(OnInputChanged != null) 
                OnInputChanged(text);
        }

        public void SetInputFieldValue(string value)
        {
            inputField?.SetTextFromData(value);
        }
    }
}
