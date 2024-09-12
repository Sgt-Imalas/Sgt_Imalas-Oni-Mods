using PeterHan.PLib.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MassMoveTo
{
	internal class ModAssets
	{
		static HashSet<Movable> cachedMovables = new();
		static HashSet<int> cachedTargetCells = new();
		static PrioritySetting cachedPriority;

		public static Sprite MassMoveToolIcon;
		public static bool HasStashed => cachedMovables.Count > 0;
		public static int TargetCellCount => cachedTargetCells.Count;

		internal static void ClearStashed(bool removeMarks = true)
		{
			if (removeMarks)
			{
				foreach (var item in cachedMovables)
				{
					if (item != null && item.gameObject != null)
						item.gameObject.RemoveTag(GameTags.MarkedForMove);
				}
			}

			cachedMovables.Clear();
		}

		internal static void MarkForMove(Movable movable, PrioritySetting priority)
		{
			cachedPriority = priority;
			if (!cachedMovables.Contains(movable) && !movable.IsMarkedForMove)
			{
				cachedMovables.Add(movable);
				movable.gameObject.AddTag(GameTags.MarkedForMove);
			}
		}

		static Ref<Storage> AddOrGetStorageProxy(int cell)
		{
			Storage targetStorage;
			Ref<Storage> targetProxy;

			if (Grid.Objects[cell, 44] != null)
			{
				targetStorage = Grid.Objects[cell, 44].GetComponent<Storage>();
			}
			else
			{
				Vector3 position = Grid.CellToPosCBC(cell, MoveToLocationTool.Instance.visualizerLayer);
				GameObject obj = Util.KInstantiate(Assets.GetPrefab(MovePickupablePlacerConfig.ID), position);
				targetStorage = obj.GetComponent<Storage>();
				obj.SetActive(value: true);
			}

			if (targetStorage == null)
			{
				Debug.LogError("Mass Move tool: target storage was null!");
			}
			targetProxy = new Ref<Storage>(targetStorage);
			return targetProxy;
		}

		public static List<T>[] Partition<T>(List<T> list, int totalPartitions)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			if (totalPartitions < 1)
				throw new ArgumentOutOfRangeException("totalPartitions");

			List<T>[] partitions = new List<T>[totalPartitions];

			int maxSize = (int)Math.Ceiling(list.Count / (double)totalPartitions);
			int k = 0;

			for (int i = 0; i < partitions.Length; i++)
			{
				partitions[i] = new List<T>();
				for (int j = k; j < k + maxSize; j++)
				{
					if (j >= list.Count)
						break;
					partitions[i].Add(list[j]);
				}
				k += maxSize;
			}

			return partitions;
		}

		internal static void MoveAllItems()
		{
			if (cachedMovables.Count > 0 && TargetCellCount > 0)
			{
				var targetCells = cachedTargetCells.ToList();
				var movableChunks = Partition<Movable>(cachedMovables.ToList(), TargetCellCount);
				for (int i = 0; i < movableChunks.Length; i++)
				{
					MoveItemsToLocation(movableChunks[i], targetCells[i]);
				}
				ClearStashed(false);
			}
			ClearCachedTargets();
		}
		private static void MoveItemsToLocation(List<Movable> items, int targetCell)
		{
			var proxy = AddOrGetStorageProxy(targetCell);
			proxy.Get().prioritizable.SetMasterPriority(cachedPriority);

			foreach (Movable movable in items)
			{
				if (movable != null
					&& movable.gameObject != null
					&& !movable.IsMarkedForMove)
				{
					movable.storageProxy = proxy;
					movable.MoveToLocation(targetCell);
				}
			}
		}


		public static class ActionKeys
		{
			public static string ACTION_MASSMOVETOOL = "massmovetool.open";
		}
		public static class Actions
		{
			public static PAction MassMoveTool_Open { get; set; }
		}
		internal static void RegisterActions()
		{
			Actions.MassMoveTool_Open = new PActionManager().CreateAction(ActionKeys.ACTION_MASSMOVETOOL,
				STRINGS.UI.TOOLS.MOVETOSELECTTOOL.NAME, new PKeyBinding(KKeyCode.K, Modifier.Shift));
		}

		internal static void ClearCachedTargets()
		{
			cachedTargetCells.Clear();
		}

		internal static void RegisterTargetCell(int cell)
		{
			if (cachedTargetCells.Contains(cell))
				return;

			cachedTargetCells.Add(cell);
		}
	}
}
