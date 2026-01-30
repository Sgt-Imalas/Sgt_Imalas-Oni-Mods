using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SodaFountainConfig_Patches
	{

        [HarmonyPatch(typeof(SodaFountainConfig), nameof(SodaFountainConfig.ConfigureBuildingTemplate))]
        public class SodaFountainConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(GameObject go)
            {
                CustomSodaFountain.ConfigureBuilding(go);

			}
        }
	}
}
