
using BlueprintsV2.BlueprintData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.Visualizers
{

    public sealed class UtilityVisual : BuildingVisual
    {


        public UtilityVisual(BuildingConfig buildingConfig, int cell):base(buildingConfig, cell) 
        {
            if (Visualizer.TryGetComponent<KBatchedAnimController>(out var batchedAnimController))
            {
                IUtilityNetworkMgr utilityNetworkManager = buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager();

                if (utilityNetworkManager != null && buildingConfig.GetConduitFlags(out int flags))
                {
                    string animation = utilityNetworkManager.GetVisualizerString((UtilityConnections)flags) + "_place";

                    if (batchedAnimController.HasAnimation(animation))
                    {
                        batchedAnimController.Play(animation);
                    }
                }

                batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
                batchedAnimController.isMovable = true;
                batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset();
                batchedAnimController.TintColour = GetVisualizerColor(cell);

                batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
            }
            VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
        }


        public override void MoveVisualizer(int cellParam , bool forceRedraw)
        {
            if (cellParam != cell || forceRedraw)
            {
                Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, Grid.SceneLayer.Building));
                VisualsUtilities.SetVisualizerColor(cellParam, GetVisualizerColor(cellParam), Visualizer, buildingConfig);

                cell = cellParam;
            }
        }      
    }
}
