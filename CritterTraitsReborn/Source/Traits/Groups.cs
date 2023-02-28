using Klei.AI;
using System;
using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  public sealed class Group
  {
    public static readonly Group SizeGroup = new Group("SizeGroup", 0.3f);
    public static readonly Group NoiseGroup = new Group("NoiseGroup", 0.05f);
    public static readonly Group SmellGroup = new Group("SmellGroup", 0.05f, inst => !inst.HasTag(GameTags.Creatures.Swimmer));
    public static readonly Group GlowGroup = new Group("GlowGroup", 0.08f, inst => inst.GetComponent<Light2D>() == null);
    public static readonly Group SpeedGroup = new Group("SpeedGroup", 0.2f, inst => inst.GetComponent<Navigator>() != null);
    public static readonly Group LifespanGroup = new Group("LifespanGroup", 0.15f, inst => HasAmount(inst, Db.Get().Amounts.Age));
    public static readonly Group FertilityGroup = new Group("FertilityGroup", 0.1f, inst => HasAmount(inst, Db.Get().Amounts.Fertility));

    public Group(string id, float probability, Predicate<GameObject> requirement = null)
    {
      Id = id;
      Probability = probability;
      HasRequirements = requirement ?? (_ => true);
    }

    public string Id { get; private set; }
    public float Probability { get; private set; }
    public Predicate<GameObject> HasRequirements { get; private set; }

    private static bool HasAmount(GameObject go, Amount amount)
    {
      return go.GetComponent<Modifiers>()?.amounts.Has(amount) ?? false;
    }
  }
}
