using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_Text : IMD_Entry
	{
		string TextKey;
		public MD_Text(string textKey) => TextKey = textKey;
		public string FormatAsMarkdown()
		{
			return L(TextKey);
		}
	}
}
