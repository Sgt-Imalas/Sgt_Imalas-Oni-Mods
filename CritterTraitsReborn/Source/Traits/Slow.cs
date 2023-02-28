using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Slow : TraitBuilder
  {
    public override string ID => "CritterSlow";
    public override string Name => "Slow";
    public override string Description => "Moves at half the speed.";

    public override Group Group => Group.SpeedGroup;

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, Name, Description,
        on_add: delegate (GameObject go)
        {
          var navigator = go.GetComponent<Navigator>();
          if (navigator != null)
          {
            navigator.defaultSpeed /= 2f;
          }
        },
        positiveTrait: false
      );
    }
  }
}
