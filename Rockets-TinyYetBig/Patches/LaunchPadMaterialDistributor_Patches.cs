using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;

namespace Rockets_TinyYetBig.Patches.RocketLoadingPatches
{
    /// <summary>
    /// Replace Filler Logic to include fuel loaders and loading of additional modules
    /// </summary>
    public class LaunchPadMaterialDistributor_Patches
    {
        [HarmonyPatch(typeof(LaunchPadMaterialDistributor.Instance), nameof(LaunchPadMaterialDistributor.Instance.FillRocket))]
        public class LaunchPadMaterialDistributor_FillRocket_Patch
		{
            public static bool Prefix(LaunchPadMaterialDistributor.Instance __instance)
            {
                var craftInterface = __instance.sm.attachedRocket.Get<RocketModuleCluster>(__instance).CraftInterface;

                HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
                __instance.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);


                Action<bool> onFillComplete = new Action<bool>((fillingOngoing) => __instance.sm.fillComplete.Set(fillingOngoing, __instance));
				RocketPortCargoLoading.ReplacedCargoLoadingMethod(craftInterface, chain, onFillComplete);
                return false;
            }
        }
    }
}
