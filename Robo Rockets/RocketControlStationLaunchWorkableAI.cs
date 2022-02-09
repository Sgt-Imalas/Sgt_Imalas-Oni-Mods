using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnastoronOniMods
{
    class RocketControlStationLaunchWorkableAI : RocketControlStationLaunchWorkable
    {
        public override float GetEfficiencyMultiplier(Worker worker) => 1f;
    }
}
