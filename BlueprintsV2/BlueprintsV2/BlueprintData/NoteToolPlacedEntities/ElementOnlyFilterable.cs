using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements
{
	internal class ElementOnlyFilterable : Filterable
	{
		static Dictionary<Tag, HashSet<Tag>> elementFilterableCategories = null;

		public static Dictionary<Tag, HashSet<Tag>> GetElementFilters()
		{
			if(elementFilterableCategories == null)
			{
				elementFilterableCategories = [];
				foreach (var element in ElementLoader.elements)
				{
					if (element.disabled)
						continue;


					Tag elementState;
					switch (element.state & Element.State.Solid)
					{
						case Element.State.Gas:
							elementState = GameTags.Gas;
							break;
						case Element.State.Liquid:
							elementState = GameTags.Liquid;
							break;
						case Element.State.Solid:
							elementState = GameTags.Solid;
							break;
						default:
						case Element.State.Vacuum:
							continue;
					}
					if(!elementFilterableCategories.ContainsKey(elementState))
					{
						elementFilterableCategories[elementState] = new HashSet<Tag>();
					}
					elementFilterableCategories[elementState].Add(element.id.CreateTag());
				}

				elementFilterableCategories.Add(SimHashes.Vacuum.CreateTag(), [SimHashes.Vacuum.CreateTag()]);
			}
			return elementFilterableCategories;
		}
	}
}
