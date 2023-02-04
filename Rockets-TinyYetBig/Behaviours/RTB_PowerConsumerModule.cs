using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Behaviours
{
    class RTB_PowerConsumerModule : EnergyConsumer
    {
        private Clustercraft clustercraft;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.IsVirtual = true;
        }
        public override void OnSpawn()
        {
            CraftModuleInterface craftInterface = this.GetComponent<RocketModuleCluster>().CraftInterface;
            this.VirtualCircuitKey = (object)craftInterface;
            this.clustercraft = craftInterface.GetComponent<Clustercraft>();
            Game.Instance.electricalConduitSystem.AddToVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
            base.OnSpawn();
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
            Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
        }
    }
}
