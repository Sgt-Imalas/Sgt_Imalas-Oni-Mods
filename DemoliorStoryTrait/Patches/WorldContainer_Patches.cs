using HarmonyLib;
using Klei.AI;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
    class WorldContainer_Patches
	{
		/// <summary>
		/// apply the fixed trait to the target asteroid of the story trait
		/// </summary>

		[HarmonyPatch(typeof(WorldContainer), nameof(WorldContainer.SetWorldDetails))]
        public class WorldContainer_SetWorldDetails_Patch
        {
            public static void Postfix(WorldContainer __instance, WorldGen world)
            {
                if (__instance.WorldTraitIds.Contains(Stories_Patches.CGM_Impactor_Path))
                    __instance.largeImpactorFragmentsFixedTrait = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NAME.ALLOWED;

			}
        }

    }
}
