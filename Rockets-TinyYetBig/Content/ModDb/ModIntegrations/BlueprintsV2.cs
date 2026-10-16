using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb.ModIntegrations
{
	internal class BlueprintsV2
	{
		public static BPV2_ApplyAdditionalBuildingDataDelegate BPV2_ApplyAdditionalBuildingData = null;
		static bool _applyAllFound = false;
		public delegate void BPV2_ApplyAdditionalBuildingDataDelegate(GameObject gameObject, BuildingDef configDef, Dictionary<string, JObject> buildingData);

		public static BPV2_GetAllAdditionalBuildingDataDelegate BPV2_GetAllAdditionalBuildingData = null;
		static bool _getAllFound = false;
		public delegate Dictionary<string, JObject> BPV2_GetAllAdditionalBuildingDataDelegate(GameObject gameObject);


		public static Dictionary<string, JObject> GetAllAdditionalBuildingData(GameObject go)
		{
			if (go == null)
				return null;
			InitTypes();
			if (_getAllFound)
				return BPV2_GetAllAdditionalBuildingData(go);
			return null;
		}

		public static void ApplyAdditionalBuildingData(GameObject go, BuildingDef configDef, Dictionary<string, JObject> buildingData)
		{
			if (go == null || configDef == null || buildingData == null || !buildingData.Any()) return;
			InitTypes();
			if (_applyAllFound)
				BPV2_ApplyAdditionalBuildingData(go, configDef, buildingData);
		}

		static bool typesInitialized = false;
		public static void InitTypes()
		{
			if (typesInitialized) return;
			typesInitialized = true;
			var BlueprintsV2_API_Methods = Type.GetType("BlueprintsV2.ModAPI.API_Methods, BlueprintsV2");
			if (BlueprintsV2_API_Methods == null)
			{
				SgtLogger.l("Blueprints Expanded types not detected, rocket templates will not transfer any data.");
				return;
			}
			_applyAllFound = ReflectionHelper.TryCreateDelegate("BlueprintsV2.ModAPI.API_Methods, BlueprintsV2", "ApplyAdditionalBuildingData", out BPV2_ApplyAdditionalBuildingData);
			_getAllFound = ReflectionHelper.TryCreateDelegate("BlueprintsV2.ModAPI.API_Methods, BlueprintsV2", "GetAllAdditionalBuildingData", out BPV2_GetAllAdditionalBuildingData);

			SgtLogger.l("BlueprintsV2 integration: " + (_applyAllFound && _getAllFound
				? "Success" : "Failed"));
		}
	}
}
