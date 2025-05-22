using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace AkiTrueTiles_SkinSelectorAddon.UI
{
	class TrueTilesSkinSelectorSideScreen : SideScreenContent
	{
		[SerializeField]
		private RectTransform buttonContainer;

		private GameObject stateButtonPrefab;
		private Dictionary<SimHashes, MultiToggle> buttons = new Dictionary<SimHashes, MultiToggle>();
		TrueTiles_OverrideStorage Target;


		public override bool IsValidForTarget(GameObject target) => Mod.TrueTilesEnabled && target.TryGetComponent<TrueTiles_OverrideStorage>(out _);

		public override void OnSpawn()
		{
			base.OnSpawn();
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			InitLinks();

		}

		bool init = false;
		void InitLinks()
		{
			if (init) return;

			titleKey = "TrueTiles.STRINGS.UI.SETTINGSDIALOG.TITLEBAR.LABEL";
			stateButtonPrefab = transform.Find("ButtonPrefab").gameObject;
			stateButtonPrefab.SetActive(false);
			buttonContainer = transform.Find("Content/Scroll/Grid").GetComponent<RectTransform>();

			var buttons = transform.Find("Butttons");
			buttons.Find("ClearStyleButton").gameObject.SetActive(false);
			var apply = buttons.Find("ApplyButton").gameObject;

			apply.GetComponent<ToolTip>().enabled = false;
			UIUtils.TryChangeText(apply.transform, "Label", Strings.Get("STRINGS.UI.FRONTEND.INPUT_BINDINGS_SCREEN.RESET"));
			UIUtils.AddActionToButton(apply.transform, "", ResetOverrides);

			//buttons.gameObject.SetActive(false);
			GenerateStateButtons();
			init = true;
		}

		void ResetOverrides()
		{
			if (Target == null)
				return;

			Target.ResetOverride();
			RefreshButtons();
		}

		public override void SetTarget(GameObject target)
		{
			if (target != null)
			{
				base.SetTarget(target);
				if (target.TryGetComponent<TrueTiles_OverrideStorage>(out var overrideStorage))
				{
					Target = overrideStorage;
					InitLinks();
					GenerateStateButtons();
					RefreshButtons();
				}
			}
		}


		// Creates clickable card buttons for all the balloon skin types
		private void GenerateStateButtons()
		{
			ClearButtons();
			if (Target == null)
				return;
			foreach (var elementOverride in Target.GetAvailableElementOverrides())
			{
				AddButton(elementOverride, Target.IsCurrentOverride(elementOverride),
				() =>
				{
					Target.SetOverride(elementOverride);
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
			foreach (var element in Target.GetAvailableElementOverrides())
			{
				if (buttons.ContainsKey(element))
				{
					buttons[element].ChangeState(Target.IsCurrentOverride(element) ? 1 : 0);
				}
			}
		}

		private void AddButton(SimHashes element, bool enabled, System.Action onClick)
		{
			if (!ModAssets.GetSpriteForTile(Target.Def, element, out var sprite))
			{
				SgtLogger.l($"Failed to get ui sprite for {Target.Def} {element}");
				return;
			}
			//SgtLogger.l(BallonSkin+" "+enabled+" "+onClick);   
			var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);
			if (gameObject.TryGetComponent(out MultiToggle button))
			{
				button.onClick += onClick;
				button.ChangeState(enabled ? 1 : 0);
				UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = sprite;
				UIUtils.AddSimpleTooltipToObject(gameObject, Assets.TryGetPrefab(element.CreateTag()).GetProperName(), true);
				buttons.Add(element, button);
			}
		}

		private void ClearButtons()
		{
			foreach (var button in buttons)
			{
				UnityEngine.Object.Destroy(button.Value.gameObject);
			}
			buttons.Clear();
		}
	}
}
