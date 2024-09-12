using System.Collections.Generic;

namespace CritterTraitsReborn
{
	public class ModAPI
	{
		static List<string> visibleTraits = new List<string>();

		public static bool TraitInApiList(string traitId) => visibleTraits.Contains(traitId);

		/// <summary>
		/// Use this method to make a critter trait shown in the "Traits" panel.
		/// </summary>
		/// <param name="trait">The trait in question</param>
		public static void AddTraitToVisibleList(Klei.AI.Trait trait) => AddTraitToVisibleList(trait.Id);
		/// <summary>
		/// Use this method to make a critter trait shown in the "Traits" panel.
		/// </summary>
		/// <param name="traitID">ID of the trait in question</param>
		public static void AddTraitToVisibleList(string traitID)
		{
			if (visibleTraits.Contains(traitID)) return;
			visibleTraits.Add(traitID);
		}
	}
}
