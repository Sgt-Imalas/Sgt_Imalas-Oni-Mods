using LogicSatellites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites.Buildings
{
    public class SolarReciever : KMonoBehaviour, IListableOption
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

        public string GetProperName()
        {
            return gameObject.GetProperName()+" SatReciever";
        }
    }
}
