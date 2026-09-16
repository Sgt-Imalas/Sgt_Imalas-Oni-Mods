using BlueprintsV2.BlueprintData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI;

namespace BlueprintsV2.BlueprintsV2.UnityUI
{
	internal class NoteVisualizerSettings : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		MultiToggle multiToggle;
		GameObject sliderContainer;
		FSlider slider;
		ToolTip multiToggleTooltip;
		public void Init(MultiToggle toggle)
		{
			if (toggle == null)
				SgtLogger.error("NoteVisualizerSettings Multitoggle is null");
			multiToggle = toggle;
			multiToggle.transform.transform.SetAsFirstSibling();
			if(multiToggle.TryGetComponent<LayoutElement>(out var le))
			{
				le.minHeight = 30;
			}
			if (multiToggle.transform.Find("FG")?.TryGetComponent<Image>(out var image) ?? false)
			{
				image.sprite = ModAssets.NoteToolIcon_Sprite;
				image.overrideSprite = ModAssets.NoteToolIcon_Sprite;
			}
			var buttonColor = UIUtils.rgb(31, 161, 255);
			multiToggle.states[2].color = buttonColor;
			multiToggle.states[2].color_on_hover = UIUtils.Lighten(buttonColor, 20);

			sliderContainer = transform.Find("SliderContainer").gameObject;
			slider = transform.Find("SliderContainer/Slider").gameObject.AddOrGet<FSlider>();
			slider.SetMinMaxCurrent(0, 1, BlueprintState.NoteOpacity);
			slider.OnChange += BlueprintState.SetNoteOpacity;
			UIUtils.AddSimpleTooltipToObject(slider.transform, NOTEOPTIONS.OPACITY_TOOLTIP);

			multiToggle.onClick += BlueprintState.ToggleNoteVisibility;

			multiToggleTooltip = UIUtils.AddSimpleTooltipToObject(multiToggle.gameObject, ButtonTT());
			BlueprintState.RefreshToggle = RefreshState;
			SetSliderVisible(false);
			RefreshState(true);
		}
		void SetSliderVisible(bool visible)
		{
			sliderContainer?.SetActive(visible);
		}
		void RefreshState() => RefreshState(false);
		void RefreshState(bool force)
		{
			multiToggle?.ChangeState(BlueprintState.NoteVisibility ? 2 : 1, force);
			multiToggleTooltip?.SetSimpleTooltip(ButtonTT());
		}
		string ButtonTT() => STRINGS.UI.ACTIONS.TOGGLENOTEVIS + " " + GameUtil.GetHotkeyString(ModAssets.Actions.BlueprintsToggleNoteVisibility.GetKAction());
		
		public void OnPointerExit(PointerEventData eventData)
		{
			SetSliderVisible(false);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			SetSliderVisible(true);
		}
	}
}
