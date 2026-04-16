using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace SmartAreaFill.Content.Scripts
{
	public class SmartFillToolInjector : KMonoBehaviour
	{
		public float ExpansionDelay = 0.5f;

		List<GameObject> VisPool = new List<GameObject>();
		List<GameObject> ActiveVis = new List<GameObject>();
		HashSet<int> CachedClaimedCells = new();
		Queue<int> CachedUnclaimedCells = new();

		Queue<int> WalkableCells = new();
		HashSet<int> VisitedCells = new();
		byte StartCellWorldIdx;
		bool includeDoorsAsSolids = true;

		enum ExpansionRules
		{
			None = 0,
			SameElement,
			NonSolidTile,
			SolidTile,
			TileExpansion,
			FollowSourceTileState,
			FilterTool,
		}
		enum ExpansionElementTypeRequirement
		{
			None, Solid, Liquid, Gas
		}


		[MyCmpReq] DragTool dragTool;
		[MyCmpGet] FilteredDragTool filteredDragTool;
		[MyCmpGet] BuildTool buildTool;
		bool useDragComplete = false;

		ExpansionRules Rule = ExpansionRules.None;
		ExpansionElementTypeRequirement ElementRequirement = ExpansionElementTypeRequirement.None;

		bool mouseHeldDown = false;
		float timeSinceMouseDown = 0f;
		int startCell = Grid.InvalidCell;
		int elementIdx = -1;
		HashSet<ObjectLayer> cachedLayers = [];
		BuildingDef cachedDef;

		Coroutine HoldDownCoroutine = null;

		public void OnLeftClickDown()
		{
			startCell = Grid.PosToCell(PlayerController.Instance.GetCursorPos());
			if (!Grid.IsValidCell(startCell))
			{
				startCell = Grid.InvalidCell;
				return;
			}
			mouseHeldDown = true;
			timeSinceMouseDown = 0;
			//CacheCells();
			PreCacheWalk();
			HoldDownCoroutine = StartCoroutine(FloodFillCoroutine());
		}
		public void OnLeftClickUp()
		{
			mouseHeldDown = false;
			StopFloodfillCoroutine();
			ConsumeCachedCells();
		}
		void StopFloodfillCoroutine()
		{
			if (HoldDownCoroutine != null)
			{
				StopCoroutine(HoldDownCoroutine);
				HoldDownCoroutine = null;
			}
		}

		public void OnDeactivateTool()
		{
			StopFloodfillCoroutine();
			RecycleVisualizers();
			CachedUnclaimedCells.Clear();
			CachedClaimedCells.Clear();
			mouseHeldDown = false;
		}
		void ConsumeCachedCells()
		{
			RecycleVisualizers();
			foreach (var cell in CachedClaimedCells)
			{
				UseTool(cell);
			}
			CachedUnclaimedCells.Clear();
			CachedClaimedCells.Clear();
		}

		void UseTool(int cell)
		{
			if (useDragComplete)
			{
				dragTool.OnDragComplete(Grid.CellToPos(cell), Grid.CellToPos(cell));
			}
			else
			{
				dragTool.OnDragTool(cell, Grid.GetCellDistance(startCell, cell));
			}
		}

		void RecycleVisualizers()
		{
			foreach (var vis in ActiveVis)
			{
				vis.SetActive(false);
				VisPool.Add(vis);
			}
			ActiveVis.Clear();
		}
		void PickNextCellForVisualization()
		{
			if (!CachedUnclaimedCells.Any())
				return;

			var cell = CachedUnclaimedCells.Dequeue();

			CachedClaimedCells.Add(cell);
			AddVisualizer(cell);
		}

		void AddVisualizer(int cell)
		{
			GameObject vis;
			if (VisPool.Count > 0)
			{
				vis = VisPool[0];
				VisPool.RemoveAt(0);
			}
			else
			{
				vis = Util.KInstantiateUI(dragTool.visualizer);
			}
			vis.transform.position = Grid.CellToPosCBC(cell, dragTool.visualizerLayer);
			vis.gameObject.SetActive(true);
			ActiveVis.Add(vis);
		}

		//void Update()
		//{
		//	if (PlayerController.Instance.activeTool != dragTool)
		//		return;

		//	if (!mouseHeldDown)
		//		return;

		//	timeSinceMouseDown += Time.unscaledDeltaTime;
		//	if (timeSinceMouseDown < ExpansionDelay)
		//		return;

		//	if (CellChanged())
		//	{
		//		OnDeactivateTool();
		//		return;
		//	}
		//	CacheNextCells();
		//	CacheNextCells();
		//	CacheNextCells();
		//	CacheNextCells();
		//	PickNextCellForVisualization();
		//}

		IEnumerator FloodFillCoroutine()
		{
			if (PlayerController.Instance.activeTool != dragTool || !mouseHeldDown)
			{
				SgtLogger.l("Canceling coroutine: tool not active");
				yield break;
			}


			while (timeSinceMouseDown < ExpansionDelay)
			{
				timeSinceMouseDown += Time.unscaledDeltaTime;
				yield return null;
			}

			do
			{
				if (CellChanged())
				{
					SgtLogger.l("Canceling coroutine: cell changed");
					OnDeactivateTool();
					yield break;
				}
				while(WalkableCells.Any() && !CachedUnclaimedCells.Any())
					CacheNextCells();
				PickNextCellForVisualization();
				yield return null;
			}
			while (WalkableCells.Any() || CachedUnclaimedCells.Any());
		}

		bool CellChanged()
		{
			int mouseCell = Grid.PosToCell(PlayerController.Instance.GetCursorPos());
			return mouseCell != startCell || !Grid.IsValidCell(mouseCell);
		}

		public override void OnSpawn()
		{
			ExpansionDelay = Config.Instance.ActivationDelay;
			includeDoorsAsSolids = Config.Instance.SolidDoors;
			base.OnSpawn();
			if (VisPool.Any())
			{
				for (int i = VisPool.Count - 1; i >= 0; i--)
				{
					var poolItem = VisPool[i];
					if (poolItem.IsNullOrDestroyed())
						VisPool.RemoveAt(i);
				}
			}

			var toolType = dragTool.GetType().Name;
			SgtLogger.l("FloodFillTool OnSpawn, ToolType: " + toolType);
			switch (toolType)
			{
				case nameof(BuildTool):
					Rule = ExpansionRules.TileExpansion;
					break;
				case nameof(DigTool):
					Rule = ExpansionRules.SameElement;
					ElementRequirement = ExpansionElementTypeRequirement.Solid;
					break;
				case nameof(AttackTool):
				case nameof(CaptureTool):
					useDragComplete = true;
					Rule = ExpansionRules.NonSolidTile;
					break;
				case nameof(HarvestTool):
				case nameof(DisinfectTool):
				case nameof(ClearTool):
				case "FilteredMoveSelectTool": //Mass move tool
					Rule = ExpansionRules.NonSolidTile;
					break;
				case nameof(MopTool):
					Rule = ExpansionRules.NonSolidTile;
					ElementRequirement = ExpansionElementTypeRequirement.Liquid;
					break;
				case nameof(PrioritizeTool):
				//	Rule = ExpansionRules.FollowSourceTileState;
				//	break;
				case nameof(DeconstructTool):
				case nameof(CancelTool):
				case nameof(EmptyPipeTool):
					Rule = ExpansionRules.FilterTool;
					break;
			}
		}

		void PreCacheAllCells()
		{
			CachedUnclaimedCells.Clear();
			var visited = new HashSet<int>();
			var world = Grid.WorldIdx[startCell];
			if (filteredDragTool != null)
			{
				CacheFilterableLayers();
			}
			else if (buildTool != null)
			{
				SgtLogger.l("BuildTool caching, current build tool def: " + buildTool.def.name);
				if (buildTool.def.WidthInCells == 1 && buildTool.def.HeightInCells == 1 && buildTool.def.ObjectLayer == ObjectLayer.Backwall)
				{
					cachedDef = buildTool.def;
				}
				else
					cachedDef = null;

				SgtLogger.l(cachedDef == null ? "invalid def for spread" : "valid def for spread");
			}
			bool followSource = (Rule == ExpansionRules.FollowSourceTileState);
			if (followSource)
				Rule = Grid.IsSolidCell(startCell) ? ExpansionRules.SolidTile : ExpansionRules.NonSolidTile;

			elementIdx = Grid.ElementIdx[startCell];
			Queue<int> toVisit = [];
			toVisit.Enqueue(startCell);
			if (IsValidCell(startCell))
			{
				CacheCells(toVisit, ref world, ref visited);
				//CachedUnclaimedCells.Sort(CellSort);
			}

			if (followSource)
				Rule = ExpansionRules.FollowSourceTileState;
		}
		void PreCacheWalk()
		{
			WalkableCells.Clear();
			VisitedCells.Clear();
			StartCellWorldIdx = Grid.WorldIdx[startCell];
			if (filteredDragTool != null)
			{
				CacheFilterableLayers();
			}
			else if (buildTool != null)
			{
				SgtLogger.l("BuildTool caching, current build tool def: " + buildTool.def.name);
				if (buildTool.def.WidthInCells == 1 && buildTool.def.HeightInCells == 1 && buildTool.def.ObjectLayer == ObjectLayer.Backwall)
				{
					cachedDef = buildTool.def;
				}
				else
					cachedDef = null;

				SgtLogger.l(cachedDef == null ? "invalid def for spread" : "valid def for spread");
			}
			bool followSource = (Rule == ExpansionRules.FollowSourceTileState);
			if (followSource)
				Rule = Grid.IsSolidCell(startCell) ? ExpansionRules.SolidTile : ExpansionRules.NonSolidTile;

			elementIdx = Grid.ElementIdx[startCell];
			if (IsValidCell(startCell))
				WalkableCells.Enqueue(startCell);

			if (followSource)
				Rule = ExpansionRules.FollowSourceTileState;
		}
		//int CellSort(int cell1, int cell2)
		//{
		//	int distance1 = Grid.GetCellDistance(startCell, cell1);
		//	int distance2 = Grid.GetCellDistance(startCell, cell2);
		//	return distance1 - distance2;
		//}
		void CacheFilterableLayers()
		{
			cachedLayers.Clear();
			if (filteredDragTool == null)
				return;

			for (int i = 0; i < (int)ObjectLayer.NumLayers; i++)
			{
				var layer = (ObjectLayer)i;
				//SgtLogger.l(i + ": " + layer);

				if (layer == ObjectLayer.Building)
					continue;

				var objectAtCell = Grid.Objects[startCell, i];
				if(objectAtCell == null) 
					continue;

				string filterLayer = filteredDragTool.GetFilterLayerFromGameObject(objectAtCell);


				bool inFilters = filteredDragTool.IsActiveLayer(filterLayer);
				if (!inFilters && layer == ObjectLayer.FoundationTile)
					inFilters = filteredDragTool.IsActiveLayer(ToolParameterMenu.FILTERLAYERS.BUILDINGS);

				if (inFilters)
				{
					cachedLayers.Add(layer);
					SgtLogger.l("Caching object layer " + layer);
				}
			}
			SgtLogger.l("total cached layers: " + cachedLayers.Count);
		}
		void CacheCells(Queue<int> toWalk, ref byte worldId, ref HashSet<int> visitedCells)
		{
			if (!toWalk.Any())
				return;
			do
			{
				int toCheck = toWalk.Dequeue();
				if (visitedCells.Contains(toCheck))
					continue;

				visitedCells.Add(toCheck);
				if (!IsValidCell(toCheck))
					continue;

				CachedUnclaimedCells.Enqueue(toCheck);

				int above = Grid.CellAbove(toCheck);
				int below = Grid.CellBelow(toCheck);
				int left = Grid.CellLeft(toCheck);
				int right = Grid.CellRight(toCheck);

				if (Grid.IsValidCellInWorld(above, worldId))
					toWalk.Enqueue(above);
				if (Grid.IsValidCellInWorld(below, worldId))
					toWalk.Enqueue(below);
				if (Grid.IsValidCellInWorld(left, worldId))
					toWalk.Enqueue(left);
				if (Grid.IsValidCellInWorld(right, worldId))
					toWalk.Enqueue(right);
			}
			while (toWalk.Any() && visitedCells.Count < 3000);
		}


		void CacheNextCells()
		{

			bool followSource = (Rule == ExpansionRules.FollowSourceTileState);
			if (followSource)
				Rule = Grid.IsSolidCell(startCell) ? ExpansionRules.SolidTile : ExpansionRules.NonSolidTile;

			if (!WalkableCells.Any())
				return;

			int toCheck = WalkableCells.Dequeue();
			if (VisitedCells.Contains(toCheck))
				return;

			VisitedCells.Add(toCheck);
			if (!IsValidCell(toCheck))
				return;

			CachedUnclaimedCells.Enqueue(toCheck);

			int above = Grid.CellAbove(toCheck);
			int below = Grid.CellBelow(toCheck);
			int left = Grid.CellLeft(toCheck);
			int right = Grid.CellRight(toCheck);

			if (Grid.IsValidCellInWorld(above, StartCellWorldIdx))
				WalkableCells.Enqueue(above);
			if (Grid.IsValidCellInWorld(below, StartCellWorldIdx))
				WalkableCells.Enqueue(below);
			if (Grid.IsValidCellInWorld(left, StartCellWorldIdx))
				WalkableCells.Enqueue(left);
			if (Grid.IsValidCellInWorld(right, StartCellWorldIdx))
				WalkableCells.Enqueue(right);

			if (followSource)
				Rule = ExpansionRules.FollowSourceTileState;
		}

		bool IsValidCell(int targetCell)
		{
			if (startCell == targetCell)
				return true;

			if (!Grid.IsVisible(targetCell))
				return false;

			switch (Rule)
			{
				case ExpansionRules.FilterTool:
					if (!cachedLayers.Any())
						return false;

					bool objectFound = false;
					foreach (var layer in cachedLayers)
					{
						if (Grid.Objects[targetCell, (int)layer] != null)
						{
							objectFound = true;
							break;
						}
					}
					if (!objectFound)
						return false;
					break;
				case ExpansionRules.None:
					return false;
				case ExpansionRules.SameElement:
					if (elementIdx != Grid.ElementIdx[targetCell] || Grid.Objects[targetCell, (int)ObjectLayer.FoundationTile] != null)
						return false;
					break;
				case ExpansionRules.TileExpansion:
					if (Grid.IsSolidCell(targetCell))
						return false;
					if (cachedDef == null)
						return false;
					if (!cachedDef.IsValidBuildLocation(buildTool.visualizer, Grid.CellToPos(targetCell), buildTool.GetBuildingOrientation))
						return false;
					break;
				case ExpansionRules.NonSolidTile:
					if (Grid.IsSolidCell(targetCell) || includeDoorsAsSolids && Grid.HasDoor[targetCell])
						return false;
					break;
				case ExpansionRules.SolidTile:
					if (!Grid.IsSolidCell(targetCell))
						return false;
					break;
				case ExpansionRules.FollowSourceTileState:
					break;
			}
			switch (ElementRequirement)
			{
				default:
				case ExpansionElementTypeRequirement.None:
					return true;
				case ExpansionElementTypeRequirement.Solid:
					return Grid.IsSolidCell(targetCell);
				case ExpansionElementTypeRequirement.Liquid:
					return Grid.IsLiquid(targetCell);
				case ExpansionElementTypeRequirement.Gas:
					return Grid.IsGas(targetCell);
			}
		}

	}
}
