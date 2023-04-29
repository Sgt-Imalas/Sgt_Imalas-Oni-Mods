using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class VariedSizeTravelTubePiece : TravelTubeBridge
    {
        [MyCmpReq]
        private Building building;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            foreach (var offset in building.Def.PlacementOffsets)
            {
                SgtLogger.l("adding tube to " + Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(this), offset)).ToString()+ " (offset: "+offset.ToString()+")");
                Grid.HasTube[Grid.OffsetCell(Grid.PosToCell(this), offset)] = true;
            }
        }

        public override void OnCleanUp()
        {
            foreach (var offset in building.Def.PlacementOffsets)
            {
                Grid.HasTube[Grid.OffsetCell(Grid.PosToCell(this), offset)] = false;
            }
            base.OnCleanUp();
        }

    }
}
