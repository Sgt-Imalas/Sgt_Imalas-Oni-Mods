using Klei.AI;
using PeterHan.PLib.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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

		static List<Building> LastHoveredBuildings = [];

		static void GatherHoveredBuildings(int cell)
		{
			if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
				return;
			LastHoveredBuildings.Clear();


			for (int i = 0; i < (int)ObjectLayer.NumLayers; ++i)
			{
				var obj = Grid.Objects[cell, i];
				if(obj != null && obj.TryGetComponent<Building>(out var building) && !LastHoveredBuildings.Contains(building))
				{
					LastHoveredBuildings.Add(building);
				}
			}
		}
		private static int SortHoverCards(ScenePartitionerEntry x, ScenePartitionerEntry y)
		{
			return SortSelectables(x.obj as KMonoBehaviour, y.obj as KMonoBehaviour);
		}
		private static int SortSelectables(KMonoBehaviour x, KMonoBehaviour y)
		{
			if (x == null && y == null)
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;
			int num = x.transform.GetPosition().z.CompareTo(y.transform.GetPosition().z);
			return num != 0 ? num : x.GetInstanceID().CompareTo(y.GetInstanceID());
		}

		static void RefreshHoveredBuildings(int cell)
		{
			GatherHoveredBuildings(cell);
		}

		internal static void SelectNextBuilding()
		{
			var mousePos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
			int cell = Grid.PosToCell(mousePos);
			if (!Grid.IsValidBuildingCell(cell))
			{
				return;
			}

			if (cell != LastCell || !LastHoveredBuildings.Any())
			{
				SgtLogger.l("old cell: " + LastCell + " new cell: " + cell);
				LastCell = cell;
				SelectedIndex = 0;
				RefreshHoveredBuildings(cell);
			}

			if (!LastHoveredBuildings.Any())
			{
				SelectedIndex = 0;
				return;
			}

			if (SelectedIndex >= LastHoveredBuildings.Count())
			{
				SelectedIndex = 0;
			}
			var targetBuilding = LastHoveredBuildings.ElementAt(SelectedIndex);
			SgtLogger.l("Selected building: " + targetBuilding.GetProperName() + " at index " + SelectedIndex);
			SelectedIndex++;

			PlanScreen.Instance.CopyBuildingOrder(targetBuilding);
		}
	}
}
