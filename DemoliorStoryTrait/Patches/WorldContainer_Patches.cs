using HarmonyLib;
using Klei.AI;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
	class WorldContainer_Patches
	{
		/// <summary>
		/// apply the fixed trait to the target asteroid of the story trait
		/// </summary>

		[HarmonyPatch(typeof(WorldContainer), nameof(WorldContainer.SetWorldDetails))]
		public class WorldContainer_SetWorldDetails_Patch
		{
			public static void Postfix(WorldContainer __instance, WorldGen world)
			{
				if (__instance.StoryTraitIds?.Contains(Stories_Patches.CGM_Impactor_Path) ?? false)
				{
					__instance.largeImpactorFragmentsFixedTrait = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NAME.ALLOWED;
					__instance.largeImpactorFragments = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.ALLOWED;
					SgtLogger.l($"WorldContainer_SetWorldDetails_Patch: Applied {FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NAME.ALLOWED} to {__instance.name}");
				}
			}
		}

		[HarmonyPatch(typeof(WorldContainer), nameof(WorldContainer.OnSpawn))]
		public class WorldContainer_OnSpawn_Patch
		{
			public static void Prefix(WorldContainer __instance)
			{
				if (__instance.StoryTraitIds?.Contains(Stories_Patches.CGM_Impactor_Path) ?? false)
				{
					__instance.largeImpactorFragmentsFixedTrait = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NAME.ALLOWED;
					__instance.largeImpactorFragments = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.ALLOWED;
					SgtLogger.l($"WorldContainer_OnSpawn_Patch: Applied {FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NAME.ALLOWED} to {__instance.name}");
				}
			}
		}

	}
}
