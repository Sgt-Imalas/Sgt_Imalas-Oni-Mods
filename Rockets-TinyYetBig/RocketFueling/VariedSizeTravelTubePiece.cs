using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class VariedSizeTravelTubePiece : KMonoBehaviour, ITravelTubePiece
    {
        private static readonly EventSystem.IntraObjectHandler<VariedSizeTravelTubePiece> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<VariedSizeTravelTubePiece>((component, data) => component.OnBuildingBroken(data));
        private static readonly EventSystem.IntraObjectHandler<VariedSizeTravelTubePiece> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<VariedSizeTravelTubePiece>((component, data) => component.OnBuildingFullyRepaired(data));
        public Vector3 Position => transform.GetPosition();

        [MyCmpReq]
        private Building building;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            foreach (var offset in building.Def.PlacementOffsets)
            {

                SgtLogger.l("adding tube to " + Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(this), offset)).ToString());
                Grid.HasTube[Grid.OffsetCell(Grid.PosToCell(this), offset)] = true;
            }

            Components.ITravelTubePieces.Add(this);
            Subscribe(774203113, OnBuildingBrokenDelegate);
            Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
        }

        public override void OnCleanUp()
        {
            Unsubscribe(774203113, OnBuildingBrokenDelegate);
            Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate);
            foreach (var offset in building.Def.PlacementOffsets)
            {
                Grid.HasTube[Grid.OffsetCell(Grid.PosToCell(this), offset)] = false;
            }
            Components.ITravelTubePieces.Remove(this);
            base.OnCleanUp();
        }

        private void OnBuildingBroken(object data)
        {
        }

        private void OnBuildingFullyRepaired(object data)
        {
        }
    }
}
