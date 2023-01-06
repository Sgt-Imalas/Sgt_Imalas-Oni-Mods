using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites.Buildings
{
    internal class ScatteringLens : KMonoBehaviour, ISim200ms
    {
        public Light2D lightSource;
        public void Sim200ms(float dt)
        {
            int cellAbove = Grid.GetCellInDirection(Grid.PosToCell(this.gameObject), Direction.Up);
            int cellBelow = Grid.GetCellInDirection(Grid.PosToCell(this.gameObject), Direction.Down);
            //int cell = Grid.PosToCell(this.gameObject);
            //Debug.Log(Grid.LightIntensity[cell]+" Source; "+lightSource.Lux);
            int lux = Grid.LightIntensity[cellAbove];
            lightSource.Lux = lux/3; ///or /2, dependant on balance decision.
            lightSource.Range = ((float)lux)/2000f;
            lightSource.FullRefresh();
        }
    }
}
