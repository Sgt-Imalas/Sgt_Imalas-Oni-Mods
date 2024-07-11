using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BathTub
{
    internal class ModAssets
    {
        public static GameObject FoamFX;

        internal static void InitFoam()
        {
            SgtLogger.l("initiating foam particle system");
            var movingGo = FoamFX.transform.Find("SparkleStreakMovement")?.gameObject;
            var stationaryGo = FoamFX.transform.Find("SparkleStreakIdle")?.gameObject;

            if (movingGo != null 
                && movingGo.TryGetComponent<ParticleSystem>(out var ps_moving) 
                && movingGo.TryGetComponent<ParticleSystemRenderer>(out var psr_moving))
            {
                UtilMethods.ListAllPropertyValues(psr_moving);
                UtilMethods.ListAllFieldValues(psr_moving);
            }
            if (stationaryGo != null 
                && stationaryGo.TryGetComponent<ParticleSystem>(out var ps_stationary)
                && stationaryGo.TryGetComponent<ParticleSystemRenderer>(out var psr_stationary))
            {
                
            }
        }
    }
}
