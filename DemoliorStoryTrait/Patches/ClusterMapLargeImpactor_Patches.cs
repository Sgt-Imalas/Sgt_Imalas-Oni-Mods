using HarmonyLib;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UtilLibs;
using static AttackProperties;
using static ResearchTypes;
using static STRINGS.UI;
using static STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORSHOWERS;

namespace DemoliorStoryTrait.Patches
{
	class ClusterMapLargeImpactor_Patches
	{
		/// <summary>
		/// REQUIRED: Klei checks for GO position of the impactor, which is always in 0,0 , aka the most left asteroid: this assumption breaks for moonlets
		/// </summary>
		[HarmonyPatch(typeof(LargeImpactorNotificationMonitor), nameof(LargeImpactorNotificationMonitor.OnDuplicantReachedSpace))]
		public class LargeImpactorNotificationMonitor_OnDuplicantReachedSpace_Patch
		{
			public static void Prefix(
				LargeImpactorNotificationMonitor.Instance smi,
				object obj)
			{

				///the dupe that reached space
				int myWorldId = ((GameObject)obj).GetMyWorldId();
				///this is the Klei code and its fucking garbage, it assumes the most left asteroid is the one with the impactor, which is not true if on a moonlet
				//int impactorTargetWorldId = smi.gameObject.GetMyWorldId();
				///replace it with a proper check
				GameplayEventInstance impactorEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id);
				if (impactorEventInstance == null)
				{
					SgtLogger.error("Impactor event instance was null");
					return;
				}
				int impactorTargetWorldId = impactorEventInstance.worldId;

				if (myWorldId == impactorTargetWorldId)
				{
					LargeImpactorNotificationMonitor.Discover(smi);
				}
			}
		}
		/// <summary>
		/// REQUIRED: klei assumes the target asteroid of the impactor is always at world index 0 and in the center of the starmap.
		/// if thats not the case, the impactor targets the first asteroid to generate, eg. desolands in moonlet clusters.
		/// its starmap pathing also breaks, causing it to instantly hit the asteroid.
		/// </summary>
		[HarmonyPatch(typeof(LargeImpactorEvent), nameof(LargeImpactorEvent.CreateSpacedOutImpactorInstance))]
		public class LargeImpactorEvent_CreateSpacedOutImpactorInstance_Patch
		{
			public static void Postfix(LargeImpactorEvent.StatesInstance smi, GameObject __result)
			{
				//in base game
				if (__result == null)
					return;

				ClusterMapLargeImpactor.Def def = __result.AddOrGetDef<ClusterMapLargeImpactor.Def>();
				foreach (var worldcontainer in ClusterManager.Instance.WorldContainers)
				{
					SgtLogger.l(worldcontainer.id + " : " + string.Join(",", worldcontainer.GetSeasonIds()));

					//there can only be one asteroid targeted by the impactor in the cluster at the same time
					if (worldcontainer.GetSeasonIds().Contains("LargeImpactor"))
					{
						//fixing the target world id to not be 0
						SgtLogger.l("ClusterMapLargeImpactor Pre fix worldid: " + def.destinationWorldID);
						def.destinationWorldID = worldcontainer.id;
						SgtLogger.l(" ClusterMapLargeImpactorpost fix worldid: " + def.destinationWorldID);
						//making the destination selector recalculate so it doesnt end up with an invalid path, causing it to skip the flying time 
						__result.GetComponent<ClusterDestinationSelector>().SetDestination(worldcontainer.GetComponent<ClusterGridEntity>().Location);

						return;
					}
				}
			}
		}

		/// <summary>
		/// REQUIRED: the impact zone preview initializer assumes that the first event to spawn is the impactor event
		/// on moonlets thats wrong, so the zone preview fails to init.
		/// </summary>
		[HarmonyPatch(typeof(LargeImpactorVisualizerEffect), nameof(LargeImpactorVisualizerEffect.SetupOnGameplayEventStart))]
		public class LargeImpactorVisualizerEffect_SetupOnGameplayEventStart_Patch
		{
			public static void Postfix(object data, LargeImpactorVisualizerEffect __instance)
			{
				GameplayEventInstance gameplayEventInstance = (GameplayEventInstance)data;
				if (gameplayEventInstance.eventID != Db.Get().GameplayEvents.LargeImpactor.Id)
					GameplayEventManager.Instance.Subscribe(1491341646, __instance.SetupOnGameplayEventStart);
			}

		}

		/// <summary>
		/// override the printer survival on moonlets to avoid the impactor getting cut off in the middle
		/// </summary>

		[HarmonyPatch(typeof(LargeImpactorCrashStamp), nameof(LargeImpactorCrashStamp.FindIdealLocation))]
		public class LargeImpactorCrashStamp_FindIdealLocation_Patch
		{
			public static void Prefix(LargeImpactorCrashStamp __instance) => SgtLogger.l("PREFIX WORLD:" + __instance.targetWorldId);
			public static void Postfix(LargeImpactorCrashStamp __instance, ref Vector2I __result)
			{
				var impactorTarget = ClusterManager.Instance.GetWorld(ModAssets.GetImpactorWorldID());

				//only do that to the modded impactor
				if (!CustomGameSettings.Instance.GetCurrentStories().Contains(CGMWorldGenUtils.CGM_Impactor_StoryTrait))
					return;

				var templateBounds = __instance.asteroidTemplate.GetTemplateBounds();

				int posY = __result.Y;
				int posX = __result.X;



				int worldYMin = impactorTarget.WorldOffset.Y;
				int worldYMax = impactorTarget.WorldOffset.Y + impactorTarget.WorldSize.Y;

				int ImpactMaxY = posY + (templateBounds.height / 2);
				SgtLogger.l("Upper world border: " + worldYMax + ", upper asteroid impact zone: " + ImpactMaxY);

				if (ImpactMaxY <= worldYMax + 10) 
					return;

				while (ImpactMaxY > worldYMax + 10)
					--ImpactMaxY;

				posY = ImpactMaxY - templateBounds.height;

				var adjustedPos = new Vector2I(posX, posY);
				__result = adjustedPos;

			}
		}

		[HarmonyPatch(typeof(ClusterMapLargeImpactor.Instance), nameof(ClusterMapLargeImpactor.Instance.Setup))]
		public class ClusterMapLargeImpactor_Instance_Setup_Patch
		{
			public static void Prefix(ClusterMapLargeImpactor.Instance __instance, ref int destinationWorldID, float arrivalTime)
			{
				__instance.traveler?.MarkPathDirty();
				destinationWorldID = ModAssets.GetImpactorWorldID();
			}
		}

	}
}
