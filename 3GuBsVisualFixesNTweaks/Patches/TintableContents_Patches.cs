using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class TintableContents_Patches
	{


		[HarmonyPatch]
		public static class AddTintableToBuildings
		{
			[HarmonyPrefix]
			public static void Prefix(GameObject go)
			{
				go.AddOrGet<ContentTintable>();
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(LiquidConditionerConfig).GetMethod(name);
				yield return typeof(LiquidPumpConfig).GetMethod(name);
				yield return typeof(LiquidMiniPumpConfig).GetMethod(name);
			}
		}

		[HarmonyPatch(typeof(LimitValve), nameof(LimitValve.OnMassTransfer))]
		public class LimitValve_ConduitUpdate_Patch
		{
			public static void Prefix(ValveBase __instance, SimHashes element, float transferredMass)
			{
				if (__instance.TryGetComponent<KBatchedAnimController>(out var kbac))
				{

					if (transferredMass <= 0)
					{
						kbac.SetSymbolTint("tint", Color.clear);
						return;
					}


					kbac.SetSymbolTint("tint", ModAssets.GetElementColor(element));

				}
			}
		}


		[HarmonyPatch(typeof(LiquidHeaterConfig), nameof(LiquidHeaterConfig.DoPostConfigureComplete))]
		public class LiquidHeaterConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<TintableByExterior>();
			}
		}

		[HarmonyPatch(typeof(MetalRefineryConfig), nameof(MetalRefineryConfig.DoPostConfigureComplete))]
		public class MetalRefineryConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<MetalRefineryTint>().ProductStorage = go.AddComponent<Storage>();
				
			}
		}

		[HarmonyPatch(typeof(ValveBase), nameof(ValveBase.ConduitUpdate))]
		public class ValveBase_ConduitUpdate_Patch
		{
			public static void Prefix(ValveBase __instance, float dt)
			{
				if (__instance.TryGetComponent<KBatchedAnimController>(out var kbac))
				{
					ConduitFlow flowManager = Conduit.GetFlowManager(__instance.conduitType);
					ConduitFlow.Conduit conduit = flowManager.GetConduit(__instance.inputCell);
					if (!flowManager.HasConduit(__instance.inputCell))
					{
						kbac.SetSymbolTint("tint", Color.clear);
						return;
					}
					ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);

					if (contents.mass > 0f)
					{
						kbac.SetSymbolTint("tint", ModAssets.GetElementColor(contents.element));
					}
					else
						kbac.SetSymbolTint("tint", Color.clear);
				}
			}
		}


	}

}
