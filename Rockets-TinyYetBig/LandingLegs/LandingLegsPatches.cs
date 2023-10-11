using HarmonyLib;
using Rockets_TinyYetBig._ModuleConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static LaunchConditionManager;
using static SelectModuleCondition;

namespace Rockets_TinyYetBig.LandingLegs
{
    class LandingLegsPatches
    {
        [HarmonyPatch(typeof(LaunchPad), nameof(LaunchPad.Sim1000ms))]
        public static class NoLogicUpdate1
        {
            public static bool Prefix(LaunchPad __instance)
            {
                if (__instance is RTB_LaunchPadWithoutLogic)
                {
                    __instance.CheckLandedRocketPassengerModuleStatus();
                    return false;
                }

                return true;
            }

        }
        [HarmonyPatch(typeof(LaunchPad), nameof(LaunchPad.IsLogicInputConnected))]
        public static class NoLogicUpdate2
        {
            public static bool Prefix(LaunchPad __instance)
            {
                if (__instance is RTB_LaunchPadWithoutLogic)
                    return false;

                return true;
            }
        }
        [HarmonyPatch(typeof(LaunchPad), nameof(LaunchPad.RebuildLaunchTowerHeight))]
        public static class NoLogicUpdate3
        {
            public static bool Prefix(LaunchPad __instance)
            {
                if (__instance is RTB_LaunchPadWithoutLogic)
                    return false;
                return true;
            }

        }
        [HarmonyPatch(typeof(LaunchButtonSideScreen), nameof(LaunchButtonSideScreen.IsValidForTarget))]
        public static class HideSideScreenOnDisabled2
        {
            public static void Postfix(GameObject target, ref bool __result)
            {
                if (__result && target.TryGetComponent<LaunchPad>(out var launchpad) && launchpad.enabled == false)
                    __result = false;
            }

        }
        [HarmonyPatch(typeof(LaunchPadSideScreen), nameof(LaunchPadSideScreen.IsValidForTarget))]
        public static class HideSideScreenOnDisabled
        {
            public static void Postfix(GameObject target, ref bool __result)
            {
                if (__result && target.TryGetComponent<LaunchPad>(out var launchpad) && launchpad.enabled == false)
                    __result = false;
            }

        }
        [HarmonyPatch(typeof(LaunchPadSideScreen), nameof(LaunchPadSideScreen.RefreshRocketButton))]
        public static class DisableSideScreen
        {
            public static void Postfix(LaunchPadSideScreen __instance)
            {
                if (__instance.startNewRocketbutton.isInteractable &&  __instance.selectedPad is RTB_LaunchPadWithoutLogic)
                {
                    __instance.startNewRocketbutton.isInteractable = false;
                }
                if (__instance.devAutoRocketButton.isInteractable && __instance.selectedPad is RTB_LaunchPadWithoutLogic)
                {
                    __instance.devAutoRocketButton.isInteractable = false;
                }
            }

        }

        //[HarmonyPatch(typeof(Db))]
        //[HarmonyPatch("Initialize")]
        //public static class Db_Init_Patch
        //{

        //    public static void Postfix()
        //    {
        //        System.Reflection.MethodInfo prefix = AccessTools.Method(typeof(ModifyLaunchConditionsForEdgeCase), "Prefix");
        //        foreach (var MethodToPatch in ModifyLaunchConditionsForEdgeCase.TargetMethods())
        //            Mod.haromy.Patch(MethodToPatch, new HarmonyMethod(prefix));
        //    }
        //}

        //public static class ModifyLaunchConditionsForEdgeCase
        //{
        //    public static bool Prefix(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext, ref bool __result)
        //    {
        //        ///Edge case: new rocket on landing platform, as that is an entity, not a building
        //        ///
        //        //Debug.Log((existingModule != null) + "," + existingModule.TryGetComponent<RTB_LaunchPadWithoutLogic>(out _) + "," + existingModule.TryGetComponent<KBoxCollider2D>(out _) + ", " + existingModule.TryGetComponent<OccupyArea>(out _));
        //        if (existingModule != null
        //            && existingModule.TryGetComponent<RTB_LaunchPadWithoutLogic>(out _)
        //            && existingModule.TryGetComponent<KBoxCollider2D>(out var collider2D)
        //            && existingModule.TryGetComponent<OccupyArea>(out var occupyArea))
        //        {
        //            __result = true; return false;
        //        }
        //        return true;

        //    }

        //    internal static IEnumerable<MethodBase> TargetMethods()
        //    {
        //        List<Type> TargetTypes = new List<Type>();
        //        List<MethodBase> TargetMethods = new List<MethodBase>();
        //        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //        {
        //            var subclassTypes = assembly.GetTypes()
        //                .Where(t => t.IsSubclassOf(typeof(SelectModuleCondition)));
        //            TargetTypes.AddRange(subclassTypes);
        //        }
        //        foreach (var type in TargetTypes)
        //        {
        //            MethodInfo method = AccessTools.Method(type, "EvaluateCondition", new Type[] { typeof(GameObject), typeof(BuildingDef), typeof(SelectionContext) });

        //            if (method != null && !method.IsAbstract && !method.IsGenericMethod && method.HasMethodBody() && method.DeclaringType != typeof(IBuildingConfig))
        //            {
        //                TargetMethods.Add(method);
        //            }
        //        }
        //        SgtLogger.l("All Condition Methods acquired, Count: " + TargetMethods.Count());
        //        return TargetMethods;
        //    }

        //}

    }
}
