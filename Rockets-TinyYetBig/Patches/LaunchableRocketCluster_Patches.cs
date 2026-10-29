using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class LaunchableRocketCluster_Patches
	{

        [HarmonyPatch(typeof(LaunchableRocketCluster.States), nameof(LaunchableRocketCluster.States.InitializeStates))]
        public class LaunchableRocketCluster_States_InitializeStates_Patch
		{
            public static void Postfix(LaunchableRocketCluster.States __instance)
            {
                __instance.not_grounded.launch_pst
                    .Enter(smi => Game.Instance.Trigger(ModAssets.Hashes.RocketLaunchFinalized, smi.gameObject));
                __instance.not_grounded.landing_loop
					.Enter(smi => Game.Instance.Trigger(ModAssets.Hashes.RocketLandingInitialized, smi.gameObject));
			}
        }
	}
}
