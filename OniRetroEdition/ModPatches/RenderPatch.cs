using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
    internal class RenderPatch
    {
        //[HarmonyPatch(typeof(LightBufferCompositor), nameof(LightBufferCompositor.OnRenderImage))]
        //public class LightBufferCompositor_OnRenderImage_Patch
        //{
        //    public static void Prefix(LightBufferCompositor __instance)
        //    {
        //        __instance.material.color = Color.red;
        //    }
        //}
    }
}
