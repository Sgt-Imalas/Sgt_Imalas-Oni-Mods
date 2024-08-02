using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MassMoveTo.Tools
{
    internal class TargetSelectTool : InterfaceTool
    {
        public static TargetSelectTool Instance;
        public static void DestroyInstance() => Instance = null;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
            visualizer = Util.KInstantiate(MoveToLocationTool.Instance.visualizer);
        }

        public bool CanTarget(int target_cell) => !Grid.IsSolidCell(target_cell) && Grid.IsWorldValidCell(target_cell);


        public void Activate()
        {
            PlayerController.Instance.ActivateTool(this);
        }

        public override void OnActivateTool()
        {
            base.OnActivateTool();
            visualizer.gameObject.SetActive(true);
        }
        public override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
            visualizer.gameObject.SetActive(false);
            ToolMenu.Instance.ClearSelection();
        }
        public override void OnLeftClickDown(Vector3 cursor_pos)
        {
            base.OnLeftClickDown(cursor_pos);
            int mouseCell = DebugHandler.GetMouseCell();

            if (CanTarget(mouseCell))
            {
                PlaySound(GlobalAssets.GetSound("HUD_Click"));
                ModAssets.MoveAllTo(mouseCell);
                DeactivateTool();
                SelectTool.Instance.Activate();
            }
            else
                PlaySound(GlobalAssets.GetSound("Negative"));
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
