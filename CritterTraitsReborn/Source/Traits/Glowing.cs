using UnityEngine;

namespace CritterTraitsReborn.Traits
{
  class Glowing : TraitBuilder
  {
    public override string ID => "CritterGlowing";

    public override Group Group => Group.GetGroup(Group.GlowGroupId);

    protected override void Init()
    {
      TraitHelpers.CreateTrait(ID,
        on_add: delegate (GameObject go)
        {
          CritterUtil.AddObjectLight(go, Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 0.8f), 2f, 600);
        },
        positiveTrait: true
      );
    }
  }
}
