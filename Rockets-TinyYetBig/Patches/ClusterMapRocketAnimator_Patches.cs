using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Nosecones;

namespace Rockets_TinyYetBig.Patches
{
	internal class ClusterMapRocketAnimator_Patches
    {
        [HarmonyPatch(typeof(ClusterMapRocketAnimator.StatesInstance), nameof(ClusterMapRocketAnimator.StatesInstance.SetDrillConeVisibility))]
        public class ClusterMapRocketAnimator_StatesInstance_SetDrillConeVisibility_Patch
        {
            /// <summary>
            /// drillcone symbol for laser drillcone rockets
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="shouldBeVisible"></param>
            public static void Prefix(ClusterMapRocketAnimator.StatesInstance __instance, ref bool shouldBeVisible)
            {
                if (shouldBeVisible || __instance.smi.entity is not Clustercraft cc)
                    return;

				foreach (var clusterModule in cc.ModuleInterface.ClusterModules)
				{
					ResourceHarvestModuleHEP.StatesInstance sMI = clusterModule.Get().GetSMI<ResourceHarvestModuleHEP.StatesInstance>();
					if (sMI != null)
					{
                        shouldBeVisible = true;
                        return;
					}
				}
            }
        }
	}
}
