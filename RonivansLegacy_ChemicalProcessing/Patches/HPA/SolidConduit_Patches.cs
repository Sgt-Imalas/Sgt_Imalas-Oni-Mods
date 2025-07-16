//using HarmonyLib;
//using RonivansLegacy_ChemicalProcessing.Content.Scripts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
//{
//    /// <summary>
//    /// handle broken conduits
//    /// </summary>
//    class SolidConduit_Patches
//	{
//		protected static readonly EventSystem.IntraObjectHandler<SolidConduit> OnBuildingBrokenDelegate = new((component, data) => OnSolidConduitBroken(component));
//		protected static readonly EventSystem.IntraObjectHandler<SolidConduit> OnBuildingFullyRepairedDelegate = new((component, data) => OnSolidConduitFullyRepaired(component));

//		[HarmonyPatch(typeof(SolidConduit), nameof(SolidConduit.OnSpawn))]
//        public class SolidConduit_OnSpawn_Patch
//        {
//            public static void Postfix(SolidConduit __instance)
//            {
//                __instance.Subscribe((int)GameHashes.BuildingBroken, OnBuildingBrokenDelegate);
//                __instance.Subscribe((int)GameHashes.BuildingBroken, OnBuildingFullyRepairedDelegate);
//                if (__instance.TryGetComponent<BuildingHP>(out var hp) && hp.IsBroken)
//                    HighPressureConduit.RegisterRailBrokenState(__instance, true);
//			}
//        }

//        [HarmonyPatch(typeof(SolidConduit), nameof(SolidConduit.OnCleanUp))]
//        public class SolidConduit_OnCleanUp_Patch
//        {
//            public static void Prefix(SolidConduit __instance)
//			{
//                __instance.Unsubscribe((int)GameHashes.BuildingBroken, OnBuildingBrokenDelegate);
//                __instance.Unsubscribe((int)GameHashes.BuildingBroken, OnBuildingFullyRepairedDelegate);
//				//cleanup
//				HighPressureConduit.RegisterRailBrokenState(__instance, false);
//			}
//        }
//        static void OnSolidConduitBroken(SolidConduit __instance)
//		{
//			HighPressureConduit.RegisterRailBrokenState(__instance, true);
//			__instance.GetNetworkManager().ForceRebuildNetworks();
//		}
//        static void OnSolidConduitFullyRepaired(SolidConduit __instance)
//		{
//			HighPressureConduit.RegisterRailBrokenState(__instance, false);
//			__instance.GetNetworkManager().ForceRebuildNetworks();
//        }
//    }
//}
