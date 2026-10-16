namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal interface IOnWorkTickActionProvider
	{
		public void OnWorkTick(WorkerBase worker, float dt);
	}
}
