using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

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
            animController.flipY = true;
            animController.offset = new UnityEngine.Vector3(0, 4);
            base.OnSpawn();
        }



        public void Sim200ms(float dt)
        {
            animController.Play("erupt");
            animController.onAnimComplete += (s) => SpawnBee(s);
        }

        private void SpawnBee(HashedString s)
        {
            SgtLogger.l(s.ToString(), "hashed");
        }
    }
}
