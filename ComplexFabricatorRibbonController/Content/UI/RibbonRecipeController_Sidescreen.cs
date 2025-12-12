using ComplexFabricatorRibbonController.Content.Scripts.Buildings;
using ComplexFabricatorRibbonController.Content.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;

namespace ComplexFabricatorRibbonController.Content.UI
{
	class RibbonRecipeController_Sidescreen : SideScreenContent, IRender200ms
	{
		ComplexFabricatorRecipeControlAttachment TargetComponent;

		BitButton ButtonPrefab;
		Dictionary<int, BitButton> Buttons = new();
		GameObject NotAttachedWarning;


		public static RibbonRecipeController_SecondarySidescreen SecondarySideScreen;
		public override string GetTitle() => STRINGS.UI.RIBBONSELECTIONSIDESCREEN.TITLE;

		public override bool IsValidForTarget(GameObject target)
		{
			return target.TryGetComponent<ComplexFabricatorRecipeControlAttachment>(out var carrier);
		}
		public override void ClearTarget()
		{
			TargetComponent = null;
			base.ClearTarget();
			ClearSecondarySideScreen();
		}
		public override void SetTarget(GameObject target)
		{
			base.SetTarget(target);
			if (target.TryGetComponent<ComplexFabricatorRecipeControlAttachment>(out var carrier))
				TargetComponent = carrier;
			Refresh();
		}
		void EnableSecondarySideScreen(bool enable, int bit)
		{
			if (SecondarySideScreen == null || enable)
			{
				SecondarySideScreen = (RibbonRecipeController_SecondarySidescreen)DetailsScreen.Instance.SetSecondarySideScreen(ModAssets.RecipeSelectionSecondarySidescreen, string.Format(STRINGS.UI.LISTSELECTION_SECONDARYSIDESCREEN.TITLE,bit+1));
				SecondarySideScreen.OnConfirm = recipe => OnRecipeSelected(recipe, bit);
				SecondarySideScreen.SetOpenedFrom(TargetComponent,bit);
			}
			else
				ClearSecondarySideScreen();
		}

		void OnRecipeSelected(ComplexRecipe recipe, int bit)
		{
			TargetComponent.SetRecipe(bit, recipe);
			ClearSecondarySideScreen();
			Refresh();
		}

		void ClearSecondarySideScreen()
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
		}

		bool init = false;
		void Init()
		{
			if (init)
				return;
			init = true;
			ButtonPrefab = transform.Find("Content/Button").gameObject.AddOrGet<BitButton>();
			ButtonPrefab.gameObject.SetActive(false);
			var parent = transform.Find("Content").gameObject;
			NotAttachedWarning = transform.Find("Warning").gameObject;
			UIUtils.AddSimpleTooltipToObject(NotAttachedWarning, STRINGS.UI.RIBBONSELECTIONSIDESCREEN.WARNING.TOOLTIP);

			for (int i = 0; i < 4; i++)
			{
				var button = Util.KInstantiateUI<BitButton>(ButtonPrefab.gameObject, parent, false);
				button.targetBit = i;
				button.OpenSelectionScreen = (bit) =>
				{
					EnableSecondarySideScreen(true, bit);
					SetButtonsInteractable(false);
				};
				button.gameObject.SetActive(true);
				Buttons.Add(i, button);
			}
		}
		public void Refresh()
		{
			NotAttachedWarning?.SetActive(!TargetComponent?.HasParentFab ?? false);
			SetButtonsInteractable(TargetComponent?.HasParentFab ?? false);
			RefreshRibbonVis();
		}
		void RefreshRibbonVis()
		{
			for (int i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].UpdateUI(TargetComponent.GetRecipe(i), TargetComponent.GetBitState(i));
			}
		}

		public void SetButtonsInteractable(bool interactable)
		{
			foreach (var button in Buttons)
			{
				button.Value.SetInteractable(interactable);
			}
		}

		public override void OnShow(bool show)
		{
			base.OnShow(show);
			Init();

			if (!show)
			{
				ClearSecondarySideScreen();
			}
			else
			{
				Refresh();
			}
		}

		public void Render200ms(float dt)
		{
			if (TargetComponent != null)
				RefreshRibbonVis();
		}
	}
}
