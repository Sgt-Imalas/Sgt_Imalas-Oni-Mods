using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameUtil;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_ElementConverter : IMD_Entry
	{
		private ElementConverter converter;

		public MD_ElementConverter(ElementConverter converter)
		{
			this.converter = converter;
		}

		static StringBuilder sb = new StringBuilder();
		public string FormatAsMarkdown()
		{
			sb.Clear();
			sb.AppendLine($"|{L("INPUTS_HEADER")}|{L("OUTPUTS_HEADER")}|");
			sb.AppendLine("|-|-|");
			sb.Append("|");
			foreach (var input in converter.consumedElements)
			{
				sb.Append(MarkdownUtil.GetTagName(input.Tag));
				sb.Append(" (");
				sb.Append(GameUtil.GetFormattedMass(input.MassConsumptionRate, GameUtil.TimeSlice.PerSecond));
				sb.Append(")");
				sb.Append("<br>");
			}
			sb.Append("|");
			foreach (var output in converter.outputElements)
			{
				sb.Append(MarkdownUtil.GetTagName(output.elementHash.CreateTag()));
				sb.Append(" (");
				sb.Append(GameUtil.GetFormattedMass(output.massGenerationRate, GameUtil.TimeSlice.PerSecond));
				if (!output.useEntityTemperature)
				{
					sb.Append(" ");
					sb.Append(string.Format(L("AT_TEMPERATURE"), GameUtil.GetTemperatureConvertedFromKelvin(output.minOutputTemperature, TemperatureUnit.Celsius).ToString()));
				}
				sb.Append(")");
				sb.Append("<br>");
			}
			sb.AppendLine("|");
			sb.AppendLine();
			return sb.ToString();
		}
	}
}
