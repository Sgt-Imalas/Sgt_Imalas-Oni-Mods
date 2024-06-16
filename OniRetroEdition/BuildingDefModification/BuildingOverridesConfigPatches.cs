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
        //}CreatureDeliveryPointConfig
        [HarmonyPatch(typeof(CreatureDeliveryPointConfig), "CreateBuildingDef")]
        public class CreatureDeliveryPointConfig_buildingdef
        {
            public static void Postfix(BuildingDef __result)
            {
                __result.Deprecated = false;
                __result.ShowInBuildMenu = true;
            }
        }
        [HarmonyPatch(typeof(KAnimGroupFile), nameof(KAnimGroupFile.Load))]
        public class KAnimGroupFile_Load_Patch
        {
            public static void Prefix(KAnimGroupFile __instance)
            {
                var interacts = new HashSet<HashedString>();
                BuildingModifications.Instance.LoadedBuildingOverrides.Values.ToList().ForEach(item =>
                {
                    if (item!=null && item.workableAnimOverride != null && item.workableAnimOverride.Length > 0)
                    {
                        interacts.Add(item.workableAnimOverride);
                    }
                });

                if(interacts.Count > 0)
                {
                    InjectionMethods.RegisterCustomInteractAnim(
                        __instance, interacts);
                }
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

                    //SgtLogger.l("building override config found!");

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
            private static void Postfix(Workable __instance)
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

                    
                    if (overrideParams.WorkableOffsetOverride.HasValue)
                    {

                        var offsetOverride = overrideParams.WorkableOffsetOverride.Value;

                        if(__instance.TryGetComponent<Rotatable>(out var rot) && rot.Orientation == Orientation.FlipH)
                        {
                            offsetOverride.x = -offsetOverride.x;
                        }

                        __instance.SetOffsets(new[] { offsetOverride });
                    }
                }
            }
        }


        [HarmonyPatch (typeof(Building),nameof(Building.OnSpawn))]
        public static class AnimOverrides_OnSpawn
        {
            private static void Postfix(Building __instance)
            {
                if (__instance.TryGetComponent<KBatchedAnimController>(out var kbac))
                {
                    //SgtLogger.l("Testing any overrides for " + kPrefabID.PrefabID());
                    if (!BuildingModifications.Instance.LoadedBuildingOverrides.ContainsKey(__instance.Def.PrefabID.ToString()))
                    {
                        //SgtLogger.l("no anim override for this building found..");
                        return;
                    }
                    BuildingModification overrideParams = BuildingModifications.Instance.LoadedBuildingOverrides[__instance.Def.PrefabID.ToString()];

                    if (overrideParams.AnimOffsetOverrideX.HasValue || overrideParams.AnimOffsetOverrideY.HasValue)
                    {
                        float x,y;
                        x = overrideParams.AnimOffsetOverrideX.HasValue ? overrideParams.AnimOffsetOverrideX.Value : 1;
                        y = overrideParams.AnimOffsetOverrideY.HasValue ? overrideParams.AnimOffsetOverrideY.Value : 1;

                        SgtLogger.l("changing anim offset to "+ new Vector2(x, y), __instance.name);
                        kbac.Offset = new(x,y);
                    }
                    if (overrideParams.AnimScaleWidthOverride.HasValue)
                    {
                        SgtLogger.l("changing anim scale width to "+ overrideParams.AnimScaleWidthOverride.Value, __instance.name);
                        kbac.animWidth = overrideParams.AnimScaleWidthOverride.Value;
                    }
                    if (overrideParams.AnimScaleHeightOverride.HasValue)
                    {
                        SgtLogger.l("changing anim scale height to " + overrideParams.AnimScaleHeightOverride.Value, __instance.name);
                        kbac.animHeight = overrideParams.AnimScaleHeightOverride.Value;
                    }
                    if (overrideParams.animRotation.HasValue)
                    {
                        SgtLogger.l("changing animRotation to " + overrideParams.animRotation.Value, __instance.name);
                        kbac.Rotation = overrideParams.animRotation.Value;
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

                //TranspilerHelper.PrintInstructions(codes);

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

                    if (overrideParams.requiresMinionWorker.HasValue)
                    {
                        if (go.TryGetComponent<BuildingComplete>(out var building) && go.TryGetComponent<ComplexFabricator>(out var fab))
                        {
                            building.isManuallyOperated = overrideParams.requiresMinionWorker.Value;
                            fab.duplicantOperated = overrideParams.requiresMinionWorker.Value;
                        }
                        else
                            SgtLogger.warning("could not override minion worker requirement for " + go.name);
                    }

                    if (go.TryGetComponent<Workable>(out var workable))
                    {
                        
                        if (overrideParams.workableAnimOverride != null && overrideParams.workableAnimOverride.Length > 0)
                        {
                            var anim = Assets.GetAnim(overrideParams.workableAnimOverride);
                            if (anim != null)
                            {
                                workable.overrideAnims = new KAnimFile[] { anim };
                            }
                            else
                                SgtLogger.error($"WorkingOverride Animfile {overrideParams.workableAnimOverride} not found!");
                        }
                    }

                }
            }
        }
    }
}
