using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	internal class MD_Header : IMD_Entry
	{
		public int Level = 1;
		public string Name;

		public MD_Header(string name, int level = 1)
		{
			Name = name;
			Level = level;
		}
		public string FormatAsMarkdown()
		{
			return new String('#', Level) + " "+Name;
		}
	}
}
