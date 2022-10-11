using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites.Behaviours
{
    class SatelliteTypeHolder:KMonoBehaviour
    {
        [Serialize]
        private int _satelliteType = 0;
        public int SatelliteType { 
            get 
            {
                return _satelliteType;
            }
            set
            {
                var nameHolder = gameObject.GetComponent<KSelectable>();
                var descHolder = gameObject.AddOrGet<InfoDescription>();
                if (nameHolder != null)
                {
                    _satelliteType = value;
                    nameHolder.SetName(ModAssets.SatelliteConfigurations[value].NAME);
                }
                if (descHolder != null)
                {
                    _satelliteType = value;
                    descHolder.description=(ModAssets.SatelliteConfigurations[value].DESC);
                }
            } 
        }

    }
}
