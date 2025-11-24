using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	class BugfixPatches
	{

		/// <summary>
		/// Fixes a crash that happens when an unbuild, disconnected module blocks the space of the module building task
		/// </summary>
		[HarmonyPatch(typeof(ReorderableBuilding))]
		[HarmonyPatch(nameof(ReorderableBuilding.RocketSpecificPostAdd))]
		public static class FixesVanillaCrashOnUnbuildModuleReordering
		{
			public static bool Prefix(GameObject obj, int cell)
			{
				return obj != null;
			}
		}
		[HarmonyPatch(typeof(ConditionHasResource))]
		[HarmonyPatch(nameof(ConditionHasResource.EvaluateCondition))]
		public static class ConditionHasResource_RoundingCheck
		{
			public static void Postfix(ref ProcessCondition.Status __result, ConditionHasResource __instance)
			{
				if (__result == ProcessCondition.Status.Warning && __instance.resource == SimHashes.Diamond)
				{
					var availableMass = __instance.storage.GetAmountAvailable(__instance.resource.CreateTag());
					if (Mathf.Approximately(availableMass, __instance.thresholdMass) || availableMass + (1f / 1000f) >= __instance.thresholdMass)
						__result = ProcessCondition.Status.Ready;
				}
			}
		}



		/// <summary>
		/// fixes a vanilla crash that can happen when this has eventID==null
		/// </summary>
		[HarmonyPatch(typeof(ClusterMapMeteorShower.Def))]
		[HarmonyPatch(nameof(ClusterMapMeteorShower.Def.GetDescriptors))]
		public static class FixesVanillaCrashOnPlanetSelection
		{
			public static bool Prefix(ClusterMapMeteorShower.Def __instance, ref List<Descriptor> __result)
			{
				if (__instance.eventID == string.Empty || __instance.eventID == null)
				{
					__result = new List<Descriptor>();
					return false;
				}
				return true;
			}
		}
		/// <summary>
		/// Fixes a bug with the cleanup method that would cause invisible solid tiles in the next world at that location
		/// manual patch to avoid double patching with StockBugFix
		/// </summary>
		//[HarmonyPatch(typeof(Grid))]
		//[HarmonyPatch(nameof(Grid.FreeGridSpace))]
		public static class Grid_FreeGridSpace_BugfixPatch
		{
			internal static void Prefix(Vector2I size, Vector2I offset)
			{
				int cell = Grid.XYToCell(offset.x, offset.y), width = size.x, stride =
					Grid.WidthInCells - width;
				for (int y = size.y; y > 0; y--)
				{
					for (int x = width; x > 0; x--)
					{
						if (Grid.IsValidCell(cell))
							SimMessages.ReplaceElement(cell, SimHashes.Vacuum, null, 0.0f);
						cell++;
					}
					cell += stride;
				}
			}
		}

		/// <summary>
		/// Only affects debug create rocket command, prevents crash when it tries to load element with combustibleliquid tag by converting it to petroleum
		/// </summary>
		[HarmonyPatch(typeof(AutoRocketUtility), nameof(AutoRocketUtility.AddEngine))]
		public class AutoRocketUtility_AddEngine_Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();
				var targetMethod = AccessTools.Method(typeof(ElementLoader), nameof(ElementLoader.GetElement), [typeof(Tag)]);

				// find injection point
				var index = codes.FindIndex(ci => ci.Calls(targetMethod));

				if (index == -1)
				{
					SgtLogger.error("TRANSPILER FAILED: AutoRocketUtility_AddEngine_Patch");
					return codes;
				}

				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(AutoRocketUtility_AddEngine_Patch), "InjectedMethod");

				// inject right after the found index
				codes.InsertRange(index, [new CodeInstruction(OpCodes.Call, m_InjectedMethod)]);

				return codes;
			}

			private static Tag InjectedMethod(Tag combustibleLiquidTag)
			{
				return SimHashes.Petroleum.CreateTag();
			}
		}

		[HarmonyPatch(typeof(OxidizerTank), nameof(OxidizerTank.OnCopySettings))]
		public static class Fix_OxyliteFallingOutOnReload
		{
			public static void Postfix(object data, OxidizerTank __instance)
			{
				if (DlcManager.IsExpansion1Active()
					&& ((GameObject)data).TryGetComponent<OxidizerTank>(out var sourceTank)
					&& __instance.supportsMultipleOxidizers
					&& sourceTank.supportsMultipleOxidizers)
				{
					if (__instance.TryGetComponent<FlatTagFilterable>(out var flatTagDestination) && sourceTank.TryGetComponent<FlatTagFilterable>(out var flatTagSource))
					{
						flatTagDestination.selectedTags = new List<Tag>(flatTagSource.selectedTags);

						if (__instance.TryGetComponent<TreeFilterable>(out var TreeFilter))
							TreeFilter.UpdateFilters(new HashSet<Tag>(flatTagDestination.selectedTags));
					}
				}
			}

		}
	}
}
