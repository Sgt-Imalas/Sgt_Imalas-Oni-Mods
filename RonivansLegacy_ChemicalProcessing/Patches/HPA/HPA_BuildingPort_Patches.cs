using HarmonyLib;
using Klei.AI;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class HPA_BuildingPort_Patches
    {
		static void IncreaseRocketConduitTarget(RocketConduitStorageAccess cmp)
        {
			float tweakedCapacity;

			switch (cmp.cargoType)
            {
                case CargoBay.CargoType.Liquids:
                    tweakedCapacity = Config.Instance.HPA_Capacity_Liquid;
					cmp.storage.capacityKg = tweakedCapacity;
					if(cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					break;
                case CargoBay.CargoType.Gasses:
					tweakedCapacity = Config.Instance.HPA_Capacity_Gas;
					cmp.storage.capacityKg = tweakedCapacity;
					if (cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					break;
				case CargoBay.CargoType.Solids:
					tweakedCapacity = Config.Instance.HPA_Capacity_Solid;
					cmp.storage.capacityKg = tweakedCapacity;
					if (cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					break;
			}
        }
		public static void IncreaseConsumerInput(ConduitConsumer __instance, bool increaseStorage)
		{
			if (__instance.conduitType == ConduitType.Gas)
			{
				float flowRate = Config.Instance.HPA_Capacity_Gas / ConduitFlow.MAX_GAS_MASS;
				if (increaseStorage)
					__instance.GetComponent<Storage>().capacityKg *= flowRate;
				__instance.consumptionRate *= flowRate;
			}
			else if (__instance.conduitType == ConduitType.Liquid)
			{
				float flowRate = Config.Instance.HPA_Capacity_Liquid / ConduitFlow.MAX_LIQUID_MASS;
				if (increaseStorage)
					__instance.GetComponent<Storage>().capacityKg *= flowRate;
				__instance.consumptionRate *= flowRate;
			}
		}


		[HarmonyPatch(typeof(LiquidConditionerConfig), nameof(LiquidConditionerConfig.ConfigureBuildingTemplate))]
		public class LiquidConditionerConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(GameObject go)
			{
				IncreaseConsumerInput(go.GetComponent<ConduitConsumer>(),true);
			}
		}

		[HarmonyPatch(typeof(AirConditionerConfig), nameof(AirConditionerConfig.ConfigureBuildingTemplate))]
		public class AirConditionerConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(GameObject go)
			{
				IncreaseConsumerInput(go.GetComponent<ConduitConsumer>(), false);
			}
		}

		[HarmonyPatch(typeof(RocketConduitStorageAccess), nameof(RocketConduitStorageAccess.OnSpawn))]
		public class RocketConduitStorageAccess_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(RocketConduitStorageAccess __instance)
			{
				IncreaseRocketConduitTarget(__instance);
			}
		}

		[HarmonyPatch(typeof(OperationalValve), nameof(OperationalValve.OnSpawn))]
        public class OperationalValve_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(OperationalValve __instance)
            {
				if (__instance.conduitType == ConduitType.Gas || __instance.conduitType == ConduitType.Liquid)
				{
					__instance.maxFlow *= HighPressureConduitRegistration.GetConduitMultiplier(__instance.conduitType);
				}
			}
        }

		[HarmonyPatch(typeof(WarpConduitSender), nameof(WarpConduitSender.OnSpawn))]
		public class WarpConduitSender_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Prefix(WarpConduitSender __instance)
			{
				__instance.gasStorage.capacityKg *= HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Gas);
				__instance.liquidStorage.capacityKg *= HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Liquid);
			}
		}


		[HarmonyPatch(typeof(RocketConduitSender), nameof(RocketConduitSender.OnSpawn))]
		public class RocketConduitSender_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Prefix(RocketConduitSender __instance)
			{
				__instance.conduitStorage.capacityKg *= HighPressureConduitRegistration.GetConduitMultiplier(__instance.conduitPortInfo.conduitType);
			}
		}

		[HarmonyPatch(typeof(Game), nameof(Game.OnLoadLevel))]
		public class Game_OnLoadLevel_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix()
			{
				HighPressureConduitRegistration.ClearEverything();
				LogisticConduit.ClearEverything();
			}
		}
	}
}
