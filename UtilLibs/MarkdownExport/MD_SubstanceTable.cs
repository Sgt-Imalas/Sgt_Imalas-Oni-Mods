using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_SubstanceTable : IMD_Entry
	{
		HashSet<Substance> elements;
		public MD_SubstanceTable(HashSet<Substance> substances) => elements = substances;

		static StringBuilder sb = new StringBuilder();
		public string FormatAsMarkdown()
		{
			sb.Clear();
			sb.AppendLine();
			sb.AppendLine(
				$"|<font size=\"+1\">{L("STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT")}</font> | <font size=\"+1\">{L("ELEMENT_PROPERTIES")}</font> | |");
			sb.AppendLine("|:-:|:-:|:-:|");

			foreach(var substance in elements.Distinct().OrderBy(s => MarkdownUtil.GetTagName(s.elementID.CreateTag())))
			{
				var elementID = substance.elementID.ToString();
				var element = ElementLoader.GetElement(substance.elementID.CreateTag());
				sb.Append("|");
				sb.Append($" ![{elementID}](/assets/images/elements/{elementID}.png){{width = \"150\"}} ");
				sb.Append("<br/>");
				sb.Append(MarkdownUtil.GetTagName(element.id.CreateTag()));
				sb.Append("|");
				sb.Append(MarkdownUtil.GetElementTransitionProperties(element));
				sb.Append("|");
				sb.Append(MarkdownUtil.GetElementPhysicalProperties(element));
				sb.AppendLine("|");
			}

			return sb.ToString();
		}
	}
}
