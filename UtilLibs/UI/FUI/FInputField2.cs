using TMPro;
using UnityEngine;

namespace UtilLibs.UIcmp //Source: Aki
{
    public class FInputField2 : KScreen, IInputHandler
    {
        [MyCmpReq]
        public TMP_InputField inputField;

        [SerializeField]
        public string textPath = "Text";

        [SerializeField]
        public string placeHolderPath = "Placeholder";

        private bool initialized;

        public bool IsEditing()
        {
            return isEditing;
        }

        public string Text
        {
            get => inputField.text;
            set
            {
                if (!initialized)
                {
                    // rehook references, these were lost on LocText conversion
                    SgtLogger.debuglog("SET INPUT PARTS");
                    inputField.textComponent = inputField.textViewport.transform.Find(textPath).gameObject.AddOrGet<LocText>();
                    inputField.placeholder = inputField.textViewport.transform.Find(placeHolderPath).gameObject.AddOrGet<LocText>();

                    initialized = true;
                }

             SgtLogger.debuglog("setting text " + value);
             SgtLogger.Assert("inputField", inputField);
             SgtLogger.Assert("textViewport", inputField.textViewport);
             SgtLogger.Assert("textcomponent", inputField.textComponent);
             SgtLogger.Assert("placeholder", inputField.placeholder);

                inputField.text = value;
            }
        }

        public TMP_InputField.OnChangeEvent OnValueChanged => inputField.onValueChanged;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            inputField.onFocus += OnEditStart;
            inputField.onEndEdit.AddListener(OnEditEnd);

            Activate();
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);

            if (show)
            {
                Activate();
                inputField.ActivateInputField();
            }
            else
            {
                Deactivate();
            }
        }

        public void Submit()
        {
            inputField.OnSubmit(null);
        }

        private void OnEditEnd(string input)
        {
            isEditing = false;
            inputField.DeactivateInputField();
        }

        private void OnEditStart()
        {
            isEditing = true;
            inputField.Select();
            inputField.ActivateInputField();

            KScreenManager.Instance.RefreshStack();
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (!isEditing)
            {
                base.OnKeyDown(e);
                return;
            }

            if (e.TryConsume(Action.Escape))
            {
                inputField.DeactivateInputField();
                e.Consumed = true;
                isEditing = false;
            }

            if (e.TryConsume(Action.DialogSubmit))
            {
                e.Consumed = true;
                inputField.OnSubmit(null);
            }

            if (isEditing)
            {
                e.Consumed = true;
                return;
            }

            if (!e.Consumed)
            {
                base.OnKeyDown(e);
            }
        }
    }
}