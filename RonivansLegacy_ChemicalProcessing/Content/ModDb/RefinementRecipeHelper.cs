using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public class RefinementRecipeHelper
    {
        public static HashSet<SimHashes> GetSpecialOres()
		{
			//those elements have special conversion rates, for all others its the same
			return [SimHashes.Electrum, SimHashes.FoolsGold, ModElements.Galena_Solid];
		}
		public static IEnumerable<Element> GetCrushables() => ElementLoader.elements.Where(e => e.IsSolid && e.HasTag(GameTags.Crushable));


	}
}
