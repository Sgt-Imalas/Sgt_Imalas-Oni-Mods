using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OniRetroEdition.ModPatches
{
	internal class Campfire_Patches
    {
        [HarmonyPatch(typeof(Campfire), nameof(Campfire.InitializeStates))]
        public class Campfire_InitializeStates_Patch
        {
            public static void Postfix(Campfire __instance)
            {
				__instance.operational.working
                    .PlayAnim("working_pre")
                    .QueueAnim("on",true);
                __instance.operational.working.Exit(smi => smi.ScheduleNextFrame(_ => { smi.Play("working_post"); smi.Queue("off"); }));

			}
        }
	}
}
