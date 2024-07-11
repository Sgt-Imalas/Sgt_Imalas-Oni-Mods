using HarmonyLib;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static STRINGS.UI.TOOLS;

namespace UtilLibs
{
    public static class DialogUtil
    {
        public static void CreateConfirmDialogFrontend(string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null)
        => CreateConfirmDialog(title, text, confirm_text, on_confirm, cancel_text, on_cancel, configurable_text, on_configurable_clicked, image_sprite, true);
        public static void CreateConfirmDialog(string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null, bool frontend = false)
        {
            GameObject parent = null;
            var dialogue = ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent ?? Global.Instance.globalCanvas));

            if (!frontend)
                dialogue.Subscribe(476357528, (_) => CameraController.Instance.DisableUserCameraControl = true);
            dialogue.PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, title, confirm_text, cancel_text, image_sprite);

        }
        static async Task ExecuteWithDelay(int ms, System.Action action)
        {
            await Task.Delay(ms);
            action.Invoke();
        }
        public static FileNameDialog CreateTextInputDialog(string title, string startText = null, string fillerText = null, bool allowEmpty = false, System.Action<string> onConfirm = null, System.Action onCancel = null, GameObject parent = null, bool lockCam = true, bool unlockCam = true, bool frontEnd = false, int maxCharCount = 48, bool high = false, bool undoStripping = false)
        {
            if (startText == null)
                startText = string.Empty;
            GameObject dialogueParent = parent != null ? parent : GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog textDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, dialogueParent, true);
            textDialog.transform.SetAsLastSibling();
            textDialog.name = Assembly.GetExecutingAssembly().GetName().Name + "_" + title;
            var tmp = textDialog.inputField;
            tmp.richText = false;
            tmp.characterValidation = TMP_InputField.CharacterValidation.None;
            tmp.characterLimit = maxCharCount;
            tmp.onValidateInput = null;
            tmp.inputValidator = null;
            tmp.contentType = TMP_InputField.ContentType.Standard;
            tmp.isRichTextEditingAllowed = false;

            if (undoStripping)
            {
                tmp.onValueChanged.RemoveAllListeners(); //doesnt work because it gets reapplied in onSpawn...
                textDialog.StartCoroutine(RemoveListenersDelayer());

                IEnumerator RemoveListenersDelayer()
                {
                    yield return null;
                    yield return null;//wait 1 frame (2 for max safety), to remove the input limiting listener.
                    tmp.onValueChanged.RemoveAllListeners();
                }
            }
            //ExecuteWithDelay(20,() =>
            //{
            //    tmp?.onValueChanged?.RemoveAllListeners();
            //});

            if (fillerText != null)
            {
                //
                textDialog.inputField.transform.Find("Text Area/Placeholder").GetComponent<LocText>().text = fillerText;
            }
            UtilMethods.ListAllPropertyValues(textDialog.inputField);
            UtilMethods.ListAllFieldValues(textDialog.inputField);
            //if (high)
            //    textDialog.inputField.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);

            if (lockCam && !frontEnd)
                CameraController.Instance.DisableUserCameraControl = true;

            TMP_InputField inputField = textDialog.inputField;
            KButton confirmButton = textDialog.confirmButton;

            if (!startText.IsNullOrWhiteSpace())
            {
                textDialog.SetTextAndSelect(startText);
            }
            else
                textDialog.SetTextAndSelect(string.Empty);



            if (onConfirm != null)
            {
                textDialog.onConfirm += (string result) =>
                {
                    if (result.EndsWith(".sav"))
                        result = result.Substring(0, result.Length - 4);
                    onConfirm.Invoke(result);
                };
            }
            if (allowEmpty && textDialog.onConfirm != null)
            {
                confirmButton.onClick += () =>
                {
                    if (inputField.text.Length == 0)
                    {
                        textDialog.onConfirm.Invoke(inputField.text);
                        textDialog.Deactivate();
                    }
                };
            }
            if (onCancel != null)
            {
                textDialog.onCancel += onCancel;
            }

            if (!frontEnd)
            {
                if (unlockCam)
                    textDialog.Subscribe(476357528, (_) => CameraController.Instance.DisableUserCameraControl = false);
                else
                    textDialog.Subscribe(476357528, (_) => CameraController.Instance.DisableUserCameraControl = true);
            }

            Transform titleTransform = textDialog.transform.Find("Panel")?.Find("Title_BG")?.Find("Title");
            if (titleTransform != null && titleTransform.TryGetComponent<LocText>(out var titleText))
            {
                titleText.text = title;
            }

            return textDialog;
        }

    }
}
