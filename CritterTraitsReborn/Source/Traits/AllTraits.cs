using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CritterTraitsReborn.Traits
{
  public static class AllTraits
  {
    private static readonly TraitBuilder[] traits = {
      new Enduring(),
      new Fast(),
      new Fertile(),
      new Glowing(),
      DlcManager.IsContentActive("EXPANSION1_ID") ? new Rad() : null,
      new Huge(),
      new Large(),
      new Noisy(),
      new ShortLived(),
      new Slow(),
      new Small(),
      new Stinky(),
      new Tiny()
    };

    private static readonly ILookup<string, TraitBuilder> traitLookup = traits.ToLookup(trait => trait.ID);

    private static bool traitsInitialized = false;

    /**
     * Initializes and registers all traits for use by the game if it has not already been done.
     */
    public static void InitAllTraits()
    {
      if (traitsInitialized == true) return;

      Array.ForEach(traits, trait => trait.CreateTrait());
      traitsInitialized = true;
    }

    /**
     * Chooses a random set of traits between 0 and 4. (max is capped randomly between 2 and 4)
     * Only applied traits that are relevant, i.e. Glowing trait on Shine Bug doesn't make sense.
     */
    public static List<string> ChooseTraits(GameObject inst)
    {
      var result = new List<string>();
      int numTraitsToChoose = UnityEngine.Random.Range(2, 4);

      var groups = traits.GroupBy(trait => trait.Group);
      foreach (var group in groups)
      {
        if (group.Key.HasRequirements(inst))
        {
          result.Add(ChooseTraitFrom(group));
        }
      }

      // If there are more traits than asked for we don't want to bias to the ones that were chosen first
      return result
        .Where(s => s != null)
        .OrderBy(x => Util.GaussianRandom())
        .Take(numTraitsToChoose)
        .ToList();
    }

    /**
     * Chooses a trait from the given list with a probability. If the probability check fails it returns null.
     */
    private static string ChooseTraitFrom(IGrouping<Group, TraitBuilder> group)
    {
      float prob = UnityEngine.Random.Range(0f, 1f);
      if (prob <= group.Key.Probability)
      {
        return Util.GetRandom(group.ToList()).ID;
      }
      return null;
    }

    /**
     * Checks if the trait is explicitly supported by this mod.
     */
    public static bool IsSupportedTrait(string traitId) => traitLookup.Contains(traitId);
    public static bool IsSupportedTrait(Klei.AI.Trait trait) => IsSupportedTrait(trait.Id);
  }
}
