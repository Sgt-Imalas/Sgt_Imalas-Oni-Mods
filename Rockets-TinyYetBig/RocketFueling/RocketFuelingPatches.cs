using HarmonyLib;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.NonRocketBuildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
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
            public static float TotalMassStoredOfItems(IEnumerable<GameObject> items)
            {
                float num = 0f;
                foreach (var item in items)
                {
                    if (!(item == null))
                    {
                        PrimaryElement component = item.GetComponent<PrimaryElement>();
                        if (component != null)
                        {
                            num += component.Units * component.MassPerUnit;
                        }
                    }
                }

                return (float)Mathf.RoundToInt(num * 1000f) / 1000f;
            }

            public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] arr)
            {
                foreach (IEnumerable col in arr)
                    foreach (T item in col)
                        yield return item;
            }
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
                    //IConduitConsumer conduitConsumer = smi1.GetComponent<IConduitConsumer>();
                    FuelLoaderComponent fuelLoader = smi1.GetComponent<FuelLoaderComponent>();
                    bool isLoading = false;
                    if (fuelLoader != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Load))
                    {
                        shouldDoNormal = false;
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
                                    float num1 = TotalMassStoredOfItems(AllItems);
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && storageItem.HasTag(FuelTag))
                                    {
                                        isLoading = true;
                                        flag = true;
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
                                    flag = true;
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
                                foreach (OxidizerTank oxTank in LiquidOxidizerTanks)
                                {
                                    float remainingCapacity = oxTank.storage.RemainingCapacity();
                                    float num1 = fuelLoader.solidStorage.MassStored();
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && storageItem.HasTag(oxTank.GetComponent<ConduitConsumer>().capacityTag))
                                    {
                                        isLoading = true;
                                        flag = true;
                                        Pickupable pickupable = storageItem.GetComponent<Pickupable>().Take(remainingCapacity);
                                        if (pickupable != null)
                                        {
                                            oxTank.storage.Store(pickupable.gameObject, true);
                                            //float num2 = remainingCapacity - pickupable.PrimaryElement.Mass;
                                        }
                                    }
                                }
                                foreach (OxidizerTank oxTank in SolidOxidizerTanks)
                                {
                                    float remainingCapacity = oxTank.storage.RemainingCapacity();
                                    float num1 = fuelLoader.solidStorage.MassStored();
                                    if ((double)remainingCapacity > 0.0 && (double)num1 > 0.0 && storageItem.GetComponent<KPrefabID>().HasAnyTags(oxTank.GetComponent<FlatTagFilterable>().selectedTags))
                                    {
                                        isLoading = true;
                                        flag = true;
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
                        smi2?.SetLoading(isLoading);
                    }
                }

                chain.Recycle();
                return shouldDoNormal;
            }
        }
    }
}
