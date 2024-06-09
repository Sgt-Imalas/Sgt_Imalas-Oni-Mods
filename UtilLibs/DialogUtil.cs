using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UtilLibs
{
    public static class DialogUtil
    {

        public static void CreateConfirmDialog(string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null)
        => KMod.Manager.Dialog(Global.Instance.globalCanvas, title, text, confirm_text, on_confirm, cancel_text, on_cancel, configurable_text, on_configurable_clicked, image_sprite);
        public static FileNameDialog CreateTextInputDialog(string title, bool allowEmpty = false, System.Action<string> onConfirm = null)
        {
            GameObject textDialogParent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog textDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, textDialogParent);
            textDialog.name = Assembly.GetExecutingAssembly().GetName().Name +"_"+ title;

            TMP_InputField inputField = textDialog.inputField;
            KButton confirmButton = textDialog.confirmButton;



            if (onConfirm != null)
            {
                textDialog.onConfirm += (string result)=>
                {
                    result = result.Replace(".sav", string.Empty);
                    onConfirm.Invoke(result);
                };
            }
            if (allowEmpty && textDialog.onConfirm != null)
            {
                confirmButton.onClick += ()=>
                {
                    if (inputField.text.Length == 0)
                    {
                        textDialog.onConfirm.Invoke(inputField.text);
                        textDialog.Deactivate();
                    }
                };
            }

            Transform titleTransform = textDialog.transform.Find("Panel")?.Find("Title_BG")?.Find("Title");
            if (titleTransform != null && titleTransform.TryGetComponent<LocText>( out var titleText))
            {
                titleText.text = title;
            }

            return textDialog;
        }

    }
}
