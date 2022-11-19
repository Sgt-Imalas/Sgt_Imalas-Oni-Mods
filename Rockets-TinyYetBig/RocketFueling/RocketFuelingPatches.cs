using HarmonyLib;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.RocketFueling.FuelLoaderComponent;
using static STRINGS.UI.STARMAP;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class RocketFuelingPatches
    {
        [HarmonyPatch(typeof(LaunchPadMaterialDistributor.Instance))]
        [HarmonyPatch(nameof(LaunchPadMaterialDistributor.Instance.FillRocket))]
        public class AddFuelingLogic
        {
            public static bool Prefix(LaunchPadMaterialDistributor.Instance __instance)
            {
                bool shouldDoNormal = true;
                var clusterRocketTargetParam = (StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.TargetParameter)
                    typeof(LaunchPadMaterialDistributor).GetField("attachedRocket", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance.sm);

                CraftModuleInterface craftInterface = clusterRocketTargetParam.Get<RocketModuleCluster>(__instance).CraftInterface;
                Clustercraft clustercraft = craftInterface.GetComponent<Clustercraft>();

                List<FuelTank> FuelTanks = new List<FuelTank>();
                List<OxidizerTank> SolidOxidizerTanks = new List<OxidizerTank>();
                List<OxidizerTank> LiquidOxidizerTanks = new List<OxidizerTank>();
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
                    OxidizerTank oxTank = clusterModule.Get().GetComponent<OxidizerTank>();
                    if (oxTank != null)
                    {
                        hasOxidizer = true;
                        if (oxTank.supportsMultipleOxidizers)
                            SolidOxidizerTanks.Add(oxTank);
                        else
                            LiquidOxidizerTanks.Add(oxTank);
                    }
                }
                bool flag = false;
                HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
                __instance.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
                foreach (ChainedBuilding.StatesInstance smi1 in (HashSet<ChainedBuilding.StatesInstance>)chain)
                {
                    ModularConduitPortController.Instance smi2 = smi1.GetSMI<ModularConduitPortController.Instance>();
                    IConduitConsumer conduitConsumer = smi1.GetComponent<IConduitConsumer>();
                    FuelLoaderComponent fuelLoader = smi1.GetComponent<FuelLoaderComponent>();
                    bool isLoading = false;
                    if (conduitConsumer != null && fuelLoader != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Load || smi2.SelectedMode == ModularConduitPortController.Mode.Both))
                    {
                        shouldDoNormal = false;
                        smi2.SetRocket(true);
                        for (int index = conduitConsumer.Storage.items.Count - 1; index >= 0; --index)
                        {
                            GameObject go = conduitConsumer.Storage.items[index];
                            if (fuelLoader.loaderType == LoaderType.Fuel)
                            {
                                foreach (FuelTank fueltank in FuelTanks)
                                {
                                    float remainingCapacity = fueltank.Storage.RemainingCapacity();
                                    float num1 = conduitConsumer.Storage.MassStored();
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && go.HasTag(FuelTag))
                                    {
                                        isLoading = true;
                                        flag = true;
                                        Pickupable pickupable = go.GetComponent<Pickupable>().Take(remainingCapacity);
                                        if (pickupable != null)
                                        {
                                            fueltank.storage.Store(pickupable.gameObject, true);
                                            float num2 = remainingCapacity - pickupable.PrimaryElement.Mass;
                                        }
                                    }
                                }
                            }
                            else if (fuelLoader.loaderType == LoaderType.LiquidOx)
                            {
                                foreach (OxidizerTank oxTank in LiquidOxidizerTanks)
                                {
                                    float remainingCapacity = oxTank.storage.RemainingCapacity();
                                    float num1 = conduitConsumer.Storage.MassStored();
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && go.HasTag(oxTank.GetComponent<ConduitConsumer>().capacityTag))
                                    {
                                        isLoading = true;
                                        flag = true;
                                        Pickupable pickupable = go.GetComponent<Pickupable>().Take(remainingCapacity);
                                        if (pickupable != null)
                                        {
                                            oxTank.storage.Store(pickupable.gameObject,true);
                                            float num2 = remainingCapacity - pickupable.PrimaryElement.Mass;
                                        }

                                    }
                                }
                            }
                            else if (fuelLoader.loaderType == LoaderType.SolidOx)
                            {
                                foreach (OxidizerTank oxTank in SolidOxidizerTanks)
                                {
                                    float remainingCapacity = oxTank.storage.RemainingCapacity();
                                    float num1 = conduitConsumer.Storage.MassStored();
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && go.GetComponent<KPrefabID>().HasAnyTags(oxTank.GetComponent<FlatTagFilterable>().selectedTags))
                                    {
                                        isLoading = true;
                                        flag = true;
                                        Pickupable pickupable = go.GetComponent<Pickupable>().Take(remainingCapacity);
                                        if (pickupable != null)
                                        {
                                            oxTank.storage.Store(pickupable.gameObject, true);
                                            float num2 = remainingCapacity - pickupable.PrimaryElement.Mass;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    smi2?.SetLoading(isLoading);
                }
                chain.Recycle();
                return shouldDoNormal;
            }
        }
    }
}
