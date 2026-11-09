using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	public static class StorageRefrigeratorThresholds
	{
		public static Type StorageRefrigeratorThresholdsType = null;
		public delegate void AddComponentDelegate(GameObject go);

		public static void AddComponentIfNotFound(GameObject buildingPrefab)
		{
			InitTypes();
			if (StorageRefrigeratorThresholdsType != null && buildingPrefab.GetComponent(StorageRefrigeratorThresholdsType) == null)
				buildingPrefab.AddComponent(StorageRefrigeratorThresholdsType);
		}
		static bool typesInitialized = false;
		static void InitTypes()
		{
			if (typesInitialized)
				return;
			typesInitialized = true;
			StorageRefrigeratorThresholdsType = Type.GetType("StorageRefrigeratorThresholds.RefrigeratorThresholds, StorageRefrigeratorThresholds");
			SgtLogger.l("StorageRefrigeratorThresholds integration: " + (StorageRefrigeratorThresholdsType != null ? "Active" : "Inactive"));
		}
	}
}
