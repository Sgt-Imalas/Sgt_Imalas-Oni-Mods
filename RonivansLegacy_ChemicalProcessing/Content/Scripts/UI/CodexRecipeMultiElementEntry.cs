using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.UI
{
	internal class CodexRecipeMultiElementEntry : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public System.Action OnHover = null;

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnHover?.Invoke();
			transform.localScale = Vector3.one * 1.25f;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			transform.localScale = Vector3.one;
		}
	}
}
