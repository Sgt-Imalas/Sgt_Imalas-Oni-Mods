using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _WorldGenStateCapture.WorldStateData.WorldPOIs
{
    internal class MapGeyser : MapPOI
    {
        public float EmitRate;

        public float ActivePeriod_IdleTime;
        public float ActivePeriod_EruptionTime;

        public float DormancyCycles;
        public float ActiveCycles;

    }
}
