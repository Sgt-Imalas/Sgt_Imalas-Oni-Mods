namespace Imalas_TwitchChaosEvents.Elements
{
	class BottleFlipper : KMonoBehaviour
	{
		[MyCmpGet] KBatchedAnimController kbac;
		public override void OnSpawn()
		{
			base.OnSpawn();
			kbac.flipY = true;
		}
	}
}
