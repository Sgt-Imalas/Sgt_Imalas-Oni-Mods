using STRINGS;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;

namespace NaturalConstruction.Content.Scripts
{
	internal class MaterialFilterBar : KMonoBehaviour
	{
		KButton ClearButton;
		TMP_InputField TextInput;
		MaterialSelector Target;

		public void Init(MaterialSelector instance)
		{
			Target = instance;
			var button = transform.Find("ClearButton").gameObject;
			ClearButton = button.GetComponent<KButton>();
			TextInput = transform.Find("LocTextInputField").gameObject.GetComponent<TMP_InputField>();

			if (button.TryGetComponent<ToolTip>(out var tt))
				tt.SetSimpleTooltip(global::STRINGS.UI.BUILDMENU.CLEAR_SEARCH_TOOLTIP);

			TextInput.onFocus = TextInput.onFocus + (()=>OnStartEdit());
			TextInput.onEndEdit.AddListener(OnEndEdit);

			ClearButton.ClearOnClick();
			ClearButton.onClick += () => TextInput.text = string.Empty;
		}
		void OnEndEdit(object _) => Target.isEditing = false;
		void OnStartEdit() => Target.isEditing = true;

		public void Clear()
		{
			TextInput.text = string.Empty;
		}
		public bool HasFilter() => TextInput.text.Any();

		static readonly Dictionary<Tag, string> CachedTagNames = [];

		public bool IsTagWithinFilters(Tag tag)
		{
			if (!HasFilter())
				return true;

			string filter = TextInput.text;
			if(StringInFilter(tag.ToString(),filter))
				return true;

			if(!CachedTagNames.TryGetValue(tag, out var name))
			{
				var prefab = Assets.TryGetPrefab(tag);
				if(prefab != null)
					CachedTagNames[tag] = name = UI.StripLinkFormatting(prefab.GetProperName());
				else
					CachedTagNames[tag] = name = tag.ToString();
			}
			return StringInFilter(name, filter);
		}

		bool StringInFilter(string text, string filter)
		{
			return CultureInfo.InvariantCulture.CompareInfo.IndexOf(
										text,
										filter,
										CompareOptions.IgnoreCase
									) >= 0;
		}
	}
}
