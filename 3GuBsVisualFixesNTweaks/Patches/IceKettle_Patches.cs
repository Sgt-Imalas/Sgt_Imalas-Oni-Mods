using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class IceKettle_Patches
    {

        [HarmonyPatch(typeof(IceKettle.Instance), nameof(IceKettle.Instance.StartSM))]
        public class IceKettle_Instance_StartSM_Patch
        {
            public static void Postfix(IceKettle.Instance __instance)
            {
                __instance.master.gameObject.AddOrGet<IceKettle_FuelMeterController>();
            }
        }
    }
}
