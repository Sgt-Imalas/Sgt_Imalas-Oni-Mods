﻿using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilLibs.UIcmp //Source: Aki
{
	public class FScreen : KScreen
	{
		public const float SCREEN_SORT_KEY = 300f;

#pragma warning disable IDE0051 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
		private bool shown = false;
		public bool pause = true;

		public FButton cancelButton;
		public FButton confirmButton;
		public FButton XButton;
		public FButton SteamButton;
		public FButton GithubButton;

		public override void OnPrefabInit()
		{
			SetObjects();
			activateOnSpawn = true;
			gameObject.SetActive(true);
		}

        //ancient auto button setup for settings menu, inherited from aki
        public virtual void SetObjects()
		{
			var refsDataObj = transform.Find("SettingsDialogData");

			if (refsDataObj != null && refsDataObj.gameObject.TryGetComponent(out Text text))
			{
				var buttonRefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(text.text);

				SgtLogger.debuglog("deserialized", text.text, buttonRefs["apply"]);

				cancelButton = SetButton("cancel", buttonRefs);
				confirmButton = SetButton("apply", buttonRefs);
				XButton = SetButton("close", buttonRefs);
				SteamButton = SetButton("steam", buttonRefs);
				GithubButton = SetButton("github", buttonRefs);

				Destroy(refsDataObj.gameObject);
			}
		}

		private FButton SetButton(string key, Dictionary<string, string> buttonRefs)
		{
			if (buttonRefs.TryGetValue(key, out string path))
			{
				var obj = transform.Find(path);
				if (obj != null)
				{
					SgtLogger.debuglog("set button " + key + " " + path);
					return obj.gameObject.AddComponent<FButton>();
				}
				else
					SgtLogger.debuglog("Not an object: " + path);
			}

			return null;
		}

		public virtual void ShowDialog()
		{
			if (transform.parent.GetComponent<Canvas>() == null && transform.parent.parent != null)
			{
				transform.SetParent(transform.parent.parent);
			}

			transform.SetAsLastSibling();

			if (cancelButton != null) cancelButton.OnClick += OnClickCancel;
			if (XButton != null) XButton.OnClick += OnClickCancel;
			if (confirmButton != null) confirmButton.OnClick += OnClickApply;
			if (GithubButton != null) GithubButton.OnClick += OnClickGithub;
			if (SteamButton != null) SteamButton.OnClick += OnClickSteam;
		}

		public void OnClickGithub() => Application.OpenURL("https://github.com/aki-art/ONI-Mods");

		public void OnClickSteam() => Application.OpenURL("https://steamcommunity.com/id/akisnothere/myworkshopfiles/?appid=457140");

		public virtual void OnClickCancel()
		{
			SgtLogger.debuglog("cancel");
			Reset();
			Deactivate();
		}

		public virtual void Reset()
		{
		}

		public virtual void OnClickApply()
		{
		}

		#region generic kscreen behaviour
		public override void OnCmpEnable()
		{
			base.OnCmpEnable();
			if (CameraController.Instance != null)
			{
				CameraController.Instance.DisableUserCameraControl = true;
			}
		}

		public override void OnCmpDisable()
		{
			base.OnCmpDisable();
			if (CameraController.Instance != null)
			{
				CameraController.Instance.DisableUserCameraControl = false;
			}
			Trigger((int)GameHashes.Close, null);
		}

		public override bool IsModal()
		{
			return true;
		}

		public override float GetSortKey()
		{
			return SCREEN_SORT_KEY;
		}

		public override void OnActivate()
		{
			OnShow(true);
		}

		public override void OnDeactivate()
		{
			OnShow(false);
		}

		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (pause && SpeedControlScreen.Instance != null)
			{
				if (show && !shown)
				{
					SpeedControlScreen.Instance.Pause(false);
				}
				else
				{
					if (!show && shown)
					{
						SpeedControlScreen.Instance.Unpause(false);
					}
				}
				shown = show;
			}
		}

		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape))
			{
				OnClickCancel();
			}
			else
			{
				base.OnKeyDown(e);
			}
		}

		public override void OnKeyUp(KButtonEvent e)
		{
			if (!e.Consumed)
			{
				KScrollRect scroll_rect = GetComponentInChildren<KScrollRect>();
				if (scroll_rect != null)
				{
					scroll_rect.OnKeyUp(e);
				}
			}
			e.Consumed = true;
		}
		#endregion

	}
}
