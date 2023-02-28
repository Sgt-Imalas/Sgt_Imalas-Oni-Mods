using Klei.AI;
using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Enduring : TraitBuilder
  {
    public override string ID => "CritterEnduring";
    public override string Name => "Enduring";
    public override string Description => "Lives 25% longer than usual.";

    public override Group Group => Group.LifespanGroup;

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, Name, Description,
        on_add: delegate (GameObject go)
        {
          var modifiers = go.GetComponent<Modifiers>();
          if (modifiers != null)
          {
            modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 0.25f, Description, is_multiplier: true));
          }
        },
        positiveTrait: true
      );
    }
  }
}
