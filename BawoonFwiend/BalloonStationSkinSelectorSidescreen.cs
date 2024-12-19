using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static BawoonFwiend.Bawoongiver;

namespace BawoonFwiend
{
	internal class BalloonStationSkinSelectorSidescreen : SideScreenContent
	{
		[SerializeField]
		private RectTransform buttonContainer;

		private GameObject stateButtonPrefab;
		private GameObject PlaceStationButton;
		private GameObject ToggleManualDeliveryButton;
		private GameObject flipButton;
		private Dictionary<BalloonSkinByIndex, MultiToggle> buttons = new Dictionary<BalloonSkinByIndex, MultiToggle>();

		public Bawoongiver TargetBalloonStand;
		Image RandomButtonImage;


		public override bool IsValidForTarget(GameObject target) => target.GetComponent<Bawoongiver>() != null;

		public override void OnSpawn()
		{
			base.OnSpawn();

			RandomButtonImage = flipButton.transform.Find("FG").GetComponent<Image>();

			if (PlaceStationButton.TryGetComponent<KImage>(out var sourceBtn) && flipButton.TryGetComponent<KImage>(out var targetButton))
			{
				targetButton.colorStyleSetting = (sourceBtn.colorStyleSetting);
				targetButton.ApplyColorStyleSetting();
			}

			UIUtils.AddActionToButton(ToggleManualDeliveryButton.transform, "", () => { TargetBalloonStand.ToggleManualDelivery(); RefreshButtons(); });
			UIUtils.AddActionToButton(PlaceStationButton.transform, "", () => { TargetBalloonStand.ToggleAll(); RefreshButtons(); });
			UIUtils.AddActionToButton(flipButton.transform, "", () => { TargetBalloonStand.ToggleFullyRandom(); RefreshButtons(); });
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			InitLinks();
			RefreshStrings();

		}

		bool init = false;
		void InitLinks()
		{
			if (init) return;

			titleKey = "STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TITLE";
			stateButtonPrefab = transform.Find("ButtonPrefab").gameObject;
			buttonContainer = transform.Find("Content/Scroll/Grid").GetComponent<RectTransform>();
			PlaceStationButton = transform.Find("Butttons/ApplyButton").gameObject;
			flipButton = transform.Find("Butttons/ClearStyleButton").gameObject;
			RandomButtonImage = flipButton.transform.Find("FG").GetComponent<Image>();


			var buttons = transform.Find("Butttons");
			var toggle = Util.KInstantiateUI(buttons.gameObject, transform.gameObject).transform;
			toggle.SetAsLastSibling();
			toggle.Find("ClearStyleButton").gameObject.SetActive(false);
			ToggleManualDeliveryButton = toggle.transform.Find("ApplyButton").gameObject;


			GenerateStateButtons();

			PlaceStationButton.GetComponent<ToolTip>().enabled = false;
			Destroy(flipButton.GetComponent<ToolTip>());
			UIUtils.AddSimpleTooltipToObject(flipButton.transform.Find("FG"), "", true);
			init = true;
		}

		private List<int> refreshHandle = new List<int>();
		public override void SetTarget(GameObject target)
		{
			if (target != null)
			{
				foreach (int id in this.refreshHandle)
					target.Unsubscribe(id);
				refreshHandle.Clear();

				base.SetTarget(target);

				if (target.TryGetComponent<Bawoongiver>(out var bloon))
				{
					TargetBalloonStand = bloon;
					InitLinks();
					//GenerateStateButtons();
					GenerateStateButtons();
					RefreshButtons();
				}
			}
		}

		void RefreshStrings()
		{
			bool toggleAll = TargetBalloonStand == null ? false : TargetBalloonStand.ToggleAllBtnOn;
			bool manualDeliveryEnabled = TargetBalloonStand == null ? false : TargetBalloonStand.UseManualDelivery;
			bool toggleAllRandom = TargetBalloonStand == null ? false : TargetBalloonStand.AllRandom;
			UIUtils.TryChangeText(PlaceStationButton.transform, "Label", toggleAll
				? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TOGGLEALLOFF
				: STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TOGGLEALLON);

			RandomButtonImage.sprite = toggleAllRandom
				? Assets.GetSprite("icon_balloon_toggle_random_icon")
				: Assets.GetSprite("icon_archetype_random");


			UIUtils.AddSimpleTooltipToObject(flipButton.transform.Find("FG"), toggleAllRandom
				? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ALLRANDOMYES
				: STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ALLRANDOMNO);

			UIUtils.TryChangeText(ToggleManualDeliveryButton.transform, "Label", manualDeliveryEnabled
				? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.DISABLEMANUALDELIVERY
				: STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ENABLEMANUALDELIVERY);

			UIUtils.AddSimpleTooltipToObject(ToggleManualDeliveryButton.transform, manualDeliveryEnabled
				? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.DISABLEMANUALDELIVERYTOOLTIP
				: STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ENABLEMANUALDELIVERYTOOLTIP);
		}

		// Creates clickable card buttons for all the balloon skin types
		private void GenerateStateButtons()
		{
			ClearButtons();
			foreach (var BallonSkin in TargetBalloonStand.EnabledBalloonSkins)
			{
				AddButton(BallonSkin.Key, BallonSkin.Value,
				() =>
					{
						TargetBalloonStand.ToggleSkin(BallonSkin.Key);
						RefreshButtons();
					}
				);
			}
		}

		/// <summary>
		/// Refresh activation states of buttons
		/// </summary>
		void RefreshButtons()
		{
			TargetBalloonStand.UpdateActives();
			foreach (var skin in TargetBalloonStand.EnabledBalloonSkins)
			{
				if (buttons.ContainsKey(skin.Key))
				{
					buttons[skin.Key].ChangeState(skin.Value ? 1 : 0);
				}
			}
			RefreshStrings();
		}

		private void AddButton(BalloonSkinByIndex BallonSkin, bool enabled, System.Action onClick)
		{
			//SgtLogger.l(BallonSkin+" "+enabled+" "+onClick);   
			var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);
			if (gameObject.TryGetComponent(out MultiToggle button))
			{
				//Assets.TryGetAnim((HashedString)animName, out var anim);
				button.onClick += onClick;
				button.ChangeState(enabled ? 1 : 0);
				var SkinOverride = TargetBalloonStand.GetOverrideViaIndex(BallonSkin);
				var BallonSprite = ModAssets.GetSpriteFrom(SkinOverride.animFile.Unwrap(), SkinOverride.symbol.Unwrap());
				UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = BallonSprite;
				buttons.Add(BallonSkin, button);
			}
		}

		private void ClearButtons()
		{
			foreach (var button in buttons)
			{
				Util.KDestroyGameObject(button.Value.gameObject);
			}

			buttons.Clear();
		}
	}
}