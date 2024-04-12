using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;

namespace Rockets_TinyYetBig.Patches
{
    public class DrillConeSupportModulePatches
    {
        /// <summary>
        /// Add Loadable Component to Drillcone + storage filters for diamonds if not exists
        /// </summary>
        [HarmonyPatch(typeof(NoseconeHarvestConfig), "ConfigureBuildingTemplate")]
        public static class AddFilterForLoaders
        {
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<DrillConeModeHandler>();
                if (go.TryGetComponent<Storage>(out var diamondStorage))
                {
                    if (diamondStorage.storageFilters == null)
                        diamondStorage.storageFilters = new List<Tag>() { SimHashes.Diamond.CreateTag() };
                    else if (!diamondStorage.storageFilters.Contains(SimHashes.Diamond.CreateTag()))
                        diamondStorage.storageFilters.Add(SimHashes.Diamond.CreateTag());
                }
            }
        }

        /// <summary>
        /// Get Mining Skill modifier, handle legacy AI rockets with 0
        /// </summary>
        [HarmonyPatch(typeof(RocketControlStation.StatesInstance))]
        [HarmonyPatch("SetPilotSpeedMult")]
        public class RocketControlStation_SetPilotSpeedMult_Patch
        {
            [HarmonyPriority(Priority.HigherThanNormal)]
            public static void Prefix(Worker pilot, RocketControlStation.StatesInstance __instance)
            {
                AttributeConverter diggingSpeed = Db.Get().AttributeConverters.DiggingSpeed;
                if (pilot.TryGetComponent<AttributeConverters>(out var converters) && converters.GetConverter(diggingSpeed.Id) != null)
                {
                    ///digging converter is 10x as effective as rocket
                    float diggingSpeedValue = 1f + (converters.GetConverter(diggingSpeed.Id).Evaluate() / 10f);
                    diggingSpeedValue = Mathf.Max(diggingSpeedValue, 0.1f);
                    SgtLogger.l("setting digging multiplier on rocket: "+ diggingSpeedValue);
                    __instance.smi.sm.clusterCraft.Get(__instance.smi).GetComponent<Clustercraft_AdditionalComponent>().SetMiningMultiplierForRocket(diggingSpeedValue);

                }
            }
            public static Clustercraft GetRocket(RocketControlStation.StatesInstance smi)
            {
                WorldContainer world = ClusterManager.Instance.GetWorld(smi.GetMyWorldId());
                if (world == null)
                {
                    return null;
                }
                return world.gameObject.GetComponent<Clustercraft>();
            }
        }

        /// <summary>
        /// Adjusts the drilling status item to include support module speedboost in displayed number
        /// </summary>
        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), "AddHarvestStatusItems")]
        public static class FixStatusItem
        {
            public static void Prefix(GameObject statusTarget, ref float harvestRate)
            {
                statusTarget.TryGetComponent<CraftModuleInterface>(out var CraftInterface);
                float SupportModuleCount = 0;
                foreach (var otherModule in CraftInterface.ClusterModules)
                {
                    if (otherModule.Get().TryGetComponent<DrillConeAssistentModule>(out var assistantModule))
                    {
                        ++SupportModuleCount;
                    }
                }
                //SgtLogger.debuglog(__instance + ", BooserCount: " + SupportModuleCount);
                harvestRate = (1f + SupportModuleCount * ((float)Config.Instance.DrillconeSupportBoost) / 100f) * AddSpeedBuff.defaultMiningSpeed * ModAssets.GetMiningPilotSkillMultiplier(CraftInterface.m_clustercraft);
            }
        }

        /// <summary>
        /// Add drillcone info component to clustercraft prefab
        /// </summary>
        [HarmonyPatch(typeof(ClustercraftConfig), nameof(ClustercraftConfig.CreatePrefab))]
        public static class MiningBuffStorage
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGet<Clustercraft_AdditionalComponent>();
            }
        }

        /// <summary>
        /// Exclude food cargo bays from "GetCargoBaysOfType" call to prevent mined resources getting loaded into them
        /// </summary>
        [HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.GetCargoBaysOfType))]
        public static class Clustercraft_GetCargoBaysOfType
        {
            public static void Postfix(CargoBay.CargoType cargoType, List<CargoBayCluster> __result)
            {
                if(cargoType == CargoBay.CargoType.Solids)
                {
                    __result.RemoveAll(entry => entry.TryGetComponent<FridgeModule>(out _));
                }
            }
        }



        /// <summary>
        /// Adjusts the drilling speed of drillcone SMI to include support module speed boosts
        /// </summary>
        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), "HarvestFromPOI")]
        public static class AddSpeedBuff
        {
            public static float defaultMiningSpeed = ROCKETRY.SOLID_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE / 3600f;
            public static void Prefix(ResourceHarvestModule.StatesInstance __instance)
            {
                if (__instance.gameObject.TryGetComponent<RocketModuleCluster>(out var Module))
                {
                    float SupportModuleCount = 0;
                    foreach (var otherModule in Module.CraftInterface.ClusterModules)
                    {
                        if (otherModule.Get().TryGetComponent<DrillConeAssistentModule>(out var assistantModule))
                        {
                            ++SupportModuleCount;
                        }
                    }
                    //SgtLogger.debuglog(__instance + ", BooserCount: " + SupportModuleCount);
                    __instance.def.harvestSpeed = (1f + SupportModuleCount * ((float)Config.Instance.DrillconeSupportBoost) / 100f) * defaultMiningSpeed * ModAssets.GetMiningPilotSkillMultiplier(Module.CraftInterface.m_clustercraft);
                }
            }
        }

        /// <summary>
        /// caches the support modules for this drillcone on selection
        /// </summary>
        [HarmonyPatch(typeof(HarvestModuleSideScreen), "SetTarget")]
        public static class TargetSetterPatch
        {
            public static void Postfix(GameObject target)
            {
                CorrectInfoScreenForSupportModules.Flush();
                var craft = target.GetComponent<Clustercraft>();

                foreach (var otherModule in craft.ModuleInterface.ClusterModules)
                {
                    GameObject gameObject = otherModule.Get().gameObject;
                    if (gameObject.GetDef<ResourceHarvestModule.Def>() != null)
                    {
                        var instance = gameObject.GetSMI<ResourceHarvestModule.StatesInstance>();
                        CorrectInfoScreenForSupportModules.moduleInstance = instance;
                        if (instance.gameObject.TryGetComponent<Storage>(out var storageOnTarget))
                        {
                            CorrectInfoScreenForSupportModules.drillerStorage = storageOnTarget;
                        }
                    }

                    if (otherModule.Get().TryGetComponent<DrillConeAssistentModule>(out var assistantModule))
                    {
                        CorrectInfoScreenForSupportModules.helperModules.Add(assistantModule);
                    }
                }
            }
        }

        /// <summary>
        /// Way more efficient replacement for drillcone sidescreen that also includes speedboost and capacity increase from support modules
        /// </summary>
        [HarmonyPatch(typeof(HarvestModuleSideScreen), "SimEveryTick")]
        public static class CorrectInfoScreenForSupportModules
        {
            public static void Flush()
            {
                lastPercentageState = -2f;
                lastMassStored = -2f;
                helperModules.Clear();
                drillerStorage = null;
                moduleInstance = null;
            }
            public static readonly List<DrillConeAssistentModule> helperModules = new List<DrillConeAssistentModule>();
            static float globalDT = 0f;
            const float dtGate = 1 / 5f;
            static float lastPercentageState = -1f;
            static float lastMassStored = -1f;
            public static Storage drillerStorage = null;
            public static ResourceHarvestModule.StatesInstance moduleInstance;

            public static bool Prefix(float dt, HarvestModuleSideScreen __instance, Clustercraft ___targetCraft)
            {
                if (globalDT < dtGate)
                {
                    globalDT += dt;
                }
                else
                {
                    globalDT -= dtGate;

                    if (___targetCraft.IsNullOrDestroyed())
                        return false;

                    float Capacity = 0, MassStored = 0;
                    if (drillerStorage != null)
                    {
                        Capacity += drillerStorage.Capacity();
                        MassStored += drillerStorage.MassStored();
                    }
                    foreach (var module in helperModules)
                    {
                        Capacity += module.DiamondStorage.Capacity();
                        MassStored += module.DiamondStorage.MassStored();
                    }

                    __instance.TryGetComponent<HierarchyReferences>(out HierarchyReferences component1);
                    float miningProgress = moduleInstance.sm.canHarvest.Get(moduleInstance) ? (moduleInstance.timeinstate % 4f) / 4f : -1f;

                    if (!Mathf.Approximately(miningProgress, lastPercentageState))
                    {
                        GenericUIProgressBar reference1 = component1.GetReference<GenericUIProgressBar>("progressBar");
                        reference1.SetFillPercentage(miningProgress > -1f ? miningProgress : 0f);
                        reference1.label.SetText(miningProgress > -1f ? (string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_IN_PROGRESS : (string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_STOPPED);
                        lastPercentageState = miningProgress;
                    }
                    if (!Mathf.Approximately(MassStored, lastMassStored))
                    {
                        GenericUIProgressBar reference2 = component1.GetReference<GenericUIProgressBar>("diamondProgressBar");
                        reference2.SetFillPercentage(MassStored / Capacity);
                        reference2.label.SetText(ElementLoader.GetElement(SimHashes.Diamond.CreateTag()).name + ": " + GameUtil.GetFormattedMass(MassStored));
                        lastMassStored = MassStored;
                    }
                }
                return false;
            }
        }
    }
}
