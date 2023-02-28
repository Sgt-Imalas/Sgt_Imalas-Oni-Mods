using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Noisy : TraitBuilder
  {
    public override string ID => "CritterNoisy";
    public override string Name => "Noisy";
    public override string Description => "Makes a lot of noise when it moves.";

    public override Group Group => Group.NoiseGroup;

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, Name, Description,
        on_add: delegate (GameObject go)
        {
          go.AddOrGet<Components.Noisy>();
        },
        positiveTrait: false
      );
    }
  }
}
