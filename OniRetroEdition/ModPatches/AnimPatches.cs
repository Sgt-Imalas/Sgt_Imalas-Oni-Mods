using HarmonyLib;
using OniRetroEdition.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
    internal class AnimPatches
    {
        [HarmonyPatch(typeof(ManualGeneratorConfig), nameof(ManualGeneratorConfig.DoPostConfigureComplete))]
        public static class ManualGenerator_AnimFix
        {
            public static void Postfix(GameObject go)
            {
                if(go != null && go.TryGetComponent<ManualGenerator>(out var gen))
                {
                    gen.synchronizeAnims = false;
                }
            }
        }
    }
}
