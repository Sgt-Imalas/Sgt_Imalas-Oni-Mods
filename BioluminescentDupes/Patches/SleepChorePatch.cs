using BioluminescentDupes.Content.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioluminescentDupes.Patches
{
	internal class SleepChorePatch
    {
        [HarmonyPatch(typeof(SleepChore.StatesInstance), nameof(SleepChore.StatesInstance.IsGlowStick))]
        public class SleepChore_StatesInstance_IsGlowStick_Patch
        {
            public static void Postfix(SleepChore.StatesInstance __instance, ref bool __result)
            {
                if(!__result && __instance.sm.sleeper.Get(__instance.smi).TryGetComponent<BD_Bioluminescence>(out _))
                {
                    __result = true;
                }
            }
        }
	}
}
