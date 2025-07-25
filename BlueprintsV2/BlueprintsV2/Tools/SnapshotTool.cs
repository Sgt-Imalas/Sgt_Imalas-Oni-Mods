﻿using BlueprintsV2.BlueprintData;
using Database;
using HarmonyLib;
using PeterHan.PLib.Options;
using STRINGS;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.Tools
{
	public sealed class SnapshotTool : MultiFilteredDragTool
	{
		public static bool HasPreviousSnapshot => Instance != null && Instance.lastPreviousSnapshotBlueprint != null && !Instance.lastPreviousSnapshotBlueprint.IsEmpty();
		private Blueprint snapshotBlueprint, lastPreviousSnapshotBlueprint;
		private SnapshotToolHoverCard hoverCard;

		public static SnapshotTool Instance { get; private set; }
		float shiftX = 0, shiftY = 0;

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
		}

		public void DestroyVisualizer()
		{
			Destroy(visualizer);
			visualizer = null;
		}
		public void SetLastUsedBlueprint(Blueprint blueprint)
		{
			if (blueprint != null && !blueprint.IsEmpty())
			{
				lastPreviousSnapshotBlueprint = blueprint;
			}
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
			FieldInfo areaVisualizerSpriteRendererField = AccessTools.Field(typeof(DragTool), "areaVisualizerSpriteRenderer");

			GameObject areaVisualizer = Util.KInstantiate(Traverse.Create(DeconstructTool.Instance).Field("areaVisualizer").GetValue<GameObject>());
			areaVisualizer.SetActive(false);

			areaVisualizer.name = "SnapshotAreaVisualizer";
			areaVisualizerSpriteRendererField.SetValue(this, areaVisualizer.GetComponent<SpriteRenderer>());
			areaVisualizer.transform.SetParent(transform);
			areaVisualizer.GetComponent<SpriteRenderer>().color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
			areaVisualizer.GetComponent<SpriteRenderer>().material.color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;

			areaVisualizerField.SetValue(this, areaVisualizer);

			hoverCard = gameObject.AddComponent<SnapshotToolHoverCard>();
		}

		public override void OnActivateTool()
		{
			base.OnActivateTool();
			BlueprintState.IsPlacingSnapshot = true;

			if (visualizer == null)
			{
				CreateVisualizer();
			}

			hoverCard.UsingSnapshot = false;
		}

		public override void OnDeactivateTool(InterfaceTool newTool)
		{
			base.OnDeactivateTool(newTool);
			BlueprintState.ForceMaterialChange = false;
			BlueprintState.IsPlacingSnapshot = false;
			BlueprintState.ClearVisuals();
			snapshotBlueprint = null;

			MultiToolParameterMenu.Instance.HideMenu();
			ToolMenu.Instance.PriorityScreen.Show(false);
			GridCompositor.Instance.ToggleMajor(false);
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
				if (y0 < y1)
					shiftY = 1;

				if (x0 > x1)
				{
					Util.Swap(ref x0, ref x1);
				}

				if (y0 < y1)
				{
					Util.Swap(ref y0, ref y1);
				}

				var bp = BlueprintState.CreateBlueprint(new Vector2I(x0, y0), new Vector2I(x1, y1), MultiToolParameterMenu.Instance, true);
				SetLastUsedBlueprint(bp);
				Visualize(bp);
			}
		}

		public void TryVisualizeLastSnapshot()
		{
			if (lastPreviousSnapshotBlueprint != null && !lastPreviousSnapshotBlueprint.IsEmpty())
			{
				Visualize(lastPreviousSnapshotBlueprint,false);
			}
			else
			{
				PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.SNAPSHOT_TOOL.EMPTY, null, offset: PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
			}
		}

		public void Visualize(Blueprint blueprintToVisualize, bool spawnFX = true)
		{
			if (blueprintToVisualize.IsEmpty())
			{
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

		public override void OnKeyDown(KButtonEvent buttonEvent)
		{
			if (ModAssets.BlueprintFileHandling.HasBlueprints())
			{
				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSnapshotReuseAction.GetKAction()))
				{
					TryVisualizeLastSnapshot();
				}
				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsReopenSelectionAction.GetKAction()))
				{
					DeleteBlueprint();
					GridCompositor.Instance.ToggleMajor(false);
				}
				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleForce.GetKAction()))
				{
					BlueprintState.ForceMaterialChange = true;
					BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);
				}
				if (buttonEvent.TryConsume(Action.RotateBuilding))
				{
					//BlueprintState.TryRotateBlueprint();
				}

				if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsSwapAnchorAction.GetKAction()))
				{
					BlueprintState.NextAnchorState();
				}
			}

			base.OnKeyDown(buttonEvent);
		}

		public override void OnKeyUp(KButtonEvent buttonEvent)
		{
			if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsToggleForce.GetKAction()))
			{
				BlueprintState.ForceMaterialChange = false;
				BlueprintState.RefreshBlueprintVisualizers(snapshotBlueprint);

			}
		}

		public override void OnSyncChanged(bool synced)
		{
			base.OnSyncChanged(synced);

			Config.Instance.SnapshotToolSync = synced;
			POptions.WriteSettings(Config.Instance);
		}
	}
}
