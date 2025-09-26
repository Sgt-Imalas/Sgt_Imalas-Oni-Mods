using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	internal class LimitValve_Patches
	{

		/// <summary>
		/// limit valves work like bridges;
		/// make them HPA bridges to not break them
		/// </summary>
		[HarmonyPatch(typeof(LimitValve), nameof(LimitValve.OnSpawn))]
        public class LimitValve_OnSpawn_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Postfix(LimitValve __instance)
            {
                switch (__instance.conduitType)
                {
                    case ConduitType.Gas:
                        HighPressureConduitRegistration.RegisterHighPressureConduit(__instance.gameObject, ObjectLayer.GasConduitConnection); break;
                    case ConduitType.Liquid:
						HighPressureConduitRegistration.RegisterHighPressureConduit(__instance.gameObject, ObjectLayer.LiquidConduitConnection); break;
				}
			}
        }

		/// <summary>
		/// limit valves work like bridges;
		/// make them HPA bridges to not break them
		/// </summary>
        [HarmonyPatch(typeof(LimitValve), nameof(LimitValve.OnCleanUp))]
        public class LimitValve_OnCleanUp_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HighPressureApplications_Enabled;
			public static void Prefix(LimitValve __instance)
            {
				switch (__instance.conduitType)
				{
					case ConduitType.Gas:
						HighPressureConduitRegistration.UnregisterHighPressureConduit(__instance.gameObject, ObjectLayer.GasConduitConnection); break;
					case ConduitType.Liquid:
						HighPressureConduitRegistration.UnregisterHighPressureConduit(__instance.gameObject, ObjectLayer.LiquidConduitConnection); break;
				}
			}
        }
	}
}
