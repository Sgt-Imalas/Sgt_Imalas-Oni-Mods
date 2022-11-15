using HarmonyLib;
using Klei.AI;
using KnastoronOniMods;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using TUNING;
using UnityEngine;
using UtilLibs;
using System.Linq;
using GMState = GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State;
using RoboRockets;

namespace RoboRockets
{
    public class RoboRocketPatches
    {
        [HarmonyPatch(typeof(GeneShufflerRechargeConfig))]
        [HarmonyPatch(nameof(GeneShufflerRechargeConfig.CreatePrefab))]
        public static class AddTagToItem
        {
            public static void Postfix(ref GameObject __result)
            {
                var prefab = __result.GetComponent<KPrefabID>();
                prefab.AddTag(GeneShufflerRechargeConfig.tag);
            }
        }

        [HarmonyPatch(typeof(LimitOneCommandModule))]
        [HarmonyPatch(nameof(LimitOneCommandModule.EvaluateCondition))]
        public static class LimitOneCommandModule_AI_Patch
        {
            public static bool Prefix( bool __result,
                GameObject existingModule,
                SelectModuleCondition.SelectionContext selectionContext)
            {

                if ((Object)existingModule == (Object)null)
                {
                    __result = true;
                    return false;
                }
                    

                foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(existingModule.GetComponent<AttachableBuilding>()))
                {
                    if (
                        (selectionContext != SelectModuleCondition.SelectionContext.ReplaceModule ||  !((Object)gameObject == (Object)existingModule.gameObject))
                        && (
                        (Object)gameObject.GetComponent<RocketCommandConditions>() != (Object)null ||
                        (Object)gameObject.GetComponent<RocketAiConditions>() != (Object)null ||
                        (Object)gameObject.GetComponent<BuildingUnderConstruction>() != (Object)null && (Object)gameObject.GetComponent<BuildingUnderConstruction>().Def.BuildingComplete.GetComponent<RocketCommandConditions>() != (Object)null ||
                        (Object)gameObject.GetComponent<BuildingUnderConstruction>() != (Object)null && (Object)gameObject.GetComponent<BuildingUnderConstruction>().Def.BuildingComplete.GetComponent<RocketAiConditions>() != (Object)null
                        )
                     )
                    {
                        __result = false;
                        return false;
                    }
                }
                return true;
            }
        }


        [HarmonyPatch(typeof(PassengerRocketModule))]
        [HarmonyPatch("CheckPassengersBoarded")]
        public class PassengerRocketModule_CheckPassengersBoarded_Patch
        {
            public static void Postfix(PassengerRocketModule __instance, ref bool __result)
            {
                if (__instance.GetType() == typeof(AIPassengerModule))
                {
                    __result = true;
                }
            }
        }

        [HarmonyPatch(typeof(PassengerRocketModule))]
        [HarmonyPatch("RefreshAccessStatus")]
        public class PassengerRocketModule_RefreshAccessStatus_Patch
        {
            public static bool Prefix(PassengerRocketModule __instance)
            {
                if (__instance.GetType() == typeof(AIPassengerModule))
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PassengerRocketModule))]
        [HarmonyPatch("CheckPilotBoarded")]
        public class PassengerRocketModule_CheckPilotBoarded_Patch
        {
            public static void Postfix(PassengerRocketModule __instance, ref bool __result)
            {
                if (__instance.GetType() == typeof(AIPassengerModule))
                {
                    __result = true;
                }
            }
        }

        [HarmonyPatch(typeof(RocketControlStation.States))]
        [HarmonyPatch("CreateChore")]
        public class RocketControlStation_CreateChore_Patch
        {
            public static void Postfix(RocketControlStation.StatesInstance smi, ref Chore __result)
            {
                if (smi.master.GetType() == typeof(RocketControlStationNoChorePrecondition))
                {
                    Workable component = (Workable)smi.master.GetComponent<RocketControlStationLaunchWorkable>();
                    WorkChore<RocketControlStationIdleWorkable> chore =
                        new WorkChore<RocketControlStationIdleWorkable>(Db.Get().ChoreTypes.RocketControl,
                        (IStateMachineTarget)component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Work,
                        allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
                    chore.AddPrecondition(ChorePreconditions.instance.ConsumerHasTrait, AiBrainConfig.ROVER_BASE_TRAIT_ID);
                    __result = (Chore)chore;
                }
            }
        }
       
        [HarmonyPatch(typeof(RocketControlStation.States))]
        [HarmonyPatch("CreateLaunchChore")]
        public class RocketControlStation_CreateLaunchChore_Patch
        {
            public static void Postfix(RocketControlStation.StatesInstance smi, ref Chore __result)
            {
                if (smi.master.GetType() == typeof(RocketControlStationNoChorePrecondition))
                {
                    Workable component = (Workable)smi.master.GetComponent<RocketControlStationLaunchWorkable>();
                    WorkChore<RocketControlStationLaunchWorkable> launchChore = 
                        new WorkChore<RocketControlStationLaunchWorkable>(Db.Get().ChoreTypes.RocketControl, 
                        (IStateMachineTarget)component, ignore_schedule_block: true, allow_prioritization:
                        false, priority_class: PriorityScreen.PriorityClass.topPriority);
                    launchChore.AddPrecondition(ChorePreconditions.instance.ConsumerHasTrait, AiBrainConfig.ROVER_BASE_TRAIT_ID);
                    __result = (Chore)launchChore;
                }
            }
        }
        [HarmonyPatch(typeof(RocketControlStation.StatesInstance))]
        [HarmonyPatch("SetPilotSpeedMult")]
        public class RocketControlStation_SetPilotSpeedMult_Patch
        {
            public static bool Prefix(Worker pilot, RocketControlStation.StatesInstance __instance)
            {
                AttributeConverter pilotingSpeed = Db.Get().AttributeConverters.PilotingSpeed;
                if (pilot.GetComponent<AttributeConverters>().GetConverter(pilotingSpeed.Id) == null)
                {
                    Debug.Log("skippingNormalSpeedSetter in Legacy AI Rocket");
                    __instance.pilotSpeedMult = Config.Instance.NoBrainRockets;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HabitatModuleSideScreen))]
        [HarmonyPatch("RefreshModulePanel")]
        public class DisableViewInteriorSpace_Patch
        {
            public static void Postfix(PassengerRocketModule module, HabitatModuleSideScreen __instance)
            {
                if (module.gameObject.GetComponent<AIPassengerModule>() == null)
                {
                    return;
                }

                HierarchyReferences component = __instance.GetComponent<HierarchyReferences>();
                KButton reference = component.GetReference<KButton>("button");
                reference.ClearOnClick();
                reference.isInteractable = false;
            }
        }

        [HarmonyPatch(typeof(CameraController))]
        [HarmonyPatch(nameof(CameraController.ActiveWorldStarWipe))]
        [HarmonyPatch(new System.Type[] { typeof(int), typeof(System.Action) })]
        public class DisableViewInteriorWorldSelector_Patch
        {
            public static bool Prefix(int id)
            {
                if (ModAssets.ForbiddenInteriorIDs.Contains(id))
                {

                    Debug.Log("WorldID is forbidden to look into: " + id);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ClustercraftExteriorDoor))]
        [HarmonyPatch("OnSpawn")]
        public class AddInteriorToForbiddenListIfAI
        {
            public static void Postfix(ClustercraftExteriorDoor __instance)
            {
                if (__instance.gameObject.TryGetComponent<AIPassengerModule>(out var aiModue))
                {
                    int worldRefID = (int)typeof(ClustercraftExteriorDoor).GetField("targetWorldId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                    Clustercraft component = __instance.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
                    if(!aiModue.variableSpeed)
                    { 
#if DEBUG
                    Debug.Log("AI Module added; adjusting automated Speed to " + Config.Instance.NoBrainRockets);
#endif
                    component.AutoPilotMultiplier = Config.Instance.NoBrainRockets;
                }
                    Debug.Log("World forbidden to look into: " + worldRefID);
                    ModAssets.ForbiddenInteriorIDs.Add(worldRefID);
                }
            }
        }
        [HarmonyPatch(typeof(ClustercraftExteriorDoor))]
        [HarmonyPatch("OnCleanUp")]
        public class RemoveInteriorFromForbiddenListIfAI
        {
            public static void Prefix(ClustercraftExteriorDoor __instance)
            {
                if (__instance.gameObject.GetComponent<AIPassengerModule>() != null)
                {
                    int worldRefID = (int)typeof(ClustercraftExteriorDoor).GetField("targetWorldId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                    ModAssets.ForbiddenInteriorIDs.Remove(worldRefID);
                }
            }
        }

        [HarmonyPatch(typeof(ClustercraftExteriorDoor))]
        [HarmonyPatch(nameof(ClustercraftExteriorDoor.HasTargetWorld))]
        public class DisableViewInterior_Patch
        {
            public static void Postfix(ClustercraftExteriorDoor __instance, ref bool __result)
            {
                if (__instance.gameObject.GetComponent<AIPassengerModule>() != null)
                {
                    __result = false;
                }

            }
        }

        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch("RequestLaunch")]
        public class TriggerLaunchForAIRocketsPatch
        {
            public static void Prefix(Clustercraft __instance)
            {
                bool isAiRocket = false;
                if (__instance == null)
                    return;
                foreach (Ref<RocketModuleCluster> clusterModule in __instance.ModuleInterface.ClusterModules)
                {
                    var isAI = clusterModule.Get().GetComponent<AIPassengerModule>();
                    if (isAI != null)
                        isAiRocket = true;
                }
                if (isAiRocket)
                {
                    __instance.Launch();
                }

            }
        }


        /// <summary>
        /// Set interior size to bare minimum
        /// </summary>
        [HarmonyPatch(typeof(ClusterManager))]
        [HarmonyPatch("CreateRocketInteriorWorld")]
        public class ClusterManager_CreateRocketInteriorWorld_Patch
        {
            
            public static Vector2I ConditionForSize(Vector2I original, string templateString)
            {
                if (templateString.Contains("habitat_robo"))
                    original = new Vector2I(8, 8);

                else if (templateString.Contains("AIRocketV2"))
                    original = new Vector2I(1, 2);

                return original;
            }

            private static readonly MethodInfo InteriorSizeHelper = AccessTools.Method(
               typeof(ClusterManager_CreateRocketInteriorWorld_Patch),
               nameof(ConditionForSize)
            );


            private static readonly FieldInfo SizeFieldInfo = AccessTools.Field(
                typeof(TUNING.ROCKETRY),
                "ROCKET_INTERIOR_SIZE"
            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.operand is FieldInfo f && f == SizeFieldInfo);

                if (insertionIndex != -1)
                {
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldarg_2));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, InteriorSizeHelper));
                }
               
                return code;
            }
        }
    }
}
