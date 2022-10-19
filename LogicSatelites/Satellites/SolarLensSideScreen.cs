using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatellites.Satellites
{
    class SolarLensSideScreen : SideScreenContent
    {
        public override bool IsValidForTarget(GameObject target)
        {
            return target.TryGetComponent<SolarLens>(out var lens);
        }
    }
}
