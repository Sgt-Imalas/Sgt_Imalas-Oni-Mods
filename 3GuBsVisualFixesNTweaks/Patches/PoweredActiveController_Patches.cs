using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class PoweredActiveController_Patches
    {

        [HarmonyPatch(typeof(PoweredActiveController), nameof(PoweredActiveController.InitializeStates))]
        public class PoweredActiveController_InitializeStates_Patch
        {
            public static void Postfix(PoweredActiveController __instance)
            {
                __instance.working.pre.Enter(smi =>
                {
                    if(smi.gameObject.TryGetComponent<AirConditioner>(out var conditioner))
                    {
                        var consumer = conditioner.consumer;
                        if (ModAssets.TryGetCachedKbacs(smi.gameObject, out var kbac, out var fg))
                            ModAssets.TryApplyConduitTint(consumer.conduitType, consumer.GetInputCell(consumer.conduitType), kbac, fg);
                    }
				});
            }
        }
    }
}
