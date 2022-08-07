using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopod.PassengersScenario
{
    class ColonyFailPatches
    {
        //[HarmonyPatch(typeof(GameFlowManager.States))]
        //[HarmonyPatch(nameof(GameFlowManager.States.InitializeStates))]
        //public class UnlockCryopodOnArtifactScanning_Patch
        //{
        //    public static void Postfix(GameFlowManager.States __instance)
        //    {
        //        //fun Idea but nou!
        //        __instance.gameover.active.Update((__instance, dt) =>
        //        {
        //            baseAger.Instance.AgeBaseP();
        //        });
        //    }
        //}
    }
}
