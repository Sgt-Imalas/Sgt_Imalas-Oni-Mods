using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	internal class MD_BlueprintEntry : IMD_Entry
	{
		public MD_BlueprintEntry(string id)
		{
			ID = id;
		}
		static StringBuilder sb = new StringBuilder();

		public string ID;
		public string FormatAsMarkdown()
		{

			Database.BuildingFacadeResource skin = Db.GetBuildingFacades().TryGet(ID);
			if (skin == null)
				return "MISSING SKIN RESOURCE: " + ID;

			sb.Clear();

			sb.AppendLine($"| ![{ID}](/assets/images/buildings/{ID}.png){{width=\"200\"}} |");
			sb.Append("|");
			sb.AppendLine(L(FindStringKey(skin.Name)));
			sb.AppendLine("|");
			sb.Append("|");
			sb.AppendLine(L(FindStringKey(skin.Description)));
			sb.AppendLine("|");

			return sb.ToString();
		}
	}
}
