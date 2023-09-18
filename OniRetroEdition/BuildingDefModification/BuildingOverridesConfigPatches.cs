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

namespace OniRetroEdition.BuildingDefModification
{    
    
    /// <summary>
    /// Override Building Props
    /// </summary>
    internal class BuildingOverridesConfigPatches
    {
        //[HarmonyPatch(typeof(BuildingConfigManager), nameof(BuildingConfigManager.RegisterBuilding))]
        //[HarmonyPrefix]
        //public static void AddBuildingDef_Prefix(BuildingDef def)
        //{
        //    AddLogic.TryAddLogic(def);
        //}

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

                codes.InsertRange(indexCreateDef+1, new[]
                {
                    new CodeInstruction(OpCodes.Call, m_buildingdef_postfix)
                        });



                var indexPostConfigComplete = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == PostConfigureCompleteTargetMethod);
                
                codes.InsertRange(indexPostConfigComplete +1, new[]
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
                        def.HeightInCells = overrideParams.HeightOverride.Value;
                    }
                    if (overrideParams.WidthOverride.HasValue)
                    {
                        sizeChanged = true;
                        def.WidthInCells = overrideParams.HeightOverride.Value;
                    }
                    if (sizeChanged)
                    {
                        def.GenerateOffsets();
                    }
                    if(overrideParams.foundationFloorTile.HasValue && overrideParams.foundationFloorTile.Value)
                    {
                        BuildingTemplates.CreateFoundationTileDef(def);
                    }
                    if(overrideParams.UtilityInputOffsetOverride.HasValue)
                    {
                        def.UtilityInputOffset = overrideParams.UtilityInputOffsetOverride.Value;
                    }
                    if(overrideParams.UtilityOutputOffsetOverride.HasValue)
                    {
                        def.UtilityOutputOffset = overrideParams.UtilityOutputOffsetOverride.Value;
                    }



                    if(overrideParams.animOverride!=null&& overrideParams.animOverride.Length>0)
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
                if (BuildingModifications.Instance.LoadedBuildingOverrides.ContainsKey(def.PrefabID))
                {
                    BuildingModification overrideParams = BuildingModifications.Instance.LoadedBuildingOverrides[def.PrefabID];
                    
                    if (overrideParams.foundationFloorTile.HasValue && overrideParams.foundationFloorTile.Value)
                    {
                        KPrefabID component = go.GetComponent<KPrefabID>();
                        component.AddTag(GameTags.FloorTiles);
                    }

                    if(go.TryGetComponent<Workable>(out var workable))
                    {
                        if (overrideParams.requiresMinionWorker.HasValue)
                        {
                            workable.requireMinionToWork = overrideParams.requiresMinionWorker.Value;
                        }
                        if(overrideParams.workableAnimOverride!=null && overrideParams.workableAnimOverride.Length > 0)
                        {
                            var anim = Assets.GetAnim(overrideParams.workableAnimOverride);
                            if (anim != null)
                            {
                                workable.overrideAnims = new KAnimFile[] { anim };
                            }
                            else
                                SgtLogger.warning($"WorkingOverride Animfile {overrideParams.workableAnimOverride} not found!");
                        }
                    }

                }
            }
        }
    }
}
