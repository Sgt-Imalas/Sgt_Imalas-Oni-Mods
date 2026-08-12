using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
	class LargeImpactorVanillaConfig_Patches
	{

		[HarmonyPatch(typeof(LargeImpactorVanillaConfig), nameof(LargeImpactorVanillaConfig.ConfigCommon))]
		public class LargeImpactorVanillaConfig_ConfigCommon_Patch
		{
			public static void Postfix(GameObject __result)
			{
				if (Config.Instance.PipReplaceDemoliorSprite && __result.TryGetComponent<LargeImpactorCrashStamp>(out var stamp))
					stamp?.largeStampTemplate = "poi/asteroid_impacts/potato_pip_impactor";

			}
		}


		[HarmonyPatch(typeof(LargeImpactorVanillaConfig), nameof(LargeImpactorVanillaConfig.SpawnCommon))]
		public class LargeImpactorVanillaConfig_SpawnCommon_Patch
		{
			public static bool Prefix(GameObject inst)
			{
				if (inst == null)
				{
					return false;
				}

				var statusInstance = inst.GetSMI<LargeImpactorStatus.Instance>(); 
				if (statusInstance == null)
				{
					return false;
				}
				if (statusInstance != null && statusInstance.Health <= 0 && SaveGame.Instance != null && SaveGame.Instance.ColonyAchievementTracker.largeImpactorState == ColonyAchievementTracker.LargeImpactorState.Alive)
				{

					SaveGame.Instance.ColonyAchievementTracker.largeImpactorState = ColonyAchievementTracker.LargeImpactorState.Defeated;
					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(LargeImpactorVanillaConfig), nameof(LargeImpactorVanillaConfig.GetStatusMonitor))]
		public class LargeImpactorStatus_GetStatusMonitor_Patch
		{
			public static bool Prefix()
			{
				var instance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id);
				if(instance == null)
				{
					SgtLogger.warning("GameplayEventInstance for LargeImpactor is null.");
					return false;
				}
				return true;
			}
		}

		//[HarmonyPatch(typeof(LargeImpactorEvent.States), nameof(LargeImpactorEvent.States.InitializeStates))]
		//public class LargeImpactorEvent_InitializeStates_Patch
		//{
		//	public static void Postfix(LargeImpactorEvent.States __instance)
		//	{
		//		__instance.killedByPlayer.Enter((_) => LargeImpactorDestroyedSequence.Start());
		//	}
		//}
	}
}
