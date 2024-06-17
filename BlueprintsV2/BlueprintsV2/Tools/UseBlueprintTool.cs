
using BlueprintsV2.BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using PeterHan.PLib.UI;
using System.IO;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.Tools
{
    public sealed class UseBlueprintTool : InterfaceTool
    {
        public static UseBlueprintTool Instance { get; private set; }

        public UseBlueprintTool()
        {
            Instance = this;
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
            gameObject.AddComponent<UseBlueprintToolHoverCard>();
        }

        public override void OnActivateTool()
        {
            base.OnActivateTool();

            ToolMenu.Instance.PriorityScreen.Show();
            BlueprintSelectionScreen.ShowWindow(OnBlueprintSelected);

            
        }

        public void OnBlueprintSelected()
        {
            SgtLogger.l("blueprint selected ? " + (ModAssets.SelectedBlueprint != null));
            if (ModAssets.SelectedBlueprint != null)
            {
                GridCompositor.Instance.ToggleMajor(true);
                BlueprintState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), ModAssets.SelectedBlueprint);                
            }
            else
            {
                if (visualizer != null)
                {
                    Destroy(visualizer);
                }
                //deactivate tool if no bp selected:

                this.DeactivateTool();
                ToolMenu.Instance.ClearSelection();
                string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
                if (sound != null)
                    KMonoBehaviour.PlaySound(sound);
            }
        }

        public override void OnDeactivateTool(InterfaceTool newTool)
        {
            base.OnDeactivateTool(newTool);

            BlueprintState.ClearVisuals();
            ToolMenu.Instance.PriorityScreen.Show(false);
            GridCompositor.Instance.ToggleMajor(false);
            ModAssets.SelectedBlueprint = null;
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

        public override void OnKeyDown(KButtonEvent buttonEvent)
        {
            //if (BlueprintState.LoadedBlueprints.Count > 0)
            //{
            //    if (buttonEvent.TryConsume(ModAssets.Actions.BlueprintsCreateFolderAction.GetKAction()))
            //    {
            //        static void OnConfirmDelegate(string blueprintFolder, FileNameDialog parent)
            //        {
            //            string newFolder = blueprintFolder.Trim(' ', '/', '\\', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            //            if (newFolder == BlueprintState.SelectedBlueprint.Folder)
            //            {
            //                PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.USE_TOOL.FOLDERBLUEPRINT_NA, null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
            //            }

            //            else
            //            {
            //                string blueprintName = BlueprintState.SelectedBlueprint.FriendlyName;

            //                BlueprintState.SelectedBlueprint.SetFolder(newFolder);
            //                PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, string.Format(STRINGS.UI.TOOLS.USE_TOOL.MOVEDBLUEPRINT, blueprintName, newFolder), null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
            //            }

            //            SpeedControlScreen.Instance.Unpause(false);
            //            parent.Deactivate();
            //        }

            //        FileNameDialog blueprintFolderDialog = ModAssets.DialogHandling.CreateFolderDialog(OnConfirmDelegate);
            //        SpeedControlScreen.Instance.Pause(false);
            //        blueprintFolderDialog.Activate();
            //    }                 
            //}

            base.OnKeyDown(buttonEvent);
        }
    }
}
