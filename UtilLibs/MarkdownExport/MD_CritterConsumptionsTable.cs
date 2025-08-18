using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GameUtil;
using static UtilLibs.MarkdownExport.MD_Localization;


namespace UtilLibs.MarkdownExport
{
	public class MD_CritterConsumptionsTable : IMD_Entry
	{
		private List<Tuple<Tag, float, Tag, Tag>> infos;

		public MD_CritterConsumptionsTable(List<Tuple<Tag, float, Tag, Tag>> _infos)
		{
			infos = _infos;
		}

		static StringBuilder sb = new StringBuilder();
		public string FormatAsMarkdown()
		{

			infos = infos
				.OrderBy(info => MarkdownUtil.GetTagString(info.Item4))
				.ThenBy(info => MarkdownUtil.GetTagString(info.Item1))
				.ToList();

			sb.Clear();
			sb.AppendLine();
			sb.AppendLine($"|{L("INPUTS_HEADER")}|{L("STRINGS.BUILDING.STATUSITEMS.CRITTERCAPACITY.UNIT")}|{L("OUTPUTS_HEADER")}|");
			sb.AppendLine("|-|-|-|");
			foreach(var info in infos)
			{
				var critterTag = info.Item4;

				var input = info.Item1;
				var output = info.Item3;
				float conversionRate = info.Item2;

				//string inputAmount = 100+ L("STRINGS.UI.UNITSUFFIXES.PERCENT")+" ";
				string outputAmount = Mathf.RoundToInt(conversionRate * 100f).ToString() + L("STRINGS.UI.UNITSUFFIXES.PERCENT")+" ";
				//sb.Append(inputAmount);
				sb.Append(MarkdownUtil.GetTagStringWithIcon(input));
				sb.Append(" | ");
				sb.Append(MarkdownUtil.GetTagStringWithIcon(critterTag));
				sb.Append(" | ");
				sb.Append(outputAmount);
				sb.Append(MarkdownUtil.GetTagStringWithIcon(output));
				sb.AppendLine("|");
			}
			return sb.ToString();
		}
	}
}
