using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class BioDieselEngine_Patches
	{
        [HarmonyPatch(typeof(BiodieselEngineClusterConfig), nameof(BiodieselEngineClusterConfig.DoPostConfigureComplete))]
        public class BiodieselEngineClusterConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go) => go.GetComponent<RocketEngineCluster>().fuelTag = ModAssets.Tags.AIO_BioFuel;
		}
	}
}
