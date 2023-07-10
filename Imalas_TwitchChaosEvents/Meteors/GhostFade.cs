using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Meteors
{
    internal class GhostFade : KMonoBehaviour, ISim200ms
    {
        [Serialize] public float Lifetime = 60f;
        [Serialize] public float FadingStart = 20f;
        [Serialize] public float DefaultFade = 0.85f;
        [Serialize] public float CurrentLifetime;
        [MyCmpGet] KBatchedAnimController controller;

        public override void OnSpawn()
        {
            base.OnSpawn();
            CurrentLifetime = Lifetime;
            var fade = new Color(1, 1, 1, Mathf.Clamp01(CurrentLifetime / FadingStart) * DefaultFade);
            controller.TintColour = fade;
        }
        public void Sim200ms(float dt)
        {
            CurrentLifetime -= dt;
            if (CurrentLifetime >= FadingStart)
                return;

            var fade = new Color(1, 1, 1, Mathf.Clamp01(CurrentLifetime / FadingStart)* DefaultFade);
            controller.TintColour = fade;
            if (CurrentLifetime <= 0)
                Destroy(this.gameObject);

        }
    }
}
