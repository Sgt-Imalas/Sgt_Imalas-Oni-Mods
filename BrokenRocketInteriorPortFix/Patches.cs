using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static BrokenRocketInteriorPortFix.ModAssets;

namespace BrokenRocketInteriorPortFix
{
    internal class Patches
    {
        /// <summary>
        /// for those that delete their interior doors lol
        /// </summary>
        [HarmonyPatch(typeof(ClustercraftExteriorDoor))]
        [HarmonyPatch(nameof(ClustercraftExteriorDoor.GetTargetWorld))]
        public static class TFW_Killed_interior_door
        {
            public static bool Prefix(ClustercraftExteriorDoor __instance, ref WorldContainer __result)
            {
                if (__instance.targetDoor == null)
                {
                    __instance.TryGetComponent<RocketModuleCluster>(out var moduleCluster);
                    moduleCluster.CraftInterface.TryGetComponent<WorldContainer>(out __result);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ClustercraftExteriorDoor))]
        [HarmonyPatch(nameof(ClustercraftExteriorDoor.TargetCell))]
        public static class TFW_Killed_interior_door_2
        {
            public static bool Prefix(ClustercraftExteriorDoor __instance, ref int __result)
            {
                if (__instance.targetDoor == null && __instance.TryGetComponent<RocketModuleCluster>(out var moduleCluster) && moduleCluster.CraftInterface.TryGetComponent<WorldContainer>(out var worldContainer))
                {
                    var pos = worldContainer.WorldOffset + new Vector2I((int)0.5f * worldContainer.Width, (int)0.5f * worldContainer.Height);
                    __result = Grid.XYToCell(pos.x, pos.y);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(RocketConduitSender))]
        [HarmonyPatch(nameof(RocketConduitSender.OnSpawn))]
        public static class TFW_Killed_interiorPorts
        {
            public static void Postfix(RocketConduitSender __instance)
            {
                if (__instance.partnerReceiver == null
                    )
                {
                    Debug.Log("Shutting Down Conduit State machine");
                    __instance.smi.StopSM("Someone deleted the interior partner");
                }
            }
        }
        [HarmonyPatch(typeof(RocketConduitSender))]
        [HarmonyPatch(nameof(RocketConduitSender.FindPartner))]
        public static class TFW_Killed_interiorPorts2
        {
            public static void Postfix(RocketConduitSender __instance)
            {
                if (__instance.partnerReceiver == null
                    )
                {
                    Debug.Log("Shutting Down Conduit State machine");
                    __instance.smi.StopSM("Someone deleted the interior partner");
                }
            }
        }


        [HarmonyPatch]
        public static class AddPortsToBuildMenu
        {
            [HarmonyPostfix]
            public static void Postfix(BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.CreateBuildingDef);
                yield return typeof(RocketInteriorGasInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorGasOutputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidOutputPortConfig).GetMethod(name);
                yield return typeof(RocketEnvelopeWindowTileConfig).GetMethod(name);
                yield return typeof(RocketWallTileConfig).GetMethod(name);
            }
        }
        [HarmonyPatch]
        public static class AddPortsToBuildMenu2
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                if(component.PrefabID().ToString().Contains("RocketInterior"))
                    component.AddTag(GameTags.UniquePerWorld);
                component.AddTag(GameTags.RocketInteriorBuilding);
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
                yield return typeof(RocketInteriorGasInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorGasOutputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidOutputPortConfig).GetMethod(name);
                yield return typeof(RocketEnvelopeWindowTileConfig).GetMethod(name);
                yield return typeof(RocketWallTileConfig).GetMethod(name);
            }
        }
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.MoveExistingBuildingToNewPlanscreen(GameStrings.PlanMenuCategory.Rocketry, RocketInteriorGasInputPortConfig.ID, "fittings",ordering: ModUtil.BuildingOrdering.Before);
                InjectionMethods.MoveExistingBuildingToNewPlanscreen(GameStrings.PlanMenuCategory.Rocketry, RocketInteriorGasOutputPortConfig.ID, "fittings", ordering: ModUtil.BuildingOrdering.Before);
                InjectionMethods.MoveExistingBuildingToNewPlanscreen(GameStrings.PlanMenuCategory.Rocketry, RocketInteriorLiquidInputPortConfig.ID, "fittings", ordering: ModUtil.BuildingOrdering.Before);
                InjectionMethods.MoveExistingBuildingToNewPlanscreen(GameStrings.PlanMenuCategory.Rocketry, RocketInteriorLiquidOutputPortConfig.ID, "fittings", ordering: ModUtil.BuildingOrdering.Before);
                InjectionMethods.MoveExistingBuildingToNewPlanscreen(GameStrings.PlanMenuCategory.Rocketry, RocketEnvelopeWindowTileConfig.ID, "fittings", ordering: ModUtil.BuildingOrdering.Before);
                InjectionMethods.MoveExistingBuildingToNewPlanscreen(GameStrings.PlanMenuCategory.Rocketry, RocketWallTileConfig.ID, "fittings", ordering: ModUtil.BuildingOrdering.Before);
            }
        }
    }
}
