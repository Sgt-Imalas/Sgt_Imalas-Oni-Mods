using KSerialization;

namespace Imalas_TwitchChaosEvents.BeeGeyser
{
	internal class GeyserBeeLiveState : KMonoBehaviour
	{
		[Serialize]
		public BeeGeyser parent;

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (parent == null)
				return;
			parent.AddBee(this);
		}
		public override void OnCleanUp()
		{
			if (parent == null)
				return;
			parent.RemoveBee(this);
			base.OnCleanUp();
		}
	}
}
