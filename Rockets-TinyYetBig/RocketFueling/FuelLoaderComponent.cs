using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class FuelLoaderComponent : KMonoBehaviour, ISecondaryInput
    {
        [Serialize]
        public LoaderType loaderType;

        public enum LoaderType
        {
            Fuel,
            Oxidizer,
            HEP
        }

        //[MyCmpGet]
        public HighEnergyParticleStorage HEPStorage;
        public Storage gasStorage;
        public Storage liquidStorage;
        public Storage solidStorage;

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
            if (this.liquidPortInfo != null && this.liquidPortInfo.conduitType == type)
                return this.liquidPortInfo.offset;
            if (this.gasPortInfo != null && this.gasPortInfo.conduitType == type)
                return this.gasPortInfo.offset;
            if (this.solidPortInfo != null && this.solidPortInfo.conduitType == type)
                return this.solidPortInfo.offset;
            return CellOffset.none;
        }

        bool ISecondaryInput.HasSecondaryConduitType(ConduitType type) => this.liquidPortInfo != null && this.liquidPortInfo.conduitType == type || this.gasPortInfo != null && this.gasPortInfo.conduitType == type && this.loaderType == LoaderType.Fuel || this.solidPortInfo != null&& this.solidPortInfo.conduitType == type;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            //this.particleStorage = this.GetComponent<HighEnergyParticleStorage>();
            if (this.loaderType == LoaderType.Fuel)
            { 
                this.gasInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), this.gasPortInfo.offset);
                this.gasConsumer = this.CreateConduitConsumer(ConduitType.Gas, this.gasInputCell, gasStorage, out this.gasNetworkItem);
            }
            if (this.loaderType == LoaderType.Fuel || this.loaderType == LoaderType.Oxidizer)
            {
                this.liquidInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), this.liquidPortInfo.offset);
                this.liquidConsumer = this.CreateConduitConsumer(ConduitType.Liquid, this.liquidInputCell, liquidStorage, out this.liquidNetworkItem);
                this.solidInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), this.solidPortInfo.offset);
                this.solidConsumer = this.CreateSolidConduitConsumer(this.solidInputCell, solidStorage, out this.solidNetworkItem);
            }
        }

        private ConduitConsumer CreateConduitConsumer(
            ConduitType inputType,
            int inputCell,
            Storage target,
            out FlowUtilityNetwork.NetworkItem flowNetworkItem)
        {
            ConduitConsumer conduitConsumer = this.gameObject.AddComponent<ConduitConsumer>();
            conduitConsumer.conduitType = inputType;
            conduitConsumer.useSecondaryInput = true;
            conduitConsumer.storage = target;
            IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(inputType);
            flowNetworkItem = new FlowUtilityNetwork.NetworkItem(inputType, Endpoint.Sink, inputCell, this.gameObject);
            int cell = inputCell;
            FlowUtilityNetwork.NetworkItem networkItem = flowNetworkItem;
            networkManager.AddToNetworks(cell, (object)networkItem, true);
            return conduitConsumer;
        }
        private SolidConduitConsumer CreateSolidConduitConsumer(
            int inputCell,
            Storage target,
            out FlowUtilityNetwork.NetworkItem flowNetworkItem)
        {
            SolidConduitConsumer solidConduitConsumer = this.gameObject.AddComponent<SolidConduitConsumer>();
            solidConduitConsumer.useSecondaryInput = true;
            solidConduitConsumer.storage = target;
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
