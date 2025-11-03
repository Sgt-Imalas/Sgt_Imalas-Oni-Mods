using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	/// <summary>
	/// Integration with the mod HysteresisStorage
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
			SgtLogger.l("PreciselyControlled integration: " + (PreciselyFilteredStorageControllerType != null ? "Success" : "Failed"));
		}
	}
}
