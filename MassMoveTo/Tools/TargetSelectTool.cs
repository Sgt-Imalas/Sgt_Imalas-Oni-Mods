using PeterHan.PLib.Detours;
using UnityEngine;

namespace MassMoveTo.Tools
{
	internal class TargetSelectTool : DragTool
	{
		private static readonly IDetouredField<DragTool, GameObject> AREA_VISUALIZER =
			PDetours.DetourField<DragTool, GameObject>("areaVisualizer");
		private static readonly IDetouredField<DragTool, GameObject> AREA_VISUALIZER_TEXT_PREFAB =
			PDetours.DetourField<DragTool, GameObject>("areaVisualizerTextPrefab");
		private static readonly IDetouredField<DragTool, Texture2D> BOX_CURSOR =
			PDetours.DetourField<DragTool, Texture2D>("boxCursor");
		private static readonly IDetouredField<InterfaceTool, Texture2D> CURSOR =
			PDetours.DetourField<InterfaceTool, Texture2D>(nameof(cursor));
		private static readonly IDetouredField<InterfaceTool, GameObject> VISUALIZER =
			PDetours.DetourField<InterfaceTool, GameObject>(nameof(visualizer));

		public static TargetSelectTool Instance;
		public static void DestroyInstance() => Instance = null;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Instance = this;
			visualizer = Util.KInstantiate(MoveToLocationTool.Instance.visualizer);

			var inst = ClearTool.Instance;
			var avTemplate = AREA_VISUALIZER.Get(inst);
			if (avTemplate != null)
			{
				var areaVisualizer = Util.KInstantiate(avTemplate, gameObject,
					"FilteredMassMoveToolAreaVisualizer");
				areaVisualizer.SetActive(false);
				areaVisualizerSpriteRenderer = areaVisualizer.GetComponent<
					SpriteRenderer>();
				// The visualizer is private so we need to set it with reflection
				AREA_VISUALIZER.Set(this, areaVisualizer);
				AREA_VISUALIZER_TEXT_PREFAB.Set(this, AREA_VISUALIZER_TEXT_PREFAB.Get(
					inst));
			}
		}

		public bool CanTarget(int target_cell) => !Grid.IsSolidCell(target_cell) && Grid.IsWorldValidCell(target_cell);


		public void Activate()
		{
			PlayerController.Instance.ActivateTool(this);
		}
		bool wasAlreadyPaused = false;
		public override void OnActivateTool()
		{
			base.OnActivateTool();
			visualizer.gameObject.SetActive(true);
			wasAlreadyPaused = SpeedControlScreen.Instance.IsPaused;

			if (!wasAlreadyPaused)
				SpeedControlScreen.Instance.Pause(false);

		}
		public override void OnDeactivateTool(InterfaceTool new_tool)
		{
			base.OnDeactivateTool(new_tool);
			visualizer.gameObject.SetActive(false);
			ToolMenu.Instance.ClearSelection();
			if (!wasAlreadyPaused)
				SpeedControlScreen.Instance.Unpause(false);
		}

		public override void OnDragTool(int cell, int distFromOrigin)
		{
			if (CanTarget(cell))
				ModAssets.RegisterTargetCell(cell);
		}
		public override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
		{
			if (CanTarget(Grid.PosToCell(cursorDown)))
				ModAssets.RegisterTargetCell(Grid.PosToCell(cursorDown));
			if (CanTarget(Grid.PosToCell(cursorUp)))
				ModAssets.RegisterTargetCell(Grid.PosToCell(cursorUp));

			if (ModAssets.TargetCellCount > 0)
			{
				PlaySound(GlobalAssets.GetSound("HUD_Click"));
				ModAssets.MoveAllItems();
				DeactivateTool();
				SelectTool.Instance.Activate();
			}
			else
			{
				PlaySound(GlobalAssets.GetSound("Negative"));
			}
		}

		public override void OnLeftClickUp(Vector3 cursor_pos)
		{
			ModAssets.ClearCachedTargets();
			base.OnLeftClickUp(cursor_pos);
		}

		private void RefreshColor()
		{
			Color c = new Color(0.91f, 0.21f, 0.2f);
			if (CanTarget(DebugHandler.GetMouseCell()))
				c = Color.white;
			SetColor(visualizer, c);
		}

		public override void OnMouseMove(Vector3 cursor_pos)
		{
			base.OnMouseMove(cursor_pos);
			RefreshColor();
		}

		private void SetColor(GameObject root, Color c) => root.GetComponentInChildren<MeshRenderer>().material.color = c;
	}
}
