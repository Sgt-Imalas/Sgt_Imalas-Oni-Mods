using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Nosecones
{
    public class NoseConeHEPHarvest :
  GameStateMachine<NoseConeHEPHarvest, NoseConeHEPHarvest.StatesInstance, IStateMachineTarget, NoseConeHEPHarvest.Def>
    {
        public BoolParameter canHarvest;
        public FloatParameter lastHarvestTime;
        public State grounded;
        public NotGroundedStates not_grounded;
        public const float HEPConsumptionRate = 0.25f;
        public override void InitializeStates(out BaseState default_state)
        {
            default_state = grounded;
            root.Enter(smi => smi.CheckIfCanHarvest());
            grounded.TagTransition(GameTags.RocketNotOnGround, not_grounded)
                .Update((smi, dt) => smi.UpdateMeter());
            not_grounded
                .DefaultState(not_grounded.not_harvesting)
                .EventHandler(GameHashes.ClusterLocationChanged, smi => Game.Instance, smi => smi.CheckIfCanHarvest()).EventHandler(GameHashes.OnStorageChange, smi => smi.CheckIfCanHarvest()).TagTransition(GameTags.RocketNotOnGround, grounded, true);
            not_grounded.not_harvesting.PlayAnim("loaded").ParamTransition(canHarvest, not_grounded.harvesting, IsTrue).Enter(smi => StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject)).Update((smi, dt) => smi.CheckIfCanHarvest(), UpdateRate.SIM_4000ms);
            not_grounded.harvesting.PlayAnim("deploying")
                .Exit(smi =>
                {
                    smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Trigger(939543986, null);
                    StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
                }).Enter(smi =>
                {
                    Clustercraft component = smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                    component.AddTag(GameTags.POIHarvesting);
                    component.Trigger(-1762453998, null);
                    StatesInstance.AddHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject, smi.def.harvestSpeed);
                }).Exit(smi => smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().RemoveTag(GameTags.POIHarvesting))

            .Update((smi, dt) =>
            {
                smi.HarvestFromPOI(dt);
                double num = (double)lastHarvestTime.Set(Time.time, smi);
            }, UpdateRate.SIM_4000ms).ParamTransition(canHarvest, not_grounded.not_harvesting, IsFalse);
        }

        public class Def : BaseDef
        {
            public float harvestSpeed;
        }

        public class NotGroundedStates :
          State
        {
            public State not_harvesting;
            public State harvesting;
        }

        public class StatesInstance :
          GameInstance
        {
            private MeterController meter;
            private HighEnergyParticleStorage storage;

            public StatesInstance(IStateMachineTarget master, Def def)
              : base(master, def)
            {
                storage = GetComponent<HighEnergyParticleStorage>();
                GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new  ConditionHasRadbolts(storage, 6000f));
                Subscribe(-1697596308, new Action<object>(UpdateMeter));
                meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
                meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
                UpdateMeter();
            }

            public override void OnCleanUp()
            {
                base.OnCleanUp();
                Unsubscribe(-1697596308, new Action<object>(UpdateMeter));
            }

            public void UpdateMeter(object data = null)
            {
                meter.SetPositionPercent(storage.Particles / storage.Capacity());
            }

            public void HarvestFromPOI(float dt)
            {
                Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                if (!CheckIfCanHarvest())
                    return;
                ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
                if (atCurrentLocation == null || atCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>() == null)
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
                float harvestSpeedDT = Mathf.Min(GetMaxExtractKGFromHEPsAvailable(), def.harvestSpeed * dt);
                float consumptionMass = 0.0f;
                float num4 = 0.0f;
                float wastedHarvestingMass = 0.0f;
                foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
                {
                    if ((double)consumptionMass < (double)harvestSpeedDT)
                    {
                        SimHashes key = keyValuePair.Key;
                        float num6 = keyValuePair.Value / num1;
                        float num7 = def.harvestSpeed * dt * num6;
                        consumptionMass += num7;
                        Element elementByHash = ElementLoader.FindElementByHash(key);
                        CargoBay.CargoType stateToCargoType = CargoBay.ElementStateToCargoTypes[elementByHash.state & Element.State.Solid];
                        List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(stateToCargoType);
                        float mineMassPerTick = num7;
                        foreach (CargoBayCluster cargoBayCluster in cargoBaysOfType)
                        {
                            float mass = Mathf.Min(cargoBayCluster.RemainingCapacity, mineMassPerTick);
                            if ((double)mass != 0.0)
                            {
                                num4 += mass;
                                mineMassPerTick -= mass;
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
                            if ((double)mineMassPerTick == 0.0)
                                break;
                        }
                        wastedHarvestingMass += mineMassPerTick;
                    }
                    else
                        break;
                }
                smi.DeltaPOICapacity(-consumptionMass);
                ConsumeParticles(consumptionMass * HEPConsumptionRate);
                if ((double)wastedHarvestingMass > 0.0)
                    component.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIWasting, (float)((double)wastedHarvestingMass / (double)dt));
                else
                    component.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIWasting);
                SaveGame.Instance.GetComponent<ColonyAchievementTracker>().totalMaterialsHarvestFromPOI += consumptionMass;
            }

            public void ConsumeParticles(float amount) => GetComponent<HighEnergyParticleStorage>().ConsumeIgnoringDisease(GameTags.HighEnergyParticle, amount);

            public float GetMaxExtractKGFromHEPsAvailable() => GetComponent<HighEnergyParticleStorage>().Particles / HEPConsumptionRate;

            public bool CheckIfCanHarvest()
            {
                Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                if (component == null)
                {
                    sm.canHarvest.Set(false, this);
                    return false;
                }
                //var UpdateMethod = typeof(RocketClusterDestinationSelector).GetMethod("OnStorageChange", BindingFlags.NonPublic | BindingFlags.Instance);
                //var loopmaster = component.GetComponent<RocketClusterDestinationSelector>();

                if ((double)GetMaxExtractKGFromHEPsAvailable() <= 0.0)
                {
                    //UpdateMethod.Invoke(loopmaster, new[] { (System.Object)null });
                    sm.canHarvest.Set(false, this);
                    return false;
                }
                ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
                bool flag = false;
                if (atCurrentLocation != null && (bool)atCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>())
                {
                    HarvestablePOIStates.Instance smi = atCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
                    if (!smi.POICanBeHarvested())
                    {
                        sm.canHarvest.Set(false, this);
                        //UpdateMethod.Invoke(loopmaster, new[] { (System.Object)null });
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
                sm.canHarvest.Set(flag, this);
                return flag;
            }

            public static void AddHarvestStatusItems(GameObject statusTarget, float harvestRate) => statusTarget.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting, harvestRate);

            public static void RemoveHarvestStatusItems(GameObject statusTarget) => statusTarget.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting);
        }
    }


}
