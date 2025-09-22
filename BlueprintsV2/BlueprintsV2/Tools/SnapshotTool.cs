using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using Database;
using HarmonyLib;
using PeterHan.PLib.Options;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.Tools
{
	public sealed class SnapshotTool : MultiFilteredDragTool
	{
		public static bool HasSnapshotsStored => Instance != null && Instance.SessionSnapshots.Any();// && !Instance.SessionSnapshots.First().IsEmpty();
		private Blueprint snapshotBlueprint;
		private SnapshotToolHoverCard hoverCard;

		public static SnapshotTool Instance { get; private set; }
		float shiftX = 0, shiftY = 0;

		List<Blueprint> SessionSnapshots = [];
		int UsedSnapshotIndex = 0;
		public static int SnapshotCount => Instance != null ? Instance.SessionSnapshots.Count : 0;
		public static int SnapshotIndex => Instance != null ? Instance.UsedSnapshotIndex: 0;
		public static Blueprint CurrentSnapshot => Instance != null ? Instance.snapshotBlueprint : null;


		public SnapshotTool()
		{
			Instance = this;
			BlueprintState.ForceMaterialChange = false;
		}

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

			visualizer = new GameObject("SnapshotVisualizer");
			visualizer.SetActive(false);

			GameObject offsetObject = new GameObject();
			SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
			spriteRenderer.color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
			spriteRenderer.sprite = ModAssets.BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE;

			offsetObject.transform.SetParent(visualizer.transform);
			offsetObject.transform.localPosition = new Vector3(0, Grid.HalfCellSizeInMeters);
			offsetObject.transform.localPosition = new Vector3(-Grid.HalfCellSizeInMeters, 0);
			var sprite = spriteRenderer.sprite;
			offsetObject.transform.localScale = new Vector3(
				Grid.CellSizeInMeters / (sprite.texture.width / sprite.pixelsPerUnit),
				Grid.CellSizeInMeters / (sprite.texture.height / sprite.pixelsPerUnit)
			);

			offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
			visualizer.transform.SetParent(transform);

			OnMouseMove(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
			CurrentBlueprintStateScreen.ShowScreen(false);
		}

		public void DestroyVisualizer()
		{
			Destroy(visualizer);
			visualizer = null;
			CurrentBlueprintStateScreen.ShowScreen(false);
		}
		public void SetLastUsedBlueprint(Blueprint blueprint)
		{
			SgtLogger.l("SessionSnapshots Count: " + SessionSnapshots.Count);
			if (blueprint != null && !blueprint.IsEmpty())
			{
				if (!SessionSnapshots.Contains(blueprint))
					SessionSnapshots.Insert(0, blueprint);
			}
			SgtLogger.l("SessionSnapshots Count2: " + SessionSnapshots.Count);
		}
		public void DeleteBlueprint()
		{
			SetLastUsedBlueprint(snapshotBlueprint);
			snapshotBlueprint = null;

			hoverCard.UsingSnapshot = false;

			MultiToolParameterMenu.Instance.PopulateMenu(DefaultParameters);
			MultiToolParameterMenu.Instance.ShowMenu();
			ToolMenu.Instance.PriorityScreen.Show(false);
			BlueprintState.ClearVisuals();

			CreateVisualizer();
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

			FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");

			GameObject areaVisualizer = Util.KInstantiate(DigTool.Instance.areaVisualizer);
			areaVisualizer.SetActive(false);

			areaVisualizer.name = "SnapshotAreaVisualizer";

			areaVisualizerSpriteRenderer = areaVisualizer.GetComponent<SpriteRenderer>();
			areaVisualizer.transform.SetParent(transform);
			areaVisualizer.GetComponent<SpriteRenderer>().color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
			areaVisualizer.GetComponent<SpriteRenderer>().material.color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
			this.areaVisualizer = areaVisualizer;
			this.areaVisualizerTextPrefab = DigTool.Instance.areaVisualizerTextPrefab;

			hoverCard = gameObject.AddComponent<SnapshotToolHoverCard>();

		}

		public override void OnActivateTool()
		{
			base.OnActivateTool();
			BlueprintState.IsPlacingSnapshot = true;
			UsedSnapshotIndex = 0;
			if (visualizer == null)
			{
				CreateVisualizer();
			}
			hoverCard.UsingSnapshot = false;
		}

		public override void OnDeactivateTool(InterfaceTool newTool)
		{
			DeleteBlueprint();
			base.OnDeactivateTool(newTool);
			BlueprintState.ForceMaterialChange = false;
			BlueprintState.IsPlacingSnapshot = false;
			BlueprintState.ClearVisuals();
			snapshotBlueprint = null;

			MultiToolParameterMenu.Instance.HideMenu();
			ToolMenu.Instance.PriorityScreen.Show(false);
			GridCompositor.Instance.ToggleMajor(false);
			CurrentBlueprintStateScreen.ShowScreen(false);
		}

		public override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
		{
			base.OnDragComplete(cursorDown, cursorUp);

			if (hasFocus)
			{
				Grid.PosToXY(cursorDown, out int x0, out int y0);
				Grid.PosToXY(cursorUp, out int x1, out int y1);

				if (x0 < x1)
					shiftX = 1;
				else
					shiftX = 0;

				if (y0 < y1)
					shiftY = 1;
				else
					shiftY = 0;

				if (x0 > x1)
				{
					Util.Swap(ref x0, ref x1);
				}

				if (y0 < y1)
				{
					Util.Swap(ref y0, ref y1);
				}

				var bp = BlueprintState.CreateBlueprint(new Vector2I(x0, y0), new Vector2I(x1, y1), MultiToolParameterMenu.Instance, true);
				bp.SetRandomSnapshotId();
				SetLastUsedBlueprint(bp);
				Visualize(bp);
			}
		}

		public void TryVisualizeLastSnapshot()
		{
			UsedSnapshotIndex = 0;
			if (HasSnapshotsStored)
			{
				Visualize(GetSnapshotAtCurrentIndex(), false);
			}
			else
			{
				PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.SNAPSHOT_TOOL.EMPTY, null, offset: PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
			}
		}
		Blueprint GetSnapshotAtCurrentIndex()
		{
			if (UsedSnapshotIndex < 0)
				UsedSnapshotIndex = 0;

			if (UsedSnapshotIndex > SessionSnapshots.Count - 1)
				UsedSnapshotIndex = SessionSnapshots.Count - 1;

			return SessionSnapshots[UsedSnapshotIndex];
		}

		public bool HasPrevSnapshot => UsedSnapshotIndex < SessionSnapshots.Count - 1;
		public bool HasNextSnapshot => UsedSnapshotIndex > 0;

		public void VisualizePreviousSnapshot()
		{
			if (!HasPrevSnapshot)
				return;
			UsedSnapshotIndex++;
			Visualize(GetSnapshotAtCurrentIndex(), false);
		}
		public void VisualizeNextSnapshot()
		{
			if (!HasNextSnapshot)
				return;
			UsedSnapshotIndex--;
			Visualize(GetSnapshotAtCurrentIndex(), false);
		}


		public void Visualize(Blueprint blueprintToVisualize, bool spawnFX = true)
		{
			if (blueprintToVisualize.IsEmpty())
			{
				CurrentBlueprintStateScreen.ShowScreen(false);
				snapshotBlueprint = null;
				if (spawnFX)
					PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.SNAPSHOT_TOOL.EMPTY, null, offset: PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
			}
			else
			{
				BlueprintState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), blueprintToVisualize);
				BlueprintState.SetAnchorState(shiftX, shiftY, blueprintToVisualize);

				MultiToolParameterMenu.Instance.HideMenu();
				ToolMenu.Instance.PriorityScreen.Show();

				hoverCard.UsingSnapshot = true;
				DestroyVisualizer();

				if (spawnFX)
					PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.SNAPSHOT_TOOL.TAKEN, null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
				GridCompositor.Instance.ToggleMajor(true);
				snapshotBlueprint = blueprintToVisualize;

				CurrentBlueprintStateScreen.ShowScreen(true);
				CurrentBlueprintStateScreen.Instance.SetSelectedBlueprint(snapshotBlueprint);
			}
		}


		public override void OnLeftClickDown(Vector3 cursorPos)
		{
			if (snapshotBlueprint == null)
			{
				base.OnLeftClickDown(cursorPos);
			}

			else if (hasFocus)
			{
				BlueprintState.UseBlueprint(Grid.PosToXY(cursorPos), snapshotBlueprint);
			}
		}

		public override void OnLeftClickUp(Vector3 cursorPos)
		{
			if (snapshotBlueprint == null)
			{
				base.OnLeftClickUp(cursorPos);
			}
		}

		public override void OnMouseMove(Vector3 cursorPos)
		{
			if (snapshotBlueprint == null)
			{
				base.OnMouseMove(cursorPos);
			}

			else if (hasFocus)
			{
				BlueprintState.UpdateVisual(Grid.PosToXY(cursorPos), false, snapshotBlueprint);
			}
		}

		void SetForceMaterialChange(bool enabled)
		{
			BlueprintState.ForceMaterialChange = enabled;
			BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);
			CurrentBlueprintStateScreen.Instance.SetForceMaterialChange(enabled);
		}
		public override void OnKeyDown(KButtonEvent buttonEvent)
		{
			if ((DetailsScreen.Instance?.isEditing ?? false) || (DetailsScreen.Instance?.HasFocus ?? false))
				return;

			if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleHotkeyToolTips.GetKAction()))
			{
				BlueprintState.ToggleHotkeyTooltips();
			}
			else
			if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSnapshotReuseAction.GetKAction()))
			{
				TryVisualizeLastSnapshot();
			}
			else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsReopenSelectionAction.GetKAction()))
			{
				DeleteBlueprint();
				GridCompositor.Instance.ToggleMajor(false);
			}
			else if (snapshotBlueprint != null && buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectPrevious.GetKAction()))
			{
				VisualizePreviousSnapshot();
			}
			else if (snapshotBlueprint != null && buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSelectNext.GetKAction()))
			{
				VisualizeNextSnapshot();
			}
			else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleForce.GetKAction()))
			{
				SetForceMaterialChange(true);
			}
			else if (buttonEvent.TryConsume(Action.RotateBuilding) || buttonEvent.TryConsume(ModAssets.Actions.BlueprintsRotate.GetKAction()))
			{
				BlueprintState.TryRotateBlueprint();
				BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);
			}
			else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsRotateInverse.GetKAction()))
			{
				BlueprintState.TryRotateBlueprint(true);
				BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);
			}
			else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipHorizontal.GetKAction()))
			{
				BlueprintState.FlipHorizontal();
				BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);
			}
			else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipVertical.GetKAction()))
			{
				BlueprintState.FlipVertical();
				BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);
			}
			else if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSwapAnchorAction.GetKAction()))
			{
				BlueprintState.NextAnchorState();
				BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);
			}
			base.OnKeyDown(buttonEvent);
		}

		public override void OnKeyUp(KButtonEvent buttonEvent)
		{
			if ((DetailsScreen.Instance?.isEditing ?? false) || (DetailsScreen.Instance?.HasFocus ?? false))
				return;

			if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleForce.GetKAction()))
			{
				SetForceMaterialChange(false);
			}
			buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipHorizontal.GetKAction());
			buttonEvent.TryConsume(ModAssets.Actions.BlueprintsFlipVertical.GetKAction());
			buttonEvent.TryConsume(ModAssets.Actions.BlueprintsRotate.GetKAction());
		}

		public override void OnSyncChanged(bool synced)
		{
			base.OnSyncChanged(synced);

			Config.Instance.SnapshotToolSync = synced;
			POptions.WriteSettings(Config.Instance);
		}
	}
}
