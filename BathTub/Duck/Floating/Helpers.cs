using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BathTub.Duck.Floating
{
    public static class Helpers
    {
        /**
         * Gets the mass of the cell at the given position.
         */
        public static float GetMass(Vector2 pos)
        {
            int cell = PosToCell(pos);
            return Grid.IsValidCell(cell) ? Grid.Mass[cell] : float.PositiveInfinity;
        }
        public static float GetMaxMass(Vector2 pos)
        {
            int cell = PosToCell(pos);
            return Grid.IsValidCell(cell) ? Grid.Element[cell].maxMass : float.PositiveInfinity;
        }
        /**
         * Gets the element of a cell at the given position.
         */
        public static Element GetElement(Vector2 pos)
        {
            int cell = PosToCell(pos);
            return Grid.IsValidCell(cell) ? Grid.Element[cell] : null;
        }

        public static bool IsSurfaceLiquid(Vector2 pos)
        {
            return IsVisiblyInLiquid(pos) && !IsVisiblyInLiquid(pos + Vector2.up);
        }

        /**
         * Copy of original with bug fix
         */
        public static bool IsVisiblyInLiquid(Vector2 pos)
        {
            if (!IsValidLiquidCell(pos)) 
                return false;
            int cell = PosToCell(pos);
            if (IsValidLiquidCell(Grid.CellAbove(cell)))
            {
                return true;
            }

            float positionInCell = pos.y - Mathf.Floor(pos.y);
            return GetMass(pos) / GetMaxMass(pos) > positionInCell;
        }

        public static int PosToCell(Vector2 pos)
        {
            pos.y -= 0.05f; // Compensation for PosToCell adding 0.05f
            return Grid.PosToCell(pos);
        }

        /**
         * Checks if the cell at the position is solid.
         */
        public static bool IsSolidCell(Vector2 pos)
        {
            int cell = PosToCell(pos);
            return Grid.IsSolidCell(cell);
        }

        public static float GetYExtent(GravityComponent component)
        {
            return Mathf.Max(0.2f, component.extents.y);
        }

        /**
         * check if it should float - simplified to only check for cmp and is in liquid since only the duck has this component
         */
        public static bool ShouldFloat(Transform tr, out Floater floater)
        {
            floater = null;
            if (tr == null) return false;

            var go = tr.gameObject;

            if( go==null || !go.TryGetComponent<Floater>(out floater))
                return false;
            
            if(!IsVisiblyInLiquid(floater.GetFloatingPosition()))
                return false;

            return true;
        }

        public static bool IsValidLiquidCell(int cell)
        {
            return Grid.IsValidCell(cell) && Grid.IsLiquid(cell);
        }
        public static bool IsValidLiquidCell(Vector2 pos)
        {
            return IsValidLiquidCell(PosToCell(pos));
        }

        /**
         * Gets the nearest liquid surface Y position from the given position.
         */
        public static float GetLiquidSurfaceDistanceAbove(Vector2 initialPos)
        {
            if (!IsValidLiquidCell(initialPos)) return float.PositiveInfinity;

            Vector2 pos = initialPos;
            while (IsValidLiquidCell(pos + Vector2.up))
            {
                pos += Vector2.up;
            }

            //pos.y += ((GetMass(pos) / 1000f) * 0.6f) + 0.4f;
            pos.y += (GetMass(pos) / GetMaxMass(pos));
            return pos.y - initialPos.y;
        }
    }
}
