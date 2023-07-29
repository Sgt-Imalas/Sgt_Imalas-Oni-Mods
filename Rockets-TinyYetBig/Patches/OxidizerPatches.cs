using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using YamlDotNet.Core.Tokens;
using static Rockets_TinyYetBig.Patches.AnimationFixes;

namespace Rockets_TinyYetBig.Patches
{
    internal class OxidizerPatches
    {
        /// <summary>
        /// Add additional oxidizer efficiencies
        /// </summary>
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Init_Patch
        {
            public static void Postfix()
            {
                foreach(var efficiency in ModAssets.OxidizerEfficiencies.GetOxidizerEfficiencies())
                {
                    if (!Clustercraft.dlc1OxidizerEfficiencies.ContainsKey(efficiency.Key))
                    {
                        SgtLogger.l($"Added {efficiency.Key} to oxidizer efficiencies with a value of {efficiency.Value}");
                        Clustercraft.dlc1OxidizerEfficiencies.Add(efficiency.Key, efficiency.Value);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(OxidizerTank),nameof(OxidizerTank.GetOxidizersAvailable))]
        public static class OxidizerAddition
        {
            public static void Postfix(OxidizerTank __instance, Dictionary<Tag, float> __result )
            {
                foreach (var efficiency in ModAssets.OxidizerEfficiencies.GetOxidizerEfficiencies())
                {
                    if (!__result.ContainsKey(efficiency.Key))
                    {
                        __result.Add(efficiency.Key, __instance.storage.GetAmountAvailable(efficiency.Key));
                    }
                }
            }
        }
        [HarmonyPatch(typeof(OxidizerTankLiquidClusterConfig), nameof(OxidizerTankLiquidClusterConfig.DoPostConfigureComplete))]
        public static class SwapTagInLiquidOxidizer
        {
            public static void Postfix(GameObject go)
            {
                go.GetComponent<ConduitConsumer>().capacityTag = ModAssets.Tags.LOXTankOxidizer;
                go.GetComponent<Storage>().storageFilters = new List<Tag> { ModAssets.Tags.LOXTankOxidizer };

            }
        }
        [HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.GetTotalOxidizerAvailable))]
        public static class OxidizerAddition2
        {
            public static void Postfix(OxidizerTank __instance,ref float __result)
            {
                foreach (var efficiency in ModAssets.OxidizerEfficiencies.GetOxidizerEfficiencies())
                {
                    __result += __instance.storage.GetAmountAvailable(efficiency.Key);
                }
            }
        }
        [HarmonyPatch(typeof(OxidizerTank))]
        [HarmonyPatch(nameof(OxidizerTank.TotalOxidizerPower))]
        [HarmonyPatch(MethodType.Getter)]
        public static class OxidizerTankTagConversion
        {
            public static bool Prefix(OxidizerTank __instance, ref float __result)
            {
                __result = 0f;
                foreach (GameObject item in __instance.storage.items)
                {
                    PrimaryElement component = item.GetComponent<PrimaryElement>();

                    Tag OxidizerEfficiencyTag = null;
                    if (Clustercraft.dlc1OxidizerEfficiencies.ContainsKey(component.ElementID.CreateTag()))
                    {
                        OxidizerEfficiencyTag =  component.ElementID.CreateTag();
                    }
                    else
                    {
                        var OxidizerTagInOreTags = component.Element.oreTags.Intersect(ModAssets.OxidizerEfficiencies.GetOxidizerEfficiencies().Keys);
                        if (OxidizerTagInOreTags != null && OxidizerTagInOreTags.Count() > 0)
                        {
                            OxidizerEfficiencyTag = OxidizerTagInOreTags.First();
                        }
                    }

                    if(OxidizerEfficiencyTag == null)
                    {
                       continue;
                    }
                    __result += component.Mass * Clustercraft.dlc1OxidizerEfficiencies[OxidizerEfficiencyTag];
                }
                return false;
            }
        }
    }
}
