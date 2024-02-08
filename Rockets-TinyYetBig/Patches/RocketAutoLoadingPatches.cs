using HarmonyLib;
using PeterHan.PLib.Core;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.RocketFueling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UnityEngine;
using UtilLibs;
using static Operational;
using static Rockets_TinyYetBig.RocketFueling.FuelLoaderComponent;
using static StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.UI.DEVELOPMENTBUILDS.ALPHA;
using static STRINGS.UI.STARMAP;

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
                SgtLogger.l(__instance.gameObject.GetProperName());
                ModAssets.ReplacedCargoLoadingMethod(craftInterface,chain, fillInProgressSetterAction);

                //PPatchTools.TryGetFieldValue(__instance.sm, "fillComplete", out StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.BoolParameter fillComplete);
                //fillComplete.Set(!HasLoadingProcess, __instance);
                return false;
            }
        }
    }
}
