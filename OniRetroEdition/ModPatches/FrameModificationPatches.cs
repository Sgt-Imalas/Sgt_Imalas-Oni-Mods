using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
    internal class FrameModificationPatches
    {
        [HarmonyPatch(typeof(CanvasConfig), nameof(CanvasConfig.DoPostConfigureComplete))]
        public static class ManualGenerator_AnimFix
        {
            public static void Postfix(GameObject go)
            {
                //go.AddOrGet<EditableFrame>();
                
            }
        }
    }
}
