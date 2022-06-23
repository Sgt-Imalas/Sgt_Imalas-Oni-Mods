using HarmonyLib;
using RocketryExpanded.buildings;
using RocketryExpanded.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RocketryExpanded
{
    class Patches
    {
        [HarmonyPatch(typeof(RocketEngineCluster.StatesInstance))]
        [HarmonyPatch(nameof(RocketEngineCluster.StatesInstance.DoBurn))]

        public static class AddSomeNukesInThere
        {
            public static void Postfix(float dt, RocketEngineCluster.StatesInstance __instance)
            {
                return;
                if (__instance.master.PrefabID() == NuclearPulseEngineConfig.ID)
                {
                    __instance.master.GetComponent<ExhaustDispenser>().exhaustMethod(
                        dt, __instance,
                        (KBatchedAnimController)Traverse.Create(__instance.master).Field("animController").GetValue(),
                        (int)Traverse.Create(__instance).Field("pad_cell").GetValue());
                }
            }
        }

        [HarmonyPatch(typeof(LaunchableRocketCluster.StatesInstance))]
        [HarmonyPatch(nameof(LaunchableRocketCluster.StatesInstance.SetupLaunch))]
        public static class NukeRocketGoesBrr
        {
            public static void Postfix(LaunchableRocketCluster.StatesInstance __instance)
            {
                if (__instance.master.GetEngines().First().PrefabID() == NuclearPulseEngineConfig.ID)
                {
                    Debug.Log("Nuclear Launch initiated");
                    //var bomblet = Util.KInstantiate(Assets.GetPrefab((Tag)BombletNuclearConfig.ID), __instance.master.transform.position, Quaternion.identity);
                    //bomblet.SetActive(true);
                    //bomblet.GetComponent<ExplosiveBomblet>().Detonate(5f);
                }
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(BombBuildingStationConfig.ID, BombBuildingStationConfig.NAME);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Radiation, BombBuildingStationConfig.ID);

                InjectionMethods.AddBuildingStrings(PlacableExplosiveConfig.ID, PlacableExplosiveConfig.NAME);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, PlacableExplosiveConfig.ID);

                InjectionMethods.AddBuildingStrings(HabitatModuleLongConfig.ID, HabitatModuleLongConfig.DisplayName);
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleLongConfig.ID);

                InjectionMethods.AddBuildingStrings(NuclearPulseEngineConfig.ID, NuclearPulseEngineConfig.DisplayName);
                RocketryUtils.AddRocketModuleToBuildList(NuclearPulseEngineConfig.ID);
            }
        }

        [HarmonyPatch(typeof(SolidConduitDispenser))]
        [HarmonyPatch("ConduitUpdate")]
        public class ConduitDispenserImplementOneElementTag
        {
            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(Pickupable),
                   "get_PrimaryElement"
               );
            public static Pickupable CheckForTag(Pickupable original)
            {
                Debug.Log(original.KPrefabID);
                if (original.KPrefabID.HasTag(ModAssets.Tags.SplitOnRail))
                {
                    original = original.Take(1f);
                }
                return original;
            }

            private static readonly MethodInfo PacketSizeHelper = AccessTools.Method(
               typeof(ConduitDispenserImplementOneElementTag),
               nameof(CheckForTag)
            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();

                var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);

                if (insertionIndex != -1)
                {
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Call, PacketSizeHelper));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Stloc_2));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_2));
                }
               // Debug.Log("DEBUGMETHOD: " + new CodeInstruction(OpCodes.Call, PacketSizeHelper));
                InjectionMethods.PrintInstructions(code);
                return code;
            }
        }

        [HarmonyPatch(typeof(ClusterManager))]
        [HarmonyPatch("CreateRocketInteriorWorld")]
        public class ClusterManager_PatchRocketInteriorWorldGen
        {

            public static Vector2I ConditionForSize(Vector2I original,string templateString)
            {
                if (templateString.Contains("habitat_long"))
                    original = new Vector2I(32, 44);
                return original;
            }

            private static readonly MethodInfo InteriorSizeHelper = AccessTools.Method(
               typeof(ClusterManager_PatchRocketInteriorWorldGen),
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
