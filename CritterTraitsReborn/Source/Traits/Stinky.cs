using UnityEngine;

namespace CritterTraitsReborn.Traits
{
  class Stinky : TraitBuilder
  {
    public override string ID => "CritterStinky";

    public override Group Group => Group.GetGroup(Group.SmellGroupId);

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, 
        on_add: delegate (GameObject go)
        {
          go.FindOrAddUnityComponent<Components.Stinky>();
        },
        positiveTrait: false
      );
    }
  }
}
