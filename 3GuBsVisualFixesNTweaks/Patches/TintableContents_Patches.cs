﻿using _3GuBsVisualFixesNTweaks.Scripts;
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
using static _3GuBsVisualFixesNTweaks.ModAssets;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class TintableContents_Patches
	{


		[HarmonyPatch(typeof(PoweredActiveTransitionController), nameof(PoweredActiveTransitionController.InitializeStates))]
		public class PoweredActiveTransitionController_InitializeStates_Patch
		{
			/// <summary>
			/// "on" is pretty much the "inactive but not disabled" state of the anim;
			/// these adjustments change that to treat the "on" state as "idle" 
			/// </summary>
			/// <param name="__instance"></param>
			public static void Postfix(PoweredActiveTransitionController __instance)
			{
				DoubleAnimFix_Patches.CleanAnimTransitions(__instance.off.enterActions);
				__instance.off.PlayAnim("off", KAnim.PlayMode.Loop);
				__instance.off.transitions.Clear();
				__instance.off
					//.Enter(smi =>
					//{
					//	if (!TryGetCachedKbacs(smi.gameObject, out var _, out var fg))
					//		return;
					//	fg.SetSymbolVisiblity("meter_counter_cursor_fg", true);
					//})
					//.Exit(smi =>
					//{
					//	if (!TryGetCachedKbacs(smi.gameObject, out var _, out var fg))
					//		return;
					//	fg.SetSymbolVisiblity("meter_counter_cursor_fg", false);
					//})
					.EventTransition(GameHashes.OperationalChanged, __instance.on, (smi => smi.GetComponent<Operational>().IsOperational));
				__instance.on.transitions.Clear();
				__instance.on
					.EventTransition(GameHashes.OperationalChanged, __instance.off, (smi => !smi.GetComponent<Operational>().IsOperational))
					.EventTransition(GameHashes.ActiveChanged, __instance.on_pre, (smi => smi.GetComponent<Operational>().IsActive));
				__instance.on_pre.transitions.Clear();
				__instance.on_pre
					.OnAnimQueueComplete(__instance.working);
				__instance.on_pst.transitions.Clear();
				__instance.on_pst
					.OnAnimQueueComplete(__instance.on);
				__instance.working.transitions.Clear();
				__instance.working
					.EventTransition(GameHashes.OperationalChanged, __instance.off, (smi => !smi.GetComponent<Operational>().IsOperational))
					.EventTransition(GameHashes.ActiveChanged, __instance.on_pst, (smi => !smi.GetComponent<Operational>().IsActive));


				__instance.on.Enter(smi =>
				{
					if (!TryGetCachedKbacs(smi.gameObject, out var kbac, out var kbac2))
						return;
					if (!smi.gameObject.TryGetComponent<LimitValve>(out var valve) || valve.conduitType != ConduitType.Gas && valve.conduitType != ConduitType.Liquid)
						return;

					TryApplyConduitTint(valve.conduitBridge.type, valve.conduitBridge.inputCell, kbac, kbac2);
				});
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
		public class LimitValve_OnMassTransfer_Patch
		{
			public static void Prefix(LimitValve __instance, SimHashes element, float transferredMass)
			{
				if (TryGetCachedKbacs(__instance.gameObject, out var kbac, out var kbac2))
				{
					if (transferredMass <= 0)
					{
						TryApplyConduitTint(__instance.conduitType, -1, kbac, kbac2, true, Color.clear);
					}
					else
					{
						TryApplyConduitTint(__instance.conduitType, -1, kbac, kbac2, true, ModAssets.GetElementColor(element));
					}
				}
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
	}
}
