using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	internal class NoManualDelivery
	{
		public static List<string> NoManualDelivery_BuildingToMakeAutomatable = null;
		public static bool NoManualDeliveryFound = false;

		public static void AddComponent(GameObject buildingPrefab)
		{
			InitTypes();
			if (NoManualDeliveryFound)
			{
				NoManualDelivery_BuildingToMakeAutomatable.Add(buildingPrefab.PrefabID().ToString());
			}
			else
				buildingPrefab.AddOrGet<AutomatableAutoOn>();
		}
		static bool typesInitialized = false;
		static void InitTypes()
		{
			if (typesInitialized) return;
			typesInitialized = true;
			try
			{
				var noManualDelivery_Patches = Type.GetType("NoManualDelivery.Patches, NoManualDelivery");
				if (noManualDelivery_Patches == null)
					throw new TypeAccessException("NoManualDelivery.Patches not found");

				var listField = AccessTools.Field(noManualDelivery_Patches, "BuildingToMakeAutomatable");
				if (listField == null)
					throw new MissingFieldException("NoManualDelivery.Patches", "BuildingToMakeAutomatable");

				NoManualDelivery_BuildingToMakeAutomatable = listField.GetValue(null) as List<string>;
			}
			catch
			{

			}
			finally
			{
				SgtLogger.l("NoManualDelivery integration: " + (NoManualDelivery_BuildingToMakeAutomatable != null ? "Success" : "Inactive"));
				NoManualDeliveryFound = NoManualDelivery_BuildingToMakeAutomatable != null;
			}
		}
	}
}
