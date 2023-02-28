namespace Heinermann.CritterTraits.Traits
{
  class Small : TraitBuilder
  {
    public override string ID => "CritterSmall";
    public override string Name => "Small";
    public override string Description => "Is 20% smaller than average.";

    public override Group Group => Group.SizeGroup;

    protected override void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, Name, Description, 0.8f);
    }
  }
}
