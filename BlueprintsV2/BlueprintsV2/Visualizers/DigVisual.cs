
using BlueprintsV2.BlueprintsV2.BlueprintData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
    public sealed class DigVisual : IVisual
    {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }

        public DigVisual(int cell, Vector2I offset)
        {
            Visualizer = GameUtil.KInstantiate(DigTool.Instance.visualizer, Grid.CellToPosCBC(cell, DigTool.Instance.visualizerLayer), DigTool.Instance.visualizerLayer, "BlueprintModDigVisualizer");
            Visualizer.SetActive(IsPlaceable(cell));
            Offset = offset;
        }

        public bool IsPlaceable(int cellParam)
        {
            return Grid.IsValidCell(cellParam) && Grid.IsVisible(cellParam) && Grid.Solid[cellParam] && !Grid.Foundation[cellParam] && Grid.Objects[cellParam, 7] == null;
        }

        public void MoveVisualizer(int cellParam)
        {
            Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, DigTool.Instance.visualizerLayer));
            Visualizer.SetActive(IsPlaceable(cellParam));
        }

        public bool TryUse(int cellParam)
        {
            if (IsPlaceable(cellParam))
            {
                if (BlueprintState.InstantBuild)
                {
                    WorldDamage.Instance.DestroyCell(cellParam);
                }

                else
                {
                    GameObject digVisualizer = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")));
                    digVisualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, DigTool.Instance.visualizerLayer) - new Vector3(0F, 0F, 0.15F));
                    digVisualizer.GetComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
                    digVisualizer.SetActive(true);

                    Grid.Objects[cellParam, 7] = digVisualizer;
                    Visualizer.SetActive(false);
                }

                return true;
            }

            return false;
        }
    }
}
