using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Patches;
using Rockets_TinyYetBig.RocketFueling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.RocketFueling.FuelLoaderComponent;

namespace Rockets_TinyYetBig
{
    class DockedRocketMaterialDistributor :
  GameStateMachine<DockedRocketMaterialDistributor, DockedRocketMaterialDistributor.Instance, IStateMachineTarget, DockedRocketMaterialDistributor.Def>
    {
        public State inoperational;
        public DockedRocketMaterialDistributor.OperationalStates operational;
        private TargetParameter attachedRocket;
        private BoolParameter emptyComplete;
        private BoolParameter fillComplete;

        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            default_state = this.inoperational;
            this.serializable = StateMachine.SerializeType.ParamsOnly;

            this.inoperational
                .EventTransition(GameHashes.OperationalChanged, (State)this.operational, (smi => smi.GetComponent<Operational>().IsOperational));

            this.operational
                .DefaultState(this.operational.noRocket)
                .EventTransition(GameHashes.OperationalChanged, this.inoperational, (smi => !smi.GetComponent<Operational>().IsOperational))
                .EventHandler(GameHashes.ChainedNetworkChanged, ((smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi)))
                .EventHandler(GameHashes.RocketLanded, ((smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi)));

            this.operational
                .noRocket
                    .Update(((smi, dt) => this.SetAttachedRocket(smi.GetDockedRocket(), smi)), UpdateRate.RENDER_1000ms)
                    .EventHandler(GameHashes.RocketLanded, (smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi))
                    //.EventHandler(GameHashes.RocketCreated, (smi, data) => this.SetAttachedRocket(smi.GetDockedRocket(), smi))
                    .ParamTransition<GameObject>(this.attachedRocket, this.operational.hasRocket, ((smi, p) => p != null));

            //this.operational
            //    .rocketLanding
            //        .EventTransition(GameHashes.RocketLaunched, this.operational.rocketLost)
            //        .OnTargetLost(this.attachedRocket, this.operational.rocketLost)
            //        .Target(this.attachedRocket)
            //        .TagTransition(GameTags.RocketOnGround, (State)this.operational.hasRocket)
            //        .Target(this.masterTarget);

            this.operational
                .hasRocket
                    .DefaultState((State)this.operational.hasRocket.transferring)
                    .Update(((smi, dt) => smi.EmptyRocket(dt)), UpdateRate.SIM_1000ms)
                    .Update(((smi, dt) => smi.FillRocket(dt)), UpdateRate.SIM_1000ms)
                    .EventTransition(GameHashes.RocketLaunched, this.operational.rocketLost)
                    .OnTargetLost(this.attachedRocket, this.operational.rocketLost)
                    .Target(this.attachedRocket)
                    .Target(this.masterTarget)
                    .Enter((smi) => smi.SetConnectedRocketStatusLoading(true))
            ;

            this.operational
                .hasRocket
                    .transferring
                    .DefaultState(this.operational.hasRocket.transferring.actual)
                    .ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoEmptying)
                    .ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFilling);

            this.operational
                .hasRocket
                    .transferring
                        .actual
                        .ParamTransition<bool>(this.emptyComplete, this.operational.hasRocket.transferring.delay, ((smi, p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi)))
                        .ParamTransition<bool>(this.fillComplete, this.operational.hasRocket.transferring.delay, ((smi, p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi)));

            this.operational
                .hasRocket
                    .transferring
                        .delay
                        .ParamTransition<bool>(this.fillComplete, this.operational.hasRocket.transferring.actual, IsFalse)
                        .ParamTransition<bool>(this.emptyComplete, this.operational.hasRocket.transferring.actual, IsFalse)
                        .ScheduleGoTo(4f, this.operational.hasRocket.transferComplete);

            this.operational
                .hasRocket
                    .transferComplete
                .ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFull)
                .ToggleTag(GameTags.TransferringCargoComplete)
                .ParamTransition<bool>(this.fillComplete, (State)this.operational.hasRocket.transferring, IsFalse)
                .ParamTransition<bool>(this.emptyComplete, (State)this.operational.hasRocket.transferring, IsFalse)
                .Enter((smi) => 
                { 
                    smi.SetConnectedRocketStatusLoading(false);
                    this.SetAttachedRocket(null, smi);
                });

            this.operational
                .rocketLost
                .Enter((smi =>
                {
                    this.emptyComplete.Set(false, smi);
                    this.fillComplete.Set(false, smi);
                    this.SetAttachedRocket(null, smi);
                }))
                .GoTo(this.operational.noRocket);
        }

        private void SetAttachedRocket(
          CraftModuleInterface attached,
          DockedRocketMaterialDistributor.Instance smi)
        {
            if(attached == this.attachedRocket.Get(smi))
                return;

            HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
            smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
            foreach (StateMachine.Instance smi1 in (HashSet<ChainedBuilding.StatesInstance>)chain)
                smi1.GetSMI<ModularConduitPortController.Instance>()?.SetRocket((UnityEngine.Object)attached != (UnityEngine.Object)null);
            this.attachedRocket.Set((KMonoBehaviour)attached, smi);
            chain.Recycle();

        }

        public class Def : StateMachine.BaseDef
        {
        }

        public class HasRocketStates :
          State
        {
            public DockedRocketMaterialDistributor.HasRocketStates.TransferringStates transferring;
            public State transferComplete;

            public class TransferringStates :
              State
            {
                public State actual;
                public State delay;
            }
        }

        public class OperationalStates :
          State
        {
            public State noRocket;
            public State rocketLost;
            public HasRocketStates hasRocket;
        }

        public new class Instance :
          GameStateMachine<DockedRocketMaterialDistributor, DockedRocketMaterialDistributor.Instance, IStateMachineTarget, DockedRocketMaterialDistributor.Def>.GameInstance
        {
            public Instance(IStateMachineTarget master, DockedRocketMaterialDistributor.Def def)
              : base(master, def)
            {
            }

            public CraftModuleInterface GetDockedRocket() => this.GetComponent<DockingDoor>().GetDockedCraftModuleInterface();

            public void SetConnectedRocketStatusLoading(bool isLoadingOrUnloading)
            {
                var door = this.GetComponent<DockingDoor>();
                if (door.IsConnected)
                {
                    door.GetConnec().dManager.SetCurrentlyLoadingStuff(isLoadingOrUnloading);
                }
            }

            public void EmptyRocket(float dt)
            {
                CraftModuleInterface craftInterface = this.sm.attachedRocket.Get<CraftModuleInterface>(this.smi);
                DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.PooledList, DockedRocketMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.PooledList, DockedRocketMaterialDistributor>.Allocate();
                pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
                pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
                pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
                foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)craftInterface.ClusterModules)
                {
                    CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.storageType != CargoBay.CargoType.Entities && (double)component.storage.MassStored() > 0.0)
                        pooledDictionary[component.storageType].Add(component);
                }
                bool flag = false;
                HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
                this.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
                foreach (ChainedBuilding.StatesInstance smi1 in (HashSet<ChainedBuilding.StatesInstance>)chain)
                {
                    ModularConduitPortController.Instance smi2 = smi1.GetSMI<ModularConduitPortController.Instance>();
                    IConduitDispenser component1 = smi1.GetComponent<IConduitDispenser>();
                    Operational component2 = smi1.GetComponent<Operational>();
                    bool isUnloading = false;
                    if (component1 != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Unload || smi2.SelectedMode == ModularConduitPortController.Mode.Both) && ((UnityEngine.Object)component2 == (UnityEngine.Object)null || component2.IsOperational))
                    {
                        smi2.SetRocket(true);
                        TreeFilterable component3 = smi1.GetComponent<TreeFilterable>();
                        float amount = component1.Storage.RemainingCapacity();
                        foreach (CargoBayCluster cargoBayCluster in (List<CargoBayCluster>)pooledDictionary[CargoBayConduit.ElementToCargoMap[component1.ConduitType]])
                        {
                            if (cargoBayCluster.storage.Count != 0)
                            {
                                for (int index = cargoBayCluster.storage.items.Count - 1; index >= 0; --index)
                                {
                                    GameObject go = cargoBayCluster.storage.items[index];
                                    if (component3.AcceptedTags.Contains(go.PrefabID()))
                                    {
                                        isUnloading = true;
                                        flag = true;
                                        if ((double)amount > 0.0)
                                        {
                                            Pickupable pickupable = go.GetComponent<Pickupable>().Take(amount);
                                            if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
                                            {
                                                component1.Storage.Store(pickupable.gameObject);
                                                amount -= pickupable.PrimaryElement.Mass;
                                            }
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    smi2?.SetUnloading(isUnloading);
                }
                chain.Recycle();
                pooledDictionary[CargoBay.CargoType.Solids].Recycle();
                pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
                pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
                pooledDictionary.Recycle();
                this.sm.emptyComplete.Set(!flag, this);
            }

            public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] arr)
            {
                foreach (IEnumerable col in arr)
                    foreach (T item in col)
                        yield return item;
            }

            public void FillRocket(float dt)
            {
                CraftModuleInterface craftInterface = this.sm.attachedRocket.Get<CraftModuleInterface>(this.smi);
                DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.PooledList, DockedRocketMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.PooledList, DockedRocketMaterialDistributor>.Allocate();
                pooledDictionary[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
                pooledDictionary[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();
                pooledDictionary[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, DockedRocketMaterialDistributor>.Allocate();

                List<FuelTank> FuelTanks = new List<FuelTank>();
                List<OxidizerTank> OxidizerTanks = new List<OxidizerTank>();
                List<HEPFuelTank> HEPFuelTanks = new List<HEPFuelTank>();

                Tag FuelTag = SimHashes.Void.CreateTag();

                bool hasOxidizer;

                foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)craftInterface.ClusterModules)
                {
                    RocketEngineCluster engine = clusterModule.Get().GetComponent<RocketEngineCluster>();
                    if (engine != null)
                    {
                        FuelTag = engine.fuelTag;
                        hasOxidizer = engine.requireOxidizer;
                    }

                    FuelTank fueltank = clusterModule.Get().GetComponent<FuelTank>();
                    if (fueltank != null)
                        FuelTanks.Add(fueltank);

                    HEPFuelTank hepfueltank = clusterModule.Get().GetComponent<HEPFuelTank>();
                    if (hepfueltank != null)
                        HEPFuelTanks.Add(hepfueltank);

                    OxidizerTank oxTank = clusterModule.Get().GetComponent<OxidizerTank>();
                    if (oxTank != null)
                    {
                        hasOxidizer = true;
                        OxidizerTanks.Add(oxTank);
                    }

                    CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
                    if (component != null && component.storageType != CargoBay.CargoType.Entities && component.RemainingCapacity > 0f)
                    {
                        pooledDictionary[component.storageType].Add(component);
                    }

                }
                bool HasLoadingProcess = false;
                HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
                this.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
                foreach (ChainedBuilding.StatesInstance smi1 in (HashSet<ChainedBuilding.StatesInstance>)chain)
                {
                    ModularConduitPortController.Instance smi2 = smi1.GetSMI<ModularConduitPortController.Instance>();
                    FuelLoaderComponent fuelLoader = smi1.GetComponent<FuelLoaderComponent>();
                    IConduitConsumer NormalLoaderComponent = smi1.GetComponent<IConduitConsumer>();
                    bool isLoading = false;
                    if (fuelLoader != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Load))
                    {
                        //shouldDoNormal = false;
                        smi2.SetRocket(true);
                        if (fuelLoader.loaderType == LoaderType.Fuel)
                        {
                            GameObject[] AllItems = Concat(fuelLoader.solidStorage.items, fuelLoader.liquidStorage.items, fuelLoader.gasStorage.items).ToArray();
                            for (int index = AllItems.Count() - 1; index >= 0; --index)
                            {
                                GameObject storageItem = AllItems[index];
                                foreach (FuelTank fueltank in FuelTanks)
                                {
                                    float remainingCapacity = fueltank.Storage.RemainingCapacity();
                                    float num1 = RocketAutoLoadingPatches.AddFuelingLogic.TotalMassStoredOfItems(AllItems);
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && storageItem.HasTag(FuelTag))
                                    {
                                        isLoading = true;
                                        HasLoadingProcess = true;
                                        Pickupable pickupable = storageItem.GetComponent<Pickupable>().Take(remainingCapacity);
                                        if (pickupable != null)
                                        {
                                            fueltank.storage.Store(pickupable.gameObject, true);
                                            //float num2 = remainingCapacity - pickupable.PrimaryElement.Mass;
                                        }
                                    }
                                }
                            }
                        }
                        else if (fuelLoader.loaderType == LoaderType.HEP)
                        {
                            foreach (HEPFuelTank hepTank in HEPFuelTanks)
                            {
                                float remainingCapacity = hepTank.Storage.RemainingCapacity();
                                float SourceAmount = fuelLoader.HEPStorage.Particles;
                                if ((double)remainingCapacity > 0.0 && (double)SourceAmount > 0.0 && (FuelTag == GameTags.HighEnergyParticle))
                                {
                                    isLoading = true;
                                    HasLoadingProcess = true;
                                    float ParticlesTaken = fuelLoader.HEPStorage.ConsumeAndGet(remainingCapacity);
                                    if (ParticlesTaken > 0.0f)
                                    {
                                        hepTank.hepStorage.Store(ParticlesTaken);
                                    }
                                }
                            }
                        }
                        else if (fuelLoader.loaderType == LoaderType.Oxidizer)
                        {
                            GameObject[] AllOXItems = Concat(fuelLoader.solidStorage.items, fuelLoader.liquidStorage.items).ToArray();
                            for (int index = AllOXItems.Count() - 1; index >= 0; --index)
                            {
                                GameObject storageItem = AllOXItems[index];
                                foreach (OxidizerTank oxTank in OxidizerTanks)
                                {
                                    float remainingCapacity = oxTank.storage.RemainingCapacity();
                                    float num1 = oxTank.supportsMultipleOxidizers ? fuelLoader.solidStorage.MassStored() : fuelLoader.liquidStorage.MassStored();
                                    bool tagAllowed = oxTank.supportsMultipleOxidizers
                                        ? storageItem.GetComponent<KPrefabID>().HasAnyTags(oxTank.GetComponent<FlatTagFilterable>().selectedTags)
                                        : storageItem.HasTag(oxTank.GetComponent<ConduitConsumer>().capacityTag);
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && tagAllowed)
                                    {
                                        isLoading = true;
                                        HasLoadingProcess = true;
                                        Pickupable pickupable = storageItem.GetComponent<Pickupable>().Take(remainingCapacity);
                                        if (pickupable != null)
                                        {
                                            oxTank.storage.Store(pickupable.gameObject, true);
                                            //float num2 = remainingCapacity - pickupable.PrimaryElement.Mass;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (NormalLoaderComponent != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Load || smi2.SelectedMode == ModularConduitPortController.Mode.Both))
                    {
                        smi2.SetRocket(hasRocket: true);
                        for (int num = NormalLoaderComponent.Storage.items.Count - 1; num >= 0; num--)
                        {
                            GameObject gameObject = NormalLoaderComponent.Storage.items[num];
                            foreach (CargoBayCluster item2 in pooledDictionary[CargoBayConduit.ElementToCargoMap[NormalLoaderComponent.ConduitType]])
                            {
                                float remainingCapacity = item2.RemainingCapacity;
                                float num2 = NormalLoaderComponent.Storage.MassStored();
                                if (!(remainingCapacity <= 0f) && !(num2 <= 0f) && item2.GetComponent<TreeFilterable>().AcceptedTags.Contains(gameObject.PrefabID()))
                                {
                                    isLoading = true;
                                    HasLoadingProcess = true;
                                    Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(remainingCapacity);
                                    if (pickupable != null)
                                    {
                                        item2.storage.Store(pickupable.gameObject);
                                        remainingCapacity -= pickupable.PrimaryElement.Mass;
                                    }
                                }
                            }
                        }
                    }

                    smi2?.SetLoading(isLoading);
                }

                chain.Recycle();
                pooledDictionary[CargoBay.CargoType.Solids].Recycle();
                pooledDictionary[CargoBay.CargoType.Liquids].Recycle();
                pooledDictionary[CargoBay.CargoType.Gasses].Recycle();
                pooledDictionary.Recycle();
                this.sm.fillComplete.Set(!HasLoadingProcess, this.smi);
            }
        }
    }

}
