using BioluminescentDupes.Content.Scripts;
using HarmonyLib;

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
