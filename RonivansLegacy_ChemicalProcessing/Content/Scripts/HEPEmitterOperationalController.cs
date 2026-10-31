namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class HEPEmitterOperationalController : KMonoBehaviour
	{
		[MyCmpReq] Operational operational;
		[MyCmpReq] RadiationEmitter hepEmitter;

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Sub((int)GameHashes.OperationalChanged, OnOperationalChanged);
			this.Sub((int)GameHashes.ActiveChanged, OnOperationalChanged);
			OnOperationalChanged(null);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.UnsubAll();
		}
		void OnOperationalChanged(object data)
		{
			if (operational.IsActive)
			{
				hepEmitter.SetEmitting(true);
			}
			else
			{
				hepEmitter.SetEmitting(false);
			}
		}
	}
}
