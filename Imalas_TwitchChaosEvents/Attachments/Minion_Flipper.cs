using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Attachments
{
    internal class Minion_Flipper : KMonoBehaviour, ISim1000ms
    {
        [MyCmpReq]
        Effects effects;
        [MyCmpReq]
        KBatchedAnimController animController;

        bool flippingActive = false;    

        public void Sim1000ms(float dt)
        {
            if (effects.HasEffect(ModAssets.Chaos_Effects.FLIPPEDWATERDRINK))
            {
                if (!animController.TintColour.Equals( UIUtils.rgb(255, 173, 176)) || !flippingActive)
                {
                    flippingActive = true;
                    //animController.Rotation = 180f;
                    animController.flipY = true;
                    animController.Offset = new Vector3(0, 1.5f);
                    animController.TintColour = UIUtils.rgb(255, 173, 176);
                }
            }
            else
            {
                if (!animController.TintColour.Equals(Color.white)|| flippingActive)
                {
                    flippingActive = false;
                    //animController.Rotation = 0f;
                    animController.flipY = false;
                    animController.Offset = new Vector3(0, 0);
                    animController.TintColour = Color.white;
                }
            }
        }
    }
}
