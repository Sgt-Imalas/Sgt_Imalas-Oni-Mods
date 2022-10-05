using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes
{
    class HoldMyReferences : KMonoBehaviour
    {
        [Serialize]
        public Dictionary<string,int> USEDSTATS= new();

        public bool AddOrIncreaseToStat(string stat)
        {
            if (!USEDSTATS.ContainsKey(stat))
            {

                USEDSTATS[stat] = 1;
                return true;
            }
            else
            {
                USEDSTATS[stat]++;
                return false;
            }
        }
        public bool DoesRemoveReduceStats(string stat, bool remove)
        {
            if (USEDSTATS.ContainsKey(stat))
            {
                Debug.Log(stat + ", Count " + USEDSTATS[stat]);
                if (USEDSTATS[stat] > 1)
                {
                    USEDSTATS[stat]--;
                    return false;
                }
                else
                {
                    USEDSTATS.Remove(stat);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }


    }
}
