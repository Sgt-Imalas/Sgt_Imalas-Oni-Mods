using ClipperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameUtil;
using static PathFinder;
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
				sb.Append(MarkdownUtil.GetFormattedMass(input.Tag, input.MassConsumptionRate, GameUtil.TimeSlice.PerSecond));
				sb.Append("<br>");
			}
			sb.Append("|");

			foreach (var output in converter.outputElements)
			{
				var temp = string.Empty;
				if (!output.useEntityTemperature)
				{
					temp = string.Format(L("AT_TEMPERATURE"), GameUtil.GetTemperatureConvertedFromKelvin(output.minOutputTemperature, TemperatureUnit.Celsius).ToString());
				}
				sb.Append(MarkdownUtil.GetFormattedMass(output.elementHash.CreateTag(), output.massGenerationRate, GameUtil.TimeSlice.PerSecond, temp));				
				sb.Append("<br>");
			}

			if (converter.TryGetComponent<OilWellCap>(out var oilWell))
			{
				var temp = string.Format(L("AT_TEMPERATURE"), GameUtil.GetTemperatureConvertedFromKelvin(oilWell.gasTemperature, TemperatureUnit.Celsius).ToString());				
				sb.Append(MarkdownUtil.GetFormattedMass(oilWell.gasElement.CreateTag(), oilWell.addGasRate, GameUtil.TimeSlice.PerSecond, temp));
				sb.Append("<br>");
			}

			sb.AppendLine("|");
			sb.AppendLine();
			return sb.ToString();
		}
	}
}
