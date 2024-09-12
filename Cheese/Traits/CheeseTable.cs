namespace Cheese.Traits
{
	internal class CheeseTable : KMonoBehaviour
	{

		public override void OnSpawn()
		{
			base.OnSpawn();
			ModAssets.CheeseTableTargets.Add(this);
		}
		public override void OnCleanUp()
		{
			ModAssets.CheeseTableTargets.Remove(this);
			base.OnCleanUp();
		}
	}
}
