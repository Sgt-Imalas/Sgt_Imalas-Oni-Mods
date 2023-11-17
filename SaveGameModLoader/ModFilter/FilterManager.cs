using System;
using HarmonyLib;
using TMPro;

namespace SaveGameModLoader.ModsFilter
{
	/// <summary>
	/// Original Code written by Asquared31415, reused with her permission
	/// </summary>
	public class FilterManager
	{
		private readonly KButton _clearSearchButton;
		private readonly TMP_InputField _search;

		public FilterManager(TMP_InputField search, KButton button)
		{
			if (search == null || button == null)
			{
				throw new ArgumentException("[ModsFilter] null search field or clear button!");
			}

			_search = search;
			_clearSearchButton = button;
		}

		public string Text => _search.text;

		public void ConfigureButtons(ModsScreen modsScreen)
		{
			_search.text = "";
			_search.onValueChanged.AddListener(
				_ =>
				{
					// This shouldn't ever be null, but good idea to check
					if (modsScreen != null)
					{
						Traverse.Create(modsScreen).Method("RebuildDisplay", typeof(object)).GetValue();
					}
				}
			);

			_clearSearchButton.onClick += () => _search.text = "";
			var tt = _clearSearchButton.GetComponent<ToolTip>();
			if (tt != null)
			{
				tt.toolTip = "Clear Search";
			}
		}
	}
}
