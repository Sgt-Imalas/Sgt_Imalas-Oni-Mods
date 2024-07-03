using HarmonyLib;
using System;
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

        public static void CreateConfirmDialog(string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null, bool frontend =false)
        {
            GameObject parent = null;
            var dialogue = ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent ?? Global.Instance.globalCanvas));

            if(!frontend)
                dialogue.Subscribe(476357528, (_) => CameraController.Instance.DisableUserCameraControl = true);
            dialogue.PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, title, confirm_text, cancel_text, image_sprite);
            
        }
        public static FileNameDialog CreateTextInputDialog(string title, string startText = null, bool allowEmpty = false, System.Action<string> onConfirm = null, System.Action onCancel = null, GameObject parent = null, bool lockCam = true, bool unlockCam = true, bool Frontent = false)
        {

            GameObject dialogueParent = parent != null ? parent : GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog textDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, dialogueParent);
            textDialog.transform.SetAsLastSibling();
            textDialog.name = Assembly.GetExecutingAssembly().GetName().Name +"_"+ title;
            if(lockCam && !Frontent)
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
            if(onCancel!=null)
            {
                textDialog.onCancel += onCancel;
            }

            if (!Frontent) { 
            if(unlockCam )
                textDialog.Subscribe(476357528, (_) => CameraController.Instance.DisableUserCameraControl = false);
            else
                textDialog.Subscribe(476357528, (_) => CameraController.Instance.DisableUserCameraControl = true);
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
