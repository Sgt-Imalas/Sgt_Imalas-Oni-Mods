using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
    internal class BurningCursorLogic : KMonoBehaviour, ISim33ms
    {
        [MyCmpGet]
        ParticleSystem particleSystem;

        public float energyPerSecondKJStart = 30000f;
        public float energyPerSecondKJPutOut = 3000f;
        public float distanceToPutOut = 360f;
        public float distanceToPutOutSquared;
        public float timeToPutOut = 60f;

        public void Sim33ms(float dt)
        {
            Burn(dt);
        }

        public override void OnSpawn()
        {
            base.OnSpawn(); 
            distanceToPutOutSquared = Mathf.Pow(distanceToPutOut, 2);
            distanceRemainingSquared = Mathf.Pow(distanceToPutOut,2);
            timeRemaining = timeToPutOut;
        }

        private float distanceRemainingSquared;
        private float timeRemaining;
        public float particlesStartOverTime = 240;
        public float particlesEndOverTime = 40;

        private bool scheduledForDestruction = false;

        void FixedUpdate()
        {
            var updatedPosition = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
            updatedPosition.z = -50f; //Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
            distanceRemainingSquared -= (transform.position - updatedPosition).sqrMagnitude;
            transform.SetPosition(updatedPosition);
        }

        public void Burn(float dt)
        {
            if (scheduledForDestruction) return;

            timeRemaining -= dt;

             
            float DistanceLerp = Mathf.InverseLerp(0, distanceToPutOutSquared, distanceRemainingSquared);
            float TimeLerp = Mathf.InverseLerp(0, timeToPutOut, timeRemaining);

            float SmallerLerpVal = Mathf.Min(DistanceLerp, TimeLerp);

            float energy = Mathf.Lerp(energyPerSecondKJPutOut, energyPerSecondKJStart,SmallerLerpVal);

            int particles = Mathf.RoundToInt(Mathf.Lerp(particlesEndOverTime, particlesStartOverTime, SmallerLerpVal));

            var emission = particleSystem.emission;
            emission.rateOverTime = particles;

            SgtLogger.l(particles.ToString(), "PARTICLES");

            int cell = Grid.PosToCell(this);

            if (Grid.IsValidCellInWorld(cell, ClusterManager.Instance.activeWorldId))
            {
                SimMessages.ModifyEnergy(cell, energy * dt, UtilMethods.GetKelvinFromC(666), SimMessages.EnergySourceID.Burner) ;
            }
            if(distanceRemainingSquared<=0 || timeRemaining<=0)
            {
                emission.rateOverTime = 0;
                scheduledForDestruction = true;
                ToastManager.InstantiateToast(
                STRINGS.CHAOSEVENTS.BURNINGCURSOR.TOAST,
                 STRINGS.CHAOSEVENTS.BURNINGCURSOR.TOASTTEXTENDING
                 );
                GameScheduler.Instance.Schedule("DestroyBurningCmp ", 15, (s) => Destroy(this.gameObject));
            }
        }
    }
}
