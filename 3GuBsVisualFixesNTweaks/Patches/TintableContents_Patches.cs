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
using static STRINGS.BUILDING.STATUSITEMS;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class TintableContents_Patches
	{
		static Dictionary<GameObject, KBatchedAnimController> CachedKBACs = new();
		static Dictionary<GameObject, KBatchedAnimController> CachedFGKBACs = new();

		public static bool TryGetCachedKbacs(GameObject key, out KBatchedAnimController kbac, out KBatchedAnimController fg)
		{
			kbac = null;
			fg = null;
			if (!CachedKBACs.TryGetValue(key, out kbac))
			{
				if (key.TryGetComponent<KBatchedAnimController>(out kbac))
				{
					CachedKBACs.Add(key, kbac);
					if (kbac.layering?.foregroundController is KBatchedAnimController kbac2)
					{
						CachedFGKBACs.Add(key, kbac2);
						fg = kbac2;
					}
					else
						SgtLogger.l("no fg kbac found for " + key.GetProperName());
				}
			}
			if (kbac == null)
				return false;

			CachedFGKBACs.TryGetValue(key, out fg);
			return true;

		}

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

		[HarmonyPatch(typeof(PoweredActiveTransitionController), nameof(PoweredActiveTransitionController.InitializeStates))]
		public class PoweredActiveTransitionController_InitializeStates_Patch
		{
			public static void Postfix(PoweredActiveTransitionController __instance)
			{
				__instance.on_pre.Enter(smi =>
				{
					if (!TryGetCachedKbacs(smi.gameObject, out var kbac, out var kbac2))
						return;
					if (!smi.gameObject.TryGetComponent<LimitValve>(out var valve) || valve.conduitType != ConduitType.Gas && valve.conduitType != ConduitType.Liquid)
						return;

					TryApplyConduitTint(valve.conduitBridge.type, valve.conduitBridge.inputCell, kbac, kbac2);
				});
			}
		}

		public static void TryApplyConduitTint(ConduitType type, int conduitCell, KBatchedAnimController kbac, KBatchedAnimController kbac2, Color ForceElementColor = default)
		{
			if (ForceElementColor != default)
			{
				kbac.SetSymbolTint("tint", ForceElementColor);
				kbac2?.SetSymbolTint("tint_fg", ForceElementColor);
				return;
			}


			ConduitFlow flowManager = Conduit.GetFlowManager(type);
			ConduitFlow.Conduit conduit = flowManager.GetConduit(conduitCell);
			if (!flowManager.HasConduit(conduitCell))
			{
				kbac.SetSymbolTint("tint", Color.clear);
				kbac2?.SetSymbolTint("tint", Color.clear);
				return;
			}
			ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);

			if (contents.mass > 0f)
			{
				kbac.SetSymbolTint("tint", ModAssets.GetElementColor(contents.element));
				kbac2?.SetSymbolTint("tint_fg", ModAssets.GetElementColor(contents.element));
			}
			else
			{
				kbac.SetSymbolTint("tint", Color.clear);
				kbac2?.SetSymbolTint("tint_fg", Color.clear);
			}
		}

		[HarmonyPatch(typeof(LimitValve), nameof(LimitValve.OnSpawn))]
		public class LimitValve_OnSpawn_Patch
		{
			public static void Postfix(LimitValve __instance)
			{
				if (__instance.conduitType != ConduitType.Gas && __instance.conduitType != ConduitType.Liquid)
					return;

				if (!TryGetCachedKbacs(__instance.gameObject, out var kbac, out var kbac2))
					return;

				TryApplyConduitTint(__instance.conduitBridge.type, __instance.conduitBridge.inputCell, kbac, kbac2);
			}
		}

		[HarmonyPatch(typeof(LimitValve), nameof(LimitValve.OnMassTransfer))]
		public class LimitValve_ConduitUpdate_Patch
		{
			public static void Prefix(ValveBase __instance, SimHashes element, float transferredMass)
			{
				if (TryGetCachedKbacs(__instance.gameObject, out var kbac, out var kbac2))
				{
					if (transferredMass <= 0)
					{
						TryApplyConduitTint(__instance.conduitType, __instance.inputCell, kbac, kbac2, Color.clear);
					}
					else
					{
						TryApplyConduitTint(__instance.conduitType, __instance.inputCell, kbac, kbac2, ModAssets.GetElementColor(element));
					}
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


		[HarmonyPatch(typeof(ElementFilter), nameof(ElementFilter.OnConduitTick))]
		public class ElementFilter_OnConduitTick_Patch
		{
			public static void Prefix(ElementFilter __instance)
			{
				if (TryGetCachedKbacs(__instance.gameObject, out var kbac, out var kbac2))
				{
					TryApplyConduitTint(__instance.portInfo.conduitType, __instance.inputCell, kbac, kbac2);
				}
			}
		}


		[HarmonyPatch(typeof(ValveBase), nameof(ValveBase.ConduitUpdate))]
		public class ValveBase_ConduitUpdate_Patch
		{
			public static void Prefix(ValveBase __instance, float dt)
			{
				if (TryGetCachedKbacs(__instance.gameObject, out var kbac, out var kbac2))
				{
					TryApplyConduitTint(__instance.conduitType, __instance.inputCell, kbac, kbac2);
				}
			}
		}
	}
}
