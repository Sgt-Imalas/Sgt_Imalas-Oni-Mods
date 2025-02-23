using HarmonyLib;
using Rockets_TinyYetBig.Buildings.CargoBays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class CargoBayLogicPatches
	{

		[HarmonyPatch]
		public class Cargobay_CreateBuildingDef_AddLogicPorts
		{
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.CreateBuildingDef);
				//gases
				yield return typeof(GasCargoBayClusterConfig).GetMethod(name);
				yield return typeof(GasCargoBaySmallConfig).GetMethod(name);
				//liquids
				yield return typeof(LiquidCargoBayClusterConfig).GetMethod(name);
				yield return typeof(LiquidCargoBaySmallConfig).GetMethod(name);
				//solids
				yield return typeof(SolidCargoBayClusterConfig).GetMethod(name);
				yield return typeof(SolidCargoBaySmallConfig).GetMethod(name);
			}
			[HarmonyPostfix]
			public static void Postfix(BuildingDef __result)
			{
				ModAssets.AddCargoBayLogicPorts(__result);
			}
        }
		[HarmonyPatch]
		public class Cargobay_DoPostConfigureComplete_AddLogicPortComponent
		{
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				//gases
				yield return typeof(GasCargoBayClusterConfig).GetMethod(name);
				yield return typeof(GasCargoBaySmallConfig).GetMethod(name);
				//liquids
				yield return typeof(LiquidCargoBayClusterConfig).GetMethod(name);
				yield return typeof(LiquidCargoBaySmallConfig).GetMethod(name);
				//solids
				yield return typeof(SolidCargoBayClusterConfig).GetMethod(name);
				yield return typeof(SolidCargoBaySmallConfig).GetMethod(name);
			}
			[HarmonyPostfix]
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<CargoBayStatusMonitor>();
			}
		}
	}
}
