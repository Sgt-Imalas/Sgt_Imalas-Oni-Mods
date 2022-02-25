using HarmonyLib;
using Klei.AI;
using KnastoronOniMods;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using TUNING;
using UnityEngine;
using UtilLibs;
using System.Linq;
using GMState = GameStateMachine<RocketControlStation.States, RocketControlStation.StatesInstance, RocketControlStation, object>.State;

namespace Robo_Rockets
{
    public class RoboRocketPatches
    {

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(RoboRocketConfig.ID, RoboRocketConfig.DisplayName, RoboRocketConfig.Description, RoboRocketConfig.Effect);
                InjectionMethods.AddBuildingStrings(RocketControlStationNoChorePreconditionConfig.ID, RocketControlStationNoChorePreconditionConfig.NAME, RocketControlStationNoChorePreconditionConfig.DESCR, RocketControlStationNoChorePreconditionConfig.EFFECT);
                RocketryUtils.AddRocketModuleToBuildList(RoboRocketConfig.ID, "HabitatModuleMedium");
            }
        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, RoboRocketConfig.ID);
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
        [HarmonyPatch("InitializeStates")]

        public class RocketControlStation_InitializeStates_Patch
        {
            public static void Postfix(RocketControlStation.States __instance
               , GMState ___operational
                , GMState ___running

                , GMState ___root
                )
            {
               // ___root.Update((smi, dt) => Debug.Log($"State is {smi.GetCurrentState().name}"));
                

                    ___running.QueueAnim("on", true);
                    ___operational.QueueAnim("on", true);

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
        //[HarmonyPatch(typeof(RocketControlStation))]
        //[HarmonyPatch("OnSpawn")]
        //public class RocketControlStation_SpawnBot_Patch
        //{
            //public static bool Prefix(RocketControlStation __instance)
            //{
            //    if (__instance.GetType() == typeof(RocketControlStationNoChorePrecondition))
            //    {

            //        var dis = __instance as RocketControlStationNoChorePrecondition; base.OnSpawn();
            //        dis.smi.StartSM();
            //        Components.RocketControlStations.Add(dis);
            //        dis.Subscribe<RocketControlStationNoChorePrecondition>(-801688580, RocketControlStationNoChorePrecondition.OnLogicValueChangedDelegate);
            //        dis.Subscribe<RocketControlStationNoChorePrecondition>(1861523068, RocketControlStationNoChorePrecondition.OnRocketRestrictionChanged);
            //        dis.MakeNewPilotBot();
            //        return false;
            //    }
            //    return true;
            //}

            //public static void Postfix(RocketControlStation __instance)
            //{
            //    if (__instance.GetType() == typeof(RocketControlStationNoChorePrecondition))
            //    {

            //        var dis = __instance as RocketControlStationNoChorePrecondition;
            //        dis.MakeNewPilotBot();
            //    }
            //}
        //}
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
                    //Debug.Log("skippingNormalSpeedSetter");
                    __instance.pilotSpeedMult = 1f;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ClusterManager))]
        [HarmonyPatch("CreateRocketInteriorWorld")]
        public class RocketControlStation_CreateRocketInteriorWorld_Patch
        {

            public static Vector2I ConditionForSize(string templateString)
            {
                if (templateString.Contains("robo"))
                        return new Vector2I(10, 10);

                return ROCKETRY.ROCKET_INTERIOR_SIZE;
            }

            private static readonly MethodInfo InteriorSizeHelper = AccessTools.Method(
               typeof(RocketControlStation_CreateRocketInteriorWorld_Patch),
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
                    code[insertionIndex] = new CodeInstruction(OpCodes.Ldarg_2);
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, InteriorSizeHelper));
                }
                return code.AsEnumerable();
            }
        }
    }
}
