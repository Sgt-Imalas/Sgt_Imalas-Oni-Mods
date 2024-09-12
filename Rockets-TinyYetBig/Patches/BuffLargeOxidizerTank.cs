using HarmonyLib;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	class BuffLargeOxidizerTank
	{
		/// <summary>
		/// Increases or decreases the Large oxidizer module capacity, depending on setting config.
		/// </summary>
		[HarmonyPatch(typeof(OxidizerTank), "OnSpawn")]
		public static class IncreaseCapacityto1350Oxidizers
		{
			public static void Prefix(OxidizerTank __instance)
			{
				if (Config.Instance.BuffLargeOxidizer && __instance.maxFillMass == OxidizerTankClusterConfig.FuelCapacity)
				{
					__instance.maxFillMass = 1350f;
				}
				else if (!Config.Instance.BuffLargeOxidizer && __instance.maxFillMass == 1350f)
				{
					if (__instance.targetFillMass > OxidizerTankClusterConfig.FuelCapacity)
						__instance.targetFillMass = OxidizerTankClusterConfig.FuelCapacity;
					__instance.maxFillMass = 900f;
				}
			}
		}
		[HarmonyPatch(typeof(OxidizerTankClusterConfig), "DoPostConfigureComplete")]
		public static class IncreaseCapacityto1350Oxidizers_Def
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.BuffLargeOxidizer;

			public static void Postfix(GameObject go)
			{
				if (go.TryGetComponent<Storage>(out var storage))
				{
					storage.capacityKg = 1350f;
				}
				if (go.TryGetComponent<OxidizerTank>(out var oxidizerTank))
				{
					oxidizerTank.targetFillMass = 1350f;
					oxidizerTank.maxFillMass = 1350f;
				}
			}
		}
		//[HarmonyPatch(typeof(FuelTank), "OnSpawn")]
		//public static class IncreaseCapacityto1350FuelTanks
		//{
		//    public static void Postfix(FuelTank __instance)
		//    {
		//        if (Config.Instance.BuffLargeOxidizer && __instance.physicalFuelCapacity == 900f)
		//        {
		//            __instance.physicalFuelCapacity = 1350f;
		//        }
		//        else if (!Config.Instance.BuffLargeOxidizer && __instance.physicalFuelCapacity == 1350f)
		//        {
		//            if (__instance.targetFillMass > 900f)
		//                __instance.targetFillMass = 900f;
		//            __instance.physicalFuelCapacity = 900f;
		//        }
		//    }

		//}

	}
}
//