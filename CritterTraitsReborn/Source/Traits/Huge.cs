namespace CritterTraitsReborn.Traits
{
  class Huge : TraitBuilder
  {
    public override string ID => "CritterHuge";

    public override Group Group => Group.GetGroup(Group.SizeGroupId);

    protected override void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, 1.5f);
    }

  }
}
