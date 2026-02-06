
using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.UnityUI;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.Tools
{
	public class UseBlueprintTool : InterfaceTool
	{
		public static UseBlueprintTool Instance { get; private set; }

		public UseBlueprintTool()
		{
			Instance = this;
			BlueprintState.ForceMaterialChange = false;
		}

		public UseBlueprintToolHoverCard HoverCard;
		public bool ToolActive { get; private set; }


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
			BlueprintState.IsPlacingSnapshot = false;

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
			BlueprintState.ForceMaterialChange = false;

			BlueprintState.ClearVisuals();
			ToolMenu.Instance.PriorityScreen.Show(false);
			GridCompositor.Instance.ToggleMajor(false);
			CurrentBlueprintStateScreen.ShowScreen(false);
			ToolActive = false;

		}

		public override void OnLeftClickDown(Vector3 cursorPos)
		{
			base.OnLeftClickDown(cursorPos);

			if (hasFocus)
			{
				BlueprintState.UseBlueprint(Grid.PosToXY(cursorPos));
			}
		}

		public override void OnMouseMove(Vector3 cursorPos)
		{
			base.OnMouseMove(cursorPos);

			if (hasFocus)
			{
				BlueprintState.UpdateVisual(Grid.PosToXY(cursorPos));
			}
		}
		void SetForceMaterialChange(bool enabled)
		{
			BlueprintState.ForceMaterialChange = enabled;
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
					BlueprintState.NextAnchorState();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(Action.RotateBuilding) || buttonEvent.TryConsume(ModAssets.Actions.BlueprintsRotate.GetKAction()))
				{
					BlueprintState.TryRotateBlueprint();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsRotateInverse.GetKAction()))
				{
					BlueprintState.TryRotateBlueprint(true);
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipHorizontal.GetKAction()))
				{
					BlueprintState.FlipHorizontal();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipVertical.GetKAction()))
				{
					BlueprintState.FlipVertical();
					BlueprintState.RefreshBlueprintVisualizers();
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectPrevious.GetKAction()))
				{
					SelectPrevBlueprint();
					CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(ModAssets.SelectedBlueprint);
				}
				else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectNext.GetKAction()))
				{
					SelectNextBlueprint();
					CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(ModAssets.SelectedBlueprint);
				}
			}

			base.OnKeyDown(buttonEvent);
		}

		public void SelectNextBlueprint()
		{
			ModAssets.GetCurrentFolder().SelectNext();
			VisualizeSelectedBlueprint();
		}
		public void SelectPrevBlueprint()
		{
			ModAssets.GetCurrentFolder().SelectPrev();
			VisualizeSelectedBlueprint();
		}

		public override void OnKeyUp(KButtonEvent buttonEvent)
		{
			if (DetailsScreen.Instance?.isEditing ?? false)
				return;
			if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleForce.GetKAction()))
			{
				SetForceMaterialChange(false);
			}
		}
	}
}
