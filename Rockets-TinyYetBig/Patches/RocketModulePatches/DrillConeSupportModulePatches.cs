using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TUNING;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;

namespace Rockets_TinyYetBig.Patches.RocketModulePatches
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
            public static void Prefix(WorkerBase pilot, RocketControlStation.StatesInstance __instance)
            {
                AttributeConverter diggingSpeed = Db.Get().AttributeConverters.DiggingSpeed;
                if (pilot.TryGetComponent<AttributeConverters>(out var converters) && converters.GetConverter(diggingSpeed.Id) != null)
                {
                    ///digging converter is 10x as effective as rocket
                    float diggingSpeedValue = 1f + converters.GetConverter(diggingSpeed.Id).Evaluate() / 10f;
                    diggingSpeedValue = Mathf.Max(diggingSpeedValue, 0.1f);
                    SgtLogger.l("setting digging multiplier on rocket: " + diggingSpeedValue);
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
        /// Also add an extra status item that holds more information about the drill speed composition
        /// </summary>
        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.AddHarvestStatusItems))]
        public static class FixStatusItem
        {

            public static bool Prefix(GameObject statusTarget, ResourceHarvestModule.StatesInstance smi)
            {
                if (statusTarget == null)
                    return false;

                statusTarget.TryGetComponent<CraftModuleInterface>(out var CraftInterface);
                int SupportModuleCount = 0;
                foreach (var otherModule in CraftInterface.ClusterModules)
                {
                    if (otherModule.Get().TryGetComponent<DrillConeAssistentModule>(out var assistantModule))
                    {
                        ++SupportModuleCount;
                    }
                }
                //SgtLogger.debuglog(__instance + ", BooserCount: " + SupportModuleCount);

                float supportBoost = Config.Instance.DrillconeSupportSpeedBoost / 100f;
                float totalSupportBoost = SupportModuleCount * supportBoost;
                float pilotBoost = ModAssets.GetMiningPilotSkillMultiplier(CraftInterface.m_clustercraft);
                float totalRate = totalSupportBoost + pilotBoost;

                var baseHarvestSpeed = ModAssets.DefaultDrillconeHarvestSpeed;

                float harvestRate = baseHarvestSpeed * totalRate;

                //SgtLogger.l($"supportBoost: {supportBoost}, totalSupportBoost {totalSupportBoost}, pilotBoost {pilotBoost}, totalBoostPercentage {totalBoostPercentage}");

                if (statusTarget.TryGetComponent<KSelectable>(out var selectable))
                {

                    float boostPercentagePilot = pilotBoost < 1f ? (1f - pilotBoost) * -100f : (pilotBoost - 1) * 100f;
                    float supportModuleBoostPercentage = totalSupportBoost < 0 ? (1f - totalSupportBoost) * -100f : totalSupportBoost * 100f;
                    float totalRatePercentage = totalRate < 1f ? (1f - totalRate) * -1 : totalRate-1;

                    string tooltipString = RTB_MININGINFORMATIONBOONS.TOOLTIPINFO
                        .Replace("{RATEPERCENTAGE}", totalRatePercentage.ToString("0%"))
                        .Replace("{YIELDMASS}", harvestRate.ToString("0.00 Kg"))
                        .Replace("{DRILLMATERIALMASS}", (harvestRate * 0.05f).ToString("0.00 Kg"))
                        .Replace("{DRILLMATERIAL}", ElementLoader.GetElement(SimHashes.Diamond.CreateTag()).name);

                    if (!Mathf.Approximately(boostPercentagePilot, 0f))
                    {
                        tooltipString += RTB_MININGINFORMATIONBOONS.PILOTSKILL
                            .Replace("{BOOSTPERCENTAGE}", boostPercentagePilot.ToString("0"));
                        tooltipString += "\n";
                    }
                    if (SupportModuleCount > 1)
                    {
                        tooltipString += RTB_MININGINFORMATIONBOONS.SUPPORTMODULE
                            .Replace("{COUNT}", SupportModuleCount.ToString())
                            .Replace("{BOOSTPERCENTAGE}", supportModuleBoostPercentage.ToString("0"));
                        tooltipString += "\n";

                    }
                    else if (SupportModuleCount > 0)
                    {
                        tooltipString += RTB_MININGINFORMATIONBOONS.SUPPORTMODULESINGULAR
                            .Replace("{BOOSTPERCENTAGE}", supportModuleBoostPercentage.ToString("0"));
                        tooltipString += "\n";
                    }


                    selectable.AddStatusItem(ModStatusItems.RTB_MiningInformationBoons, new Tuple<float, string>(totalRatePercentage, tooltipString));

					//selectable.SetStatusItem(Db.Get().StatusItemCategories.Suffocation, ModAssets.StatusItems.RTB_MiningInformationBoons, new Tuple<float, string>(harvestRate, tooltipString));

					selectable.AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting, new Tuple<Clustercraft, float>(CraftInterface.m_clustercraft,harvestRate));
				}

                return false;
            }

        }
        /// <summary>
        /// Removes the boost status item tooltip
        /// </summary>
        [HarmonyPatch(typeof(ResourceHarvestModule.StatesInstance), nameof(ResourceHarvestModule.StatesInstance.RemoveHarvestStatusItems))]
        public static class RemoveDrillInfoStatusItem
        {
            public static void Prefix(GameObject statusTarget)
            {
                if (statusTarget.TryGetComponent<KSelectable>(out var selectable))
                {
                    selectable.RemoveStatusItem(ModStatusItems.RTB_MiningInformationBoons);
                }
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
                if (cargoType == CargoBay.CargoType.Solids)
                {
                    __result.RemoveAll(entry => entry.TryGetComponent<FridgeModule>(out _));
                }
            }
        }

    }
}
