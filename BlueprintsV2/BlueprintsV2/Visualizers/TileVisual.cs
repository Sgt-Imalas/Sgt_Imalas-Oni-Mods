
using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.Visualizers.CustomTileRenderer;
using BlueprintsV2.Tools;
using Database;
using HarmonyLib;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;
using UtilLibs;
using static BlueprintsV2.BlueprintData.BlueprintState;
using static STRINGS.DUPLICANTS.STATUSITEMS;

namespace BlueprintsV2.Visualizers
{

	public class TileVisual : BuildingVisual, ICleanableVisual
	{
		readonly static Dictionary<ulong, Dictionary<int, BuildingDef>> ActiveTileVisuals = [];
		public static bool HasTileAt(ulong playerId, int cell, out BuildingDef visual)
		{
			visual = null;
			return ActiveTileVisuals.TryGetValue(playerId, out var dict) && dict.TryGetValue(cell, out visual);
		}
		public static void RegisterReplacementVis(int cell, BuildingDef def)
		{
			if (!ActiveTileVisuals.ContainsKey(BlueprintState.PlayerId_ReplacementTiles))
				ActiveTileVisuals[PlayerId_ReplacementTiles] = new();

			ActiveTileVisuals[PlayerId_ReplacementTiles][cell] = def;

			CustomTileRenderer.AddTileBlock(PlayerId_ReplacementTiles, def, cell);
			CustomTileRenderer.UpdateTileRendererForPlayer(PlayerId_ReplacementTiles);
		}
		public static void UnregisterReplacementVis(int cell, BuildingDef def)
		{
			if (!ActiveTileVisuals.ContainsKey(BlueprintState.PlayerId_ReplacementTiles))
				ActiveTileVisuals[PlayerId_ReplacementTiles] = new();

			if (ActiveTileVisuals[PlayerId_ReplacementTiles].TryGetValue(cell, out var exiting))
				ActiveTileVisuals[PlayerId_ReplacementTiles].Remove(cell);

			CustomTileRenderer.RemoveTileBlock(PlayerId_ReplacementTiles, def, cell);
			CustomTileRenderer.UpdateTileRendererForPlayer(PlayerId_ReplacementTiles);
		}

		public static void OnPlayerAdded(ulong playerId)
		{
			ActiveTileVisuals[playerId] = [];
		}
		public static void OnPlayerRemoved(ulong playerId)
		{
			ActiveTileVisuals.Remove(playerId);
		}

		public override PermittedRotations GetAllowedRotations() => BlueprintTransformationInfo.All;
		public override void ApplyRotation(Orientation rotation, bool flippedX, bool flippedY)
		{
			///tiles dont rotate
		}

		public int DirtyCell { get; private set; } = -1;

		private readonly bool hasReplacementLayer;
		private bool seated = false;

		public TileVisual(BuildingConfig buildingConfig, int cell, ulong playerId) : base(buildingConfig, cell, playerId)
		{
			if (!ActiveTileVisuals.ContainsKey(playerId))
				ActiveTileVisuals[playerId] = [];

			hasReplacementLayer = buildingConfig.BuildingDef.ReplacementLayer != ObjectLayer.NumLayers;
			BlueprintState.ColoredCells[playerId][cell] = GetVisualizerColor(cell);
			this.cell = -1;
			DirtyCell = cell;
			UpdateGrid(cell);
		}
		static Dictionary<ulong, Dictionary<BuildingDef, GameObject>> _tileVisualizers = [];

		protected override void CreateVisualizer()
		{
			hasKbac = false;
			kbac = null;
			if (!_tileVisualizers.TryGetValue(_playerId, out var sharedVis))
				_tileVisualizers[_playerId] = sharedVis = [];

			if (sharedVis.TryGetValue(_def, out var vis) && !vis.IsNullOrDestroyed())
			{
				Visualizer = vis;
				return;
			}
			base.CreateVisualizer();
			sharedVis[_def] = Visualizer;
		}
		public override void DestroyVisualizer()
		{
			Visualizer = null;
		}

		public override void ForceRedraw()
		{
			ApplyColorIfChanged(cell);
		}

		public override void ApplyColorIfChanged(int cellParam)
		{
			if (isTile)
			{
				BlueprintState.ColoredCells[_playerId][cell] = GetVisualizerColor(cell);
			}
		}

		public override void MoveVisualizer(int cellParam, bool forceRedraw)
		{
			if (cellParam != cell || forceRedraw)
			{
				//Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, _def.SceneLayer));
				UpdateGrid(cellParam);
				ApplyColorIfChanged(cellParam);
				cell = cellParam;
			}
		}

		public void Clean()
		{
			if (!Grid.IsValidBuildingCell(DirtyCell) || !seated)
			{
				return;
			}

			if (DirtyCell != -1 && Grid.IsValidBuildingCell(DirtyCell))
			{
				if (ActiveTileVisuals[_playerId].TryGetValue(DirtyCell, out var vis) && vis == this._def)
				{
					CustomTileRenderer.RemoveTileBlock(_playerId, _def, DirtyCell);
					ActiveTileVisuals[_playerId].Remove(DirtyCell);
				}
			}
			DirtyCell = -1;
			seated = false;
		}
		private void UpdateGrid(int cellParam)
		{
			Clean();
			if (seated)
				return;
			if (Grid.IsValidBuildingCell(cellParam) && isTile)
			{
				if (ActiveTileVisuals[_playerId].TryGetValue(cellParam, out var existing))
				{
					//SgtLogger.warning(_playerId + ": there is already a tilevisual in " + cellParam);
					return;
				}
				//bool replacing = hasReplacementLayer && CanReplace(cell);
				CustomTileRenderer.AddTileBlock(_playerId, _def, cellParam);
				ActiveTileVisuals[_playerId][cellParam] = this._def;
				DirtyCell = cellParam;
				seated = true;	
			}
		}
		public override bool AllowedForRotation(Orientation rotation, bool flippedX, bool flippedY) => true;
	}

}
