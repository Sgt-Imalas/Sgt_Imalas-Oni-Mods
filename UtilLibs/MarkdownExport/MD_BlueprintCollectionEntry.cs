using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_BlueprintCollectionEntry : IMD_Entry
	{
		public MD_BlueprintCollectionEntry(string buildingId)
		{
			BuildingID = buildingId;
		}
		static StringBuilder sb = new StringBuilder();

		public string BuildingID;
		public string FormatAsMarkdown()
		{
			sb.Clear();

			sb.Append("## ");
			sb.AppendLine(MarkdownUtil.StrippedBuildingName(BuildingID));

			if(!SupplyClosetUtils.TryGetCollectionFor(BuildingID, out var blueprints)) 
				throw new Exception("no skins for "+BuildingID+" registered");

			sb.AppendLine();
			sb.Append("|");
			sb.Append(L("STRINGS.UI.UISIDESCREENS.TABS.SKIN"));
			sb.Append("|");
			sb.Append(L("STRINGS.UI.VITALSSCREEN_NAME"));
			sb.AppendLine("|");
			sb.AppendLine("|-|-|");

			foreach (var blueprint in blueprints)
			{
				Database.BuildingFacadeResource skin = Db.GetBuildingFacades().TryGet(blueprint);
				if (skin == null)
					throw new Exception("MISSING SKIN RESOURCE: " + blueprint);

				sb.Append("|");
				sb.Append($"![{blueprint}](/assets/images/buildings/{blueprint}.png){{width=\"200\"}} ");
				sb.Append("|");
				sb.Append("**<font size=\"+1\">");
				sb.Append(L(FindStringKey(skin.Name)));
				sb.Append("</font>**");
				sb.Append("<br/>");
				sb.Append("<br/>");
				sb.Append(L(FindStringKey(skin.Description)));
				sb.AppendLine("|");

			}
			return sb.ToString();
		}
	}
}
