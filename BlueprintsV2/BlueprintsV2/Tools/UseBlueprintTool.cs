
using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.UnityUI;
using UnityEngine;

namespace BlueprintsV2.Tools
{
	public class UseBlueprintTool : InterfaceTool
	{
		public static UseBlueprintTool Instance { get; private set; }

		public UseBlueprintTool()
		{
			Instance = this;
			BlueprintState.CurrentStateInfo().ForceBuild = false;
		}

		public UseBlueprintToolHoverCard HoverCard;
		public bool ToolActive { get; private set; }

		Vector3 _gridDragStart = default;

		public static void DestroyInstance()
		{
			Instance = null;
		}

		public void CreateVisualizer()
		{
			if (visualizer != null)
			{
				Destroy(visualizer);
			}

			visualizer = new GameObject("UseBlueprintVisualizer");
			visualizer.SetActive(false);

			GameObject offsetObject = new GameObject();
			SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
			spriteRenderer.color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
			spriteRenderer.sprite = ModAssets.BLUEPRINTS_USE_VISUALIZER_SPRITE;

			offsetObject.transform.SetParent(visualizer.transform);
			//offsetObject.transform.localPosition = new Vector3(0, Grid.HalfCellSizeInMeters);
			offsetObject.transform.localPosition = new Vector3(-Grid.HalfCellSizeInMeters, 0);
			var sprite = spriteRenderer.sprite;
			offsetObject.transform.localScale = new Vector3(
				Grid.CellSizeInMeters / (sprite.texture.width / sprite.pixelsPerUnit),
				Grid.CellSizeInMeters / (sprite.texture.height / sprite.pixelsPerUnit)
			);

			offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
			visualizer.transform.SetParent(transform);

			OnMouseMove(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			HoverCard = gameObject.AddComponent<UseBlueprintToolHoverCard>();
		}

		public override void OnActivateTool()
		{
			base.OnActivateTool();
			BlueprintState.CurrentStateInfo().IsPlacingSnapshot = false;

			ToolMenu.Instance.PriorityScreen.Show();
			ShowBlueprintsWindow();
			ToolActive = true;
		}


		void ShowBlueprintsWindow()
		{
			BlueprintSelectionScreen.ShowWindow(OnBlueprintSelected, ModAssets.SelectedBlueprint, true);
		}

		public void OnBlueprintSelected(Blueprint selected)
		{
			ModAssets.SelectedBlueprint = selected;
			//SgtLogger.l("OnBlueprintSelected, selected ? " + (ModAssets.SelectedBlueprint != null));
			if (ModAssets.SelectedBlueprint != null)
			{
				GridCompositor.Instance.ToggleMajor(true);
				VisualizeSelectedBlueprint();
				CurrentBlueprintStateScreen.ShowScreen(true);
				CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(ModAssets.SelectedBlueprint);
			}
			else
			{
				CurrentBlueprintStateScreen.ShowScreen(false);
				if (visualizer != null)
				{
					Destroy(visualizer);
				}
				//deactivate tool if no bp selected:
				//SgtLogger.l("Deactivating UseBPTool");
				ToolMenu.Instance.ClearSelection();
				string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
				if (sound != null)
					KMonoBehaviour.PlaySound(sound);
				this.DeactivateTool();
			}
		}

		void VisualizeSelectedBlueprint()
		{
			BlueprintState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), ModAssets.SelectedBlueprint);
		}

		public override void OnDeactivateTool(InterfaceTool newTool)
		{
			base.OnDeactivateTool(newTool);
			BlueprintState.CurrentStateInfo().ForceBuild = false;

			BlueprintState.ClearVisuals();
			ToolMenu.Instance.PriorityScreen.Show(false);
			GridCompositor.Instance.ToggleMajor(false);
			CurrentBlueprintStateScreen.ShowScreen(false);
			ToolActive = false;

		}

		public override void OnLeftClickDown(Vector3 cursorPos)
		{
			_gridDragStart = default;
			base.OnLeftClickDown(cursorPos);

			if (hasFocus)
			{
				if (BlueprintState.CurrentStateInfo().SnapToGrid)
					_gridDragStart = cursorPos;
				BlueprintState.UseBlueprint(BlueprintState.PlayerId_DefaultTilePreviews, Grid.PosToXY(cursorPos));
			}
		}
		public override void OnLeftClickUp(Vector3 cursor_pos)
		{
			base.OnLeftClickUp(cursor_pos);
			_gridDragStart = default;
		}
		public override void OnMouseMove(Vector3 cursorPos)
		{
			base.OnMouseMove(cursorPos);

			if (hasFocus)
			{
				BlueprintState.UpdateVisual(BlueprintState.PlayerId_DefaultTilePreviews, Grid.PosToXY(cursorPos));
				OnMouseMovedGridPlacement(cursorPos);
			}
		}
		public void OnMouseMovedGridPlacement(Vector3 cursorPos)
		{
			if (ModAssets.SelectedBlueprint == null || _gridDragStart == default)
				return;

			var stateInfo = BlueprintState.CurrentStateInfo();

			if (!stateInfo.SnapToGrid)
				return;
			int xStep = stateInfo.GridSnapX;
			int yStep = stateInfo.GridSnapY;

			if (xStep == 0 || yStep == 0)
				return;

			if (Grid.PosToCell(_gridDragStart) == Grid.PosToCell(cursorPos))
				return;

			var downXY = Grid.PosToXY(_gridDragStart);

			Grid.PosToXY(cursorPos, out int X, out int Y);
			//SgtLogger.l($"Down: {downXY} current: {X},{Y} XAlign:{downXY.X - X} XAlign:{(downXY.X - X) % xStep == 0}, YAlign:{downXY.Y - Y} YAlign:{(downXY.Y - Y) % yStep == 0}, steps: {xStep}x{yStep},");
			int xDiff = (downXY.X - X + xStep);
			int yDiff = (downXY.Y - Y + yStep);
			bool xAligned = (xDiff == 0 || xDiff % xStep == 0);
			bool yAligned = (yDiff == 0 || yDiff % yStep == 0);

			if (xAligned && yAligned)
			{
				BlueprintState.UseBlueprint(BlueprintState.PlayerId_DefaultTilePreviews, new(X, Y));
				_gridDragStart = cursorPos;
			}
		}


		void SetForceMaterialChange(bool enabled)
		{
			BlueprintState.CurrentStateInfo().ForceBuild = enabled;
			BlueprintState.RefreshBlueprintVisualizers();
			CurrentBlueprintStateScreen.Instance.SetForceMaterialChange(enabled);
		}

		public override void OnKeyDown(KButtonEvent buttonEvent)
		{
			if (DetailsScreen.Instance?.isEditing ?? false)
				return;

			if (ModAssets.BlueprintFileHandling.HasBlueprints())
			{
				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleHotkeyToolTips.GetKAction()))
				{
					BlueprintState.ToggleHotkeyTooltips();
				}
				else
				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleForce.GetKAction()))
				{
					SetForceMaterialChange(true);
				}
				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsReopenSelectionAction.GetKAction()))
				{
					ShowBlueprintsWindow();
				}

				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSwapAnchorAction.GetKAction()))
				{
					BlueprintState.CurrentStateInfo().NextAnchorState();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(Action.RotateBuilding) || buttonEvent.TryConsume(ModAssets.Actions.BlueprintsRotate.GetKAction()))
				{
					BlueprintState.CurrentStateInfo().TryRotateBlueprint();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsRotateInverse.GetKAction()))
				{
					BlueprintState.CurrentStateInfo().TryRotateBlueprint(true);
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipHorizontal.GetKAction()))
				{
					BlueprintState.CurrentStateInfo().FlipHorizontal();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipVertical.GetKAction()))
				{
					BlueprintState.CurrentStateInfo().FlipVertical();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectPrevious.GetKAction()))
				{
					SelectPrevBlueprint();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectNext.GetKAction()))
				{
					SelectNextBlueprint();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectPreviousFolder.GetKAction()))
				{
					SelectPrevFolder();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectNextFolder.GetKAction()))
				{
					SelectNextFolder();
				}
			}

			base.OnKeyDown(buttonEvent);
		}

		public void SelectNextFolder()
		{
			ModAssets.SelectNextFolder();
			VisualizeSelectedBlueprint();
			CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(ModAssets.SelectedBlueprint);
		}
		public void SelectPrevFolder()
		{
			ModAssets.SelectPreviousFolder();
			VisualizeSelectedBlueprint();
			CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(ModAssets.SelectedBlueprint);
		}
		public void SelectNextBlueprint()
		{
			ModAssets.GetCurrentFolder().SelectNext();
			VisualizeSelectedBlueprint();
			CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(ModAssets.SelectedBlueprint);
		}
		public void SelectPrevBlueprint()
		{
			ModAssets.GetCurrentFolder().SelectPrev();
			VisualizeSelectedBlueprint();
			CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(ModAssets.SelectedBlueprint);
		}

		public override void OnKeyUp(KButtonEvent buttonEvent)
		{
			if (DetailsScreen.Instance?.isEditing ?? false)
				return;
			if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleForce.GetKAction()))
			{
				SetForceMaterialChange(false);
			}
			BlueprintState.OnStateChanged();
		}
	}
}
