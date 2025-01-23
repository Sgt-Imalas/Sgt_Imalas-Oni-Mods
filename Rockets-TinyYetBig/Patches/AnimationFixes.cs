using HarmonyLib;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class AnimationFixes
	{
		//[HarmonyPatch(typeof(CraftModuleInterface), nameof(CraftModuleInterface.DoLand))]
		////[HarmonyPatch(new Type[] { typeof(LaunchPad), typeof(bool) })]
		//public static class Fix_SmallRocketModulesLosetheirAttachmentsOnLand
		//{
		//    /// <summary>
		//    /// Apply Meter changes on LOX module
		//    /// </summary>
		//    /// <param name="__instance"></param>
		//    public static void Postfix(CraftModuleInterface __instance)
		//    {
		//        SgtLogger.l("statusssy: " + __instance.m_clustercraft.status);
		//        foreach (var module in __instance.ClusterModules)
		//        {
		//            SgtLogger.l(module.Get().name);
		//            if (module.Get().TryGetComponent<VerticalModuleTiler>(out var tiler))
		//            {
		//                SgtLogger.l("fixing tilers on " + tiler.name);

		//                tiler.UpdateEndCaps();
		//            }
		//        }
		//    }
		//}

		//[HarmonyPatch(typeof(VerticalModuleTiler), nameof(VerticalModuleTiler.HasWideNeighbor))]
		////[HarmonyPatch(new Type[] { typeof(LaunchPad), typeof(bool) })]
		//public static class Fix_SmallRocketModulesLosetheirAttachmentsOnLand
		//{
		//    /// <summary>
		//    /// Apply Meter changes on LOX module
		//    /// </summary>
		//    /// <param name="__instance"></param>
		//    public static void Postfix(VerticalModuleTiler __instance, ref bool __result)
		//    {
		//        if(!__result && __instance.TryGetComponent<RocketModuleCluster>(out var module))
		//        {
		//            var engine = module.CraftInterface.GetEngine();
		//            if (engine != null && engine.TryGetComponent<Building>(out var building))
		//            {
		//                if(building.Def.WidthInCells>3)
		//                    __result= true;
		//            }
		//        }
		//    }
		//}

		/// <summary>
		/// This fixes the missing carbon field anim so it uses the "carbon_asteroid_field" animation instead of the generic "cloud" animation.
		/// Patch gets called manually to execute after Db init
		/// </summary>
		//[HarmonyPatch(typeof(HarvestablePOIConfig))]
		//[HarmonyPatch(nameof(HarvestablePOIConfig.CreatePrefabs))]
		public static class FixForMissingCarbonFieldAnim
		{
			//[PLibPatch(RunAt.AfterDbInit, nameof(HarvestablePOIConfig.CreatePrefabs), RequireType = "HarvestablePOIConfig")]
			public static void Postfix(ref List<GameObject> __result)
			{
				foreach (var obj in __result)
				{
					//SgtLogger.l(obj.ToString(),"PATCHSS");
					if (obj.TryGetComponent<HarvestablePOIClusterGridEntity>(out var poi))
					{
						if (poi.PrefabID().ToString().Contains(HarvestablePOIConfig.CarbonAsteroidField))
						{
							poi.m_Anim = "carbon_asteroid_field";
							SgtLogger.l("Fixed Carbon POI sprite");
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Calls the carbon field animation fix.
		/// </summary>
		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public static class Db_Init_Patch
		{
			// using System; will allow using Type insted of System.Type
			// using System.Reflection; will allow using MethodInfo instead of System.Reflection.MethodInfo
			static System.Reflection.MethodInfo GetMethodInfo(System.Type classType, string methodName)
			{
				System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public
													| System.Reflection.BindingFlags.NonPublic
													| System.Reflection.BindingFlags.Static
													| System.Reflection.BindingFlags.Instance;

				System.Reflection.MethodInfo method = classType.GetMethod(methodName, flags);
				if (method == null)
					Debug.Log($"Error - {methodName} method is null...");

				return method;
			}

			public static void Postfix()
			{
				System.Reflection.MethodInfo patched = GetMethodInfo(typeof(HarvestablePOIConfig), "CreatePrefabs");
				System.Reflection.MethodInfo postfix = GetMethodInfo(typeof(FixForMissingCarbonFieldAnim), "Postfix");
				// TODO: Update line below
				Mod.harmonyInstance.Patch(patched, null, new HarmonyMethod(postfix));
			}
		}
	}
}
