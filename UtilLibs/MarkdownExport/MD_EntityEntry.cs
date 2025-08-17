using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.MISC.STATUSITEMS;
using static UtilLibs.MarkdownExport.MarkdownUtil;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_EntityEntry: IMD_Entry
	{
		string NAMEKEY, DESCKEY;
		string ID;

		public MD_EntityEntry(string id, string namekey = null, string descKey = null)
		{
			ID = id;
			NAMEKEY = namekey;
			DESCKEY = descKey;
		}
		internal static StringBuilder sb = new StringBuilder();
		public virtual string FormatAsMarkdown()
		{
			sb.Clear();
			var entity = Assets.GetPrefab(ID);
			if (entity == null) 
				throw new Exception($"Entity with the id {ID} does not exist");

			sb.AppendLine();
			sb.Append("## ");

			if (NAMEKEY.IsNullOrWhiteSpace())
				sb.AppendLine(MarkdownUtil.GetTagString(ID));
			else
				sb.AppendLine(L(NAMEKEY));

			//sb.AppendLine($"![{ID}](/assets/images/entities/{ID}.png){{width=\"200\";align=left}}");
			sb.Append($"| ![{ID}](/assets/images/entities/{ID}.png){{width=\"100\"}} |");
			if (DESCKEY.IsNullOrWhiteSpace())
				sb.Append(MarkdownUtil.GetTagString(ID,true));
			else
				sb.Append(L(DESCKEY));
			sb.AppendLine("|");

			sb.AppendLine("|-|-|");
			if(entity.TryGetComponent<PrimaryElement>(out var primaryElement))
			{
				sb.Append("|");
				sb.Append(Strip(L("STRINGS.UI.DETAILTABS.MATERIAL.NAME")));
				sb.Append("|");
				sb.Append(GetTagStringWithIcon(primaryElement.Element.tag));
				sb.AppendLine("|");
				sb.Append("|");
				sb.Append(Strip(L("STRINGS.UI.SANDBOXTOOLS.SETTINGS.MASS.NAME")));
				sb.Append("|");
				sb.Append(primaryElement.MassPerUnit);
				sb.AppendLine("|");
			}
			if(entity.TryGetComponent<DecorProvider>(out var decorProvider))
			{
				sb.Append("|");
				sb.Append(Strip(L("STRINGS.DUPLICANTS.ATTRIBUTES.DECOR.NAME")));
				sb.Append("|");
				sb.Append(Strip(string.Format(L("STRINGS.UI.BUILDINGEFFECTS.DECORPROVIDED"), "", decorProvider.baseDecor, decorProvider.baseRadius)));
				sb.AppendLine("|");				
			}
			if(entity.TryGetComponent<KPrefabID>(out var kPrefabID) && kPrefabID.tags.Any())
			{
				sb.Append("|");
				sb.Append(Strip(L("ELEMENT_PROPERTIES")));
				sb.Append("|");
				var filtered = kPrefabID.tags.Where(t => t != GameTags.HasChores);
				var tags = string.Join(", ",
						filtered
						.Select(t => MarkdownUtil.GetTagString(t))
						.StableSort()
						.Where(val => !val.Contains("MISSING")));
				sb.Append(tags);
				sb.AppendLine("|");
			}

			return sb.ToString();
		}
	}
}
