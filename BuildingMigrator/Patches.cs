using Database;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static BuildingMigrator.ModAssets;

namespace BuildingMigrator
{
	internal class Patches
	{
		/// <summary>
		/// register legacy building ids for save migration for merged buildings
		/// </summary>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.OnPrefabInit))]
		public class SaveManager_OnPrefabInit_Patch
		{
			public static void Postfix(SaveManager __instance)
			{
				RegisterAdditionalMigrations();
			}
		}
		internal static void RegisterAdditionalMigrations()
		{
			var savemng = SaveLoader.Instance.saveManager;
			if (savemng == null)
				return;
			TryMigrate("InsulatedSelfSealingAirLock", "FastInsulatedSelfSealingAirLock");
		}
		static void TryMigrate(string oldId, string newId)
		{
			var savemng = SaveLoader.Instance.saveManager;
			if (savemng == null)
				return;
			var prefab = Assets.TryGetPrefab(newId);
			if (prefab != null && !savemng.prefabMap.ContainsKey(oldId))
				savemng.prefabMap.Add(oldId, prefab);
		}
	}
}
