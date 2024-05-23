using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
    public interface ICleanableVisual
    {
        int DirtyCell { get; }
        void Clean();
    }
}
