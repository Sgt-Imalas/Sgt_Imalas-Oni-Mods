using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
    class Conduit_Patches
    {

        [HarmonyPatch(typeof(Conduit), nameof(Conduit.OnSpawn))]
        public class Conduit_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(Conduit __instance)
            {
                HighPressureConduitRegistration.RegisterConduit(__instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(Conduit), nameof(Conduit.OnCleanUp))]
        public class Conduit_OnCleanUp_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Prefix(Conduit __instance)
			{
				HighPressureConduitRegistration.UnregisterConduit(__instance.gameObject);
			}
        }

        [HarmonyPatch(typeof(SolidConduit), nameof(SolidConduit.OnSpawn))]
        public class SolidConduit_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(SolidConduit __instance)
			{
				HighPressureConduitRegistration.RegisterConduit(__instance.gameObject);
			}
        }

        [HarmonyPatch(typeof(SolidConduit), nameof(SolidConduit.OnCleanUp))]
        public class SolidConduit_OnCleanUp_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Prefix(SolidConduit __instance)
			{
				HighPressureConduitRegistration.UnregisterConduit(__instance.gameObject);
			}
        }
    }
}
