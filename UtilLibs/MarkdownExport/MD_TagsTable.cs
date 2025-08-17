using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UtilLibs.MarkdownExport.MD_Localization;

namespace UtilLibs.MarkdownExport
{
	public class MD_TagsTable : IMD_Entry
	{
		HashSet<Tag> elementTags;
		public MD_TagsTable(HashSet<Tag> substances) => elementTags = substances;

		static StringBuilder sb = new StringBuilder();

		static List<Tag> GetValidMaterialsFor(Tag materialTag)
		{
			var validMaterials = new List<Tag>();
			foreach (Element element in ElementLoader.elements)
			{
				if (element.tag == materialTag || element.HasTag(materialTag))
				{
					validMaterials.Add(element.tag);
				}
			}

			foreach (Tag materialBuildingElement in GameTags.MaterialBuildingElements)
			{
				if (materialBuildingElement != materialTag)
				{
					continue;
				}

				foreach (GameObject item in Assets.GetPrefabsWithTag(materialBuildingElement))
				{
					KPrefabID component = item.GetComponent<KPrefabID>();
					if (component != null && !validMaterials.Contains(component.PrefabTag))
					{
						validMaterials.Add(component.PrefabTag);
					}
				}
			}
			return validMaterials;
		}


		public string FormatAsMarkdown()
		{
			sb.Clear();
			sb.AppendLine();
			sb.AppendLine(
				$"|<font size=\"+1\">{L("STRINGS.UI.UISIDESCREENS.TABS.MATERIAL")}</font> | <font size=\"+1\">{L("STRINGS.UI.CODEX.CATEGORYNAMES.ELEMENTS")}</font> | |");
			sb.AppendLine("|:-:|:-:|:-|");

			foreach (var tag in elementTags.Distinct().OrderBy(t => MarkdownUtil.GetTagString(t)))
			{
				sb.Append("|");
				sb.Append("<font size=\"+1\">");
				sb.Append(MarkdownUtil.GetTagString(tag));
				sb.Append("</font> <br/> <br/>");
				sb.Append("|");
				sb.Append(MarkdownUtil.GetTagString(tag,true));
				sb.Append("|");
				foreach (var element in GetValidMaterialsFor(tag))
				{
					sb.Append($" ![{element}](/assets/images/elements/{element}.png){{width=\"20\"}} {MarkdownUtil.GetTagString(element)}<br/>");
				}
				sb.AppendLine("|");
			}

			return sb.ToString();
		}
	}
}
