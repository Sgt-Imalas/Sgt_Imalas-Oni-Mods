using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class Gantry_Patches
	{

        [HarmonyPatch(typeof(GantryConfig), nameof(GantryConfig.DoPostConfigureComplete))]
        public class GantryConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<AutomaticGantry>();
            }
        }
	}
}
