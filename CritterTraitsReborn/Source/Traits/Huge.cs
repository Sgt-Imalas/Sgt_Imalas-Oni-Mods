namespace Heinermann.CritterTraits.Traits
{
  class Huge : TraitBuilder
  {
    public override string ID => "CritterHuge";
    public override string Name => "Huge";
    public override string Description => "Is 50% larger than average.";

    public override Group Group => Group.SizeGroup;

    protected override void Init()
    {
      TraitHelpers.CreateScaleTrait(ID, Name, Description, 1.5f);
    }

  }
}
