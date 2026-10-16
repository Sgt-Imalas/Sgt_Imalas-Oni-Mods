namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class TutorialOxygenGenerator : KMonoBehaviour
	{
		public override void OnSpawn()
		{
			base.OnSpawn();
			Tutorial.Instance.oxygenGenerators.Add(this.gameObject);
		}
		public override void OnCleanUp()
		{
			Tutorial.Instance.oxygenGenerators.Remove(this.gameObject);
			base.OnCleanUp();
		}
	}
}
