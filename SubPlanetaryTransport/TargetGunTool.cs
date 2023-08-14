using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SubPlanetaryTransport
{
    internal class TargetGunTool : InterfaceTool
    {
        public static TargetGunTool Instance;
        public static void DestroyInstance() => Instance = null;

        private SubPlanetaryItemRailgun sourceRailgun;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
            this.visualizer = Util.KInstantiate(MoveToLocationTool.Instance.visualizer);
        }

        public bool CanTarget(int target_cell) => sourceRailgun != null ? sourceRailgun.CanAimThere(target_cell) : false;


        public void Activate(SubPlanetaryItemRailgun railgun)
        {
            this.sourceRailgun = railgun;
            PlayerController.Instance.ActivateTool((InterfaceTool)this);
        }

        public override void OnActivateTool()
        {
            base.OnActivateTool();
            this.visualizer.gameObject.SetActive(true);
        }
        public override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
            if (this.sourceRailgun != null && new_tool == SelectTool.Instance)
                SelectTool.Instance.SelectNextFrame(this.sourceRailgun.GetComponent<KSelectable>(), true);
            this.visualizer.gameObject.SetActive(false);
        }
        public override void OnLeftClickDown(Vector3 cursor_pos)
        {
            base.OnLeftClickDown(cursor_pos);

            if (this.sourceRailgun == null)
                return;
            int mouseCell = DebugHandler.GetMouseCell();

            if (this.CanTarget(mouseCell))
            {
                KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
                //Target cell by railgun
                //this.SetMoveToLocation(mouseCell);
                sourceRailgun.TargetCell = mouseCell;
                SelectTool.Instance.Activate();
            }
            else
                KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
        }

        private void RefreshColor()
        {
            Color c = new Color(0.91f, 0.21f, 0.2f);
            if (this.CanTarget(DebugHandler.GetMouseCell()))
                c = Color.white;
            this.SetColor(this.visualizer, c);
        }

        public override void OnMouseMove(Vector3 cursor_pos)
        {
            base.OnMouseMove(cursor_pos);
            this.RefreshColor();
        }

        private void SetColor(GameObject root, Color c) => root.GetComponentInChildren<MeshRenderer>().material.color = c;
    }
}
