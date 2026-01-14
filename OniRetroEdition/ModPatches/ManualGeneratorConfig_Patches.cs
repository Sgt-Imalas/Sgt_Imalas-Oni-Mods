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
	internal class ManualGeneratorConfig_Patches
	{

   //     [HarmonyPatch(typeof(ManualGeneratorConfig), nameof(ManualGeneratorConfig.CreateBuildingDef))]
   //     public class ManualGeneratorConfig_CreateBuildingDef_Patch
   //     {
   //         public static void Postfix(BuildingDef __result)
   //         {
   //            __result.GeneratorBaseCapacity = 10000;
			//}
   //     }

        [HarmonyPatch(typeof(ManualGeneratorConfig), nameof(ManualGeneratorConfig.DoPostConfigureComplete))]
        public class ManualGeneratorConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
			{
				//Battery battery = go.AddOrGet<Battery>();
				//battery.capacity = 10000f;
				//battery.joulesLostPerSecond = 1.66666663f;
				go.AddOrGet<ManualGeneratorDischargerWithMeter>();
            }
        }
	}
}
