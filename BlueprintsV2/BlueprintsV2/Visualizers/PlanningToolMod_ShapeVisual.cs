using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration.EnumMirrors;
using BlueprintsV2.Visualizers;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;
using static STRINGS.UI;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
	internal class PlanningToolMod_ShapeVisual : IVisual
	{
		public GameObject Visualizer { get; private set; }
		public Vector2I Offset { get; private set; }

		public PlanScreen.RequirementsState RequirementsState => PlanScreen.RequirementsState.Complete;

		public string BuildingID => null;
		PlanShape Shape;
		PlanColor Color;


		public PlanningToolMod_ShapeVisual(int cell, Vector2I offset, PlanShape shape, PlanColor color)
		{
			Visualizer = GameUtil.KInstantiate(Assets.GetPrefab(PlanningToolShapePreviewConfig.ID), Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront), Grid.SceneLayer.FXFront, nameof(PlanningToolShapePreviewConfig) + shape);
			Visualizer.SetActive(IsPlaceable(cell));
			Offset = offset;
			if (Visualizer.TryGetComponent<PlanningToolShapePreview>(out var shapePreview))
			{
				shapePreview.SetVisuals(shape, color);
			}
			Shape = shape;
			Color = color;
		}

		public bool IsPlaceable(int cellParam)
		{
			return Grid.IsValidCell(cellParam) && Grid.IsVisible(cellParam);
		}

		public void MoveVisualizer(int cellParam, bool forceRedraw)
		{
			Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, Grid.SceneLayer.FXFront));
			Visualizer.SetActive(IsPlaceable(cellParam));
		}
		public void ForceRedraw() { }

		public bool TryUse(int cellParam)
		{
			if (IsPlaceable(cellParam))
			{
				PlanningTool_Integration.PlacePlan(cellParam, Shape, Color);				
				return true;
			}

			return false;
		}

		public PermittedRotations GetAllowedRotations() => BlueprintState.All;
		public void ApplyRotation(Orientation rotation, bool flipped, bool flippedY)
		{
			//digging doesnt get rotated
		}

		public void RefreshColor()
		{
			//no tinting
		}
	}
}
