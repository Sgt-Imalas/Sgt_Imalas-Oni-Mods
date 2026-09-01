using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	internal class ModIntegrationHelper
	{
		/// <summary>
		/// Integration for NoManualDelivery, HysteresisStorage and PreciseStorageControl mods
		/// </summary>
		/// <param name="go"></param>
		public static void AllStorageIntegations(GameObject go)
		{
			PreciseStorageControl.AddComponent(go);
			HysteresisStorage.AddComponent(go);
			NoManualDelivery.AddComponent(go);
		}
		/// <summary>
		/// Integration for HysteresisStorage and PreciseStorageControl mods
		/// </summary>
		/// <param name="go"></param>
		public static void RailLoaderStorageIntegations(GameObject go)
		{
			PreciseStorageControl.AddComponent(go);
			HysteresisStorage.AddComponent(go);
		}
	}
}
