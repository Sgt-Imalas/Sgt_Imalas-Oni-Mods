using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class OilWellCap_Patches
    {

        [HarmonyPatch(typeof(OilWellCap), nameof(OilWellCap.OnStartWork))]
        public class OilWellCap_OnStartWork_Patch
        {
            public static void Postfix(OilWellCap __instance, WorkerBase worker)
            {
                if(worker != null && worker.TryGetComponent<Transform>(out var workerTransform))
                {
                    var pos = workerTransform.position;
                    pos.x -= 0.2f;
                    workerTransform.SetPosition(pos);
                }
            }
        }
    }
}
