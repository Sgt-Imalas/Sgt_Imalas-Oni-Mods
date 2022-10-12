using LogicSatellites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites.Buildings
{
    public class SolarReciever : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            ModAssets.SolarRecievers.Add(this);
        }


        protected override void OnCleanUp()
        {
            ModAssets.SolarRecievers.Remove(this);
            base.OnCleanUp();
        }
    }
}
