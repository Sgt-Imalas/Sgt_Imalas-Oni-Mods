using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace ModOriginInfo.Patches
{
	internal class EntityConfigManager_Patches
	{
		static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> orig, MethodInfo injection)
		{
			var m_GetComponent = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent)).MakeGenericMethod(typeof(KPrefabID));
			foreach (var ci in orig)
			{
				yield return ci;
				if (ci.Calls(m_GetComponent))
				{
					yield return new CodeInstruction(OpCodes.Call, injection);
				}
			}
		}

		[HarmonyPatch(typeof(EntityConfigManager), nameof(EntityConfigManager.RegisterEntities))]
		public class EntityConfigManager_RegisterEntities_Patch
		{
			static IMultiEntityConfig cachedConfigs;

			public static void Prefix(IMultiEntityConfig config)
			{
				SgtLogger.l("Register Entities: " + config.GetType().Name);
				cachedConfigs = config;
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var m_Injection = AccessTools.Method(typeof(EntityConfigManager_RegisterEntities_Patch), nameof(FetchEntityList));
				var patched = Patch(orig, m_Injection).ToList();
				//TranspilerHelper.PrintInstructions(patched);
				return patched;
			}
			private static KPrefabID FetchEntityList(KPrefabID prefabId)
			{
				ModAssets.RegisterMultiEntity(cachedConfigs, prefabId);
				return prefabId;
			}
		}
		[HarmonyPatch(typeof(EntityConfigManager), nameof(EntityConfigManager.RegisterEntity))]
		public class EntityConfigManager_RegisterEntity_Patch
		{
			static IEntityConfig cachedConfig;

			public static void Prefix(IEntityConfig config)
			{
				cachedConfig = config;
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var m_Injection = AccessTools.Method(typeof(EntityConfigManager_RegisterEntity_Patch), nameof(FetchEntity));
				var patched = Patch(orig, m_Injection).ToList();
				//TranspilerHelper.PrintInstructions(patched);
				return patched;
			}

			private static KPrefabID FetchEntity(KPrefabID prefabId)
			{
				ModAssets.RegisterEntity(cachedConfig, prefabId);
				return prefabId;
			}
		}
	}
}
