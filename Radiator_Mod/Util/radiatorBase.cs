using RoboRockets.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;

namespace RadiatorMod.Util
{
    public class RadiatorBase : KMonoBehaviour, ISim1000ms
    {
        //[MyCmpReq]
        //protected Storage storage;
        //[MyCmpReq]
        //protected Operational operational;
        //[MyCmpReq]
        //private ConduitConsumer consumer;
        [MyCmpGet]
        private Rotatable rotatable;

        private int RadiatorStrength=0;
        private static readonly int MaxAreaLength = 5;
        public void Sim1000ms(float dt)
        {
            //Debug.Log("RECALCULATION");
            RecalculateRadiationArea();
        }

        public void RecalculateRadiationArea()
        {
            int _radStrength = 0;
            CellOffset offset = new CellOffset(1, MaxAreaLength);
            offset = Rotatable.GetRotatedCellOffset(offset, rotatable.Orientation);

            var lPos = Grid.CellToXY(Grid.PosToCell(this));
            int lx = 0, ly = 0;

            if (offset.x < 0) lx += offset.x;
            if (offset.y < 0) ly += offset.y;
            //Debug.Log(lx + " " + offset.x + " " + ly + " " + offset.y);
            for (int i = lx; i <= (offset.x-lx); i++)
            {
                for (int j = ly; j <= (offset.y-ly); j++)
                {
                    int l2x = lPos.x+i;
                    int l2y = lPos.y+j;

                    if (Grid.IsCellOpenToSpace(Grid.XYToCell(l2x, l2y))){
                        _radStrength += 1;
                    }
                }
            }
            RadiatorStrength = _radStrength;
            //Debug.Log(RadiatorStrength);
        }



    }
}
