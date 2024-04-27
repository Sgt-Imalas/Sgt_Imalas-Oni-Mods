using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.SlurpTool
{
    internal class SlurpTool : DragTool
    {
        private GameObject Placer;
        public static SlurpTool Instance;

        public static void DestroyInstance() => SlurpTool.Instance = null;
       
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.Placer = Util.KInstantiate(Assets.GetPrefab(new Tag(SlurpPlacerConfig.ID)));
            this.interceptNumberKeysForPriority = true;
            SlurpTool.Instance = this;
        }

        public void Activate() => PlayerController.Instance.ActivateTool(this);

        public override void OnDragTool(int cell, int distFromOrigin)
        {
            if (!Grid.IsValidCell(cell))
                return;

            GameObject gameObject = Grid.Objects[cell, (int)ObjectLayer.MopPlacer];
            if (!Grid.Solid[cell] && gameObject == null && Grid.Element[cell].IsLiquid)
            {
                gameObject = Util.KInstantiate(this.Placer);
                Grid.Objects[cell, 8] = gameObject;
                Vector3 posCbc = Grid.CellToPosCBC(cell, this.visualizerLayer);
                float num = -0.15f;
                posCbc.z += num;
                gameObject.transform.SetPosition(posCbc);
                gameObject.SetActive(true);
            }
            if (!(gameObject != null))
                return;
            Prioritizable component = gameObject.GetComponent<Prioritizable>();
            if (!(component != null))
                return;
            component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());

        }

        public override void OnActivateTool()
        {
            base.OnActivateTool();
            ToolMenu.Instance.PriorityScreen.Show();
        }

        public override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
            ToolMenu.Instance.PriorityScreen.Show(false);
        }
    }
}
