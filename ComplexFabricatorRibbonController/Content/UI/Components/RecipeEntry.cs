using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.UIcmp;
using UtilLibs;
using UnityEngine.UI;
using UtilLibs.UI.FUI;
using UnityEngine;
using static ComplexFabricatorRibbonController.STRINGS.UI;

namespace ComplexFabricatorRibbonController.Content.UI.Components
{
	class RecipeEntry : KMonoBehaviour
	{
		public ComplexRecipe targetRecipe;
		public LocText RecipeLabel;
		public ToolTip ToolTip;
		public Image RecipeIcon;
		FToggleButton SelectButton;
		public Action<ComplexRecipe> SelectRecipe;
		private bool init = false;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			UpdateUI();
		}

		void OnEntryClicked()
		{
			SelectRecipe?.Invoke(targetRecipe);
		}

		private void InitUi()
		{
			if (init)
				return;
			init = true;
			RecipeLabel = transform.Find("Label")?.gameObject.GetComponent<LocText>();
			RecipeIcon = transform.Find("Image")?.gameObject.GetComponent<Image>();
			SelectButton = gameObject.AddOrGet<FToggleButton>();
			SelectButton.OnClick += OnEntryClicked;
			ToolTip = UIUtils.AddSimpleTooltipToObject(gameObject, "");
		}


		public void UpdateInUseState(int inUseBit = -1, bool sameMachine = false, bool isSelected = false)
		{
			bool inUse = inUseBit >= 0;
			SelectButton.SetIsSelected(isSelected);
			SelectButton.SetInteractable(!inUse || isSelected);
			string targetRecipeDesc = targetRecipe != null ? targetRecipe.description : STRINGS.UI.RFRC_NO_RECIPE;

			if (isSelected)
			{
				ToolTip.SetSimpleTooltip(targetRecipeDesc + "\n\n" + RIBBONSELECTIONSECONDARYSIDESCREEN.USETOOLTIPS.CURRENTRECIPE);
			}
			else if (inUse)
			{
				ToolTip.SetSimpleTooltip(targetRecipeDesc + "\n\n" + string.Format(sameMachine ? RIBBONSELECTIONSECONDARYSIDESCREEN.USETOOLTIPS.ALREADYINUSE : RIBBONSELECTIONSECONDARYSIDESCREEN.USETOOLTIPS.ALREADYINUSEOTHER, ++inUseBit));
			}			
			else
			{
				ToolTip.SetSimpleTooltip(targetRecipeDesc);
			}
		}

		public void UpdateUI()
		{
			if (!init)
			{
				InitUi();
			}
			//SgtLogger.l("Group Name: " + GroupName);
			if (targetRecipe != null)
			{
				RecipeIcon.sprite = targetRecipe.GetUIIcon();
				RecipeIcon.color = targetRecipe.GetUIColor();
				RecipeLabel.SetText(targetRecipe.GetUIName(false));
			}
			UpdateInUseState();
		}
	}
}
