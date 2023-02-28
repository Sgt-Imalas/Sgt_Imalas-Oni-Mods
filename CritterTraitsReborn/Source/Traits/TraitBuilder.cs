namespace Heinermann.CritterTraits.Traits
{
  /**
   * Base class for traits in this mod.
   */
  public abstract class TraitBuilder
  {
    public abstract string ID { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract Group Group { get; }

    protected abstract void Init();

    public void CreateTrait()
    {
      Init();
    }
  }
}
