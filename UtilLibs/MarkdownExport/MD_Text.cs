using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.MarkdownExport
{
	public class MD_Text : IMD_Entry
	{
		string Text;
		public MD_Text(string text) => Text = text;
		public string FormatAsMarkdown()
		{
			return Text;
		}
	}
}
