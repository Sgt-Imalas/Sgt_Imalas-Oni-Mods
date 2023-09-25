using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents.BeeGeyser
{
    internal class BeeGeyser : KMonoBehaviour, ISim200ms
    {
        float beeIntervalMin = 20;
        float beeIntervalMax = 0.5f;


        [Serialize] float timeToNextBee = 0;

        [MyCmpGet]
        KBatchedAnimController animController;
        public override void OnSpawn()
        {
            base.OnSpawn();
            animController.flipY = true;
        }



        public void Sim200ms(float dt)
        {

        }
    }
}
