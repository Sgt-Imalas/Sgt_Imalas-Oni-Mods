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
            //interiorTemplateName.Contains("robo") ? new Vector2I(4,4): ROCKETRY.ROCKET_INTERIOR_SIZE;
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();

                int insertionIndex = -1;
                for (int i = 0; i < code.Count - 1; i++) // -1 since we will be checking i + 1
                {
                    if (code[i].opcode == OpCodes.Ldsfld && (Vector2I)code[i].operand == TUNING.ROCKETRY.ROCKET_INTERIOR_SIZE && code[i + 1].opcode == OpCodes.Stloc_1)
                    {
                        insertionIndex = i;
                        break;
                    }
                }
                code[insertionIndex].opcode = OpCodes.Nop;
                code[insertionIndex+1].opcode = OpCodes.Nop;

                var instructionsToInsert = new List<CodeInstruction>();

                Label trueLabel = il.DefineLabel();
                Label falseLabel = il.DefineLabel();

                //                                  IL_0014: ldarg.2
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_2));
                //                                  IL_0015: ldstr     "robo"
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldstr, "robo"));
                //                                  IL_001A: callvirt instance bool[mscorlib] System.String::Contains(string)
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string), "Contains" , new Type[] { typeof(string) })));
                //                                  IL_001F: brtrue.s IL_0028
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brtrue_S, trueLabel));
                //                                  IL_0021: ldsfld valuetype['Assembly-CSharp-firstpass']Vector2I TUNING.ROCKETRY::ROCKET_INTERIOR_SIZE
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldsfld,AccessTools.Field(typeof(Vector2I), nameof(ROCKETRY.ROCKET_INTERIOR_SIZE))));
                //                                  IL_0026: br.s IL_002F
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Br_S, falseLabel));
                //                                  IL_0028: ldc.i4.4
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_4));
                instructionsToInsert.Last().labels.Add(trueLabel);
                //                                  IL_0029: ldc.i4.4
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_4));
                //                                  IL_002A: newobj instance void ['Assembly-CSharp-firstpass']Vector2I::.ctor(int32, int32)
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Newobj,AccessTools.Method(typeof(Vector2I),"ctor", new Type[] { typeof(Int32), typeof(Int32) })));
                //                                  IL_002F: stloc.1
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc_1));
                instructionsToInsert.Last().labels.Add(falseLabel);
                //////                                  IL_0030: ldloc.1
                //instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_1));

                if (insertionIndex != -1)
                {
                    code.InsertRange(insertionIndex, instructionsToInsert);
                }

                return code.AsEnumerable();
            }
        }
        ////As long as I dont understand Transpiling...
        //[HarmonyPatch(typeof(ClusterManager))]
        //[HarmonyPatch("CreateRocketInteriorWorld")]
        //public class RocketControlStation_CreateRocketInteriorWorld_Patch
        //{
        //    public static bool Prefix(GameObject craft_go, string interiorTemplateName, System.Action callback, ClusterManager __instance)
        //        {
        //            Vector2I rocketInteriorSize = interiorTemplateName.Contains("robo") ?new Vector2I(4,4): ROCKETRY.ROCKET_INTERIOR_SIZE;
        //            Vector2I offset;
        //            if (Grid.GetFreeGridSpace(rocketInteriorSize, out offset))
        //            {
        //                int nextWorldId = __instance.GetNextWorldId();
        //                craft_go.AddComponent<WorldInventory>();
        //                WorldContainer rocketInteriorWorld = craft_go.AddComponent<WorldContainer>();
        //                rocketInteriorWorld.SetRocketInteriorWorldDetails(nextWorldId, rocketInteriorSize, offset);
        //                Vector2I vector2I = offset + rocketInteriorSize;
        //                for (int y = offset.y; y < vector2I.y; ++y)
        //                {
        //                    for (int x = offset.x; x < vector2I.x; ++x)
        //                    {
        //                        int cell = Grid.XYToCell(x, y);
        //                        Grid.WorldIdx[cell] = (byte)nextWorldId;
        //                        Pathfinding.Instance.AddDirtyNavGridCell(cell);
        //                    }
        //                }
        //                Debug.Log((object)string.Format("Created new rocket interior id: {0}, at {1} with size {2}", (object)nextWorldId, (object)offset, (object)rocketInteriorSize));
        //                rocketInteriorWorld.PlaceInteriorTemplate(interiorTemplateName, (System.Action)(() =>
        //                {
        //                    if (callback != null)
        //                        callback();
        //                    craft_go.GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.RocketInteriorComplete, (object)null);
        //                }));
        //                craft_go.AddOrGet<OrbitalMechanics>().CreateOrbitalObject(Db.Get().OrbitalTypeCategories.landed.Id);
        //                __instance.Trigger(-1280433810, (object)rocketInteriorWorld.id);
        //                return rocketInteriorWorld;
        //            }
        //            Debug.LogError((object)"Failed to create rocket interior.");
        //            return (WorldContainer)null;
        //        }
                
            
        //}

    }

}
