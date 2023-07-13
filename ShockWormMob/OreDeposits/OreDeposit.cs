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
        [Serialize]public string DepositTypeID;
        //[Serialize]public float MiningRate;
        //[Serialize]public SimHashes MiningElement;
        List<KeyValuePair<SimHashes, float>> materialsMined;

        /// <summary>
        /// Allows varying the outputs of a deposit if you change the logic, atm it just gives the first one
        /// </summary>
        /// <returns></returns>
        /// 
        public override void OnSpawn()
        {
            base.OnSpawn();
            materialsMined = OreDepositsConfig.DepositConfigurations[DepositTypeID].mineableElements.ToList();
        }
        public KeyValuePair<SimHashes,float> GetCurrentMiningElement()
        {
            return materialsMined.First();
        }

    }    
}
