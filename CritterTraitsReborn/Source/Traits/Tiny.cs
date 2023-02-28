namespace Heinermann.CritterTraits.Traits
{
  class Tiny : TraitBuilder
  {
    public override string ID => "CritterTiny";
    public override string Name => "Tiny";
    public override string Description => "Is 40% smaller than average.";

    public override Group Group => Group.SizeGroup;

    protected override void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, Name, Description, 0.6f);
    }
  }
}
