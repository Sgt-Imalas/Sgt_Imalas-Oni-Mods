namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class ModuleEnergyConsumer : EnergyConsumer
	{
		[MyCmpReq] RocketModuleCluster moduleCluster;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.IsVirtual = true;
		}
		public override void OnSpawn()
		{
			this.VirtualCircuitKey = moduleCluster.CraftInterface;
			base.OnSpawn();
		}

	}
}
