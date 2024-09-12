using Klei.AI;
using System;
using UnityEngine;

namespace CritterTraitsReborn
{
	public static class TraitHelpers
	{
		public static void CreateTrait(string id, Action<GameObject> on_add, bool positiveTrait)
		{
			GetCritterTraitStrings(id, out var name, out var desc);
			Trait trait = Db.Get().CreateTrait(
			  id: id,
			  name: name,
			  description: desc,
			  group_name: null,
			  should_save: true,
			  disabled_chore_groups: null,
			  positive_trait: positiveTrait,
			  is_valid_starter_trait: false);

			trait.OnAddTrait = on_add;
		}
		public static void GetCritterTraitStrings(string id, out string name, out string description)
		{
			name = Strings.Get("STRINGS.CRITTERTRAITS." + id.ToUpper() + ".NAME");
			GetCritterTraitDesc(id, out description);
		}
		public static void GetCritterTraitDesc(string id, out string description)
		{
			description = Strings.Get("STRINGS.CRITTERTRAITS." + id.ToUpper() + ".DESC");
		}


		/**
         * Creates a trait that modifies the object's scale (including max health and calories).
         */
		public static void CreateScaleTrait(string id, float scale)
		{
			GetCritterTraitDesc(id, out var desc);

			CreateTrait(id,
			  on_add: delegate (GameObject go)
			  {
				  CritterUtil.SetObjectScale(go, scale, desc);
			  },
			  positiveTrait: scale >= 1.0f
			);
		}
	}
}
