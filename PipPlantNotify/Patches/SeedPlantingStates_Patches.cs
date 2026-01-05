using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipPlantNotify.Patches
{
	internal class SeedPlantingStates_Patches
	{

        [HarmonyPatch(typeof(SeedPlantingStates), nameof(SeedPlantingStates.PlantComplete))]
        public class SeedPlantingStates_PlantComplete_Patch
        {
            public static void Prefix(SeedPlantingStates.Instance smi)
            {
                if (smi.targetSeed == null || !smi.targetSeed.TryGetComponent<PlantableSeed>(out var seed))
                    return;

                if(SeedPlantingStates.CheckValidPlotCell(smi,seed,smi.targetDirtPlotCell, out _))
                {
                    smi.master.gameObject.BoxingTrigger(ModAssets.PipHasPlantedSeed, smi.targetDirtPlotCell);
                }
            }
        }
	}
}
