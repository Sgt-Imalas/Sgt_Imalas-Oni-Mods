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
            if (learnedSpeed < 1.0f)
            {
                learnedSpeed += hexes / 100f;
            }
            else if (learnedSpeed < 1.25f)
            {
                learnedSpeed += hexes / 125f;
            }
            else if (learnedSpeed < 1.50f)
            {
                learnedSpeed += hexes / 150f;
            }
            else if (learnedSpeed < 1.75f)
            {
                learnedSpeed += hexes / 175f;
            }
            else if (learnedSpeed < 2f)
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
