using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace AllRefinedMetalsAsOres
{
	class Patches
	{

		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.FinaliseElementsTable))]
		public class ElementLoader_FinaliseElementsTable_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix(ElementLoader __instance)
			{
				var refinedMetals = ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.RefinedMetal) && !e.HasTag(GameTags.Metal));

				foreach (var refinedMetal in refinedMetals)
				{
					List<Tag> oreTags = refinedMetal.oreTags.ToList();
					oreTags.Add(GameTags.Metal);
					refinedMetal.oreTags = oreTags.ToArray();
				}
			}
		}
	}
}
