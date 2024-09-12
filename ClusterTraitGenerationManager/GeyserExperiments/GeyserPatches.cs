using ClusterTraitGenerationManager.ClusterData;
using HarmonyLib;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TemplateClasses;
using UtilLibs;
using static GeyserGenericConfig;

namespace ClusterTraitGenerationManager.GeyserExperiments
{
	[HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.GenerateConfigs))]
	public static class CheckInitPoint
	{
		[HarmonyPriority(Priority.VeryLow)] //grab all the mod added configs
		public static void Postfix(List<GeyserPrefabParams> __result)
		{
			foreach (var entry in __result)
			{
				SgtLogger.l("creating geyser data entry: " + entry.id);
				ModAssets.AllGeysers[entry.id] = new GeyserDataEntry(entry.id, Strings.Get(entry.nameStringKey), Strings.Get(entry.descStringKey), entry.anim, entry.isGenericGeyser);
			}
			SgtLogger.l("AllGeysersCount: " + __result.Count);
			ModAssets.AllGenericGeysers = new();
			foreach (var entry in __result)
			{
				if (entry.isGenericGeyser)
					ModAssets.AllGenericGeysers.Add(entry.id);
			}
			SgtLogger.l("GenericGeysersCount: " + ModAssets.AllGenericGeysers.Count);
		}
	}
	//[HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreatePrefabs))]
	//public static class Test_CreatePrefabs
	//{
	//    public static bool Prefix(ref List<GameObject> __result, GeyserGenericConfig __instance)
	//    {
	//        SgtLogger.l("geyserGeneric CreatePrefabs");
	//        List<GameObject> list = new List<GameObject>();
	//        List<GeyserPrefabParams> configs = __instance.GenerateConfigs();
	//        foreach (GeyserPrefabParams item in configs)
	//        {
	//            list.Add(CreateGeyser(item.id, item.anim, item.width, item.height, Strings.Get(item.nameStringKey), Strings.Get(item.descStringKey), item.geyserType.idHash, item.geyserType.geyserTemperature));
	//        }

	//        configs.RemoveAll((GeyserPrefabParams x) => !x.isGenericGeyser);
	//        GameObject gameObject = EntityTemplates.CreateEntity("GeyserGeneric", "Random Geyser Spawner");
	//        gameObject.AddOrGet<SaveLoadRoot>();
	//        gameObject.GetComponent<KPrefabID>().prefabInitFn += delegate (GameObject inst)
	//        {
	//            SgtLogger.l("geyserGeneric OnSpawn");
	//            int num = 0;
	//            if (SaveLoader.Instance.clusterDetailSave != null)
	//            {
	//                num = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
	//            }
	//            else
	//            {
	//                Debug.LogWarning("Could not load global world seed for geysers");
	//            }

	//            num = num + (int)inst.transform.GetPosition().x + (int)inst.transform.GetPosition().y;
	//            int index = new KRandom(num).Next(0, configs.Count);
	//            SgtLogger.l(configs[index].id + " "+(int)inst.transform.GetPosition().x+", " + (int)inst.transform.GetPosition().y+", seed: "+ SaveLoader.Instance.clusterDetailSave.globalWorldSeed, "GEYSERGENERIC ONSPAWNTYPE");
	//            GameUtil.KInstantiate(Assets.GetPrefab(configs[index].id), inst.transform.GetPosition(), Grid.SceneLayer.BuildingBack).SetActive(value: true);
	//            inst.DeleteObject();
	//        };
	//        list.Add(gameObject);
	//        __result = list;
	//        return false;
	//    }
	//}
	[HarmonyPatch(typeof(WorldGen), "GenerateOffline")]
	public static class WorldGen_GrabPlanet
	{
		public static List<string> GeysersToOverride = new();
		public static HashSet<string> BlacklistedGeysers = new();
		public static bool ReplaceNonGenerics = false;
		public static int seed = 0;
		public static void Prefix(WorldGen __instance)
		{
			if (!CGSMClusterManager.LoadCustomCluster)
			{
				return;
			}

			GeysersToOverride = new();
			BlacklistedGeysers = new();
			ReplaceNonGenerics = false;

			string planetID = __instance.Settings.world.filePath;
			if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster.HasStarmapItem(planetID, out var planet))
			{
				GeysersToOverride = new(planet.GeyserOverrideIDs);
				BlacklistedGeysers = new HashSet<string>(planet.GeyserBlacklistIDs);
				ReplaceNonGenerics = planet.GeyserBlacklistAffectsNonGenerics;
				SgtLogger.l("override geyser count: " + GeysersToOverride.Count);
			}
		}
	}
	[HarmonyPatch(typeof(GameSpawnData), nameof(GameSpawnData.AddTemplate))]
	public class GameSpawnData_AddTemplate_Patch
	{
		static Vector2I targetPos;
		public static void Prefix(GameSpawnData __instance, Vector2I position)
		{
			targetPos = position;
		}

		static Prefab GetGeyserPrefab(Prefab original, string ID)
		{
			var clone = original.Clone(new(0, 0));
			clone.id = ID;
			return clone;
		}

		private static Prefab SwapGeyserTemplate(Prefab existing)
		{
			if (!CGSMClusterManager.LoadCustomCluster)
			{
				return existing;
			}


			if (existing.id == "GeyserGeneric")
			{
				if (WorldGen_GrabPlanet.GeysersToOverride.Count > 0) //apply guaranteed geyser from override
				{
					string geyserID = WorldGen_GrabPlanet.GeysersToOverride[0];
					var clone = GetGeyserPrefab(existing, geyserID);
					SgtLogger.l("applying Geyser Override for generic geyser: " + geyserID);
					WorldGen_GrabPlanet.GeysersToOverride.RemoveAt(0);
					return clone;
				}

				var geyser = ModAssets.GetGenericGeyserAt(CGSMClusterManager.GlobalWorldSeed, targetPos); //get geyser that would spawn
				if (WorldGen_GrabPlanet.BlacklistedGeysers.Contains(geyser)) // if on blacklist, replace with random generic geyser
				{
					geyser = ModAssets.GetGenericGeyserAt(CGSMClusterManager.GlobalWorldSeed, targetPos, WorldGen_GrabPlanet.BlacklistedGeysers);
					var clone = GetGeyserPrefab(existing, geyser);

					SgtLogger.l("Blacklisted generic geyser " + existing.id + " found, overriding with generic geyser: " + geyser);
					return clone;
				}
			}
			else
			if (WorldGen_GrabPlanet.ReplaceNonGenerics && WorldGen_GrabPlanet.BlacklistedGeysers.Contains(existing.id)) //replace non generics
			{
				var geyserID = ModAssets.GetGenericGeyserAt(CGSMClusterManager.GlobalWorldSeed, targetPos, WorldGen_GrabPlanet.BlacklistedGeysers);
				var clone = GetGeyserPrefab(existing, geyserID);
				SgtLogger.l("Blacklisted non generic geyser " + existing.id + " found, overriding with generic geyser: " + geyserID);
				return clone;
			}

			return existing;
		}
		public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
		{
			var codes = orig.ToList();

			var indexGetterAnchorMethod = AccessTools.Method(typeof(GameSpawnData), "IsWarpTeleporter");
			var CloneMethod = AccessTools.Method(typeof(TemplateClasses.Prefab), "Clone", new Type[] { typeof(Vector2I) });
			var indexGetterAnchorIndex = codes.FindLastIndex(ci => ci.Calls(indexGetterAnchorMethod));
			if (indexGetterAnchorIndex < 0)
			{
				SgtLogger.error("GEYSER TRANSPILER FAILED, INDEX GETTER!");
				return codes;
			}
			var template_locIndex = TranspilerHelper.FindIndexOfNextLocalIndex(codes, indexGetterAnchorIndex);

			int insertionIndex = codes.FindIndex(indexGetterAnchorIndex, ci => ci.Calls(CloneMethod));
			if (insertionIndex < 0)
			{
				SgtLogger.error("GEYSER TRANSPILER FAILED, INJECTION FAULT!");
				return codes;
			}
			var callIndex = TranspilerHelper.FindIndexOfNextLocalIndexWithPosition(codes, insertionIndex);
			if (callIndex == null)
			{
				SgtLogger.error("GEYSER TRANSPILER FAILED, CALL INDEX FAULT!");
				return codes;
			}
			var insertAt = callIndex.second;

			var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(GameSpawnData_AddTemplate_Patch), "SwapGeyserTemplate");

			// inject right after the found index
			codes.InsertRange(insertAt + 1, new[]
			{
                            //new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Call, m_InjectedMethod)
						});

			TranspilerHelper.PrintInstructions(codes);

			return codes;
		}
	}

	//[HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreatePrefabs))]
	//public class GeyserGenericConfig_CreatePrefabs_OverrideRandomGeyserGeneration
	//{
	//    public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
	//    {
	//        var codes = orig.ToList();

	//        find injection point
	//       var index = codes.FindLastIndex(ci => ci.opcode == OpCodes.Ldftn);

	//        if (index == -1)
	//        {
	//            SgtLogger.error("GEYSERGENERICCONFIG TRANSPILER FAILED");
	//            return codes;
	//        }

	//        var m_GenerateConfigs = AccessTools.DeclaredMethod(typeof(GeyserGenericConfig), "GenerateConfigs");
	//        var m_ReplaceprefabInitFn = AccessTools.DeclaredMethod(typeof(GeyserGenericConfig_CreatePrefabs_OverrideRandomGeyserGeneration), "ReplaceprefabInitFn");



	//        replace random geyser onprefabinit
	//        codes[index].operand = m_ReplaceprefabInitFn.MethodHandle.GetFunctionPointer();


	//        return codes;
	//    }

	//    private static KPrefabID.PrefabFn ReplaceprefabInitFn()
	//    {
	//        return (inst =>
	//        {
	//            List<GeyserPrefabParams> configs = new(GeyserConfigsGetter.GeyserConfigs);
	//            int num = 0;
	//            if (SaveLoader.Instance.clusterDetailSave != null)
	//                num = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
	//            else
	//                Debug.LogWarning((object)"Could not load global world seed for geysers");
	//            string GeyserToPlace = configs[new KRandom(num + (int)inst.transform.GetPosition().x + (int)inst.transform.GetPosition().y).Next(0, configs.Count)].id;
	//            if (SaveGame.Instance != null && SaveGame.Instance.TryGetComponent<SaveGameData>(out var data))
	//            {
	//                if (data.TryGetGeyserOverride(inst, out string overrideID))
	//                {
	//                    GeyserToPlace = overrideID;
	//                }
	//            }

	//            GameUtil.KInstantiate(Assets.GetPrefab((Tag)GeyserToPlace), inst.transform.GetPosition(), Grid.SceneLayer.BuildingBack).SetActive(true);
	//            inst.DeleteObject();
	//        });
	//    }
	//}
	//[HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.GenerateConfigs))]
	//public static class GeyserConfigsGetter
	//{
	//    public static List<GeyserPrefabParams> GeyserConfigs;
	//    public static void Postfix(ref List<GeyserPrefabParams> __result)
	//    {
	//        GeyserConfigs = new(__result);

	//    }

	//}

}
