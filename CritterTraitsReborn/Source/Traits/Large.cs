namespace Heinermann.CritterTraits.Traits
{
  class Large : TraitBuilder
  {
    public override string ID => "CritterLarge";
    public override string Name => "Large";
    public override string Description => "Is 25% larger than average.";

    public override Group Group => Group.SizeGroup;

    protected override void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, Name, Description, 1.25f);
    }
  }
}
