using HarmonyLib;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.RocketModulePatches
{
    class KeroseneEngine_AllCombustables_Patches
    {
        [HarmonyPatch(typeof(KeroseneEngineClusterConfig), "DoPostConfigureComplete")]
        public static class ReplaceFuelTagInPetrolRocket
        {
            public static void Postfix(GameObject go)
            {
                if (Config.Instance.EthanolEngines)
                {
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
                    Object.Destroy(go.GetComponent<ManualDeliveryKG>());
                }
                consumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
                go.AddOrGet<DropAllWorkable>().dropWorkTime = 60f;
            }

        }
    }
}