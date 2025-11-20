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
			return [.. GetCombustibleSolids(), .. GetWoods(), ModElements.BioMass_Solid];
		}

		public static HashSet<SimHashes> GetWoodsWithBiomass() => [.. GetWoods(), ModElements.BioMass_Solid];

		public static HashSet<SimHashes> GetWoods()
		{
			return ElementLoader.elements.Where(e => e.IsSolid && e.HasTag(GameTags.BuildingWood)).Select(e => e.id).ToHashSet();
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
		public static IEnumerable<SimHashes> GetPlasticIds() => GetPlasticIds([]);
		public static IEnumerable<SimHashes> GetPlasticIds(SimHashes? exclude = null) => GetPlasticIds(exclude.HasValue ? [exclude.Value] : null);
		public static IEnumerable<SimHashes> GetPlasticIds(HashSet<SimHashes> exclude = null)
		{
			return ElementLoader.elements.FindAll(e => e.IsSolid
			&& e.HasTag(GameTags.Plastic)
			&& e.id != SimHashes.SolidViscoGel
			&& (exclude == null || !exclude.Contains(e.id)))
				.Select(e => e.id);
		}

		public static IEnumerable<SimHashes> GetStarterMetals()
		{
			return ElementLoader.elements.FindAll(e => e.IsSolid
			&& e.HasTag(GameTags.StartingRefinedMetal)
			&& e.id != SimHashes.Iron //not considered a starting metal for my purpose
			&& e.id != SimHashes.Aluminum //way too soft material
			&& e.id != ModElements.Silver_Solid
			).Select(e => e.id);
		}

		public static IEnumerable<Tag> GetSteelLikes()
		{
			return [
				SimHashes.Steel.CreateTag(),
				ModElements.StainlessSteel_Solid.Tag,
				ModElements.Permendur_Solid.Tag,
				ModElements.Invar_Solid.Tag,
				ModElements.Brass_Solid.Tag
				];
		}
	}
}
