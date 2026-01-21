using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.Visualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
	internal class TextNoteVisual : IVisual
	{
		public GameObject Visualizer { get; private set; }
		public Vector2I Offset { get; private set; }

		public PlanScreen.RequirementsState RequirementsState => PlanScreen.RequirementsState.Complete;

		public string BuildingID => null;

		string Title, Text;
		Color Tint;

		public TextNoteVisual(int cell, Vector2I offset, string title, string text, Color tint)
		{
			Visualizer = GameUtil.KInstantiate(Assets.GetPrefab(TextNoteConfig.ID), Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront), Grid.SceneLayer.FXFront, "BlueprintModLiquidIndicatorVisual");
			Visualizer.SetActive(IsPlaceable(cell));
			Offset = offset;
			if (Visualizer.TryGetComponent<TextNote>(out var info))
			{
				info.SetInfo(title, text, tint);
			}
			Title = title;
			Text = text;
			Tint = tint;
		}

		public bool IsPlaceable(int cellParam)
		{
			return Grid.IsValidCell(cellParam) && Grid.IsVisible(cellParam) && !(Grid.Solid[cellParam] && Grid.Foundation[cellParam]);
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
				var existingItem = Grid.Objects[cellParam, (int)ModAssets.BlueprintNotesLayer];

				if (existingItem != null)
				{
					existingItem.DeleteObject();
					Grid.Objects[cellParam, (int)ModAssets.BlueprintNotesLayer] = null;
				}

				var infoIndicator = Util.KInstantiate(Assets.GetPrefab(TextNoteConfig.ID));
				Grid.Objects[cellParam, (int)ModAssets.BlueprintNotesLayer] = infoIndicator;
				Vector3 posCbc = Grid.CellToPosCBC(cellParam, MopTool.Instance.visualizerLayer);
				posCbc.z -= 0.15f;
				infoIndicator.transform.SetPosition(posCbc);
				if (infoIndicator.TryGetComponent<TextNote>(out var info))
				{
					info.SetInfo(Title, Text, Tint, true);
				}
				infoIndicator.SetActive(true);

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
