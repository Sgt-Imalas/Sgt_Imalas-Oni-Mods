using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.ElementUtilNamespace
{
	public class ElementGrouping
	{
		public Tag GroupTag { get; private set; }
		public List<Element> GroupedElements { get; private set; }

		public ElementGrouping Exclude(Func<Element,bool> predicate)
		{
			GroupedElements.RemoveAll(e => predicate(e));
			return this;
		}
		public ElementGrouping Include(Func<Element, bool> predicate, bool addGroupTagToElement = true)
		{
			var elementsToAdd = ElementLoader.elements.Where(predicate).ToList();
			foreach (var element in elementsToAdd)
			{
				if (!GroupedElements.Contains(element))
				{
					if(addGroupTagToElement && !element.oreTags.Contains(GroupTag))
						element.oreTags = element.oreTags.Append(GroupTag);
					GroupedElements.Add(element);
				}
			}
			return this;
		}
		public static ElementGrouping GroupAllWith(Tag groupTag)
		{
			ElementGrouping grouping = new ElementGrouping();
			grouping.GroupTag = groupTag;
			grouping.GroupedElements = ElementLoader.elements.Where(e => e.HasTag(groupTag)).ToList();
			return grouping;
		}

		public static implicit operator Tag(ElementGrouping info) //single Tag filter
		{
			return info.GroupTag;
		}
		public static implicit operator List<Tag>(ElementGrouping info) //recipeingredient
		{
			return info.GroupedElements.Select(e => e.tag).ToList();
		}
		public static implicit operator Tag[](ElementGrouping info) //recipeingredient
		{
			return info.GroupedElements.Select(e => e.tag).ToArray();
		}
		public static implicit operator string(ElementGrouping info) //buildmenu
		{
			return string.Join("&",info.GroupedElements.Select(t=>t.tag.ToString()));
		}
	}
}
