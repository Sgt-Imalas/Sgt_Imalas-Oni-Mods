using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static BetterCritterMorpher.ModAssets;
using static FertilityMonitor;
using static STRINGS.CREATURES.MODIFIERS;
using static EggProtectionMonitor.Instance;

namespace BetterCritterMorpher
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GravitasCreatureManipulator.Instance))]
        [HarmonyPatch(nameof(GravitasCreatureManipulator.Instance.SpawnMorph))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix(Brain brain)
            {
                SgtLogger.l(brain.gameObject.name, "Critter To Morph found");
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
            }
        }
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        [HarmonyPatch(typeof(GravitasCreatureManipulator.Instance))]
        [HarmonyPatch(nameof(GravitasCreatureManipulator.Instance.DropCritter))]
        public static class AllowMorphingAnyCritter
        {
            public static bool HasTagAlways(Tag tag)
            {
                return true;
            }

            private static readonly MethodInfo AllowAllCritterTag = AccessTools.Method(
               typeof(AllowMorphingAnyCritter),
               nameof(HasTagAlways)
            );

            private static readonly MethodInfo TargetMethod = AccessTools.Method(
                    typeof(KPrefabIDExtensions),
                    nameof(KPrefabIDExtensions.HasTag),
                    new Type[] { typeof(Component), typeof(Tag) });

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();

                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == TargetMethod);

                if (insertionIndex != -1)
                {
                    SgtLogger.l("Critter Tag found");
                    code[insertionIndex] = new CodeInstruction(OpCodes.Call, AllowAllCritterTag);
                    
                }

                return code;
            }
        }
        [HarmonyPatch(typeof(GravitasCreatureManipulator.Instance))]
        [HarmonyPatch(nameof(GravitasCreatureManipulator.Instance.SpawnMorph))]
        public static class AllowMorphingAnyCritter_vol2
        {
            static Brain brainRef;
            public static void Prefix(Brain brain)
            {
                brainRef = brain;
            }

            public static Tag GetEggRollWithoutCurrentCritter(List<BreedingChance> breedingChances, int excludeOriginalCreature
                //, Brain brain
                )
            {

                BabyMonitor.Instance smi1 = brainRef.GetSMI<BabyMonitor.Instance>();
                FertilityMonitor.Instance smi2 = brainRef.GetSMI<FertilityMonitor.Instance>();

                var list = new List<BreedingChance>();

                float totalChance = 0f;
                foreach (var change in breedingChances)
                {
                    if (change.egg.IsValid)
                    {
                        var CritterFromEgg = Assets.GetPrefab(change.egg).GetDef<IncubationMonitor.Def>().spawnedCreature;
                        Tag CompareCritter;

                        if(smi1!= null)
                        {
                            CompareCritter = CritterFromEgg;
                        }
                        else
                        {
                            var Baby = Assets.GetPrefab(CritterFromEgg).GetDef<BabyMonitor.Def>();
                            CompareCritter = Baby.adultPrefab;
                        }

                        SgtLogger.l(CompareCritter.ToString());
                        if (brainRef.gameObject.name != CompareCritter.ToString())
                        {
                            var newChance = new BreedingChance();
                            newChance.weight = change.weight;
                            newChance.egg = change.egg;
                            //SgtLogger.l("added to rolls");
                            list.Add(newChance);
                            totalChance += change.weight;
                        }
                    }
                }
                var multiplier = 100f / totalChance;
                foreach(var change in list)
                {
                    change.weight *= multiplier;
                }

                return EggBreedingRoll(list, false);
            }

            private static readonly MethodInfo RollEgg = AccessTools.Method(
               typeof(AllowMorphingAnyCritter_vol2),
               nameof(GetEggRollWithoutCurrentCritter)
            );

            private static readonly MethodInfo TargetMethod = AccessTools.Method(
                    typeof(FertilityMonitor),
                    nameof(FertilityMonitor.EggBreedingRoll));

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();

                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == TargetMethod);
                var insertionIndex2 = code.FindLastIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == TargetMethod);

                

                if (insertionIndex != -1)
                {
                    code[insertionIndex] = new CodeInstruction(OpCodes.Call, RollEgg);
                    //code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_1));

                }
                if (insertionIndex2 != -1)
                {
                    code[insertionIndex2] = new CodeInstruction(OpCodes.Call, RollEgg);
                    //code.Insert(insertionIndex2, new CodeInstruction(OpCodes.Ldarg_1));
                }
                

                return code;
            }
        }
    }
}
