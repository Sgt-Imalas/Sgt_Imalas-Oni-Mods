using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static STRINGS.UI.DEVELOPMENTBUILDS.ALPHA;

namespace Rockets_TinyYetBig.Patches.RocketLoadingPatches
{
	public class RocketConduitStorageAccess_Patches
	{

		[HarmonyPatch(typeof(RocketConduitStorageAccess), nameof(RocketConduitStorageAccess.OnSpawn))]
		public class RocketConduitStorageAccess_OnSpawn_Patch
		{
			public static void Postfix(RocketConduitStorageAccess __instance) => __instance.gameObject.AddOrGet<RocketConduitSenderSleepMode>();
		}

		[HarmonyPatch(typeof(RocketConduitStorageAccess), nameof(RocketConduitStorageAccess.Sim200ms))]
		public static class RocketConduitStorageAccess_Sim200ms_Patch
		{
			public static bool Prefix(RocketConduitStorageAccess __instance) => RocketConduitStorageAccessRewrite.Sim200_ReplacementPrefix(__instance);
		}
	}
}
