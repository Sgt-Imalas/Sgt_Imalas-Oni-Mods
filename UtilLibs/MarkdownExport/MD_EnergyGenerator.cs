using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameUtil;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_EnergyGenerator : IMD_Entry
	{
		private EnergyGenerator.Formula formula;

		public MD_EnergyGenerator(EnergyGenerator gen)
		{
			this.formula = gen.formula;
		}

		static StringBuilder sb = new StringBuilder();
		public string FormatAsMarkdown()
		{
			sb.Clear();
			sb.AppendLine($"|{L("INPUTS_HEADER")}|{L("OUTPUTS_HEADER")}|");
			sb.AppendLine("|-|-|");
			sb.Append("|");
			foreach (var input in formula.inputs)
			{
				sb.Append(MarkdownUtil.GetTagName(input.tag));
				sb.Append(" (");
				sb.Append(GameUtil.GetFormattedMass(input.consumptionRate, GameUtil.TimeSlice.PerSecond));
				sb.Append(")");
				sb.Append("<br>");
			}
			sb.Append("| ");
			foreach (var output in formula.outputs)
			{
				sb.Append(MarkdownUtil.GetTagName(output.element.CreateTag()));
				sb.Append(" (");
				sb.Append(GameUtil.GetFormattedMass(output.creationRate, GameUtil.TimeSlice.PerSecond));
				if (output.minTemperature > 0)
				{
					sb.Append(" ");
					sb.Append(string.Format(L("AT_TEMPERATURE"), GameUtil.GetTemperatureConvertedFromKelvin(output.minTemperature, TemperatureUnit.Celsius).ToString()));
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
