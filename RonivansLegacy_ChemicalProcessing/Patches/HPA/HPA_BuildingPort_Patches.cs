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
					tweakedCapacity = HighPressureConduitRegistration.LiquidCap_HP;
					cmp.storage.capacityKg = tweakedCapacity;
					if (cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					break;
				case CargoBay.CargoType.Gasses:
					tweakedCapacity = HighPressureConduitRegistration.GasCap_HP;
					cmp.storage.capacityKg = tweakedCapacity;
					if (cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					break;
				case CargoBay.CargoType.Solids:
					tweakedCapacity = HighPressureConduitRegistration.SolidCap_HP;
					cmp.storage.capacityKg = tweakedCapacity;
					if (cmp.targetLevel > 0)
						cmp.targetLevel = tweakedCapacity;
					if (cmp.TryGetComponent<SolidConduitConsumer>(out var scc))
						scc.capacityKG = tweakedCapacity;
					else if (cmp.TryGetComponent<SolidConduitDispenser>(out _))
						cmp.gameObject.AddOrGet<HPA_DynamicSolidConduitDispenser>();
						break;
			}
		}
		public static void IncreaseConsumerInput(ConduitConsumer __instance, bool increaseStorage)
		{
			if (__instance.conduitType == ConduitType.Gas)
			{
				float flowRate = HighPressureConduitRegistration.GetConduitMultiplier(__instance.conduitType);
				if (increaseStorage)
					__instance.GetComponent<Storage>().capacityKg *= flowRate;
				__instance.consumptionRate *= flowRate;
			}
			else if (__instance.conduitType == ConduitType.Liquid)
			{
				float flowRate = HighPressureConduitRegistration.GetConduitMultiplier(__instance.conduitType);
				if (increaseStorage)
					__instance.GetComponent<Storage>().capacityKg *= flowRate;
				__instance.consumptionRate *= flowRate;
			}
		}

		/// <summary>
		/// aquatuner
		/// </summary>
		[HarmonyPatch(typeof(LiquidConditionerConfig), nameof(LiquidConditionerConfig.ConfigureBuildingTemplate))]
		public class LiquidConditionerConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			[HarmonyPriority(Priority.Low)]
			public static void Postfix(GameObject go)
			{
				IncreaseConsumerInput(go.GetComponent<ConduitConsumer>(), true);
			}
		}

		/// <summary>
		/// AC
		/// </summary>
		[HarmonyPatch(typeof(AirConditionerConfig), nameof(AirConditionerConfig.ConfigureBuildingTemplate))]
		public class AirConditionerConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			[HarmonyPriority(Priority.Low)]
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

		[HarmonyPatch(typeof(ValveBase), nameof(ValveBase.OnSpawn))]
		public class ValveBase_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(ValveBase __instance)
			{
				if (__instance.conduitType == ConduitType.Gas || __instance.conduitType == ConduitType.Liquid)
				{
					float conduitMax = HighPressureConduitRegistration.GetMaxConduitCapacity(__instance.conduitType, true);
					if (__instance.maxFlow < conduitMax)
						__instance.maxFlow *= HighPressureConduitRegistration.GetConduitMultiplier(__instance.conduitType);
				}
			}
		}

		/// <summary>
		/// teleporter
		/// </summary>
		[HarmonyPatch(typeof(WarpConduitSender), nameof(WarpConduitSender.OnSpawn))]
		public class WarpConduitSender_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			[HarmonyPriority(Priority.Low)]
			public static void Prefix(WarpConduitSender __instance)
			{
				__instance.gasStorage.capacityKg *= HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Gas);
				__instance.liquidStorage.capacityKg *= HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Liquid);
				__instance.solidStorage.capacityKg *= HighPressureConduitRegistration.GetConduitMultiplier(ConduitType.Solid);
			}
		}


		[HarmonyPatch(typeof(WarpConduitReceiverConfig), nameof(WarpConduitReceiverConfig.DoPostConfigureComplete))]
		public class WarpConduitReceiverConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<HPA_DynamicSolidConduitDispenser>();
			}
		}


		/// <summary>
		/// spacefarer module ports
		/// </summary>
		[HarmonyPatch(typeof(RocketConduitSender), nameof(RocketConduitSender.OnSpawn))]
		public class RocketConduitSender_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;

			[HarmonyPriority(Priority.Low)]
			public static void Prefix(RocketConduitSender __instance)
			{
				__instance.conduitStorage.capacityKg *= HighPressureConduitRegistration.GetConduitMultiplier(__instance.conduitPortInfo.conduitType);
			}
		}

		[HarmonyPatch(typeof(BaseModularLaunchpadPortConfig), nameof(BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate))]
		public class BaseModularLaunchpadPortConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled && DlcManager.IsExpansion1Active();
			public static void Prefix(GameObject go, ConduitType conduitType, ref float storageSize, bool isLoader)
			{
				storageSize *= HighPressureConduitRegistration.GetConduitMultiplier(conduitType);

				if (!isLoader && conduitType == ConduitType.Solid)
					go.AddOrGet<HPA_DynamicSolidConduitDispenser>();
			}
		}
	}
}
