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
	public static class HysteresisStorage
	{
		public static AddComponentDelegate AddComponent_Delegate = null;
		public delegate void AddComponentDelegate(GameObject go);

		public static void AddComponent(GameObject buildingPrefab)
		{
			InitTypes();
			if (AddComponent_Delegate != null)
				AddComponent_Delegate(buildingPrefab);
		}
		static bool typesInitialized = false;
		static void InitTypes()
		{
			if (typesInitialized) return;
			typesInitialized = true;
			var hysteresisStoragePatches = Type.GetType("HysteresisStorage.HysteresisStoragePatches, HysteresisStorage");
			if (hysteresisStoragePatches == null)
			{
				SgtLogger.l("HysteresisStorage types not found.");
				//UtilMethods.ListAllTypesWithAssemblies();
				return;
			}
			var m_AddComponent = AccessTools.Method(hysteresisStoragePatches, "AddComponent", [typeof(GameObject)]);
			if (m_AddComponent == null)
			{
				SgtLogger.error("HysteresisStoragePatches.AddComponent method not found.");
				return;
			}
			try
			{
				AddComponent_Delegate = (AddComponentDelegate)Delegate.CreateDelegate(typeof(AddComponentDelegate), m_AddComponent);
			}
			catch (Exception e)
			{
				SgtLogger.error("Failure to create AddComponentDelegate for HysteresisStorage component:\n" + e.Message);
			}
			SgtLogger.l("HysteresisStorage integration: " + (AddComponent_Delegate != null ? "Success" : "Failed"));
		}
	}
}
