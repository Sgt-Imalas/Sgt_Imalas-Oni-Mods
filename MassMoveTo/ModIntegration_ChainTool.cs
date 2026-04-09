using HarmonyLib;
using MassMoveTo.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace MassMoveTo
{
	public static class ModIntegration_ChainTool
	{
		public static bool ChainToolActive = false;
		internal static void TryInit(Harmony harmony)
		{
			var targetType = Type.GetType("ChainErrand.ChainedErrandPacks.MoveToPack, ChainErrand");
			if (targetType == null)
			{
				SgtLogger.l("ChainTool not found, integration is sleeping now. zzz......");
				return;
			}
			ChainToolActive = true;
			return;
			try
			{
				var targetMethod = AccessTools.Method(targetType, "GetChoreFromErrand", [typeof(Movable)]);
				var m_prefix = new HarmonyMethod(AccessTools.Method(typeof(ModIntegration_ChainTool), (nameof(PrefixSkip))));
				harmony.Patch(targetMethod, m_prefix);


				var targetMethod2 = AccessTools.Method(targetType, "ClearMovePrefix", [typeof(Movable)]);
				var m_prefix2 = new HarmonyMethod(AccessTools.Method(typeof(ModIntegration_ChainTool), (nameof(PrefixSkip2))));
				harmony.Patch(targetMethod2, m_prefix2);

				var targetMethod3 = AccessTools.Method(targetType, "CollectErrands");
				var m_prefix3 = new HarmonyMethod(AccessTools.Method(typeof(ModIntegration_ChainTool), (nameof(PrefixSkip3))));
				harmony.Patch(targetMethod3, m_prefix3);

				var targetMethod4 = AccessTools.Method(targetType, "OnCancelPostfix");
				var m_prefix4 = new HarmonyMethod(AccessTools.Method(typeof(ModIntegration_ChainTool), (nameof(PrefixSkip4))));
				harmony.Patch(targetMethod4, m_prefix4);


				var targetType2 = Type.GetType("ChainErrand.AutoChainUtils, ChainErrand");
				if(targetType2 != null)
				{
					var targetMethod5 = AccessTools.Method(targetType2, "TryAddToAutomaticChain");
					if (targetMethod5 != null)
					{
						var m_prefix5 = new HarmonyMethod(AccessTools.Method(typeof(ModIntegration_ChainTool), (nameof(PrefixSkip5))));
						harmony.Patch(targetMethod5, m_prefix5);
					}
					else
						SgtLogger.l("ChainErrand.AutoChainUtils not found");

				}

			}
			catch (Exception ex)
			{
				SgtLogger.error($"Failed to patch ChainTool\n: {ex}");
				return;
			}
			SgtLogger.l("Patched chain tool to work with MassMoveTool's multi fetch.");
		}

		/// <summary>
		/// gotta skip the chore getter from executing because its always null for the MultiFetch chore
		/// </summary>
		/// <param name="errand"></param>
		/// <returns></returns>
		static bool PrefixSkip(Movable errand)
		{
			if (errand.StorageProxy?.TryGetComponent(out MultiFetch_CancellableMove _) ?? false)
			{
				return false;
			}
			return true;
		}
		static bool PrefixSkip2(Movable __instance)
		{
			if (__instance != null && (__instance.StorageProxy?.TryGetComponent(out MultiFetch_CancellableMove _) ?? false) || __instance == null)
			{
				return false;
			}
			return true;
		}

		static bool PrefixSkip3(GameObject gameObject, ref bool __result)
		{
			if (gameObject.TryGetComponent(out MultiFetch_CancellableMove _))
			{
				__result = false;
				return false;
			}
			return true;
		}
		static bool PrefixSkip4(Movable cancel_movable, CancellableMove __instance)
		{
			if (__instance is MultiFetch_CancellableMove _)
			{
				return false;
			}
			return true;
		}
		static bool PrefixSkip5(GameObject chainNumberBearer)
		{
			if (chainNumberBearer.TryGetComponent(out MultiFetch_CancellableMove _))
			{
				return false;
			}
			return true;
		}
	}
}
