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

namespace Robo_Rockets
{
    public class RoboRocketPatches
    {
        //[HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        //public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        //{
        //    public static void Postfix(Dictionary<string, CodexEntry> __result)
        //    {
        //        InjectionMethods.AddRobotStrings(AiBrainConfig.ID, AiBrainConfig.NAME, AiBrainConfig.DESCR);
        //        InjectionMethods.Action(AiBrainConfig.ID, AiBrainConfig.NAME, __result);
        //    }
        //}

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(RoboRocketConfig.ID, RoboRocketConfig.DisplayName, RoboRocketConfig.Description, RoboRocketConfig.Effect);
                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, RoboRocketConfig.ID);
                
                InjectionMethods.AddBuildingStrings(RocketAiControlstationConfig.ID, "Ai core");
                InjectionMethods.AddBuildingStrings(RocketControlStationNoChorePreconditionConfig.ID, "Rocket Control Station (automated)");
                InjectionMethods.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Rocketry, RocketAiControlstationConfig.ID);
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

        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch("OnSpawn")]
        public class SelectModuleSideScreen_OnSpawn_Patch
        {

            public static void Prefix()
            {
                int i = SelectModuleSideScreen.moduleButtonSortOrder.IndexOf("HabitatModuleMedium");
                int j = (i == -1) ? SelectModuleSideScreen.moduleButtonSortOrder.Count : ++i;
                SelectModuleSideScreen.moduleButtonSortOrder.Insert(j, RoboRocketConfig.ID);
            }
        }

        [HarmonyPatch(typeof(PassengerRocketModule))]
        [HarmonyPatch("CheckPassengersBoarded")]
        public class PassengerRocketModule_CheckPassengersBoarded_Patch
        {
            public static void Postfix(PassengerRocketModule __instance, ref bool __result)
            {
                if (__instance.GetType()== typeof(AIPassengerModule))
                {
                    __result = true;
                }
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
                if(smi.master.GetType() == typeof(RocketControlStationNoChorePrecondition))
                {
                    Workable component = (Workable)smi.master.GetComponent<RocketControlStationLaunchWorkable>();
                    WorkChore<RocketControlStationIdleWorkable> chore = new WorkChore<RocketControlStationIdleWorkable>(Db.Get().ChoreTypes.RocketControl, (IStateMachineTarget)component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Work, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
                    __result = (Chore)chore;
                    Debug.Log("Patching of Chore Method successful");
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
                    Debug.Log("Patching of LaunchChore Method successful");
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
                    Debug.Log("skippingNormalSpeedSetter");
                    __instance.pilotSpeedMult = 100f;
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
                        return new Vector2I(4, 6);

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

            //interiorTemplateName.Contains("robo") ? new Vector2I(4,4): ROCKETRY.ROCKET_INTERIOR_SIZE;
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();

                //int insertionIndex = -1;
                //foreach(var v in code)
                //{
                //    Debug.Log(v.opcode + " <<->>" + v.operand);
                //}
                var insertionIndex = code.FindIndex(ci => ci.operand is FieldInfo f && f == SizeFieldInfo);
               

                if (insertionIndex != -1)
                {
                    code[insertionIndex++] = new CodeInstruction(OpCodes.Ldarg_2);
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Call, InteriorSizeHelper));
                }
                return code.AsEnumerable();
            }
        }

    }

}
