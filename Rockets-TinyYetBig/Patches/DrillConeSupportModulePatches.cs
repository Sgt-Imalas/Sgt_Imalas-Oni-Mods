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
    internal class DrillConeSupportModulePatches
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
                //Debug.Log(__instance + ", BooserCount: " + SupportModuleCount);
                harvestRate = (1f + SupportModuleCount * 0.2f) * AddSpeedBuff.defaultMiningSpeed;
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
                    //Debug.Log(__instance + ", BooserCount: " + SupportModuleCount);
                    __instance.def.harvestSpeed = (1f + SupportModuleCount * 0.2f) * defaultMiningSpeed;
                }
            }
        }

        [HarmonyPatch(typeof(HarvestModuleSideScreen), "SimEveryTick")]
        public static class CorrectInfoScreen
        {
            static float globalDT = 0f;
            const float dtGate = 1 / 5f;

            public static bool Prefix(float dt, HarvestModuleSideScreen __instance, Clustercraft ___targetCraft)
            {
                Debug.Log(dt+", global dt == "+globalDT);
                if (globalDT < dtGate)
                {
                    globalDT += dt;
                    return false;
                }
                else
                {
                    globalDT -= dtGate;

                    if (___targetCraft.IsNullOrDestroyed())
                        return false;

                    float Capacity = 0, MassStored = 0;
                    ResourceHarvestModule.StatesInstance instance = null;
                    if (___targetCraft.TryGetComponent<CraftModuleInterface>(out var @interface))
                    {
                        foreach (Ref<RocketModuleCluster> clusterModule in @interface.ClusterModules)
                        {
                            GameObject gameObject = clusterModule.Get().gameObject;
                            if (gameObject.GetDef<ResourceHarvestModule.Def>() != null)
                            {
                                instance = gameObject.GetSMI<ResourceHarvestModule.StatesInstance>();
                                
                                if(instance.gameObject.TryGetComponent<Storage>(out var storageOnTarget))
                                {
                                    Capacity += storageOnTarget.Capacity();
                                    MassStored += storageOnTarget.MassStored();
                                }
                            }
                            if (gameObject.TryGetComponent<DrillConeAssistentModule>(out var module))
                            {

                                Capacity += module.DiamondStorage.Capacity();
                                MassStored += module.DiamondStorage.MassStored();
                            }
                        }
                        if (instance == null)
                            return false;
                    }
                    else
                    {
                        return false;
                    }

                    __instance.TryGetComponent<HierarchyReferences>(out HierarchyReferences component1);

                    GenericUIProgressBar reference1 = component1.GetReference<GenericUIProgressBar>("progressBar");
                    float miningProgress = instance.timeinstate % 4f;
                    if (instance.sm.canHarvest.Get(instance))
                    {
                        reference1.SetFillPercentage(miningProgress / 4f);
                        reference1.label.SetText((string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_IN_PROGRESS);
                    }
                    else
                    {
                        reference1.SetFillPercentage(0.0f);
                        reference1.label.SetText((string)global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_STOPPED);
                    }

                    GenericUIProgressBar reference2 = component1.GetReference<GenericUIProgressBar>("diamondProgressBar");

                    reference2.SetFillPercentage(MassStored / Capacity);
                    reference2.label.SetText(ElementLoader.GetElement(SimHashes.Diamond.CreateTag()).name + ": " + GameUtil.GetFormattedMass(MassStored));
                    return false;
                }
            }
        }
    }
}
