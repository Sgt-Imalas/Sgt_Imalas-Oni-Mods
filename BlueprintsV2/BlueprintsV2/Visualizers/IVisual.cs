using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
    public interface IVisual
    {
        GameObject Visualizer { get; }
        Vector2I Offset { get; }

        bool IsPlaceable(int cellParam);
        void MoveVisualizer(int cellParam);
        bool TryUse(int cellParam);
                
    }
}
