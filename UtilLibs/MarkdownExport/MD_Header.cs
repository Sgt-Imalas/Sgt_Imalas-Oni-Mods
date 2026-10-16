using System;
using static UtilLibs.MarkdownExport.MD_Localization;

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
