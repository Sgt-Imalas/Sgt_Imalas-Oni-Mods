using ClipperLib;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameUtil;
using static PathFinder;
using static STRINGS.DUPLICANTS.ATTRIBUTES;
using static UtilLibs.MarkdownExport.MarkdownUtil;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_GeyserEntry : MD_EntityEntry
	{
		string NAMEKEY, DESCKEY;
		string ID;

		public MD_GeyserEntry(string id, string namekey = null, string descKey = null) : base(id, namekey, descKey)
		{
			ID = id;
			NAMEKEY = namekey;
			DESCKEY = descKey;
		}
		public override string FormatAsMarkdown()
		{
			sb.Clear();
			var entity = Assets.GetPrefab(ID);
			if (entity == null)
				throw new Exception($"Geyser with the id {ID} does not exist");

			sb.AppendLine();
			sb.Append("## ");

			if (NAMEKEY.IsNullOrWhiteSpace())
				sb.AppendLine(MarkdownUtil.GetTagString(ID));
			else
				sb.AppendLine(L(NAMEKEY));

			sb.AppendLine();
			if (DESCKEY.IsNullOrWhiteSpace())
				sb.AppendLine(MarkdownUtil.GetTagString(ID, true));
			else
				sb.AppendLine(L(DESCKEY));

			sb.AppendLine();
			sb.AppendLine("| | | |");
			sb.AppendLine("|-|-|-|");
			sb.Append($"| ![{ID}](/assets/images/geysers/{ID}.png) {{rowspan=\"3\"}} ");

			if (entity.TryGetComponent<OccupyArea>(out var area))
			{
				sb.AppendLine($"|{L("BUILDING_DIMENSIONS_LABEL")} | {string.Format(L("BUILDING_DIMENSIONS_INFO"), area.GetWidthInCells(), area.GetHeightInCells())}|");
			}
			bool isGeyser =false;
			if (entity.TryGetComponent<GeyserConfigurator>(out var configurator))
			{
				var type = GeyserConfigurator.FindType(configurator.presetType);

				float maxRate = type.maxRatePerCycle/1000;
				float minRate = type.minRatePerCycle / 1000;
				float avgRate = (maxRate+minRate) / 2;

				string avg = MarkdownUtil.GetFormattedMass(avgRate, TimeSlice.PerSecond);
				string min = MarkdownUtil.GetFormattedMass(minRate, TimeSlice.PerSecond);
				string max = MarkdownUtil.GetFormattedMass(maxRate, TimeSlice.PerSecond);

				string temp = string.Format(L("AT_TEMPERATURE"), GameUtil.GetTemperatureConvertedFromKelvin(type.temperature, TemperatureUnit.Celsius).ToString());

				sb.AppendLine($"|{L("STRINGS.MISC.STATUSITEMS.SPOUTEMITTING.NAME").Replace("{StudiedDetails}",string.Empty)} | {MarkdownUtil.GetTagStringWithIcon(type.element.CreateTag()) +" "+ temp}|{EmptyTableCell} |");
				sb.AppendLine($"|{L("STRINGS.UI.BUILDINGEFFECTS.GEYSER_YEAR_AVR_OUTPUT")} | {avg} ({min} - {max})|{EmptyTableCell} |");

				isGeyser = true;
			}
			if(entity.TryGetComponent<DecorProvider>(out var decorProvider) && !isGeyser)
			{
				sb.Append("|");
				sb.Append(Strip(L("STRINGS.DUPLICANTS.ATTRIBUTES.DECOR.NAME")));
				sb.Append("|");
				string decor = decorProvider.baseDecor > 0 ? "+" + decorProvider.baseDecor: decorProvider.baseDecor.ToString() ;
				sb.Append(Strip(string.Format(L("STRINGS.UI.BUILDINGEFFECTS.DECORPROVIDED"),"", decor, decorProvider.baseRadius)));
				sb.AppendLine($"|{EmptyTableCell}|");				
			}


			return sb.ToString();
		}
	}
}
