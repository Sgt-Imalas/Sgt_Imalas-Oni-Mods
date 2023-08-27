using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.Creeper
{
    public class CreeperController : KMonoBehaviour, ISim200ms
    {
        public static CreeperController instance;

        public void Sim200ms(float dt)
        {
        }
    }
}
