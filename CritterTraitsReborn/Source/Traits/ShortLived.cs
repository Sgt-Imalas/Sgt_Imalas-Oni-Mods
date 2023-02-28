using Klei.AI;
using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class ShortLived : TraitBuilder
  {
    public override string ID => "CritterShortLived";
    public override string Name => "Short-lived";
    public override string Description => "Has a 20% shorter lifespan.";

    public override Group Group => Group.LifespanGroup;

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, Name, Description,
        on_add: delegate (GameObject go)
        {
          var modifiers = go.GetComponent<Modifiers>();
          if (modifiers != null)
          {
            modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, -0.20f, Description, is_multiplier: true));
          }
        },
        positiveTrait: false
      );
    }
  }
}
