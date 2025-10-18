using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ItemDropPrevention.Content.Scripts
{

	internal class DroppablesHolder : KMonoBehaviour
	{
		[MyCmpReq] Storage internalStorage;
		[MyCmpReq] KPrefabID kprefabID;
		[MyCmpReq] ChoreDriver choreDriver;
		[MyCmpReq] StandardWorker worker;

		StaminaMonitor.Instance staminaMonitor;
		Narcolepsy narcolepsy;
		bool hasStamina,hasNarcolepsy;

		static Dictionary<GameObject, DroppablesHolder> droppablesHolders = [];
		HashSet<int> MarkedForDrop = [];

		public override void OnSpawn()
		{
			base.OnSpawn();
			staminaMonitor = this.GetSMI<StaminaMonitor.Instance>();
			narcolepsy = this.GetSMI<Narcolepsy>();
			hasStamina = staminaMonitor != null;
			hasNarcolepsy = narcolepsy != null;
			Subscribe((int)GameHashes.PathAdvanced, OnPathAdvanced);
			droppablesHolders.Add(this.gameObject, this);
		}
		public override void OnCleanUp()
		{
			droppablesHolders.Remove(this.gameObject);
			Unsubscribe((int)GameHashes.PathAdvanced, OnPathAdvanced);
			base.OnCleanUp();
		}

		public static bool TryGet(GameObject go, out DroppablesHolder holder) => droppablesHolders.TryGetValue(go, out holder);

		bool InterruptedByCurrentChore(Chore chore)
		{
			if(hasStamina && staminaMonitor.IsSleeping())			
				return true;
			
			if(hasNarcolepsy && narcolepsy.IsNarcolepsing())
				return true;

			if (chore == null)
				return false;

			if(chore is RecoverBreathChore)
				return true;

			return false;
		}
			

		void OnPathAdvanced(object _)
		{
			if (!MarkedForDrop.Any())
				return;

			///dont drop when not above solid tile
			if (!AboveSolidGround())
				return;

			///dont drop in transit tube
			if (kprefabID.HasTag(GameTags.InTransitTube))
				return;

			///if deliverable chunks of the current chore have merged into those marked for drop, prevent them from dropping
			Chore currentChore = choreDriver.GetCurrentChore();

			///exhaustion, narcolepsy, breath recovery
			if (InterruptedByCurrentChore(currentChore))
				return;

			if (currentChore != null && currentChore is FetchAreaChore fac)
			{
				var fetchInstance = fac.smi;
				var deliverables = fetchInstance.deliverables;
				foreach (var item in deliverables)
				{
					var id = item.gameObject?.GetInstanceID();
					if (id.HasValue)
						MarkedForDrop.Remove(id.Value);
				}
			}
			///if the workable is something in the hands of the dupe, dont drop it
			var task = worker.GetWorkable();
			if (task != null)
			{
				var id = task.gameObject?.GetInstanceID();
				if (id.HasValue)
					MarkedForDrop.Remove(id.Value);
			}


			var items = internalStorage.items;

			for (int i = items.Count - 1; i >= 0; --i)
			{
				var item = items[i];

				if (item == null)
					continue;
				int instanceID = item.GetInstanceID();
				if (MarkedForDrop.Contains(instanceID))
				{
					MarkItemInvisible(item, false);
					internalStorage.Drop(item);

					//if (item.TryGetComponent<Clearable>(out var markForSweep) && markForSweep.isClearable)
					//{
					//	markForSweep.MarkForClear();
					//	if (item.TryGetComponent<Prioritizable>(out var prioritizable))
					//		prioritizable.SetMasterPriority(new(PriorityScreen.PriorityClass.basic, 5));
					//}
				}
			}
			MarkedForDrop.Clear();
		}

		void MarkItemInvisible(GameObject item, bool setInvis)
		{
			if (item.TryGetComponent<Pickupable>(out var pickupable))
			{
				pickupable.prevent_absorb_until_stored = setInvis;
				if (setInvis)
					pickupable.ClearReservations();
			}
		}

		bool AboveSolidGround()
		{
			var cell = Grid.PosToCell(this);
			if (!Grid.IsValidCell(cell))  //should never happen, but check anyway
				return false;

			var floorCell = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(floorCell))
				return false;

			bool isSolidGround = Grid.Solid[floorCell];
			//SgtLogger.l("cell "+floorCell + " is solid? " + isSolidGround);
			return isSolidGround;
		}

		internal void MarkAllItemsForDrop()
		{
			foreach (var item in internalStorage.items)
			{
				MarkForDrop(item);
			}
		}

		void MarkForDrop(GameObject gameObject)
		{
			if (gameObject == null)
				return;
			MarkedForDrop.Add(gameObject.GetInstanceID());
			MarkItemInvisible(gameObject, true);
		}
		void UnmarkForDrop(GameObject gameObject)
		{
			if (gameObject == null)
				return;
			MarkedForDrop.Remove(gameObject.GetInstanceID());
			MarkItemInvisible(gameObject, false);
		}

		internal bool IsItemMarkedForDrop(GameObject gameObject)
		{
			if(gameObject == null)
				return false;
			return MarkedForDrop.Contains(gameObject.GetInstanceID());
		}

		internal Pickupable FindFetchTargetFromDroppables(FetchChore chore, ChoreConsumerState consumer_state)
		{
			foreach (var item in internalStorage.items)
			{
				if (!IsItemMarkedForDrop(item.gameObject))
				{
					continue;
				}
				if (!item.TryGetComponent<Pickupable>(out var pickupable))
				{
					continue;
				}

				if (FetchManager.IsFetchablePickup(pickupable, chore, consumer_state.storage))
				{
					UnmarkForDrop(item);
					return pickupable;					
				}
			}
			return null;
		}
	}
}
