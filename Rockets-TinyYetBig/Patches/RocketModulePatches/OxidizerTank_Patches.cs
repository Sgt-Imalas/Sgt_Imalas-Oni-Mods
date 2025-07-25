using HarmonyLib;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.RocketModulePatches
{
	class OxidizerTank_Patches
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

		static void AddCustomSolidOxidizersToFilter(GameObject go)
		{
			if (go.TryGetComponent<FlatTagFilterable>(out var filter))
			{
				foreach (var element in ElementLoader.elements)
				{
					if(element.state == Element.State.Solid && element.HasTag(ModAssets.Tags.RocketSolidOxidizerTag))
					{
						var tag = element.tag;
						if (filter.tagOptions.Contains(tag))
							continue;

						filter.tagOptions.Add(tag);
					}
				}
			}
		}

		[HarmonyPatch(typeof(OxidizerTankClusterConfig), "DoPostConfigureComplete")]
		public static class IncreaseCapacityto1350Oxidizers_Def
		{
			public static void Postfix(GameObject go)
			{
				if (Config.Instance.BuffLargeOxidizer)
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
					AddCustomSolidOxidizersToFilter(go);
				}
			}
		}

		[HarmonyPatch(typeof(SmallOxidizerTankConfig), nameof(SmallOxidizerTankConfig.DoPostConfigureComplete))]
		public class SmallOxidizerTankConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				AddCustomSolidOxidizersToFilter(go);
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