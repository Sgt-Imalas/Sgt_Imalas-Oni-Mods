using Database;
using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class LaunchedCraft_Patches
	{

        [HarmonyPatch(typeof(LaunchedCraft), nameof(LaunchedCraft.Success))]
        public class TargetType_TargetMethod_Patch
        {
            public static void Postfix(LaunchedCraft __instance, ref bool __result)
            {
                if (!__result)
                    return;

				foreach (Clustercraft clustercraft in Components.Clustercrafts)
				{
					if (clustercraft is SpaceStation)
						continue;

					if (clustercraft.Status == Clustercraft.CraftStatus.InFlight)
						return;
				}
				__result = false;
			}
        }
	}
}
