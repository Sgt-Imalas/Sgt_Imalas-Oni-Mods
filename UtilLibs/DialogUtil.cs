﻿using HarmonyLib;
using PeterHan.PLib.Core;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UtilLibs
{
	public static class DialogUtil
	{
		public static void CreateConfirmDialogFrontend(string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null, bool useScreenSpaceOverlay = false, GameObject parent = null)
		=> CreateConfirmDialog(title, text, confirm_text, on_confirm, cancel_text, on_cancel, configurable_text, on_configurable_clicked, image_sprite, true, useScreenSpaceOverlay, parent);
		public static ConfirmDialogScreen CreateConfirmDialog(string title = null, string text = null, string confirm_text = null, System.Action on_confirm = null, string cancel_text = null, System.Action on_cancel = null, string configurable_text = null, System.Action on_configurable_clicked = null, Sprite image_sprite = null, bool frontend = false, bool useScreenSpaceOverlay = false, GameObject parent = null)
		{
			if(parent == null)
				parent = frontend && !useScreenSpaceOverlay ? Global.Instance.globalCanvas : GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			var dialogue = ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent));

			if (!frontend)
				dialogue.Subscribe(476357528, (_) => CameraController.Instance.DisableUserCameraControl = true);
			dialogue.PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, title, confirm_text, cancel_text, image_sprite);
			return dialogue;
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
			FileNameDialog textDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, dialogueParent);
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

			if (fillerText != null)
			{
				var text = textDialog?.inputField?.transform?.Find("Text Area/Placeholder")?.GetComponent<LocText>()?.text;
				if (text != null)
					text = fillerText;
			}
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
