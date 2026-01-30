using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Growing_Patches
	{

        [HarmonyPatch(typeof(Growing), nameof(Growing.OnPrefabInit))]
        public class Growing_OnPrefabInit_Patch
        {
            public static void Postfix(Growing __instance)
            {
                __instance.gameObject.AddOrGet<PlantNitrogenConsumer>();
            }
        }
	}
}
