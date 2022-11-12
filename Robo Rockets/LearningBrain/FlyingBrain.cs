using KSerialization;
using RoboRockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboRockets.LearningBrain
{
    internal class FlyingBrain : KMonoBehaviour
    {
        [Serialize]
        float learnedSpeed = 0.75f;
        [Serialize]
        bool awakened = false;

        public float GetCurrentSpeed() => learnedSpeed;

        

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (!awakened)
            {
                learnedSpeed = Config.Instance.AiLearnStart;
                awakened = true;
            }
            this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.ExperienceLevel, (object)this);
        }
        public void TraveledDistance(int hexes = 1)
        {
            if (learnedSpeed < 2.0f)
            {
                learnedSpeed += hexes / 100f;
            }
            else if (learnedSpeed < 3.0f)
            {
                learnedSpeed += hexes / 150f;
            }
            else if (learnedSpeed < 4.0f)
            {
                learnedSpeed += hexes / 200f;
            }
            else
            {
                learnedSpeed += hexes / 1000f;
            }
        }

    }
}
