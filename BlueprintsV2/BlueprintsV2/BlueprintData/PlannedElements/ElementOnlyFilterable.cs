using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements
{
	internal class ElementOnlyFilterable : Filterable
	{
		public static TagSet elementFilterableCategories = new TagSet(GameTags.MaterialCategories, GameTags.MaterialBuildingElements);
	}
}
