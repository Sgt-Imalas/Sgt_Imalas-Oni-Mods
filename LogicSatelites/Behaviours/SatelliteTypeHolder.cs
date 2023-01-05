using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

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
                _satelliteType = value;
                OverrideSatDescAndName();
            } 
        }
        void OverrideSatDescAndName()
        {
            var nameHolder = gameObject.GetComponent<KSelectable>();
            var descHolder = gameObject.AddOrGet<InfoDescription>();
            if (nameHolder != null)
            {
                nameHolder.SetName(ModAssets.SatelliteConfigurations[_satelliteType].NAME);
            }
            if (descHolder != null)
            {
                descHolder.description = (ModAssets.SatelliteConfigurations[_satelliteType].DESC);
            }
        }

        protected override void OnSpawn()
        {
            base.OnSpawn(); OverrideSatDescAndName();
        }

    }
}
