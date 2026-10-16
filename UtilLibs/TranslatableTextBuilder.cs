using System.Collections.Generic;

namespace UtilLibs
{
	/// <summary>
	/// used for single string mods from code
	/// </summary>
	public class TranslatableTextBuilder
	{
		Dictionary<string, string> translations = new Dictionary<string, string>();
		string defaultText = null;
		string text = string.Empty;
		public TranslatableTextBuilder(string defaultValue)
		{
			this.defaultText = defaultValue;
		}

		public TranslatableTextBuilder Add(string languagKey, string translation)
		{
			translations[languagKey] = translation;
			return this;
		}
		public string Translate()
		{
			text = defaultText;
			string languageCode = GetNormalizedLanguageCode();
			if (languageCode != Localization.DEFAULT_LANGUAGE_CODE && translations.TryGetValue(languageCode, out var translation))
				text = translation;
			return text;
		}
		static string GetNormalizedLanguageCode() => Localization.GetCurrentLanguageCode().Replace("_klei", string.Empty);
		public override string ToString() => text;
	}
}
