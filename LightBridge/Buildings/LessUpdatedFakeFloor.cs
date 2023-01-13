using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDING.STATUSITEMS.ACCESS_CONTROL;

namespace LightBridge.Buildings
{
    internal class LessUpdatedFakeFloor : KMonoBehaviour
    {
        public CellOffset[] floorOffsets;

        public bool initiallyActive = true;

        private bool isActive;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (initiallyActive)
            {
                SetFloor(active: true);
            }
        }
        public class OffsetComparer : IEqualityComparer<CellOffset>
        {
            public bool Equals(CellOffset l1, CellOffset l2)
            {
                return l1 == l2;
            }

            public int GetHashCode(CellOffset obj)
            {
                return obj.GetHashCode();
            }

        }

        public void UpdateFloorCells(CellOffset[] NewCells)
        {
            int cell = Grid.PosToCell(this);
            Building component = GetComponent<Building>();
            var comparer = new OffsetComparer();
            CellOffset[] OldCells = floorOffsets;
            CellOffset[] ToActivate = NewCells.Except(OldCells).ToArray();
            CellOffset[]  ToDeactivate = OldCells.Except(NewCells).ToArray();

            foreach (CellOffset offset in ToDeactivate)
            {
                CellOffset rotatedOffset = component.GetRotatedOffset(offset);
                int num = Grid.OffsetCell(cell, rotatedOffset);
                Grid.FakeFloor.Remove(num);
                Pathfinding.Instance.AddDirtyNavGridCell(num);
            }
            foreach (CellOffset offset in ToActivate)
            {
                CellOffset rotatedOffset = component.GetRotatedOffset(offset);
                int num = Grid.OffsetCell(cell, rotatedOffset);
                Grid.FakeFloor.Add(num);
                Pathfinding.Instance.AddDirtyNavGridCell(num);
            }
            floorOffsets = NewCells;

        }

        public void SetFloor(bool active)
        {
            if (isActive == active)
            {
                return;
            }

            int cell = Grid.PosToCell(this);
            Building component = GetComponent<Building>();
            CellOffset[] array = floorOffsets;
            foreach (CellOffset offset in array)
            {
                CellOffset rotatedOffset = component.GetRotatedOffset(offset);
                int num = Grid.OffsetCell(cell, rotatedOffset);
                if (active)
                {
                    Grid.FakeFloor.Add(num);
                }
                else
                {
                    Grid.FakeFloor.Remove(num);
                }

                Pathfinding.Instance.AddDirtyNavGridCell(num);
            }

            isActive = active;
        }

        protected override void OnCleanUp()
        {
            SetFloor(active: false);
            base.OnCleanUp();
        }
    }
}
