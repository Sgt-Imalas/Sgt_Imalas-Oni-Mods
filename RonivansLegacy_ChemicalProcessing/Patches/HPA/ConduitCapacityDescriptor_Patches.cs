using HarmonyLib;
using Klei.AI;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
    class ConduitCapacityDescriptor_Patches
    {

		[HarmonyPatch]
		public static class AddCapacityDescriptorToVanillaConduits
		{
			[HarmonyPostfix]
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<ConduitCapacityDescriptor>();
			}

			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.ConfigureBuildingTemplate);
				yield return typeof(SolidConduitBridgeConfig).GetMethod(name, AccessTools.all);
				yield return typeof(LiquidConduitBridgeConfig).GetMethod(name, AccessTools.all);
				yield return typeof(GasConduitBridgeConfig).GetMethod(name, AccessTools.all);

				yield return typeof(SolidConduitConfig).GetMethod(name, AccessTools.all);

				yield return typeof(LiquidConduitConfig).GetMethod(name, AccessTools.all);
				yield return typeof(LiquidConduitRadiantConfig).GetMethod(name, AccessTools.all);
				yield return typeof(InsulatedLiquidConduitConfig).GetMethod(name, AccessTools.all);

				yield return typeof(GasConduitConfig).GetMethod(name, AccessTools.all);
				yield return typeof(GasConduitRadiantConfig).GetMethod(name, AccessTools.all);
				yield return typeof(InsulatedGasConduitConfig).GetMethod(name, AccessTools.all);

				var PlasticGasConduitConfig = AccessTools.TypeByName("PlasticGasConduitConfig");
				if (PlasticGasConduitConfig != null && AccessTools.Method(PlasticGasConduitConfig, name) != null)
					yield return PlasticGasConduitConfig.GetMethod(name, AccessTools.all);
				var PlasticLiquidConduitConfig = AccessTools.TypeByName("PlasticLiquidConduitConfig");
				if (PlasticLiquidConduitConfig != null && AccessTools.Method(PlasticLiquidConduitConfig, name) != null)
					yield return PlasticLiquidConduitConfig.GetMethod(name, AccessTools.all);

			}
		}
		/// <summary>
		/// those following patches are for any modded buildings that arent found by the patch above^
		/// </summary>
		[HarmonyPatch(typeof(Conduit), nameof(Conduit.OnPrefabInit))]
        public class Conduit_OnPrefabInit_Patch
		{
            public static void Postfix(Conduit __instance)
			{
				__instance.gameObject.AddOrGet<ConduitCapacityDescriptor>();
			}
        }
		[HarmonyPatch(typeof(ConduitBridge), nameof(ConduitBridge.OnPrefabInit))]
		public class ConduitBridge_OnPrefabInit_Patch
		{
			public static void Postfix(ConduitBridge __instance)
			{
				__instance.gameObject.AddOrGet<ConduitCapacityDescriptor>();
			}
		}

		[HarmonyPatch(typeof(SolidConduit), nameof(SolidConduit.OnSpawn))]
        public class SolidConduit_OnSpawn_Patch
        {
            public static void Postfix(SolidConduit __instance)
			{
				__instance.gameObject.AddOrGet<ConduitCapacityDescriptor>();
			}
        }
		[HarmonyPatch(typeof(SolidConduitBridge), nameof(SolidConduitBridge.OnSpawn))]
		public class SolidConduitBridge_OnSpawn_Patch
		{
			public static void Postfix(SolidConduitBridge __instance)
			{
				__instance.gameObject.AddOrGet<ConduitCapacityDescriptor>();
			}
		}
	}
}
