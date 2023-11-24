
//using HarmonyLib;
//using Rendering;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using UnityEngine;
//using static Rendering.BlockTileRenderer;

//namespace OniRetroEdition
//{
//    public class BlockTileRendererPatch
//    {
//        FoodDehydrator;
//        FoodDehydratorConfig;
//        public static MethodInfo GetRenderInfoLayerMethod;
//        public static MethodInfo GetRenderLayerForTileMethod;

//        private const int OFFSET = 451;

//        private static int lastCheckedCell = -1;

//        [HarmonyPatch(typeof(BlockTileRenderer), "GetConnectionBits")]
//        public class BlockTileRenderer_GetConnectionBits_Patch
//        {
//            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> orig)
//            {
//                var codes = orig.ToList();

//                var m_MatchesDef = typeof(BlockTileRenderer).GetMethod("MatchesDef", BindingFlags.NonPublic | BindingFlags.Static);
//                var m_MatchesElement = typeof(BlockTileRenderer_GetConnectionBits_Patch).GetMethod("MatchesElement", new Type[]
//                {
//                    typeof(bool),
//                    typeof(int),
//                    typeof(int),
//                    typeof(int)
//                });

//                for (var i = 0; i < codes.Count; i++)
//                {
//                    var code = codes[i];
//                    if (code.opcode == OpCodes.Call && code.operand is MethodInfo m && m == m_MatchesDef)
//                    {
//                        // bool is loaded onto stack
//                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_1)); // load argument x
//                        codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldarg_2)); // load argument y
//                        codes.Insert(i + 3, new CodeInstruction(OpCodes.Ldarg_3)); // load argument query_layer
//                        codes.Insert(i + 4, new CodeInstruction(OpCodes.Call, m_MatchesElement)); // call MatchesElement
//                    }
//                }

//                Log.PrintInstructions(codes);
//                return codes;
//            }

//            public static bool MatchesElement(bool matchesDef, int x, int y, int layer)
//            {
//                if (!matchesDef || lastCheckedCell == -1)
//                {
//                    return false;
//                }

//                if (layer == (int)ObjectLayer.ReplacementTile)
//                {
//                    return true;
//                }

//                var cell = Grid.XYToCell(x, y);

//                return ElementGrid.elementIdx[cell] == ElementGrid.elementIdx[lastCheckedCell];
//            }
//        }

//        // save last looked at tile for connection
//        // this is called by the GetConnectionBits, which i always call the above transpiler right after
//        [HarmonyPatch(typeof(BlockTileRenderer), "MatchesDef")]
//        public class BlockTileRenderer_MatchesDef_Patch
//        {
//            public static void Prefix(GameObject go, BuildingDef def, ref bool __result)
//            {
//                if (go == null)
//                {
//                    lastCheckedCell = -1;
//                    return;
//                }

//                lastCheckedCell = Grid.PosToCell(go);
//            }
//        }

//        [HarmonyPatch(typeof(BlockTileRenderer), "AddBlock")]
//        public static class Rendering_BlockTileRenderer_AddBlock_Patch
//        {
//            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> orig)
//            {
//                var codes = orig.ToList();
//                var index = FindEntryPoint(codes);

//                // didn't find anything, return original
//                if (index == -1)
//                {
//                    return codes;
//                }

//                //insert after
//                index++;

//                // RenderInfoLayer is loaded to stack
//                codes.Insert(index++, new CodeInstruction(OpCodes.Ldarg_2)); // load BuildingDef
//                codes.Insert(index++, new CodeInstruction(OpCodes.Ldarg_S, 4)); // load SimHashes
//                codes.Insert(index++, new CodeInstruction(OpCodes.Call, GetRenderLayerForTileMethod));  // call GetRenderLayerForTile

//                Log.PrintInstructions(codes);
//                return codes;
//            }
//        }

//        [HarmonyPatch(typeof(BlockTileRenderer), "RemoveBlock")]
//        public static class Rendering_BlockTileRenderer_RemoveBlock_Patch
//        {
//            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> orig)
//            {
//                var codes = orig.ToList();
//                var index = FindEntryPoint(codes);

//                if (index == -1)
//                {
//                    return codes;
//                }

//                index++;

//                // RenderInfoLayer is loaded to stack
//                codes.Insert(index++, new CodeInstruction(OpCodes.Ldarg_1)); // load BuildingDef
//                codes.Insert(index++, new CodeInstruction(OpCodes.Ldarg_3)); // load SimHashes
//                codes.Insert(index++, new CodeInstruction(OpCodes.Call, GetRenderLayerForTileMethod)); // call GetRenderLayerForTile

//                Log.PrintInstructions(codes);
//                return codes;
//            }
//        }

//        private static int FindEntryPoint(List<CodeInstruction> codes)
//        {
//            return codes.FindIndex(c => c.operand is MethodInfo m && m == GetRenderInfoLayerMethod);
//        }

//        internal static RenderInfoLayer GetRenderLayerForTile(RenderInfoLayer layer, BuildingDef def, SimHashes elementId)
//        {
//            if (layer == RenderInfoLayer.Built && def.BuildingComplete.HasTag(ModAssets.Tags.texturedTile)) //TileAssets.Instance.ContainsDef(def.PrefabID))
//            {
//                // Assign an element specific render info layer with a random offset so there is no overlap
//                return (RenderInfoLayer)(elementId + OFFSET);
//            }

//            return layer;
//        }
//    }
//}
