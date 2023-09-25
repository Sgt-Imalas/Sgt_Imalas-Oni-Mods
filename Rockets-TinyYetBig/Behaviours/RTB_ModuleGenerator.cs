using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using static STRINGS.BUILDINGS.PREFABS;

namespace Rockets_TinyYetBig.Behaviours
{
    public class RTB_ModuleGenerator : Generator, ISidescreenButtonControl
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
        private Guid ActiveStatusItemHandle;
        // private Guid notPoweringStatusItemHandle;
        public Guid FuelStatusHandle;

        public bool AlwaysActive = false;
        [Serialize]
        public bool produceWhileLanded = false;
        [Serialize]
        private bool _producingEnergy = false;
        //[Serialize]
        //public bool AllowRefill = true;


        //[Serialize]
        //public bool RefillingPaused = false;

        [Serialize]
        public bool OutputToOwnStorage = false;
        [Serialize]
        public Vector3 ElementOutputCellOffset = new Vector3(0, 0);

        [Serialize]
        public CargoBay.CargoType PullFromRocketStorageType = CargoBay.CargoType.Entities;
        [Serialize]
        public CargoBay.CargoType PushToRocketStorageType = CargoBay.CargoType.Entities;


        public Tag consumptionElement = SimHashes.Void.CreateTag();
        public float consumptionRate = 1f;
        public float consumptionMaxStoredMass = 100f;

        public SimHashes outputElement = SimHashes.Void;
        public float outputProductionRate = -1f;
        public float outputProductionTemperature = 293.15f;

        public string SidescreenButtonText => STRINGS.UI.ROCKETGENERATOR.BUTTONTEXT;

        public string SidescreenButtonTooltip => STRINGS.UI.ROCKETGENERATOR.TOOLTIP;

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
            StatusItemUpdate(false, true);
            UpdateLandedStatusItem();
        }
        public Tuple<float, float> GetConsumptionStatusStats()
        {
            var returnVals = new Tuple<float, float>(0, 0);
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
            if (VirtualCircuitKey != null)
                Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(this.VirtualCircuitKey, (object)this, true);
            //if(clustercraft!= null && clustercraft.TryGetComponent<KSelectable>(out var selectable))
            //{
            //    selectable.RemoveStatusItem(FuelStatusHandle, true);
            //}
            base.OnCleanUp();

        }
        public override bool IsProducingPower() => _producingEnergy;
        public bool ShouldProducePower() =>
            AlwaysActive 
            || this.clustercraft.Status != Clustercraft.CraftStatus.Grounded && BatteriesNotFull() 
            || produceWhileLanded && BatteriesNotFull();


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
            //if (!AllowRefill)
            //{
            //    ToggleFill(RefillingPaused);
            //}
            base.EnergySim200ms(dt);
            var emitter = this.gameObject.GetComponent<RadiationEmitter>();

            if (this.ShouldProducePower())
            {
                if (ConsumptionSatisfied(dt))
                {
                    if (emitter != null)
                    {
                        emitter.SetEmitting(true);
                    }

                    this.GenerateJoules(this.WattageRating * dt);
                    _producingEnergy = true;
                    StatusItemUpdate(true);
                }
                else
                {
                    if (emitter != null)
                    {
                        emitter.SetEmitting(false);
                        StatusItemUpdate(false);
                    }
                    _producingEnergy = false;
                }
            }
            else
            {
                _producingEnergy = false;
                StatusItemUpdate(false);
            }

            //this.selectable.GetStatusItemGroup().SetStatusItem(FuelStatusHandleGrounded, Db.Get().StatusItemCategories.Main, ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)this);
            ResetRefillStatus();
        }

        bool lastState = false;
        void StatusItemUpdate(bool active = false, bool force = false)
        {
            if (lastState == active && !force)
                return;

            lastState = active;

            if (active)
            {
                this.ActiveStatusItemHandle = this.AlwaysActive ?
                    this.selectable.ReplaceStatusItem(this.ActiveStatusItemHandle, ModAssets.StatusItems.RTB_AlwaysActiveOn, (object)this) :
                    this.selectable.ReplaceStatusItem(this.ActiveStatusItemHandle, ModAssets.StatusItems.RTB_ModuleGeneratorPowered, (object)this);
            }
            else
            {
                this.ActiveStatusItemHandle = this.AlwaysActive ?
                        this.selectable.ReplaceStatusItem(this.ActiveStatusItemHandle, ModAssets.StatusItems.RTB_AlwaysActiveOff, (object)this) :
                        this.selectable.ReplaceStatusItem(this.ActiveStatusItemHandle, ModAssets.StatusItems.RTB_ModuleGeneratorNotPowered, (object)this);

            }
        }


        public bool ConsumptionSatisfied(float dt)
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
                this.ProduceRessources(dt, ratio);

                //}

                return true;
            }

        }

        private void PullFuelFromRocketStorage(float dt)
        {

            foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)clustercraft.ModuleInterface.ClusterModules)
            {
                if (clusterModule.Get().TryGetComponent<CargoBayCluster>(out var component) && component.storageType == this.PullFromRocketStorageType)
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
                if (clusterModule.Get().TryGetComponent<CargoBayCluster>(out var carbobay) && carbobay.storageType == this.PushToRocketStorageType)
                {
                    if ((double)carbobay.RemainingCapacity >= outputProductionRate * dt && carbobay.GetComponent<TreeFilterable>().ContainsTag(outputElement.CreateTag()))
                    {
                        PutOutputElementToStorate(outputElement, outputProductionRate * dt, carbobay.storage);
                        putAwaySuccess = true;
                    }
                }
            }
            return putAwaySuccess;
        }

        void PutOutputElementToStorate(SimHashes key, float mass, Storage storage)
        {
            Element elementByHash = ElementLoader.FindElementByHash(key);
            switch (elementByHash.state & Element.State.Solid)
            {
                case Element.State.Gas:
                    storage.AddGasChunk(key, mass, outputProductionTemperature, byte.MaxValue, 0, false);
                    break;
                case Element.State.Liquid:
                    storage.AddLiquid(key, mass, outputProductionTemperature, byte.MaxValue, 0);
                    break;
                case Element.State.Solid:
                    storage.AddOre(key, mass, outputProductionTemperature, byte.MaxValue, 0);
                    break;
            }
        }



        private void ResetRefillStatus()
        {
            if (
                //!AllowRefill && 
                storage.GetAmountAvailable(consumptionElement) == 0)
            {
                if (clustercraft.Status == Clustercraft.CraftStatus.Grounded)
                {
                    if (TryGetComponent<ManualDeliveryKG>(out var delivery))
                    {
                        if (delivery.IsPaused)
                        {
                            delivery.Pause(false, "one Refill allowed.");
                            //RefillingPaused = false;
                        }
                        if (outputElement != SimHashes.Void && storage.GetAmountAvailable(outputElement.CreateTag()) > 0f)
                            storage.DropAll(this.transform.position, true, true);


                    }
                }
            }
        }

        void ToggleFill(bool shouldPause)
        {
            if (this.TryGetComponent<ManualDeliveryKG>(out var delivery)
                //&& !AllowRefill
                )
            {
                delivery.Pause(shouldPause, "no Refill allowed.");
            }
        }

        private void RemoveRefillOnSatisfied()
        {
            this.TryGetComponent<ManualDeliveryKG>(out var delivery);

            if (delivery == null ||
                //AllowRefill ||
                consumptionElement == SimHashes.Void.CreateTag())
                return;

            if (storage.GetMassAvailable(consumptionElement) < consumptionMaxStoredMass)
                return;

            delivery.Pause(true, "no Refill allowed.");
            //RefillingPaused = true;
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

            if (this.OutputToOwnStorage)
            {
                PutOutputElementToStorate(outputElement, amount, this.storage);
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
                        elementByHash.substance.SpawnResource(output, amount, outputProductionTemperature, byte.MaxValue, 0).SetActive(true);
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

        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
        {
        }

        public bool SidescreenEnabled() => (clustercraft.status == Clustercraft.CraftStatus.Grounded) && !AlwaysActive;

        public bool SidescreenButtonInteractable() => !AlwaysActive;

        public void UpdateLandedStatusItem()
        {
            if (produceWhileLanded)
                this.selectable.AddStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorLandedEnabled);
            else
                this.selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorLandedEnabled);

        }

        public void OnSidescreenButtonPressed()
        {
            produceWhileLanded = !produceWhileLanded;
            UpdateLandedStatusItem();
        }
        public int HorizontalGroupID() => -1;

    }
}
