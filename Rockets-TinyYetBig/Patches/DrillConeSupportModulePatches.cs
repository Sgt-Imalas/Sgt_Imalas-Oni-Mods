using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    public class DrillConeSupportModulePatches
    {
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
                    else
                        diamondStorage.storageFilters.Add(SimHashes.Diamond.CreateTag());
                }
            }
        }

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
                harvestRate = (1f + SupportModuleCount * ((float)Config.Instance.DrillconeSupportBoost)/100f) * AddSpeedBuff.defaultMiningSpeed;
            }
        }

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
                    __instance.def.harvestSpeed = (1f + SupportModuleCount * ((float)Config.Instance.DrillconeSupportBoost) / 100f) * defaultMiningSpeed;
                }
            }
        }


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


        [HarmonyPatch(typeof(HarvestModuleSideScreen), "SimEveryTick")]
        public static class CorrectInfoScreenForSupportModules
        {
            public static void Flush()
            {
                lastPercentageState = -2f;
                lastMassStored = -2f;
                helperModules.Clear();
                drillerStorage = null;
                moduleInstance=null;
            }
            public static readonly List<DrillConeAssistentModule> helperModules= new List<DrillConeAssistentModule>();
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
                    if (drillerStorage!=null)
                    {
                        Capacity += drillerStorage.Capacity();
                        MassStored += drillerStorage.MassStored();
                    }
                    foreach(var module in helperModules)
                    {
                        Capacity += module.DiamondStorage.Capacity();
                        MassStored += module.DiamondStorage.MassStored();
                    }

                    __instance.TryGetComponent<HierarchyReferences>(out HierarchyReferences component1);
                    float miningProgress = moduleInstance.sm.canHarvest.Get(moduleInstance) ? (moduleInstance.timeinstate % 4f)/4f : -1f;

                    if (!Mathf.Approximately(miningProgress,lastPercentageState))
                    {
                        GenericUIProgressBar reference1 = component1.GetReference<GenericUIProgressBar>("progressBar");
                        reference1.SetFillPercentage(miningProgress > -1f ? miningProgress : 0f);
                        reference1.label.SetText(miningProgress > -1f ? (string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_IN_PROGRESS : (string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_STOPPED);
                        lastPercentageState = miningProgress;
                    }
                    if (!Mathf.Approximately(MassStored,lastMassStored))
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
