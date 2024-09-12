using HarmonyLib;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	public class InfinitePOIPatches
	{
		/// <summary>
		/// When infinite POI Capacity is enabled, mining the POI keeps its (internal) capacity at max capacity. 
		/// Also prevents the capacity from becoming negative
		/// </summary>
		[HarmonyPatch(typeof(HarvestablePOIStates.Instance))]
		[HarmonyPatch(nameof(HarvestablePOIStates.Instance.DeltaPOICapacity))]
		public static class InstaRecharge
		{
			public static void Postfix(HarvestablePOIStates.Instance __instance, ref float delta)
			{
				if (Config.Instance.InfinitePOI)
				{
					__instance.poiCapacity = __instance.configuration.GetMaxCapacity();
				}
				__instance.poiCapacity = Mathf.Max(0f, __instance.poiCapacity);
			}
		}

		/// <summary>
		/// When infinite POIs are enabled, its capacity text is set to an infinite symbol.
		/// </summary>
		[HarmonyPatch(typeof(SpacePOISimpleInfoPanel), "RefreshMassHeader")]
		public static class InstaRechargeStatusItem
		{
			public static void Postfix(HarvestablePOIStates.Instance harvestable, GameObject ___massHeader)
			{
				if (Config.Instance.InfinitePOI)
				{
					HierarchyReferences component = ___massHeader.GetComponent<HierarchyReferences>();
					component.GetReference<LocText>("ValueLabel").text = "<b>∞</b>";
				}
			}

		}
	}
}
