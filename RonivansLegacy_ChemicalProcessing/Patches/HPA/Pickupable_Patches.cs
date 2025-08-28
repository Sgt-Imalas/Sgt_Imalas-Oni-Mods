using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Pickupable_Patches
    {

		//      [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnSpawn))]
		//      public class Pickupable_OnSpawn_Patch
		//{
		//	[HarmonyPrepare]
		//	public static bool Prepare() => Config.Instance.HPA_Rails_Enabled;
		//	public static void Prefix(Pickupable __instance)
		//          {
		//              HighPressureConduitRegistration.RegisterPickupable(__instance);
		//	}
		//      }

		[HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnCleanUp))]
		public class Pickupable_OnCleanUp_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HPA_Rails_Insulation_Enabled;
			public static void Prefix(Pickupable __instance)
			{
				HighPressureConduitRegistration.CleanupPickupable(__instance);
			}
		}
	}
}
