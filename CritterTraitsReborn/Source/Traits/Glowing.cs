using UnityEngine;

namespace Heinermann.CritterTraits.Traits
{
  class Glowing : TraitBuilder
  {
    public override string ID => "CritterGlowing";
    public override string Name => "Bioluminescent";
    public override string Description => "Gives off a faint, steady glow.";

    public override Group Group => Group.GlowGroup;

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID, Name, Description,
        on_add: delegate (GameObject go)
        {
          CritterUtil.AddObjectLight(go, Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 0.8f), 2f, 600);
          //go.AddOrGetDef<CreatureLightToggleController.Def>();
        },
        positiveTrait: true
      );
    }
  }
}
