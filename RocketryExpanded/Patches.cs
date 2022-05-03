using HarmonyLib;
using RocketryExpanded.buildings;
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
                if (__instance.master.PrefabID() == NuclearPulseEngineConfig.ID)
                {
                    __instance.master.GetComponent<ExhaustDispenser>().exhaustMethod(
                        dt, __instance,
                        (KBatchedAnimController)Traverse.Create(__instance.master).Field("animController").GetValue(),
                        (int)Traverse.Create(__instance).Field("pad_cell").GetValue());
                }
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(HabitatModuleLongConfig.ID, HabitatModuleLongConfig.DisplayName);
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleLongConfig.ID);
                InjectionMethods.AddBuildingStrings(NuclearPulseEngineConfig.ID, NuclearPulseEngineConfig.DisplayName);
                RocketryUtils.AddRocketModuleToBuildList(NuclearPulseEngineConfig.ID);
            }
        }
        [HarmonyPatch(typeof(ClusterManager))]
        [HarmonyDebug]
        [HarmonyPatch("CreateRocketInteriorWorld")]
        public class ClusterManager_PatchRocketInteriorWorldGen
        {
            public static void PrintInstructions(List<HarmonyLib.CodeInstruction> codes)
            {
                Debug.Log("\n");
                for (int i = 0; i < codes.Count; i++)
                {
                    Debug.Log(i + ": " + codes[i]);
                }
            }

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

                PrintInstructions(code);
                return code;
            }
        }
    }
}
