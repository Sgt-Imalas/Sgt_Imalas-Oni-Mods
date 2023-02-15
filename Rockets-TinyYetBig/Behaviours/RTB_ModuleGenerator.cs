using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace Rockets_TinyYetBig.Behaviours
{
    public class RTB_ModuleGenerator : Generator//, ISidescreenButtonControl
    {

        /// <summary>
        /// IDEAS:
        /// Rocket Engine thats not limited to the bottom
        /// Advanced Starting Platform with Ribbons
        /// Bunkered Starting Platform
        /// Steam Generator; Coal Generator ; Status msg for generators
        /// 
        /// Swimming pool Chlordispenser disinfectant
        /// </summary>

        [MyCmpGet] 
        private Storage storage;


        private Clustercraft clustercraft;
        private Guid poweringStatusItemHandle;
        private Guid notPoweringStatusItemHandle;
        public Guid FuelStatusHandle;

        [SerializeField]
        public bool AlwaysActive = false;
        //[SerializeField]
        //public bool produceWhileLanded = false;
        [SerializeField]
        public bool AllowRefill = true;
        [SerializeField]
        public bool OutputToOwnStorage = false;
        [SerializeField]
        public Vector3 ElementOutputCellOffset = new Vector3(0,0);

        [SerializeField]
        public CargoBay.CargoType PullFromRocketStorageType = CargoBay.CargoType.Entities;
        [SerializeField]
        public CargoBay.CargoType PushToRocketStorageType = CargoBay.CargoType.Entities;


        public Tag consumptionElement = SimHashes.Void.CreateTag();
        public float consumptionRate = 1f;
        public float consumptionMaxStoredMass = 100f;

        public SimHashes outputElement = SimHashes.Void;
        public float outputProductionRate = -1f;
        public float outputProductionTemperature = 293.15f;


        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.connectedTags = new Tag[0];
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
        public Tuple<float,float> GetConsumptionStatusStats()
        {
            var returnVals = new Tuple<float, float>(0,0);
            if (storage == null)
                return returnVals;
            if (this.PullFromRocketStorageType == CargoBay.CargoType.Entities)
            {
                returnVals.second = storage.Capacity();
                returnVals.first = storage.GetMassAvailable(consumptionElement);
            }
            else
            {
                returnVals.second = -1;
                returnVals.first = -1;

                return returnVals;
                ///Too expensive
                //if (clustercraft == null)
                //    return returnVals;

                //foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)clustercraft.ModuleInterface.ClusterModules)
                //{
                //    if (clusterModule.Get().TryGetComponent<CargoBayCluster>(out var component) && component.storageType == this.PullFromRocketStorageType)
                //    {
                //        if ((double)component.storage.MassStored() >= consumptionRate)
                //        {
                //            returnVals.first += component.storage.GetMassAvailable(consumptionElement);
                //            returnVals.second += component.storage.Capacity();
                //        }
                //    }
                //}
            }
            return returnVals;
        }
        public override void OnCleanUp()
        {
            clustercraft.GetComponent<KSelectable>().RemoveStatusItem(FuelStatusHandle, true);
            base.OnCleanUp();

            Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
        }

        public override bool IsProducingPower() => AlwaysActive && ConsumptionSatisfied()|| this.clustercraft.Status != Clustercraft.CraftStatus.Grounded && BatteriesNotFull() && ConsumptionSatisfied();// || produceWhileLanded && BatteriesNotFull();
        public bool ShouldProduciePower() => AlwaysActive || this.clustercraft.Status != Clustercraft.CraftStatus.Grounded && BatteriesNotFull();// || produceWhileLanded && BatteriesNotFull();


        public bool BatteriesNotFull()
        {
            List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(this.CircuitID);
            if (!AlwaysActive && batteriesOnCircuit.Count > 0)
            {
                foreach (Battery battery in batteriesOnCircuit)
                {
                    if (battery.PercentFull <= 0.95f)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void EnergySim200ms(float dt)
        {
            //selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)this);
            RemoveRefillOnSatisfied();
            base.EnergySim200ms(dt);
            var emitter = this.gameObject.GetComponent<RadiationEmitter>();

            if (this.ShouldProduciePower())
            {
                if (ConsumptionSatisfied(dt))
                {
                    if (emitter != null)
                    {
                        emitter.SetEmitting(true);
                    }

                    this.GenerateJoules(this.WattageRating * dt);

                    if (!(this.poweringStatusItemHandle == Guid.Empty))
                        return;
                    this.poweringStatusItemHandle = this.AlwaysActive ?
                        this.selectable.ReplaceStatusItem(this.notPoweringStatusItemHandle, ModAssets.StatusItems.RTB_AlwaysActiveOn , (object)this):
                        this.selectable.ReplaceStatusItem(this.notPoweringStatusItemHandle, ModAssets.StatusItems.RTB_ModuleGeneratorPowered, (object)this);
                    this.notPoweringStatusItemHandle = Guid.Empty;
                }
                else
                {
                    if (emitter != null)
                    {
                        emitter.SetEmitting(false);
                    }
                }
            }
            else
            {
                if (!(this.notPoweringStatusItemHandle == Guid.Empty))
                    return;
                this.notPoweringStatusItemHandle = this.AlwaysActive ?
                        this.selectable.ReplaceStatusItem(this.notPoweringStatusItemHandle, ModAssets.StatusItems.RTB_AlwaysActiveOff, (object)this) :
                        this.selectable.ReplaceStatusItem(this.notPoweringStatusItemHandle, ModAssets.StatusItems.RTB_ModuleGeneratorNotPowered, (object)this);
                this.poweringStatusItemHandle = Guid.Empty;


            }

            //this.selectable.GetStatusItemGroup().SetStatusItem(FuelStatusHandleGrounded, Db.Get().StatusItemCategories.Main, ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)this);
            ResetRefillStatus();
        }

        public bool ConsumptionSatisfied(float dt = 1)
        {
            if (consumptionElement == SimHashes.Void.CreateTag())
                return true;
            else
            {
                if (this.PullFromRocketStorageType != CargoBay.CargoType.Entities)
                {
                    PullFuelFromRocketStorage(dt);
                }

                if (storage.GetMassAvailable(consumptionElement) < consumptionRate * dt)
                    return false;
                var ratio = this.ConsumeRessources(dt);
                this.ProduceRessources(dt,ratio);
                
                //}

                return true;
            }

        }

        private void PullFuelFromRocketStorage(float dt)
        {
            
            foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)clustercraft.ModuleInterface.ClusterModules)
            {
                CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
                if (component != null && component.storageType == this.PullFromRocketStorageType)
                {
                    if ((double)component.storage.MassStored() >= consumptionRate * dt)
                    {
                        for (int index = component.storage.items.Count - 1; index >= 0; --index)
                        {
                            GameObject go = component.storage.items[index];
                            if (go.PrefabID() == this.consumptionElement)
                            {
                                Pickupable pickupable = go.GetComponent<Pickupable>().Take(consumptionRate * dt);
                                if (pickupable != null)
                                {
                                    this.storage.Store(pickupable.gameObject, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private bool TryPuttingOutputIntoStorage(float dt)
        {
            bool putAwaySuccess = false;
            foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)clustercraft.ModuleInterface.ClusterModules)
            {
                CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
                if (component != null && component.storageType == this.PushToRocketStorageType)
                {
                    if ((double)component.RemainingCapacity >= outputProductionRate * dt && component.GetComponent<TreeFilterable>().ContainsTag(outputElement.CreateTag())) 
                    {
                        component.storage.Store(ElementLoader.FindElementByHash(outputElement).substance.SpawnResource(this.transform.GetPosition(), outputProductionRate * dt, outputProductionTemperature, byte.MaxValue, 0), true);
                        putAwaySuccess = true;
                    }
                }
            }
            return putAwaySuccess;
        }




        private void ResetRefillStatus()
        {
            if (!AllowRefill &&storage.GetAmountAvailable(consumptionElement) == 0)
            {
                if(clustercraft.Status == Clustercraft.CraftStatus.Grounded)
                {
                    var delivery = this.GetComponent<ManualDeliveryKG>();
                    if(delivery.IsPaused)
                        delivery.Pause(false, "one Refill allowed.");
                    if (outputElement != SimHashes.Void && storage.GetAmountAvailable(outputElement.CreateTag())>0f)
                        storage.DropAll(this.transform.position, true, true); 

                }
            }
        }

        private void RemoveRefillOnSatisfied()
        {
            

            var delivery = this.GetComponent<ManualDeliveryKG>();

            if (delivery == null || AllowRefill || consumptionElement == SimHashes.Void.CreateTag())
                return;
            
            if (storage.GetMassAvailable(consumptionElement) < consumptionMaxStoredMass)
                return;

            delivery.Pause(true, "no Refill allowed.");
        }

        private float ConsumeRessources(float dt)
        {

            if (consumptionElement == SimHashes.Void.CreateTag())
                return 0;


                var remainingMats = storage.GetAmountAvailable(consumptionElement);

                float amount = consumptionRate * dt;
                float ratio = 1f;
                ratio = remainingMats < amount ? remainingMats / amount : 1;


                this.storage.ConsumeIgnoringDisease(consumptionElement, amount);
            return ratio;
        }

        private void ProduceRessources(float dt, float amountLeftMultiplier = 1f)
        {
            if (outputElement == SimHashes.Void)
                return;
            //foreach (var producable in formula.outputs)
            //{
                float amount = outputProductionRate * dt * amountLeftMultiplier;

                Element elementByHash = ElementLoader.FindElementByHash(outputElement);

            if (this.OutputToOwnStorage) { 
                if (elementByHash.IsGas)
                    this.storage.AddGasChunk(outputElement, amount, outputProductionTemperature, byte.MaxValue, 0, true);
                else if (elementByHash.IsLiquid)
                    this.storage.AddLiquid(outputElement, amount, outputProductionTemperature, byte.MaxValue, 0, true);
                else
                    this.storage.Store(elementByHash.substance.SpawnResource(this.transform.GetPosition(), amount, outputProductionTemperature, byte.MaxValue, 0), true);
            }
            else
            {
                if (this.PushToRocketStorageType != CargoBay.CargoType.Entities)
                {
                    if (TryPuttingOutputIntoStorage(dt)) 
                    { 
                        return;
                    }
                }

                if (clustercraft.Status == Clustercraft.CraftStatus.Grounded)
                {
                    Vector3 output = this.transform.GetPosition() + ElementOutputCellOffset; 
                    
                    if (elementByHash.IsGas || elementByHash.IsLiquid)
                        SimMessages.AddRemoveSubstance(Grid.PosToCell(output), outputElement, CellEventLogger.Instance.ElementEmitted, amount, outputProductionTemperature, byte.MaxValue, 0);
                    else if (elementByHash.IsSolid)
                        elementByHash.substance.SpawnResource(output, amount, outputProductionTemperature, byte.MaxValue, 0);
                    //SgtLogger.debuglog("dumped Element " + outputElement + " with " + amount + " amount");
                }
            }
        }


        //public string SidescreenButtonText => produceWhileLanded ? "Disable generator on ground" : "Enable generator on ground";

        //public string SidescreenButtonTooltip => "Select if the generator module should produce power while the rocket is grounded";

        /// <summary>
        /// No generators on land.
        /// </summary>
        /// <returns></returns>
        //public bool SidescreenEnabled() => false;//!this.AlwaysActive; 

        // public bool SidescreenButtonInteractable() => !this.AlwaysActive;

        //public void OnSidescreenButtonPressed()
        //{
        //    produceWhileLanded = !produceWhileLanded;
        //}

        public int ButtonSideScreenSortOrder() => 20;
    }
}
