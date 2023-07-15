using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    class PetrolRocketUseAllCombustiblesPatch
    {
        [HarmonyPatch(typeof(KeroseneEngineClusterConfig), "DoPostConfigureComplete")]
        public static class ReplaceFuelTagInPetrolRocket
        {
            public static void Postfix(GameObject go)
            {
                if (Config.Instance.EthanolEngines) { 
                RocketEngineCluster rocketEngineCluster = go.GetComponent<RocketEngineCluster>();
                rocketEngineCluster.fuelTag = GameTags.CombustibleLiquid;
                }
            }

        }
        [HarmonyPatch(typeof(KeroseneEngineClusterSmallConfig), "DoPostConfigureComplete")]
        public static class ReplaceFuelTagInSmallPetrolRocket
        {
            public static void Postfix(GameObject go)
            {
                FuelTank tank = go.GetComponent<FuelTank>();
                RocketEngineCluster rocketEngineCluster = go.GetComponent<RocketEngineCluster>();
                if (Config.Instance.EthanolEngines)
                {
                    rocketEngineCluster.fuelTag = GameTags.CombustibleLiquid;
                    tank.FuelType = GameTags.CombustibleLiquid;
                }
                ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                conduitConsumer.conduitType = ConduitType.Liquid;
                conduitConsumer.consumptionRate = 10f;
                conduitConsumer.capacityTag = rocketEngineCluster.fuelTag;
                conduitConsumer.capacityKG = tank.storage.capacityKg;
                conduitConsumer.forceAlwaysSatisfied = true;
                conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            }

        }
        [HarmonyPatch(typeof(LiquidFuelTankClusterConfig), nameof(LiquidFuelTankClusterConfig.DoPostConfigureComplete))]
        public static class ReplaceFuelTagInBigFuelTankRocket
        {
            public static void Postfix(GameObject go)
            {
                var consumer = go.GetComponent<ConduitConsumer>();
                if (Config.Instance.EthanolEngines)
                {
                    consumer.capacityTag = ModAssets.Tags.RocketFuelTag;
                    UnityEngine.Object.Destroy(go.GetComponent<ManualDeliveryKG>());
                }
                consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
                go.AddOrGet<DropAllWorkable>().dropWorkTime = 60f;
            }

        }
        [HarmonyPatch(typeof(Localization))]
        [HarmonyPatch("Initialize")]
        public static class StringReplacementPetroleum
        {
            public static void Prefix()
            {
                if (Config.Instance.EthanolEngines)
                {
                    global::STRINGS.BUILDINGS.PREFABS.KEROSENEENGINECLUSTER.EFFECT = STRINGS.MODIFIEDVANILLASTRINGS.KEROSENEENGINECLUSTER_EFFECT;
                    global::STRINGS.BUILDINGS.PREFABS.KEROSENEENGINECLUSTERSMALL.EFFECT = STRINGS.MODIFIEDVANILLASTRINGS.KEROSENEENGINECLUSTERSMALL_EFFECT;
                }
                
            }

        }
    }
}
//