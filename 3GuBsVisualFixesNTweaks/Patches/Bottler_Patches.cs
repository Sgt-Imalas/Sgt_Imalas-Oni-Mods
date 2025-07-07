using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class Bottler_Patches
    {

        [HarmonyPatch(typeof(Bottler), nameof(Bottler.OnStartWork))]
        public class Bottler_OnStartWork_Patch
        {
            public static void Postfix(Bottler __instance, WorkerBase worker)
			{
				if (worker != null && worker.TryGetComponent<Transform>(out var workerTransform) && worker.TryGetComponent<MinionIdentity>(out _))
				{
					var pos = workerTransform.position;
					pos.x -= 0.5f;
					workerTransform.SetPosition(pos);
				}
			}
		}
    }
}
