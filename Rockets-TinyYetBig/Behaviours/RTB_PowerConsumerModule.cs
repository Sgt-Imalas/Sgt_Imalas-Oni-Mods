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
            var isVirtualSetter = typeof(RTB_PowerConsumerModule).GetProperty("IsVirtual");
            isVirtualSetter.SetValue(this, true);
        }
        public override void OnSpawn()
        {
            CraftModuleInterface craftInterface = this.GetComponent<RocketModuleCluster>().CraftInterface;

            var virtualCircuitKeySetter = typeof(RTB_PowerConsumerModule).GetProperty("VirtualCircuitKey");
            virtualCircuitKeySetter.SetValue(this, (object)craftInterface);
            this.VirtualCircuitKey = craftInterface;
            this.clustercraft = craftInterface.m_clustercraft;
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
