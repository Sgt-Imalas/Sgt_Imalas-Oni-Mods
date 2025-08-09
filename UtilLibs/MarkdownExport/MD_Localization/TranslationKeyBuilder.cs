using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	public class TranslationKeyBuilder
	{
		private string internalID;

		public TranslationKeyBuilder(string formatKey, List<string> formatValueKeys)
		{

			internalID = formatKey.GetHashCode().ToString();
			foreach (var key in formatValueKeys)
				internalID += key.GetHashCode();

			MD_Localization.Add(internalID, formatKey, formatValueKeys);
		}
		public override string ToString()
		{
			return internalID.ToString();
		}
		public static implicit operator string(TranslationKeyBuilder info)
		{
			return info.ToString();
		}
	}
}
