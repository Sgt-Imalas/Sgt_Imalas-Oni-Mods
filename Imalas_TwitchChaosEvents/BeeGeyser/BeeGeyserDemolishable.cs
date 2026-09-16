namespace Imalas_TwitchChaosEvents.BeeGeyser
{
	internal class BeeGeyserDemolishable:Demolishable
	{
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			SetWorkTime(666f);
		}
	}
}
