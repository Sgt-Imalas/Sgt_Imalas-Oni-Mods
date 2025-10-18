using HarmonyLib;
using ItemDropPrevention.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ItemDropPrevention.Patches
{
	internal class FetchChore_Patches
	{

		[HarmonyPatch(typeof(FetchChore), nameof(FetchChore.FindFetchTarget), [typeof(ChoreConsumerState)])]
		public class FetchChore_FindFetchTarget_Patch
		{
			public static bool Prefix(FetchChore __instance, ChoreConsumerState consumer_state, ref Pickupable __result)
			{
				if (consumer_state.hasSolidTransferArm || __instance.destination == null || !DroppablesHolder.TryGet(consumer_state.consumer.gameObject, out var holder))
					return true;

				__result = holder.FindFetchTargetFromDroppables(__instance, consumer_state);
				return __result == null;
			}
		}

		//[HarmonyPatch(typeof(FetchManager), nameof(FetchManager.IsFetchablePickup))]
		//public class FetchManager_IsFetchablePickup_Patch
		//{
		//	public static bool Prefix(FetchManager __instance, Pickupable pickup, FetchChore chore, Storage destination, ref bool __result)
		//	{
		//		return true;
		//		__result = IsFetchablePickup(pickup,chore,destination);
		//		return false;
		//	}
		//	public static bool IsFetchablePickup(Pickupable pickup, FetchChore chore, Storage destination)
		//	{
		//		KPrefabID kPrefabID = pickup.KPrefabID;
		//		Storage storage = pickup.storage;
		//		if (pickup.UnreservedFetchAmount <= 0f)
		//		{
		//			SgtLogger.l("Pickup " + pickup.name + " has no unreserved fetch amount");
		//			return false;
		//		}

		//		if (pickup.PrimaryElement.MassPerUnit > 1f && pickup.PrimaryElement.MassPerUnit > chore.originalAmount)
		//		{
		//			return false;
		//		}

		//		if (kPrefabID == null)
		//		{
		//			return false;
		//		}

		//		if (!pickup.isChoreAllowedToPickup(chore.choreType))
		//		{
		//			return false;
		//		}

		//		if (chore.criteria == FetchChore.MatchCriteria.MatchID && !chore.tags.Contains(kPrefabID.PrefabTag))
		//		{
		//			SgtLogger.l("Pickup " + pickup.name + " does not match required ID tags");
		//			foreach(var tag in chore.tags)
		//			{
		//				SgtLogger.l("Required tag: " + tag);
		//			}
		//			return false;
		//		}

		//		if (chore.criteria == FetchChore.MatchCriteria.MatchTags && !kPrefabID.HasTag(chore.tagsFirst))
		//		{
		//			SgtLogger.l("Pickup " + pickup.name + " does not match required tags");
		//			SgtLogger.l("Required tag: " + chore.tagsFirst);
		//			return false;
		//		}

		//		if (chore.requiredTag.IsValid && !kPrefabID.HasTag(chore.requiredTag))
		//		{
		//			SgtLogger.l("Pickup " + pickup.name + " does not have required tag " + chore.requiredTag);
		//			return false;
		//		}

		//		if (kPrefabID.HasAnyTags(chore.forbiddenTags))
		//		{
		//			SgtLogger.l("Pickup " + pickup.name + " has forbidden tags");
		//			return false;
		//		}

		//		if (kPrefabID.HasTag(GameTags.MarkedForMove))
		//		{
		//			SgtLogger.l("Pickup " + pickup.name + " is marked for move");
		//			return false;
		//		}

		//		if (storage != null)
		//		{
		//			if (!storage.ignoreSourcePriority && destination.ShouldOnlyTransferFromLowerPriority && destination.masterPriority <= storage.masterPriority)
		//			{
		//				SgtLogger.l("Pickup " + pickup.name + " is in storage with higher or equal priority than destination");
		//				return false;
		//			}

		//			if (destination.storageNetworkID != -1 && destination.storageNetworkID == storage.storageNetworkID)
		//			{
		//				SgtLogger.l("Pickup " + pickup.name + " is in the same storage network as destination");
		//				return false;
		//			}
		//		}

		//		return true;
		//	}
		//}
	}
}
