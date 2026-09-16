using System;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	/// <summary>
	/// Integration with the mod PreciseStorageControl
	/// </summary>
	public static class PreciseStorageControl
	{
		public static Type PreciselyFilteredStorageControllerType = null;
		public delegate void AddComponentDelegate(GameObject go);

		public static void AddComponent(GameObject buildingPrefab)
		{
			InitTypes();
			if (PreciselyFilteredStorageControllerType != null)
				buildingPrefab.AddComponent(PreciselyFilteredStorageControllerType);
		}
		static bool typesInitialized = false;
		static void InitTypes()
		{
			if (typesInitialized) return;
			typesInitialized = true;
			PreciselyFilteredStorageControllerType = Type.GetType("PreciselyControlled.PreciselyFilteredStorageController, PreciseControlStorage");
			SgtLogger.l("PreciselyControlled integration: " + (PreciselyFilteredStorageControllerType != null ? "Active" : "Inactive"));
		}
	}
}
