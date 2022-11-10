using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robo_Rockets
{
    internal class FlyingBrain : KMonoBehaviour
    {
        [Serialize]
        float learnedSpeed = 0.75f;
        [Serialize]
        bool awakened = false;


        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (!awakened)
            {
                learnedSpeed = Config.Instance.AiLearnStart;
                awakened = true;
            }
        }
        public void TraveledDistance(int hexes = 1)
        {
            if (learnedSpeed < 2.0f)
            {
                learnedSpeed+=(float)hexes / 100f;
            }
            else if(learnedSpeed < 3.0f)
            {
                learnedSpeed += (float)hexes / 150f;
            }
            else if(learnedSpeed < 4.0f)
            {
                learnedSpeed += (float)hexes / 200f;
            }
            else
            {
                learnedSpeed += (float)hexes / 1000f;
            }
        }

    }
}
