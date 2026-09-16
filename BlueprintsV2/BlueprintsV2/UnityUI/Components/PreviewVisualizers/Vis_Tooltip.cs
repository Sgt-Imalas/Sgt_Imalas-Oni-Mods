using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers
{
	internal class Vis_Tooltip : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public bool spawned = false;
		string cachedTitle = string.Empty;
		string cachedTooltip = string.Empty;
		LocText TitleText, LabelText;
		GameObject TooltipContainer;
		bool anyTextToDisplay;

		bool init = false;
		bool active = false;
		void Init()
		{
			if (init)
				return;
			TooltipContainer = transform.Find("HoverTooltip").gameObject;
			TitleText = transform.Find("HoverTooltip/Title").gameObject.GetComponent<LocText>();
			TitleText.key = string.Empty;
			TitleText.text = string.Empty;
			LabelText = transform.Find("HoverTooltip/Label").gameObject.GetComponent<LocText>(); 
			LabelText.key = string.Empty;
			LabelText.text = string.Empty;
			init = TooltipContainer != null;
		}
		void ApplyPending()
		{
			if (cachedTitle != null && cachedTitle.Any())
				TitleText.text = cachedTitle;
			cachedTitle = null;

			if (cachedTooltip != null && cachedTooltip.Any())
				LabelText.text = cachedTooltip;
			cachedTooltip = null;
		}

		public override void OnPrefabInit()
		{
			Init();
			base.OnPrefabInit();
		}
		public override void OnSpawn()
		{
			Init(); 
			spawned = true;
			ApplyPending();
			base.OnSpawn();
		}
		public void SetText(string title, string text)
		{
			Init();
			anyTextToDisplay = title.Trim().Any() || text.Trim().Any();
			if (!spawned || !active)
			{
				cachedTitle = title;
				cachedTooltip = text;
			}
			else
			{
				TitleText.text = title;
				TitleText.gameObject.SetActive(title.Any());
				LabelText.text = text;
				LabelText.gameObject.SetActive(text.Any());
			}
			RefreshVisibility(false);
		}
		void RefreshVisibility(bool show)
		{
			bool showing = show && anyTextToDisplay;
			TooltipContainer.gameObject.SetActive(showing);
			if (showing)
				ApplyPending();

		}

		public void OnPointerExit(PointerEventData eventData)
		{
			active = false;
			RefreshVisibility(false);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			active = true;
			RefreshVisibility(true);
		}
	}
}
