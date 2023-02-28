using Klei.AI;
using System;
using UnityEngine;

namespace Heinermann.CritterTraits
{
  public static class TraitHelpers
  {
    public static void CreateTrait(string id, string name, string desc, Action<GameObject> on_add, bool positiveTrait)
    {
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

    /**
     * Creates a trait that modifies the object's scale (including max health and calories).
     */
    public static void CreateScaleTrait(string id, string name, string desc, float scale)
    {
      CreateTrait(id, name, desc,
        on_add: delegate (GameObject go)
        {
          CritterUtil.SetObjectScale(go, scale, desc);
        },
        positiveTrait: scale >= 1.0f
      );
    }
  }
}
