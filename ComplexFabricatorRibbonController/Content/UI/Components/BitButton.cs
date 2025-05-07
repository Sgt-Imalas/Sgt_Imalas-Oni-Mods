using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace ComplexFabricatorRibbonController.Content.UI.Components
{
	class BitButton : KMonoBehaviour
	{
		public int targetBit;
		public Image On, Off, SelectedRecipe;
		FButton SelectButton;
		public Action<int> OpenSelectionScreen;
		private bool init = false;
		ToolTip toolTip;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			UpdateUI(null,false);
		}

		void OnButtonClicked()
		{
			OpenSelectionScreen?.Invoke(targetBit);
		}

		private void InitUi()
		{
			if (init)
				return;
			init = true;

			SelectedRecipe = transform.Find("Image")?.gameObject.GetComponent<Image>();
			Off = transform.Find("Off")?.gameObject.GetComponent<Image>();
			On = transform.Find("On")?.gameObject.GetComponent<Image>();
			SelectButton = gameObject.AddOrGet<FButton>();
			toolTip = UIUtils.AddSimpleTooltipToObject(gameObject, "");
			SelectButton.OnClick += OnButtonClicked;
		}
		public void UpdateUI(ComplexRecipe recipe, bool logicOn)
		{
			if (!init)
			{
				InitUi();
			}
			var colourOn = (Color)GlobalAssets.Instance.colorSet.logicOn;
			colourOn.a = 1; //a is 0 for these by default, but that doesnt allow tinting the symbols here

			var colourOff = (Color)GlobalAssets.Instance.colorSet.logicOff;
			colourOff.a = 1; //a is 0 for these by default, but that doesnt allow tinting the symbols here
			On.color = colourOn;
			Off.color = colourOff;
			if (recipe == null)
			{
				SelectedRecipe.sprite = Assets.GetSprite("unknown");
				SelectedRecipe.color = Color.white;
				toolTip.SetSimpleTooltip(global::STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION);
			}
			else
			{
				SelectedRecipe.sprite = recipe.GetUIIcon();
				SelectedRecipe.color = recipe.GetUIColor();
				toolTip.SetSimpleTooltip(recipe.GetUIName(false));
			}
			On.gameObject.SetActive(logicOn);
			Off.gameObject.SetActive(!logicOn);

		}

		internal void SetInteractable(bool interactable)
		{
			SelectButton?.SetInteractable(interactable);
		}
	}
}
