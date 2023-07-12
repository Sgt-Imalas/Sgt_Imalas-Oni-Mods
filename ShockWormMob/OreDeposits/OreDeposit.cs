using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShockWormMob.OreDeposits
{
    internal class OreDeposit : KMonoBehaviour
    {
        [Serialize]public Dictionary<SimHashes,float> MiningRates;
        public Dictionary<SimHashes, float> getMiningRates() => MiningRates;


        /// <summary>
        /// Allows varying the outputs of a deposit if you change the logic, atm it just gives the first one
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<SimHashes,float> GetCurrentMiningElement()
        {
            return MiningRates.FirstOrDefault();
        }

    }    
}
