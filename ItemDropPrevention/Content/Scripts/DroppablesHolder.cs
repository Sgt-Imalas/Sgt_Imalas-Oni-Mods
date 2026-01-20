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

	internal class DroppablesHolder : KMonoBehaviour, ISim33ms
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
		int handle = -1;

		public override void OnSpawn()
		{
			base.OnSpawn();
			staminaMonitor = this.GetSMI<StaminaMonitor.Instance>();
			narcolepsy = this.GetSMI<Narcolepsy>();
			hasStamina = staminaMonitor != null;
			hasNarcolepsy = narcolepsy != null;
			handle = Subscribe((int)GameHashes.PathAdvanced, OnPathAdvanced);
			droppablesHolders.Add(this.gameObject, this);
		}
		public override void OnCleanUp()
		{
			droppablesHolders.Remove(this.gameObject);
			Unsubscribe(handle);
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
			DropMarkedItemsIfValid();
		}

		public void Sim33ms(float dt)
		{
			DropMarkedItemsIfValid();
		}

		void DropMarkedItemsIfValid()
		{
			if (!MarkedForDrop.Any())
				return;

			///dont drop when not above solid tile
			//SgtLogger.l(this.GetProperName() + " in cell: " + Grid.PosToCell(this)+", can drop?: "+AboveSolidGround());
			if (!AboveSolidGround())
				return;


			///dont drop in transit tube
			if (kprefabID.HasTag(GameTags.InTransitTube))
				return;

			Chore currentChore = choreDriver.GetCurrentChore();

			///exhaustion, narcolepsy, breath recovery
			if (InterruptedByCurrentChore(currentChore))
				return;

			///if deliverable chunks of the current chore have merged into those marked for drop, prevent them from dropping
			if (currentChore != null && currentChore is FetchAreaChore fac)
			{
				var fetchInstance = fac.smi;
				var deliverables = fetchInstance.deliverables;
				foreach (var item in deliverables)
				{
					var id = item.gameObject?.GetInstanceID();
					if (id.HasValue)
					{
						MarkItemInvisible(item.gameObject, false);
						MarkedForDrop.Remove(id.Value);
					}
				}
			}
			///if the workable is something in the hands of the dupe, dont drop it
			var task = worker.GetWorkable();
			if (task != null)
			{
				var id = task.gameObject?.GetInstanceID();
				if (id.HasValue)
				{
					MarkItemInvisible(task.gameObject, false);
					MarkedForDrop.Remove(id.Value);
				}
			}


			var items = internalStorage.items;

			bool reWrangleCritters = Config.Instance.WrangleDroppedCritters;
			bool sweepDroppedItems = Config.Instance.SweepDroppedItems;

			for (int i = items.Count - 1; i >= 0; --i)
			{
				var item = items[i];

				if (item == null)
					continue;
				int instanceID = item.GetInstanceID();
				if (MarkedForDrop.Contains(instanceID))
				{
					MarkItemInvisible(item, false);
					PostProcessDroppedItem(CenterInCell(internalStorage.Drop(item)), reWrangleCritters, sweepDroppedItems);
				}
			}
			MarkedForDrop.Clear();
		}

		/// <summary>
		/// Force the item position to be in the center of the cell
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		GameObject CenterInCell(GameObject item)
		{
			var middleOfCell = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore);
			item.transform.SetPosition(middleOfCell);
			return item;
		}

		static void PostProcessDroppedItem(GameObject item, bool reWrangleCritters, bool sweepDroppedItems)
		{
			if (!reWrangleCritters && !sweepDroppedItems)
				return;

			if(item.IsNullOrDestroyed()) return;

			if(item.TryGetComponent<Capturable>(out var wrangleable) && wrangleable.IsCapturable())
			{
				if(reWrangleCritters)
					wrangleable.MarkForCapture(true);
			}
			else if(item.TryGetComponent<Clearable>(out var markForSweep) && markForSweep.isClearable)
			{
				if(sweepDroppedItems)
					markForSweep.MarkForClear(true);
			}
		}

		void MarkItemInvisible(GameObject item, bool setInvis)
		{
			//SgtLogger.l($"{(setInvis?"Marking":"Unmarking")} {item} as invisible");
			if (item.TryGetComponent<Pickupable>(out var pickupable))
			{
				pickupable.prevent_absorb_until_stored = setInvis;
				if (setInvis)
					pickupable.ClearReservations();
			}
			if(item.TryGetComponent<KPrefabID>(out var prefabID))
			{
				if (setInvis)
					prefabID.AddTag(GameTags.MarkedForMove);
				else
					prefabID.RemoveTag(GameTags.MarkedForMove);
			}
		}

		bool AboveSolidGround()
		{
			var cell = Grid.PosToCell(this);
			if (!Grid.IsValidCell(cell))  //should never happen, but check anyway
				return false;

			if (Grid.HasDoor[cell]) //dont drop inside of doors
				return false;

			var floorCell = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(floorCell))
				return false;

			bool isSolidGround = Grid.IsSolidCell(floorCell);

			if (isSolidGround)
				return true;

			//check for 1 dropheight
			int cellBelowFloorCell = Grid.CellBelow(floorCell);

			if (!Grid.IsValidCell(cellBelowFloorCell))
				return false;

			return Grid.IsSolidCell(cellBelowFloorCell);
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
			//SgtLogger.l($"Marking {gameObject} as drop later");
			if (gameObject.IsNullOrDestroyed())
				return;
			MarkedForDrop.Add(gameObject.GetInstanceID());
			MarkItemInvisible(gameObject, true);
		}
		void UnmarkForDrop(GameObject gameObject)
		{
			//SgtLogger.l($"Unarking {gameObject} as drop later");
			if (gameObject.IsNullOrDestroyed())
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
				if (!item.TryGetComponent<KPrefabID>(out var prefabID))
				{
					continue;
				}
				prefabID.RemoveTag(GameTags.MarkedForMove);
				if (FetchManager.IsFetchablePickup(pickupable, chore, consumer_state.storage))
				{
					UnmarkForDrop(item);
					return pickupable;
				}
				prefabID.AddTag(GameTags.MarkedForMove);
			}
			return null;
		}
	}
}
