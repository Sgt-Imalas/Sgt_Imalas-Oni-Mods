namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class ActiveLightController : KMonoBehaviour
	{
		[MyCmpReq] Operational operational;
		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Sub((int)GameHashes.ActiveChanged, OnActiveChanged);
			OnActiveChanged(null);
		}
		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();

		}
		void OnActiveChanged(object sender)
		{
			bool enable = operational.IsActive;
			var lights = GetComponents<Light2D>();
			foreach (Light2D light in lights)
			{
				light.enabled = enable;
			}

		}
	}
}
