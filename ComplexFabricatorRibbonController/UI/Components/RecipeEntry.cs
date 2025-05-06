using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.UIcmp;
using UtilLibs;
using UnityEngine.UI;

namespace ComplexFabricatorRibbonController.UI.Components
{
    class RecipeEntry : KMonoBehaviour
	{
		public ComplexRecipe targetRecipe;
		public LocText RecipeLabel;
		public Image RecipeIcon;
		FButton SelectButton;
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
			SelectButton = gameObject.AddOrGet<FButton>();
			SelectButton.OnClick += OnEntryClicked;
		}
		public void UpdateUI()
		{
			if (!init)
			{
				InitUi();
			}
			//SgtLogger.l("Group Name: " + GroupName);
			if(targetRecipe != null)
			{
				RecipeIcon.sprite = targetRecipe.GetUIIcon();
				RecipeIcon.color = targetRecipe.GetUIColor();
				RecipeLabel.SetText(targetRecipe.GetUIName(false));
			}
		}
	}
}
