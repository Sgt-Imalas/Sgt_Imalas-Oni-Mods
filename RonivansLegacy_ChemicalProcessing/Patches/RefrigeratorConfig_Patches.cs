using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class RefrigeratorConfig_Patches
    {

        [HarmonyPatch(typeof(RefrigeratorConfig), nameof(RefrigeratorConfig.DoPostConfigureComplete))]
        public class RefrigeratorConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
			{
				go.AddOrGet<FridgeSaverDescriptor>().Cache();
			}
        }
    }
}
