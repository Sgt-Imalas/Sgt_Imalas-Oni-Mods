using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace BionicBoostersPlus
{
	public class RefinementRecipeHelper
	{
		public static IEnumerable<Element> GetCrushables(HashSet<SimHashes> exclude = null) =>
			ElementLoader.elements.Where(e => e.IsSolid && e.HasTag(GameTags.Crushable) && e.id != SimHashes.SuperInsulator && (exclude == null || !exclude.Contains(e.id)));

		public static IEnumerable<Element> GetAllOres()
		{
			var normalOres = ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.Metal));
			normalOres.RemoveAll(e => e.highTempTransition?.lowTempTransition == e);
			return normalOres;
		}
		public static IEnumerable<Tag> GetAllMetals()
		{
			var normalOres = ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.RefinedMetal));
			return normalOres.Select(e => e.tag);
		}
		public static IEnumerable<Tag> GetPlastics() => GetPlasticIds([]);
		public static IEnumerable<Tag> GetPlasticIds(SimHashes? exclude = null) => GetPlasticIds(exclude.HasValue ? [exclude.Value] : null);
		public static IEnumerable<Tag> GetPlasticIds(HashSet<SimHashes> exclude = null)
		{
			return ElementLoader.elements.FindAll(e => e.IsSolid
			&& e.HasTag(GameTags.Plastic)
			&& e.id != SimHashes.SolidViscoGel
			&& (exclude == null || !exclude.Contains(e.id)))
				.Select(e => e.tag);
		}
		public static IEnumerable<Tag> GetGlassyIds() => GetGlassyIds([]);
		public static IEnumerable<Tag> GetGlassyIds(HashSet<SimHashes> exclude = null)
		{
			return ElementLoader.elements.FindAll(e => e.IsSolid
			&& e.HasTag(MATERIALS.ALL_GLASSES)
			&& (exclude == null || !exclude.Contains(e.id)))
				.Select(e => e.tag);
		}

		public static IEnumerable<SimHashes> GetStarterMetals()
		{
			return ElementLoader.elements.FindAll(e => e.IsSolid
			&& e.HasTag(GameTags.StartingRefinedMetal)
			&& e.id != SimHashes.Iron //not considered a starting metal for my purpose
			&& e.id != SimHashes.Aluminum //way too soft material
			).Select(e => e.id);
		}
	}
}
