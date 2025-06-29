using Mineral_Processing_Mining.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.UI
{
    public class MiningGuidanceDeviceProgramSelectorSideScreen : SideScreenContent
	{
		[SerializeField]
		private RectTransform buttonContainer;

		private GameObject stateButtonPrefab;
		private Dictionary<Tag, MultiToggle> buttons = new ();
		ProgrammableGuidanceModule Target;

		KButton ApplyReprogram, CancelReprogram;
		Tag ReprogramInto;


		public override bool IsValidForTarget(GameObject target) => target.TryGetComponent<ProgrammableGuidanceModule>(out _);

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

			titleKey = "STRINGS.UI.MININGGUIDANCEDEVICEPROGRAMSELECTORSIDESCREEN.TITLE";
			stateButtonPrefab = transform.Find("ButtonPrefab").gameObject;
			stateButtonPrefab.SetActive(false);
			buttonContainer = transform.Find("Content/Scroll/Grid").GetComponent<RectTransform>();

			var buttons = transform.Find("Butttons");
			var cancel = buttons.Find("ClearStyleButton").gameObject;
			var apply = buttons.Find("ApplyButton").gameObject;

			apply.GetComponent<ToolTip>().enabled = false;
			UIUtils.TryChangeText(apply.transform, "Label", Strings.Get("STRINGS.UI.MININGGUIDANCEDEVICEPROGRAMSELECTORSIDESCREEN.APPLY"));
			ApplyReprogram = apply.GetComponent<KButton>();
			CancelReprogram = cancel.AddOrGet<KButton>();
			ApplyReprogram.ClearOnClick();
			ApplyReprogram.onClick += ApplyPending;
			CancelReprogram.ClearOnClick();
			CancelReprogram.onClick += CancelPending;

			//buttons.gameObject.SetActive(false);
			GenerateStateButtons();
			init = true;
		}

		void ApplyPending()
		{
			if (Target == null || ReprogramInto == null || Target.IsThisConfiguration(ReprogramInto.ToString()))
				return;

			Target.StartReprogramTask(ReprogramInto);
			RefreshButtons();
		}
		void CancelPending()
		{
			if (Target == null)
				return;

			Target.CancelReprogrammTask();
			RefreshButtons();
		}

		public override void SetTarget(GameObject target)
		{
			if (target != null)
			{
				base.SetTarget(target);
				if (target.TryGetComponent<ProgrammableGuidanceModule>(out var overrideStorage))
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
			foreach (var elementOverride in Mining_Drillbits_GuidanceDevice_ItemConfig.ProgrammedGuidanceModules)
			{
				AddButton(elementOverride, Target.IsThisConfiguration(elementOverride.ToString()),
				() =>
				{
					ReprogramInto = elementOverride;
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
			ApplyReprogram.isInteractable = (init && ReprogramInto != null && Target != null && !Target.IsThisConfiguration(ReprogramInto.ToString()));
			CancelReprogram.interactable = (init && Target != null && Target.HasPendingReprogramming());

			foreach (var button in buttons)
			{
				bool isReprogramTarget = ReprogramInto == button.Key;
				bool isCurrentConfig = Target.IsThisConfiguration(button.Key.ToString());

				button.Value.gameObject.SetActive(!isCurrentConfig);
				if(!isCurrentConfig)
					button.Value.ChangeState(isReprogramTarget ? 1 : 0);
			}
		}

		private void AddButton(Tag item, bool enabled, System.Action onClick)
		{
			var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);
			if (gameObject.TryGetComponent(out MultiToggle button))
			{
				var prefab = Assets.GetPrefab(item);
				button.onClick += onClick;
				button.ChangeState(enabled ? 1 : 0);
				var image = Def.GetUISprite(prefab);

				UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = image.first;
				UIUtils.AddSimpleTooltipToObject(gameObject, prefab.GetProperName(), true);
				buttons.Add(item, button);
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
