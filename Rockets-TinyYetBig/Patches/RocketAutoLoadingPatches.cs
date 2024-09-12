using HarmonyLib;
using System;

namespace Rockets_TinyYetBig.Patches
{
	/// <summary>
	/// Replace Filler Logic to include fuel loaders and loading of additional modules
	/// </summary>
	public class RocketAutoLoadingPatches
	{
		[HarmonyPatch(typeof(LaunchPadMaterialDistributor.Instance))]
		[HarmonyPatch(nameof(LaunchPadMaterialDistributor.Instance.FillRocket))]
		public class AddFuelingLogic
		{
			public static bool Prefix(LaunchPadMaterialDistributor.Instance __instance)
			{
				var craftInterface = __instance.sm.attachedRocket.Get<RocketModuleCluster>(__instance).CraftInterface;

				HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
				__instance.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);


				System.Action<bool> fillInProgressSetterAction = new Action<bool>((fillingOngoing) => __instance.sm.fillComplete.Set(fillingOngoing, __instance));
				//SgtLogger.l(__instance.gameObject.GetProperName());
				ModAssets.ReplacedCargoLoadingMethod(craftInterface, chain, fillInProgressSetterAction);

				//PPatchTools.TryGetFieldValue(__instance.sm, "fillComplete", out StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.BoolParameter fillComplete);
				//fillComplete.Set(!HasLoadingProcess, __instance);
				return false;
			}
		}
	}
}
