using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Fast : TraitBuilder
  {
    public override string ID => "CritterFast";
    public override string Name => "Fast";
    public override string Description => "Is twice as fast as its peers.";

    public override Group Group => Group.SpeedGroup;

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, Name, Description,
        on_add: delegate (GameObject go)
        {
          var navigator = go.GetComponent<Navigator>();
          if (navigator != null)
          {
            navigator.defaultSpeed *= 2f;
          }
        },
        positiveTrait: true
      );
    }
  }
}
