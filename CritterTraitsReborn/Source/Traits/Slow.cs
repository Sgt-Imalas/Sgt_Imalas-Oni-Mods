using UnityEngine;

namespace CritterTraitsReborn.Traits
{
  class Slow : TraitBuilder
  {
    public override string ID => "CritterSlow";

    public override Group Group => Group.GetGroup(Group.SpeedGroupId);

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, 
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
