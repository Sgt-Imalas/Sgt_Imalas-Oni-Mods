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
		HashSet<Element> elements;
		public MD_SubstanceTable(HashSet<Substance> substances) => elements = substances.Select(s => ElementLoader.FindElementByHash(s.elementID)).ToHashSet();
		public MD_SubstanceTable(HashSet<SimHashes> substances) => elements = substances.Select(s => ElementLoader.FindElementByHash(s)).ToHashSet();
		public MD_SubstanceTable(HashSet<Element> substances) => elements = substances;

		static StringBuilder sb = new StringBuilder();

		public MD_SubstanceTable Add(SimHashes ele) { elements.Add(ElementLoader.FindElementByHash(ele)); return this; }
		public MD_SubstanceTable AddEnabled(SimHashes ele) { elements.Add(ElementLoader.FindElementByHash(ele)); return this; }


		public string FormatAsMarkdown()
		{
			sb.Clear();
		//	sb.AppendLine(
			//	$"|<font size=\"+1\">{L("STRINGS.UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT")}</font> | <font size=\"+1\">{L("ELEMENT_PROPERTIES")}</font> | |");
			//sb.AppendLine("|:-:|:-:|:-:|");

			foreach(var substance in elements.Distinct().OrderBy(s => MarkdownUtil.GetTagString(s.id.CreateTag())))
			{
				var elementID = substance.id.ToString();
				var element = ElementLoader.GetElement(substance.tag);
				sb.AppendLine();
				sb.Append("#### ");
				sb.AppendLine(MarkdownUtil.GetTagString(element.id.CreateTag()));
				sb.AppendLine();
				sb.AppendLine(FormatLineBreaks(MarkdownUtil.GetTagString(element.id.CreateTag(), true)));
				sb.AppendLine();

				sb.AppendLine($"| |<font size=\"+1\">{L("ELEMENT_PROPERTIES")}</font>| |");
				sb.AppendLine($"|-|-|-|");
				sb.Append($"| ![{elementID}](/assets/images/elements/{elementID}.png){{width=\"200\"}} |");
				sb.Append(MarkdownUtil.GetElementTransitionProperties(element));
				sb.Append("|");
				sb.Append(MarkdownUtil.GetElementPhysicalProperties(element));
				sb.AppendLine("|");
			}

			return sb.ToString();
		}
	}
}
