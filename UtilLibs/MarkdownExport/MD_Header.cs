using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UtilLibs.MarkdownExport.MD_Localization;
using static UtilLibs.MarkdownExport.MarkdownUtil;

namespace UtilLibs.MarkdownExport
{
	public class MD_Header : IMD_Entry
	{
		public int Level = 1;
		public string NameKey;

		public MD_Header(string namekey, int level = 1)
		{
			NameKey = namekey;
			Level = level;
		}
		public string FormatAsMarkdown()
		{
			return new String('#', Level) + " "+L(NameKey);
		}
	}
}
