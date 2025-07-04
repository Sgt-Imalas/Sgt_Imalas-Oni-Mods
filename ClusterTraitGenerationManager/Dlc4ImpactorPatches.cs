using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
    class Dlc4ImpactorPatches
	{     
		/// <summary>
		/// REQUIRED: Klei checks for GO position of the impactor, which is always in 0,0 , aka the most left asteroid: this assumption breaks for moonlets or when any other asteroid is taller than the starter
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
				///this is the Klei code and its very fragile, it assumes the most left asteroid is the one with the impactor, which is not true if on a moonlet
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
	}
}
