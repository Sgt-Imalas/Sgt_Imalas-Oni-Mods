using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
	internal class HoverableWithDelay : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public float delayS = 0.20f;
		float timeSincePointerEnter = 0;
		public System.Action OnPointerEnterAction, OnPointerExitAction;

		public override void OnCmpEnable()
		{
			base.OnCmpEnable();
		}
		public override void OnCmpDisable()
		{
			base.OnCmpDisable();
			StopDelayedHoverCoroutine();
		}

		Coroutine HoverCoroutine = null;

		public void OnPointerExit(PointerEventData eventData)
		{
			StopDelayedHoverCoroutine();
			if (OnPointerExitAction != null)
				OnPointerExitAction();
		}
		IEnumerator DelayedHoverTrigger()
		{
			while (timeSincePointerEnter < delayS)
			{
				timeSincePointerEnter += Time.unscaledDeltaTime;
				yield return null;
			}
			if (OnPointerEnterAction != null)
				OnPointerEnterAction();
		}

		void StopDelayedHoverCoroutine()
		{
			if (HoverCoroutine != null)
			{
				StopCoroutine(HoverCoroutine);
				HoverCoroutine = null;
			}
		}
		public void OnPointerEnter(PointerEventData eventData)
		{
			timeSincePointerEnter = 0;
			HoverCoroutine = StartCoroutine(DelayedHoverTrigger());
		}

		internal void Init(System.Action onHoverEnter, System.Action onHoverExit)
		{
			OnPointerEnterAction = onHoverEnter;
			OnPointerExitAction = onHoverExit;
		}
	}
}
