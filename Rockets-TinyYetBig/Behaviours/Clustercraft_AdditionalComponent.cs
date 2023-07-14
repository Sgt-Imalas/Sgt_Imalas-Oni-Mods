using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Behaviours
{
    internal class Clustercraft_AdditionalComponent : KMonoBehaviour
    {
        [Serialize] float _drillcone_MiningSkillMultiplier = 0.75f;
        public void SetMiningMultiplierForRocket(float miningMultiplier)
        {

            _drillcone_MiningSkillMultiplier = miningMultiplier;
        }
        public float Drillcone_MiningSkillMultiplier => _drillcone_MiningSkillMultiplier;
    }
}
