using Klei.AI;
using KSerialization;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Heinermann.CritterTraits.Components
{
  /**
   * This class applies the necessary traits on object spawn, only if they haven't
   * been applied before. The state of whether the traits were applied or not is
   * saved via this class.
   */
  public class CritterTraits : KMonoBehaviour, ISaveLoadable
  {
    [SerializeField]
    [Serialize]
    private bool appliedCritterTraits = false;

        public override void OnPrefabInit()
    {
      Traits.AllTraits.InitAllTraits();
      gameObject.Subscribe((int)GameHashes.SpawnedFrom, from => transferTraits(from as GameObject));
    }

    public override void OnSpawn()
    {
      if (!appliedCritterTraits)
      {
        var traitsToAdd = Traits.AllTraits.ChooseTraits(gameObject).Select(Db.Get().traits.Get);
        addTraits(traitsToAdd);

        appliedCritterTraits = true;
      }
    }

    // Transfer critter traits owned by the `from` object to this object
    private void transferTraits(GameObject from)
    {
      var fromTraits = from.GetComponent<Klei.AI.Traits>();
      if (fromTraits == null) return;

      var traitsToAdd = fromTraits.TraitList.Where(Traits.AllTraits.IsSupportedTrait);
      addTraits(traitsToAdd);

      appliedCritterTraits = true;
    }

    // Adds the provided list of traits to this object's Traits component
    private void addTraits(IEnumerable<Trait> traitsToAdd)
    {
      var traits = gameObject.AddOrGet<Klei.AI.Traits>();
      traitsToAdd.ToList().ForEach(traits.Add);
    }
  }
}
