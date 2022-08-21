using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
    public class RTB_ModuleGenerator : Generator
    {
        [MyCmpGet]
        private Storage storage;

        private Clustercraft clustercraft;
        private Guid poweringStatusItemHandle;
        private Guid notPoweringStatusItemHandle;

        public bool AlwaysActive = false;
        public bool AllowRefill = true;

        public Tag consumptionElement = SimHashes.Void.CreateTag();
        public float consumptionRate = 1f;
        public float consumptionMaxStoredMass = 100f;

        public SimHashes outputElement = SimHashes.Void;
        public float OutputCreationRate = -1f;
        public float OutputTemperature = 293.15f;




        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.connectedTags = new Tag[0];
            this.IsVirtual = true;
        }

        protected override void OnSpawn()
        {
            CraftModuleInterface craftInterface = this.GetComponent<RocketModuleCluster>().CraftInterface;
            this.VirtualCircuitKey = (object)craftInterface;
            this.clustercraft = craftInterface.GetComponent<Clustercraft>();
            Game.Instance.electricalConduitSystem.AddToVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
            base.OnSpawn();
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
        }

        public override bool IsProducingPower() => AlwaysActive || this.clustercraft.IsFlightInProgress();

        public override void EnergySim200ms(float dt)
        {

            RemoveRefillOnSatisfied();
            base.EnergySim200ms(dt);
            if (this.IsProducingPower())
            {
                if (ConsumptionSatisfied(dt))
                {
                    this.GenerateJoules(this.WattageRating * dt);

                    if (!(this.poweringStatusItemHandle == Guid.Empty))
                        return;
                    this.poweringStatusItemHandle = this.selectable.ReplaceStatusItem(this.notPoweringStatusItemHandle, Db.Get().BuildingStatusItems.ModuleGeneratorPowered, (object)this);
                    this.notPoweringStatusItemHandle = Guid.Empty;
                }
            }
            else
            {
                if (!(this.notPoweringStatusItemHandle == Guid.Empty))
                    return;
                this.notPoweringStatusItemHandle = this.selectable.ReplaceStatusItem(this.poweringStatusItemHandle, Db.Get().BuildingStatusItems.ModuleGeneratorNotPowered, (object)this);
                this.poweringStatusItemHandle = Guid.Empty;
            }
        }

        public bool ConsumptionSatisfied(float dt)
        {
            if (consumptionElement == SimHashes.Void.CreateTag())
                return true;
            else
            {
                //foreach (var consumable in formula.inputs)
                //{
                 if (storage.GetMassAvailable(consumptionElement) < consumptionRate * dt)
                     return false;
                //}

                this.ConsumeRessources(dt);
                this.ProduceRessources(dt);
                return true;
            }

        }

        private void RemoveRefillOnSatisfied()
        {
            Debug.Log("Name: " + this.GetProperName());
            Debug.Log("Allow Refill: " + AllowRefill);

            var delivery = this.GetComponent<ManualDeliveryKG>();

            Debug.Log(delivery);

            if (delivery == null || AllowRefill || consumptionElement == SimHashes.Void.CreateTag())
                return;

            //foreach (var consumable in consumptionElement)
            //{
                Debug.Log(consumptionElement);
                if (storage.GetMassAvailable(consumptionElement) < consumptionMaxStoredMass)
                    return;
            //}
            Destroy(delivery);
            Debug.Log("Delivery was fullfilled; removed DeliveryComponent on " + this.GetProperName());
        }

        private void ConsumeRessources(float dt)
        {

            if (consumptionElement == SimHashes.Void.CreateTag())
                return;

            //foreach (var consumable in formula.inputs)
            //{
                float amount = consumptionRate * dt;
                this.storage.ConsumeIgnoringDisease(consumptionElement, amount);
            //}
        }

        private void ProduceRessources(float dt)
        {
            if (outputElement == SimHashes.Void)
                return;
            //foreach (var producable in formula.outputs)
            //{
                float amount = OutputCreationRate * dt;

                Element elementByHash = ElementLoader.FindElementByHash(outputElement);

                if (elementByHash.IsGas)
                    this.storage.AddGasChunk(outputElement, amount, OutputTemperature, byte.MaxValue, 0, true);
                else if (elementByHash.IsLiquid)
                    this.storage.AddLiquid(outputElement, amount, OutputTemperature, byte.MaxValue, 0, true);
                else
                    this.storage.Store(elementByHash.substance.SpawnResource(this.transform.GetPosition(), amount, OutputTemperature, byte.MaxValue, 0), true);
            //}
        }

    }
}
