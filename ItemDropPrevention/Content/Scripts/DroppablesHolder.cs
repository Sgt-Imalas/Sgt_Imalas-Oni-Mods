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

		HashSet<int> MarkedForDrop = [];

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.PathAdvanced, OnPathAdvanced);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.PathAdvanced, OnPathAdvanced);
			base.OnCleanUp();
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

			var items = internalStorage.items;

			///if deliverable chunks of the current chore have merged into those marked for drop, prevent them from dropping
			Chore currentChore = choreDriver.GetCurrentChore();
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


			for (int i = items.Count - 1; i >= 0; --i)
			{
				var item = items[i];

				if (item == null)
					continue;
				int instanceID = item.GetInstanceID();
				if (MarkedForDrop.Contains(instanceID))
				{
					if (item.TryGetComponent<Pickupable>(out var pickupable))
						pickupable.prevent_absorb_until_stored = false;
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
				//SgtLogger.l("Marking " + item + " as drop");
				MarkedForDrop.Add(item.GetInstanceID());
				if(item.TryGetComponent<Pickupable>(out var pickupable))
					pickupable.prevent_absorb_until_stored = true;
			}
		}
	}
}
