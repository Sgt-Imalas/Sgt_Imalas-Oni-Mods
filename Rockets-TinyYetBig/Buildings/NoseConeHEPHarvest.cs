using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings
{
    public class NoseConeHEPHarvest :
  GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>
    {
        public StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.BoolParameter canHarvest;
        public StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.FloatParameter lastHarvestTime;
        public GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State grounded;
        public NoseConeHEPHarvest.NotGroundedStates not_grounded;
        public const float HEPConsumtionRate = 0.25f;
        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            default_state = (StateMachine.BaseState)this.grounded;
            this.root.Enter((StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State.Callback)(smi => smi.CheckIfCanHarvest()));
            this.grounded.TagTransition(GameTags.RocketNotOnGround, (GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State)this.not_grounded)
                .Update((smi,dt) => smi.UpdateMeter());
            this.not_grounded
                .DefaultState(this.not_grounded.not_harvesting)
                .EventHandler(GameHashes.ClusterLocationChanged, (Func<NoseConeHEPHarvest.StatesInstance, KMonoBehaviour>)(smi => (KMonoBehaviour)Game.Instance), (StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State.Callback)(smi => smi.CheckIfCanHarvest())).EventHandler(GameHashes.OnStorageChange, (StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State.Callback)(smi => smi.CheckIfCanHarvest())).TagTransition(GameTags.RocketNotOnGround, this.grounded, true);
            this.not_grounded.not_harvesting.PlayAnim("loaded").ParamTransition<bool>((StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.Parameter<bool>)this.canHarvest, this.not_grounded.harvesting, GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.IsTrue).Enter((StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State.Callback)(smi => NoseConeHEPHarvest.StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject))).Update((System.Action<NoseConeHEPHarvest.StatesInstance, float>)((smi, dt) => smi.CheckIfCanHarvest()), UpdateRate.SIM_4000ms);
            this.not_grounded.harvesting.PlayAnim("deploying").Exit((StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State.Callback)(smi =>
            {
                smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Trigger(939543986, (object)null);
                NoseConeHEPHarvest.StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
            })).Enter((StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State.Callback)(smi =>
            {
                Clustercraft component = smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                component.AddTag(GameTags.POIHarvesting);
                component.Trigger(-1762453998, (object)null);
                NoseConeHEPHarvest.StatesInstance.AddHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject, smi.def.harvestSpeed);
            })).Exit((StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State.Callback)(smi => smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().RemoveTag(GameTags.POIHarvesting))).Update((System.Action<NoseConeHEPHarvest.StatesInstance, float>)((smi, dt) =>
            {
                smi.HarvestFromPOI(dt);
                double num = (double)this.lastHarvestTime.Set(Time.time, smi);
            }), UpdateRate.SIM_4000ms).ParamTransition<bool>((StateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.Parameter<bool>)this.canHarvest, this.not_grounded.not_harvesting, GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.IsFalse);
        }

        public class Def : StateMachine.BaseDef
        {
            public float harvestSpeed;
        }

        public class NotGroundedStates :
          GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State
        {
            public GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State not_harvesting;
            public GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.State harvesting;
        }

        public class StatesInstance :
          GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>.GameInstance
        {
            private MeterController meter;
            private HighEnergyParticleStorage storage;

            public StatesInstance(IStateMachineTarget master, NoseConeHEPHarvest.Def def)
              : base(master, def)
            {
                this.storage = this.GetComponent<HighEnergyParticleStorage>();
                this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition)new ConditionHasRadbolts(this.storage, 6000f));
                this.Subscribe(-1697596308, new System.Action<object>(this.UpdateMeter));
                this.meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
                this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
                this.UpdateMeter();
            }

            protected override void OnCleanUp()
            {
                base.OnCleanUp();
                this.Unsubscribe(-1697596308, new System.Action<object>(this.UpdateMeter));
            }

            public void UpdateMeter(object data = null) => this.meter.SetPositionPercent(this.storage.Particles / this.storage.Capacity());

            public void HarvestFromPOI(float dt)
            {
                Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                if (!this.CheckIfCanHarvest())
                    return;
                ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
                if ((UnityEngine.Object)atCurrentLocation == (UnityEngine.Object)null || (UnityEngine.Object)atCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>() == (UnityEngine.Object)null)
                    return;
                HarvestablePOIStates.Instance smi = atCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
                Dictionary<SimHashes, float> elementsWithWeights = smi.configuration.GetElementsWithWeights();
                float num1 = 0.0f;
                foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
                    num1 += keyValuePair.Value;
                foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
                {
                    Element elementByHash = ElementLoader.FindElementByHash(keyValuePair.Key);
                    if (!DiscoveredResources.Instance.IsDiscovered(elementByHash.tag))
                        DiscoveredResources.Instance.Discover(elementByHash.tag, elementByHash.GetMaterialCategoryTag());
                }
                float num2 = Mathf.Min(this.GetMaxExtractKGFromHEPsAvailable(), this.def.harvestSpeed * dt);
                float num3 = 0.0f;
                float num4 = 0.0f;
                float num5 = 0.0f;
                foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
                {
                    if ((double)num3 < (double)num2)
                    {
                        SimHashes key = keyValuePair.Key;
                        float num6 = keyValuePair.Value / num1;
                        float num7 = this.def.harvestSpeed * dt * num6;
                        num3 += num7;
                        Element elementByHash = ElementLoader.FindElementByHash(key);
                        CargoBay.CargoType stateToCargoType = CargoBay.ElementStateToCargoTypes[elementByHash.state & Element.State.Solid];
                        List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(stateToCargoType);
                        float b = num7;
                        foreach (CargoBayCluster cargoBayCluster in cargoBaysOfType)
                        {
                            float mass = Mathf.Min(cargoBayCluster.RemainingCapacity, b);
                            if ((double)mass != 0.0)
                            {
                                num4 += mass;
                                b -= mass;
                                switch (elementByHash.state & Element.State.Solid)
                                {
                                    case Element.State.Gas:
                                        cargoBayCluster.storage.AddGasChunk(key, mass, elementByHash.defaultValues.temperature, byte.MaxValue, 0, false);
                                        break;
                                    case Element.State.Liquid:
                                        cargoBayCluster.storage.AddLiquid(key, mass, elementByHash.defaultValues.temperature, byte.MaxValue, 0);
                                        break;
                                    case Element.State.Solid:
                                        cargoBayCluster.storage.AddOre(key, mass, elementByHash.defaultValues.temperature, byte.MaxValue, 0);
                                        break;
                                }
                            }
                            if ((double)b == 0.0)
                                break;
                        }
                        num5 += b;
                    }
                    else
                        break;
                }
                smi.DeltaPOICapacity(-num3);
                this.ConsumeParticles(num3 * HEPConsumtionRate);
                if ((double)num5 > 0.0)
                    component.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIWasting, (object)(float)((double)num5 / (double)dt));
                else
                    component.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIWasting);
                SaveGame.Instance.GetComponent<ColonyAchievementTracker>().totalMaterialsHarvestFromPOI += num3;
            }

            public void ConsumeParticles(float amount) => this.GetComponent<HighEnergyParticleStorage>().ConsumeIgnoringDisease(GameTags.HighEnergyParticle, amount);

            public float GetMaxExtractKGFromHEPsAvailable() => this.GetComponent<HighEnergyParticleStorage>().Particles / HEPConsumtionRate;

            public bool CheckIfCanHarvest()
            {
                Clustercraft component = this.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                {
                    this.sm.canHarvest.Set(false, this);
                    return false;
                }
                if ((double)this.master.GetComponent<HighEnergyParticleStorage>().Particles <= 0.0)
                {
                    this.sm.canHarvest.Set(false, this);
                    return false;
                }
                ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
                bool flag = false;
                if ((UnityEngine.Object)atCurrentLocation != (UnityEngine.Object)null && (bool)(UnityEngine.Object)atCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>())
                {
                    HarvestablePOIStates.Instance smi = atCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
                    if (!smi.POICanBeHarvested())
                    {
                        this.sm.canHarvest.Set(false, this);
                        return false;
                    }
                    foreach (KeyValuePair<SimHashes, float> elementsWithWeight in smi.configuration.GetElementsWithWeights())
                    {
                        Element elementByHash = ElementLoader.FindElementByHash(elementsWithWeight.Key);
                        CargoBay.CargoType stateToCargoType = CargoBay.ElementStateToCargoTypes[elementByHash.state & Element.State.Solid];
                        List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(stateToCargoType);
                        if (cargoBaysOfType != null && cargoBaysOfType.Count > 0)
                        {
                            foreach (CargoBayCluster cargoBayCluster in cargoBaysOfType)
                            {
                                if ((double)cargoBayCluster.storage.RemainingCapacity() > 0.0)
                                    flag = true;
                            }
                            if (flag)
                                break;
                        }
                    }
                }
                this.sm.canHarvest.Set(flag, this);
                return flag;
            }

            public static void AddHarvestStatusItems(GameObject statusTarget, float harvestRate) => statusTarget.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting, (object)harvestRate);

            public static void RemoveHarvestStatusItems(GameObject statusTarget) => statusTarget.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting);
        }
    }


}
