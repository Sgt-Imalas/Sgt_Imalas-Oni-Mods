using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Stinky : TraitBuilder
  {
    public override string ID => "CritterStinky";
    public override string Name => "Stinky";
    public override string Description => "Gives off a funny smell.";

    public override Group Group => Group.SmellGroup;

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, Name, Description,
        on_add: delegate (GameObject go)
        {
          go.FindOrAddUnityComponent<Components.Stinky>();
        },
        positiveTrait: false
      );
    }
  }
}
