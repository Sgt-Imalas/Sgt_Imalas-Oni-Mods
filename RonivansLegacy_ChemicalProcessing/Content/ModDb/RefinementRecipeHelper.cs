using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public class RefinementRecipeHelper
	{
		public static HashSet<SimHashes> GetCombustableSolidsWithWood()
		{
			return [.. GetCombustibleSolids(), SimHashes.WoodLog];
		}

		public static HashSet<SimHashes> GetCombustibleSolids()
		{
			return [SimHashes.Carbon, SimHashes.Peat, SimHashes.RefinedCarbon];
		}

		public static HashSet<SimHashes> GetSpecialOres()
		{
			//those elements have special conversion rates, for all others its the same
			return [SimHashes.Electrum, SimHashes.FoolsGold, ModElements.Galena_Solid];
		}
		public static IEnumerable<Element> GetCrushables(HashSet<SimHashes> exclude = null) =>
			ElementLoader.elements.Where(e => e.IsSolid && e.HasTag(GameTags.Crushable) && e.id != SimHashes.SuperInsulator && (exclude == null || !exclude.Contains(e.id)));

		public static IEnumerable<Element> GetNormalOres()
		{
			var normalOres = ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.Metal));
			var specials = GetSpecialOres();
			normalOres.RemoveAll(e => specials.Contains(e.id) || e.HasTag(GameTags.Noncrushable) || e.HasTag(ModAssets.Tags.RandomSand));
			normalOres.RemoveAll(e => e.highTempTransition?.lowTempTransition == e);
			return normalOres;
		}
		public static IEnumerable<SimHashes> GetPlasticIds(SimHashes? exclude = null)
		{
			return ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.Plastic) && (exclude==null||!exclude.HasValue||exclude.Value != e.id)).Select(e => e.id);
		}
	}
}
