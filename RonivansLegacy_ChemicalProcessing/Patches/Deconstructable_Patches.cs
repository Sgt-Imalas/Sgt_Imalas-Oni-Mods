using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI.SANDBOXTOOLS.SETTINGS;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Deconstructable_Patches
	{

        [HarmonyPatch(typeof(Deconstructable), nameof(Deconstructable.SpawnItemsFromConstruction), [typeof(float), typeof(byte), typeof(int), typeof(WorkerBase)])]
        public class Deconstructable_SpawnItemsFromConstruction_Patch
        {
            public static void Prefix(Deconstructable __instance, ref float temperature)
            {
                if (!__instance.TryGetComponent<ContinuousLiquidCooledFabricatorAddon>(out var thermalBatteryComponent))
                    return;
                temperature += thermalBatteryComponent.GetThermalBatteryHeatIncrease();
			}
        }
	}
}
