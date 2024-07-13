//using ClusterTraitGenerationManager.ClusterData;
//using HarmonyLib;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using UnityEngine;
//using UtilLibs;
//using static GeyserGenericConfig;

//namespace ClusterTraitGenerationManager.GeyserExperiments
//{
//    //[HarmonyPatch(typeof(GeyserGenericConfig))]
//    //[HarmonyPatch(nameof(GeyserGenericConfig.CreatePrefabs))]
//    //public static class CheckInitPoint
//    //{
//    //    public static void Postfix(GeyserGenericConfig __instance, List<GameObject> __result)
//    //    {
//    //        List<GeyserPrefabParams> configs = __instance.GenerateConfigs();

//    //        foreach (var entry in __result)
//    //        {

//    //        }
//    //    }
//    //}

//    [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreatePrefabs))]
//    public class GeyserGenericConfig_CreatePrefabs_OverrideRandomGeyserGeneration
//    {
//        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
//        {
//            var codes = orig.ToList();

//            // find injection point
//            var index = codes.FindLastIndex(ci => ci.opcode == OpCodes.Ldftn);

//            if (index == -1)
//            {
//                SgtLogger.error("GEYSERGENERICCONFIG TRANSPILER FAILED");
//                return codes;
//            }

//            var m_GenerateConfigs = AccessTools.DeclaredMethod(typeof(GeyserGenericConfig), "GenerateConfigs");
//            var m_ReplaceprefabInitFn = AccessTools.DeclaredMethod(typeof(GeyserGenericConfig_CreatePrefabs_OverrideRandomGeyserGeneration), "ReplaceprefabInitFn");



//            // replace random geyser onprefabinit
//            codes[index].operand = m_ReplaceprefabInitFn.MethodHandle.GetFunctionPointer();
            

//            return codes;
//        }

//        private static KPrefabID.PrefabFn ReplaceprefabInitFn()
//        {
//            return (inst =>
//            {
//                List<GeyserPrefabParams> configs = new(GeyserConfigsGetter.GeyserConfigs);
//                int num = 0;
//                if (SaveLoader.Instance.clusterDetailSave != null)
//                    num = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
//                else
//                    Debug.LogWarning((object)"Could not load global world seed for geysers");
//                string GeyserToPlace = configs[new KRandom(num + (int)inst.transform.GetPosition().x + (int)inst.transform.GetPosition().y).Next(0, configs.Count)].id;
//                if(SaveGame.Instance != null && SaveGame.Instance.TryGetComponent<SaveGameData>(out var data))
//                {
//                    if(data.TryGetGeyserOverride(inst, out string overrideID))
//                    {
//                        GeyserToPlace = overrideID;
//                    }
//                }

//                GameUtil.KInstantiate(Assets.GetPrefab((Tag)GeyserToPlace), inst.transform.GetPosition(), Grid.SceneLayer.BuildingBack).SetActive(true);
//                inst.DeleteObject();
//            });
//        }
//    }
//    [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.GenerateConfigs))]
//    public static class GeyserConfigsGetter
//    {
//        public static List<GeyserPrefabParams> GeyserConfigs;
//        public static void Postfix(ref List<GeyserPrefabParams> __result)
//        {
//            GeyserConfigs = new(__result);

//        }

//    }

//}
