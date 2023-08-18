using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.OmegaSawblade
{
    internal class OmegaSawblade : KMonoBehaviour, ISim33ms
    {
        float WorldDamagePerSecond = 1f / 3f;
        float BuildingDamagePerSecond = 100f / 3f;
        float EntityDamagePerSecond = 20f;

        public void Sim33ms(float dt)
        {

        }
    }
}
