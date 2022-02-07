using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnastoronOniMods
{
    class RocketControlStationIdleWorkableAI: RocketControlStationIdleWorkable
    {
        public override float GetEfficiencyMultiplier(Worker worker) => 0.1f;
    }
}
