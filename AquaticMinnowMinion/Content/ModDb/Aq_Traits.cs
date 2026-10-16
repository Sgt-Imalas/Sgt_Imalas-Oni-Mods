using TUNING;
using UnityEngine;
using static TUNING.DUPLICANTSTATS;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Traits
	{
		public static readonly string Aquatic_Freediver = "Aquatic_Minnow";

		public static void Register(Db __instance)
		{
			DUPLICANTSTATS.CONGENITALTRAITS.Add(new TraitVal { id = Aquatic_Freediver});
			var trait = __instance.CreateTrait(Aquatic_Freediver, global::STRINGS.DUPLICANTS.CONGENITALTRAITS.MINNOW.NAME, global::STRINGS.DUPLICANTS.CONGENITALTRAITS.MINNOW.DESC, null, true, null, true, true);
			trait.OnAddTrait = AddAquaticMinnowTraitEffects;
		}
		public static void AddAquaticMinnowTraitEffects(GameObject go)
		{
			go.AddOrGet<MinnowSwimmer>();
		}
	}
}
