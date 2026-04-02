using Klei.AI;
using PeterHan.PLib.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HoverPipetteTool
{
	internal class ModAssets
	{
		public static PAction PickHoveredBuilding { get; set; }

		internal static void RegisterActions()
		{
			PickHoveredBuilding = new PActionManager().CreateAction("HPT_PickHoveredBuilding", STRINGS.UI.ACTIONS.PIPETTE_TOOL_SELECT.NAME, new PKeyBinding(KKeyCode.Q));
		}

		static int LastCell = -1, SelectedIndex = 0;

		//static IEnumerable<Building> GetHoveredBuildings()
		//{
		//	ListPool<ScenePartitionerEntry, SelectTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
		//	GameScenePartitioner.Instance.GatherEntries((int)pos2.x, (int)pos2.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
		//	pooledList.Sort((ScenePartitionerEntry x, ScenePartitionerEntry y) => SortHoverCards(x, y));
		//	foreach (ScenePartitionerEntry item in pooledList)
		//	{
		//		KCollider2D kCollider2D = item.obj as KCollider2D;
		//		if (!(kCollider2D == null) && kCollider2D.Intersects(new Vector2(pos2.x, pos2.y)))
		//		{
		//			KSelectable kSelectable = kCollider2D.GetComponent<KSelectable>();
		//			if (kSelectable == null)
		//			{
		//				kSelectable = kCollider2D.GetComponentInParent<KSelectable>();
		//			}

		//			if (!(kSelectable == null) && kSelectable.isActiveAndEnabled && !hits.Contains(kSelectable) && kSelectable.IsSelectable)
		//			{
		//				hits.Add(kSelectable);
		//			}
		//		}
		//	}

		//	pooledList.Recycle();
		//}

		internal static void SelectNextBuilding()
		{
			InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
			if (activeTool == null)
				return;

			var mousePos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
			int cell = Grid.PosToCell(mousePos);
			if (!Grid.IsValidBuildingCell(cell))
			{
				return;
			}

			if(cell != LastCell)
			{
				LastCell = cell;
				SelectedIndex = 0;
			}

			var targets = activeTool.hits.Where(hit => hit.TryGetComponent<Building>(out _)).Select(t => t.GetComponent<Building>());
			if(!targets.Any())
			{
				SelectedIndex = 0;
				return;
			}

			if (SelectedIndex >= targets.Count())
			{
				SelectedIndex = 0;
			}
			var targetBuilding = targets.ElementAt(SelectedIndex);

			PlanScreen.Instance.CopyBuildingOrder(targetBuilding);
		}
	}
}
