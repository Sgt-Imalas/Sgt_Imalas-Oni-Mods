using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShockWormMob.OreDeposits
{
    internal class OreDeposit : KMonoBehaviour
    {
        [Serialize]public Dictionary<SimHashes,float> MiningRates;
    }
}
