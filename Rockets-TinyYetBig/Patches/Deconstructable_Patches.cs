using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class Deconstructable_Patches
	{
		[HarmonyPatch(typeof(Deconstructable), nameof(Deconstructable.SpawnItemsFromConstruction), [typeof(float), typeof(byte), typeof(int), typeof(WorkerBase)])]
		public class Deconstructable_SpawnItemsFromConstruction_Patch
		{
			public static void Prefix(Deconstructable __instance, ref List<GameObject> __result)
			{
				if (__instance.TryGetComponent<SpaceStationAttachablePartStorage>(out var partStorage))
					partStorage.DismantleStoredPart();

				if (__instance.TryGetComponent<RocketModuleUpgradeStorage>(out var upgradeStorage))
					upgradeStorage.DismantleAllUpgrades();
			}
		}
	}
}
