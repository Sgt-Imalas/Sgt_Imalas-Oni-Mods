
using BlueprintsV2.BlueprintsV2.BlueprintData;
using HarmonyLib;
using ModFramework;
using PeterHan.PLib.Options;
using System.Reflection;
using UnityEngine;

namespace Blueprints {
    public sealed class SnapshotTool : MultiFilteredDragTool {
        private Blueprint blueprint;

        public static SnapshotTool Instance { get; private set; }

        public SnapshotTool() {
            Instance = this;
        }

        public static void DestroyInstance() {
            Instance = null;
        }

        public void CreateVisualizer() {
            if (visualizer != null) {
                Destroy(visualizer);
            }

            visualizer = new GameObject("SnapshotVisualizer");
            visualizer.SetActive(false);

            GameObject offsetObject = new GameObject();
            SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = BlueprintsAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
            spriteRenderer.sprite = BlueprintsAssets.BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE;

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

        public void DestroyVisualizer() {
            Destroy(visualizer);
            visualizer = null;
        }

        public void DeleteBlueprint() {
            blueprint = null;

            gameObject.GetComponent<SnapshotToolHoverCard>().UsingSnapshot = false;

            MultiToolParameterMenu.Instance.PopulateMenu(DefaultParameters);
            MultiToolParameterMenu.Instance.ShowMenu();
            ToolMenu.Instance.PriorityScreen.Show(false);
            BlueprintsState.ClearVisuals();

            CreateVisualizer();
        }

        public override void OnPrefabInit() {
            base.OnPrefabInit();

            FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");
            FieldInfo areaVisualizerSpriteRendererField = AccessTools.Field(typeof(DragTool), "areaVisualizerSpriteRenderer");

            GameObject areaVisualizer = Util.KInstantiate(Traverse.Create(DeconstructTool.Instance).Field("areaVisualizer").GetValue<GameObject>());
            areaVisualizer.SetActive(false);

            areaVisualizer.name = "SnapshotAreaVisualizer";
            areaVisualizerSpriteRendererField.SetValue(this, areaVisualizer.GetComponent<SpriteRenderer>());
            areaVisualizer.transform.SetParent(transform);
            areaVisualizer.GetComponent<SpriteRenderer>().color = BlueprintsAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
            areaVisualizer.GetComponent<SpriteRenderer>().material.color = BlueprintsAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;

            areaVisualizerField.SetValue(this, areaVisualizer);

            gameObject.AddComponent<SnapshotToolHoverCard>();
        }

        public override void OnActivateTool() {
            base.OnActivateTool();

            if (visualizer == null) {
                CreateVisualizer();
            }

            gameObject.GetComponent<SnapshotToolHoverCard>().UsingSnapshot = false;
        }

        public override void OnDeactivateTool(InterfaceTool newTool) {
            base.OnDeactivateTool(newTool);

            BlueprintsState.ClearVisuals();
            blueprint = null;

            MultiToolParameterMenu.Instance.HideMenu();
            ToolMenu.Instance.PriorityScreen.Show(false);
            GridCompositor.Instance.ToggleMajor(false);
        }

        public override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp) {
            base.OnDragComplete(cursorDown, cursorUp);

            if (hasFocus) {
                Grid.PosToXY(cursorDown, out int x0, out int y0);
                Grid.PosToXY(cursorUp, out int x1, out int y1);

                if (x0 > x1) {
                    Util.Swap(ref x0, ref x1);
                }

                if (y0 < y1) {
                    Util.Swap(ref y0, ref y1);
                }

                var blueprint1 = BlueprintsState.CreateBlueprint(new Vector2I(x0, y0), new Vector2I(x1, y1), MultiToolParameterMenu.Instance);
                if (blueprint1.IsEmpty()) {
                    PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_EMPTY, null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.Options.FXTime);
                }

                else {
                    BlueprintsState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), blueprint1);

                    MultiToolParameterMenu.Instance.HideMenu();
                    ToolMenu.Instance.PriorityScreen.Show();

                    gameObject.GetComponent<SnapshotToolHoverCard>().UsingSnapshot = true;
                    DestroyVisualizer();

                    PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_TAKEN, null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.Options.FXTime);
                    GridCompositor.Instance.ToggleMajor(true);
                    blueprint = blueprint1;
                }
            }
        }

        public override void OnLeftClickDown(Vector3 cursorPos) {
            if (blueprint == null) {
                base.OnLeftClickDown(cursorPos);
            }

            else if (hasFocus) {
                BlueprintsState.UseBlueprint(Grid.PosToXY(cursorPos));
            }
        }

        public override void OnLeftClickUp(Vector3 cursorPos) {
            if (blueprint == null) {
                base.OnLeftClickUp(cursorPos);
            }
        }

        public override void OnMouseMove(Vector3 cursorPos) {
            if (blueprint == null) {
                base.OnMouseMove(cursorPos);
            }

            else if (hasFocus) {
                BlueprintsState.UpdateVisual(Grid.PosToXY(cursorPos));
            }
        }

        public override void OnKeyDown(KButtonEvent buttonEvent) {
            if (buttonEvent.TryConsume(Integration.BlueprintsDeleteAction.GetKAction())) {
                Instance.DeleteBlueprint();
                GridCompositor.Instance.ToggleMajor(false);
            }

            base.OnKeyDown(buttonEvent);
        }

        public override void OnSyncChanged(bool synced) {
            base.OnSyncChanged(synced);

            BlueprintsAssets.Options.SnapshotToolSync = synced;
            POptions.WriteSettings(BlueprintsAssets.Options);
        }
    }
}
