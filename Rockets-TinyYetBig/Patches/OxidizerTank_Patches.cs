using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class OxidizerTank_Patches
	{
		/// <summary>
		/// Increases or decreases the Large oxidizer module capacity, depending on setting config.
		/// </summary>
		[HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.OnSpawn))]
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

		[HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.GetOxidizersAvailable))]
		public static class OxidizerTank_GetOxidizersAvailable_Patch
		{
			public static void Postfix(OxidizerTank __instance, Dictionary<Tag, float> __result)
			{
				foreach (var efficiency in CustomOxidizers.GetOxidizerEfficiencies())
				{
					if (!__result.ContainsKey(efficiency.Key))
					{
						__result.Add(efficiency.Key, __instance.storage.GetAmountAvailable(efficiency.Key));
					}
				}
			}
		}

		[HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.GetTotalOxidizerAvailable))]
		public static class OxidizerTank_GetTotalOxidizerAvailable_Patch
		{
			public static void Postfix(OxidizerTank __instance, ref float __result)
			{
				foreach (var efficiency in CustomOxidizers.GetOxidizerEfficiencies())
				{
					__result += __instance.storage.GetAmountAvailable(efficiency.Key);
				}
			}
		}

		[HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.TotalOxidizerPower), MethodType.Getter)]
		public static class OxidizerTank_TotalOxidizerPower_Patch
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
						OxidizerEfficiencyTag = component.ElementID.CreateTag();
					}
					else
					{
						var OxidizerTagInOreTags = component.Element.oreTags.Intersect(CustomOxidizers.GetOxidizerEfficiencies().Keys);
						if (OxidizerTagInOreTags != null && OxidizerTagInOreTags.Count() > 0)
						{
							OxidizerEfficiencyTag = OxidizerTagInOreTags.First();
						}
					}

					if (OxidizerEfficiencyTag == null)
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
