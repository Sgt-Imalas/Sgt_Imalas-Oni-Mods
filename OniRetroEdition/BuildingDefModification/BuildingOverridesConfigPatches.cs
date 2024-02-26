using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static KAnim;

namespace OniRetroEdition.BuildingDefModification
{

    /// <summary>
    /// Override Building Props
    /// </summary>
    internal class BuildingOverridesConfigPatches
    {
        //[HarmonyPrefix]
        //public static void AddBuildingDef_Prefix(BuildingDef def)
        //{
        //    AddLogic.TryAddLogic(def);
        //}

        [HarmonyPatch(typeof(KAnimGroupFile), "Load")]
        public class KAnimGroupFile_Load_Patch
        {
            public static void Prefix(KAnimGroupFile __instance)
            {
                var interacts = new HashSet<HashedString>();
                BuildingModifications.Instance.LoadedBuildingOverrides.Values.ToList().ForEach(item =>
                {
                    if (item.workableAnimOverride != null && item.workableAnimOverride.Length > 0)
                    {
                        interacts.Add(item.workableAnimOverride);
                    }
                });


                InjectionMethods.RegisterCustomInteractAnim(
                    __instance, interacts);
            }
        }


        [HarmonyPatch(typeof(Workable), nameof(Workable.OnSpawn))]
        public static class Workable_OnSpawnAnimOverride
        {
            private static void Prefix(Workable __instance)
            {
                if (__instance.TryGetComponent<KPrefabID>(out var kPrefabID))
                {
                    //SgtLogger.l("Testing any overrides for " + kPrefabID.PrefabID());
                    if (!BuildingModifications.Instance.LoadedBuildingOverrides.ContainsKey(kPrefabID.PrefabID().ToString()))
                    {
                        //SgtLogger.l("no anim override for this building found..");
                        return;
                    }
                    BuildingModification overrideParams = BuildingModifications.Instance.LoadedBuildingOverrides[kPrefabID.PrefabID().ToString()];

                    SgtLogger.l("building override config found!");


                    if (overrideParams.requiresMinionWorker.HasValue)
                    {
                        __instance.requireMinionToWork = overrideParams.requiresMinionWorker.Value;
                    }
                    if (overrideParams.workableAnimOverride != null && overrideParams.workableAnimOverride.Length > 0)
                    {
                        SgtLogger.l("anim override config found, name of the override anim: " + overrideParams.workableAnimOverride);
                        var anim = Assets.GetAnim(overrideParams.workableAnimOverride);
                        if (anim != null)
                        {
                            __instance.overrideAnims = new KAnimFile[] { anim };
                        }
                        else
                            SgtLogger.error($"WorkingOverride Animfile {overrideParams.workableAnimOverride} not found!");
                    }


                }
            }
        }



        [HarmonyPatch(typeof(BuildingConfigManager), "RegisterBuilding")]
        public class BuildingConfigManager_RegisterBuilding_Patch
        {
            private static readonly MethodInfo BuildingDefTargetMethod = AccessTools.Method(
                    typeof(IBuildingConfig),
                    nameof(IBuildingConfig.CreateBuildingDef));


            private static readonly MethodInfo PostConfigureCompleteTargetMethod = AccessTools.Method(
                    typeof(IBuildingConfig),
                    nameof(IBuildingConfig.DoPostConfigureComplete));

            private static readonly MethodInfo LocalGOTargetMethod = AccessTools.Method(
                    typeof(UnityEngine.Object),
                    nameof(UnityEngine.Object.DontDestroyOnLoad));



            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var indexCreateDef = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == BuildingDefTargetMethod);
                var loc_BuildingDef_Index = TranspilerHelper.FindIndexOfNextLocalIndex(codes, indexCreateDef, false);

                var index2 = codes.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == LocalGOTargetMethod);
                var loc_GO_Index = TranspilerHelper.FindIndexOfNextLocalIndex(codes, index2);


                if (indexCreateDef == -1)
                {
                    return codes;
                }

                var m_buildingdef_postfix = AccessTools.DeclaredMethod(typeof(BuildingConfigManager_RegisterBuilding_Patch), "CreateBuildingDef_Postfix");
                var m_postconfigurecomplete_postfix = AccessTools.DeclaredMethod(typeof(BuildingConfigManager_RegisterBuilding_Patch), "DoPostConfigureComplete_Postfix");

                codes.InsertRange(indexCreateDef + 1, new[]
                {
                    new CodeInstruction(OpCodes.Call, m_buildingdef_postfix)
                        });



                var indexPostConfigComplete = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == PostConfigureCompleteTargetMethod);
                indexPostConfigComplete++;
                codes.InsertRange(indexPostConfigComplete, new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_S,loc_BuildingDef_Index),
                    new CodeInstruction(OpCodes.Ldloc_S,loc_GO_Index),
                    new CodeInstruction(OpCodes.Call, m_postconfigurecomplete_postfix)
                        });

                TranspilerHelper.PrintInstructions(codes);

                return codes;
            }

            private static BuildingDef CreateBuildingDef_Postfix(BuildingDef def)
            {
                if (SkinsAdder.Instance.TargetIDWithAnimnameForSoundCopy.ContainsKey(def.PrefabID))
                {
                    var sourceanim = def.AnimFiles.FirstOrDefault();
                    foreach (var targetAnim in SkinsAdder.Instance.TargetIDWithAnimnameForSoundCopy[def.PrefabID])
                    {
                        if (!Assets.TryGetAnim(targetAnim, out _))
                        {
                            SgtLogger.warning(targetAnim + " coulnt be found for sound copying");
                            continue;
                        }

                        SoundUtils.CopySoundsToAnim(targetAnim, sourceanim.name);
                    }

                }


                if (BuildingModifications.Instance.LoadedBuildingOverrides.ContainsKey(def.PrefabID))
                {
                    SgtLogger.l($"Applying Settings Override for {def.PrefabID}.");

                    def.ShowInBuildMenu = true;
                    def.Deprecated = false;

                    BuildingModification overrideParams = BuildingModifications.Instance.LoadedBuildingOverrides[def.PrefabID];
                    bool sizeChanged = false;
                    if (overrideParams.HeightOverride.HasValue)
                    {
                        sizeChanged = true;
                        SgtLogger.l(def.HeightInCells + " to " + overrideParams.HeightOverride, "Changing Height");
                        def.HeightInCells = overrideParams.HeightOverride.Value;
                    }
                    if (overrideParams.WidthOverride.HasValue)
                    {
                        sizeChanged = true;

                        SgtLogger.l(def.WidthInCells + " to " + overrideParams.WidthOverride, "Changing Width");
                        def.WidthInCells = overrideParams.WidthOverride.Value;
                    }
                    if (sizeChanged)
                    {
                        def.GenerateOffsets();
                    }
                    if (overrideParams.foundationFloorTile.HasValue && overrideParams.foundationFloorTile.Value)
                    {
                        BuildingTemplates.CreateFoundationTileDef(def);
                    }
                    if (overrideParams.UtilityInputOffsetOverride.HasValue)
                    {
                        def.UtilityInputOffset = overrideParams.UtilityInputOffsetOverride.Value;
                    }
                    if (overrideParams.UtilityOutputOffsetOverride.HasValue)
                    {
                        def.UtilityOutputOffset = overrideParams.UtilityOutputOffsetOverride.Value;
                    }
                    if (overrideParams.PowerInputOffsetOverride.HasValue)
                    {
                        def.PowerInputOffset = overrideParams.PowerInputOffsetOverride.Value;
                    }



                    if (overrideParams.animOverride != null && overrideParams.animOverride.Length > 0)
                    {
                        var anim = Assets.GetAnim(overrideParams.animOverride);
                        if (anim != null)
                        {
                            def.AnimFiles = new KAnimFile[] { anim };
                        }
                        else
                            SgtLogger.warning($"Animfile {overrideParams.animOverride} not found!");
                    }
                }
                return def;
            }
            private static void DoPostConfigureComplete_Postfix(BuildingDef def, GameObject go)
            {
                if (def.BaseNoisePollution > 0)
                {
                    NoisePolluter polluter = go.AddOrGet<NoisePolluter>();
                    polluter.radius = def.BaseNoisePollutionRadius;
                    polluter.noise = def.BaseNoisePollution;
                }


                if (BuildingModifications.Instance.LoadedBuildingOverrides.ContainsKey(def.PrefabID))
                {
                    BuildingModification overrideParams = BuildingModifications.Instance.LoadedBuildingOverrides[def.PrefabID];

                    if (overrideParams.foundationFloorTile.HasValue && overrideParams.foundationFloorTile.Value)
                    {
                        KPrefabID component = go.GetComponent<KPrefabID>();
                        component.AddTag(GameTags.FloorTiles);
                    }

                    //if (go.TryGetComponent<Workable>(out var workable))
                    //{
                    //    if (overrideParams.requiresMinionWorker.HasValue)
                    //    {
                    //        workable.requireMinionToWork = overrideParams.requiresMinionWorker.Value;
                    //    }
                    //    if (overrideParams.workableAnimOverride != null && overrideParams.workableAnimOverride.Length > 0)
                    //    {
                    //        var anim = Assets.GetAnim(overrideParams.workableAnimOverride);
                    //        if (anim != null)
                    //        {
                    //            workable.overrideAnims = new KAnimFile[] { anim };
                    //        }
                    //        else
                    //            SgtLogger.error($"WorkingOverride Animfile {overrideParams.workableAnimOverride} not found!");
                    //    }
                    //}

                }
            }
        }
    }
}
