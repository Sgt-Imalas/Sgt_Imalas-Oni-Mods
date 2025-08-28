using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	public class TranslationGroup
	{
		private string internalID;
		public Dictionary<string, string> LocalizedStrings;

		public TranslationGroup(string text)
		{
			internalID = text.GetHashCode().ToString();
			LocalizedStrings = [];
			LocalizedStrings.Add(Localization.DEFAULT_LANGUAGE_CODE, text);
			MD_Localization.Values[Localization.DEFAULT_LANGUAGE_CODE].Add(internalID, text);
		}
		public TranslationGroup Add(string code, string text)
		{
			LocalizedStrings[code] = text;
			MD_Localization.Values[code].Add(internalID,text);
			return this;
		}
		public bool TryGetTranslated(string languageKey, out string translated) => LocalizedStrings.TryGetValue(languageKey, out translated);

		public override string ToString()
		{
			return internalID.ToString();
		}
		public static implicit operator string(TranslationGroup info)
		{
			return info.ToString();
		}
	}
}
