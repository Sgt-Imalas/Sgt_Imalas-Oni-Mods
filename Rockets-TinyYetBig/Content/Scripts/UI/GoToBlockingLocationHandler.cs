using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;
using UtilLibs.UIcmp;

namespace Rockets_TinyYetBig.Content.Scripts.UI
{
	internal class GoToBlockingLocationHandler : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
	{
		[SerializeField]
		public bool FlightMode = false;
		private bool mouseOver = false;
		public void OnPointerEnter(PointerEventData eventData)
		{
			mouseOver = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			mouseOver = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			GoToBlockingLocation();
		}

		void GoToBlockingLocation()
		{
			if (FlightMode)
				ModAssets.SelectBlocker_Flight();
			else
				ModAssets.SelectBlocker_Construction();
		}
	}
}
