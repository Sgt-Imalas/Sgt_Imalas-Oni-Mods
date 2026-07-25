using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs.UIcmp;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
	internal class BuildingFilterDropdown : FMultiSelectDropdown
	{
		public class FHoverableDropDownEntry : FDropDownEntry
		{
			public System.Action OnHoverEnter, OnHoverExit;
			public FHoverableDropDownEntry(string title, System.Action<bool> onToggled, bool enabled = true, string tooltip = "", System.Action onHoverEnter = null, System.Action onHoverExit = null) : base(title, onToggled, enabled, tooltip)
			{
				OnHoverEnter = onHoverEnter;
				OnHoverExit = onHoverExit;
			}
		}


		private bool _anyFilterUnchecked = false;

		protected override GameObject InitializeToggle(FDropDownEntry entry)
		{
			var go = base.InitializeToggle(entry);
			if(entry is FHoverableDropDownEntry e)
			{
				var hover = go.AddOrGet<HoverableWithDelay>();
				hover.Init(e.OnHoverEnter, e.OnHoverExit);
			}
			return go;
		}
		public override void OnButtonClickedAddition()
		{
			base.OnButtonClickedAddition();
		}
		public override void OnToggleClickedAddition(bool toggled)
		{
			base.OnToggleClickedAddition(toggled);
			RefreshAnyFilterUnchecked();
		}
		public override void ToggleDropdownVisibility(bool on)
		{
			if (_anyFilterUnchecked)
				on = true;
			base.ToggleDropdownVisibility(on);
		}

		void RefreshAnyFilterUnchecked()
		{
			_anyFilterUnchecked = false;
			foreach (var entry in DropDownEntries)
			{
				if (entry.Toggle != null && !entry.Toggle.On)
				{
					_anyFilterUnchecked = true;
					break;
				}
			}
		}

		internal void ResetAllToggles(bool targetState)
		{
			foreach (var entry in DropDownEntries)
				if (entry.Toggle != null)
					entry.Toggle.SetOnFromCode(targetState);
			_anyFilterUnchecked = false;
		}
	}
}
