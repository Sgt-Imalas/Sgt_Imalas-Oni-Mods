using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs.UIcmp;

namespace UtilLibs.UI.FUI
{
	public class FOrdeByParamToggle : KMonoBehaviour
	{
		[SerializeField] public string Label;
		[SerializeField] public bool StartDescending;

		[SerializeField] public Image Default, Ascending, Descending;
		[SerializeField] public FButton Toggle;

		int state = 0;
		public System.Action OnAscending, OnDescending;
		bool init = false;
		void Init()
		{
			if (init)
				return;
			init = true;

			Default = transform.Find("SortButton/IconInactive").gameObject.GetComponent<Image>();
			Ascending = transform.Find("SortButton/IconAscending").gameObject.GetComponent<Image>();
			Descending = transform.Find("SortButton/IconDescending").gameObject.GetComponent<Image>();
			Toggle = transform.Find("SortButton").gameObject.AddOrGet<FButton>();
			Toggle.ClearOnClick();
			Toggle.OnClick += OnToggleClicked;

			UpdateImages();
		}

		public override void OnPrefabInit()
		{
			Init();
			base.OnPrefabInit();

		}

		public void SetActions(System.Action sortAscended, System.Action sortDescended)
		{
			Init();
			OnAscending = sortAscended;
			OnDescending = sortDescended;
		}


		void OnToggleClicked()
		{
			state = state switch
			{
				0 => StartDescending ? 2 : 1,
				1 => 2,
				2 => 1,
				_ => 0
			};
			UpdateImages();
			if(state == 1)
				OnAscending?.Invoke();
			if(state == 2)
				OnDescending?.Invoke();
		}
		public void ActivateToggle(int stateOverride = -1)
		{
			Init();
			state = StartDescending ? 2 : 1;
			if(stateOverride>=0)
				state = stateOverride;
			UpdateImages();
		}
		public void DeactivateToggle()
		{
			Init();
			state = 0;
			UpdateImages();
		}
		void UpdateImages()
		{
			Default.gameObject.SetActive(state == 0);
			Ascending.gameObject.SetActive(state == 1);
			Descending.gameObject.SetActive(state == 2);
		}
	}
}
