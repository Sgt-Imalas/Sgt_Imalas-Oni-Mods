using HarmonyLib;
using OniRetroEdition.ModPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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
            this.Placer = Assets.GetPrefab(new Tag(SlurpPlacerConfig.ID));

            visualizer = new GameObject("SlurpVisualizer");
            visualizer.SetActive(false);

            GameObject offsetObject = new GameObject();
            SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = UIUtils.rgb(251, 176, 59);
            spriteRenderer.sprite = Assets.GetSprite(SpritePatch.SlurpIcon);

            offsetObject.transform.SetParent(visualizer.transform);
            offsetObject.transform.localPosition = new Vector3(-Grid.HalfCellSizeInMeters,0 );
            var sprite = spriteRenderer.sprite;
            offsetObject.transform.localScale = new Vector3(
                Grid.CellSizeInMeters / (sprite.texture.width / sprite.pixelsPerUnit),
                Grid.CellSizeInMeters / (sprite.texture.height / sprite.pixelsPerUnit)
            );

            offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
            visualizer.transform.SetParent(transform);

            FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");
            FieldInfo areaVisualizerSpriteRendererField = AccessTools.Field(typeof(DragTool), "areaVisualizerSpriteRenderer");

            GameObject areaVisualizer = Util.KInstantiate((GameObject)AccessTools.Field(typeof(DeconstructTool), "areaVisualizer").GetValue(DeconstructTool.Instance));
            areaVisualizer.SetActive(false);

            areaVisualizer.name = "SlurpAreaVisualizer";
            areaVisualizerSpriteRendererField.SetValue(this, areaVisualizer.GetComponent<SpriteRenderer>());
            areaVisualizer.transform.SetParent(transform);
            areaVisualizer.GetComponent<SpriteRenderer>().color = UIUtils.rgb(251, 176, 59);
            areaVisualizer.GetComponent<SpriteRenderer>().material.color = UIUtils.rgb(251, 176, 59);

            areaVisualizerField.SetValue(this, areaVisualizer);
            interceptNumberKeysForPriority = true;
        }
        public SlurpTool()
        {
            Instance = this;
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
            if (gameObject==null)
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
