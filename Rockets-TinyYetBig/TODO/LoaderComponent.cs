using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class LoaderComponent : KMonoBehaviour, ISecondaryInput
    {
        [MyCmpGet]
        public HighEnergyParticleStorage particleStorage;
        public Storage resourceStorage;

        [SerializeField]
        public ConduitPortInfo liquidPortInfo;
        private int liquidInputCell = -1;
        private FlowUtilityNetwork.NetworkItem liquidNetworkItem;
        private ConduitConsumer liquidConsumer;
        [SerializeField]
        public ConduitPortInfo gasPortInfo;
        private int gasInputCell = -1;
        private FlowUtilityNetwork.NetworkItem gasNetworkItem;
        private ConduitConsumer gasConsumer;
        [SerializeField]
        public ConduitPortInfo solidPortInfo;
        private int solidInputCell = -1;
        private FlowUtilityNetwork.NetworkItem solidNetworkItem;
        private SolidConduitConsumer solidConsumer;

        public CellOffset GetSecondaryConduitOffset(ConduitType type)
        {
            if (this.liquidPortInfo.conduitType == type)
                return this.liquidPortInfo.offset;
            if (this.gasPortInfo.conduitType == type)
                return this.gasPortInfo.offset;
            return this.solidPortInfo.conduitType == type ? this.solidPortInfo.offset : CellOffset.none;
        }

        bool ISecondaryInput.HasSecondaryConduitType(ConduitType type) => this.liquidPortInfo.conduitType == type || this.gasPortInfo.conduitType == type || this.solidPortInfo.conduitType == type;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.resourceStorage = this.GetComponent<Storage>();
            //this.particleStorage = this.GetComponent<HighEnergyParticleStorage>();

            this.gasInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), this.gasPortInfo.offset);
            this.gasConsumer = this.CreateConduitConsumer(ConduitType.Gas, this.gasInputCell, out this.gasNetworkItem);
            this.liquidInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), this.liquidPortInfo.offset);
            this.liquidConsumer = this.CreateConduitConsumer(ConduitType.Liquid, this.liquidInputCell, out this.liquidNetworkItem);
            this.solidInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), this.solidPortInfo.offset);
            this.solidConsumer = this.CreateSolidConduitConsumer(this.solidInputCell, out this.solidNetworkItem);
        }

        private ConduitConsumer CreateConduitConsumer(
            ConduitType inputType,
            int inputCell,
            out FlowUtilityNetwork.NetworkItem flowNetworkItem)
        {
            ConduitConsumer conduitConsumer = this.gameObject.AddComponent<ConduitConsumer>();
            conduitConsumer.conduitType = inputType;
            conduitConsumer.useSecondaryInput = true;
            IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(inputType);
            flowNetworkItem = new FlowUtilityNetwork.NetworkItem(inputType, Endpoint.Sink, inputCell, this.gameObject);
            int cell = inputCell;
            FlowUtilityNetwork.NetworkItem networkItem = flowNetworkItem;
            networkManager.AddToNetworks(cell, (object)networkItem, true);
            return conduitConsumer;
        }
        private SolidConduitConsumer CreateSolidConduitConsumer(
            int inputCell,
            out FlowUtilityNetwork.NetworkItem flowNetworkItem)
        {
            SolidConduitConsumer solidConduitConsumer = this.gameObject.AddComponent<SolidConduitConsumer>();
            solidConduitConsumer.useSecondaryInput = true;
            flowNetworkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, inputCell, this.gameObject);
            Game.Instance.solidConduitSystem.AddToNetworks(inputCell, (object)flowNetworkItem, true);
            return solidConduitConsumer;
        }

        protected override void OnCleanUp()
        {
            Conduit.GetNetworkManager(this.liquidPortInfo.conduitType).RemoveFromNetworks(this.liquidInputCell, (object)this.liquidNetworkItem, true);
            Conduit.GetNetworkManager(this.gasPortInfo.conduitType).RemoveFromNetworks(this.gasInputCell, (object)this.gasNetworkItem, true);
            Game.Instance.solidConduitSystem.RemoveFromNetworks(this.solidInputCell, (object)this.solidConsumer, true);
            base.OnCleanUp();
        }
    }
}
