namespace CritterTraitsReborn.Traits
{
  /**
   * Base class for traits in this mod.
   */
  public abstract class TraitBuilder
  {
    public abstract string ID { get; }

    public abstract Group Group { get; }

    protected abstract void Init();

    public void CreateTrait()
    {
      Init();
    }
  }
}
