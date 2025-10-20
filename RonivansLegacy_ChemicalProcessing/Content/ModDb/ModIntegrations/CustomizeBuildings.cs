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
	internal class CustomizeBuildings
	{
		/// <summary>
		/// wrapper that allows fetching a config state value of type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryGetConfigValue<T>(string propertyName, out T value)
		{
			value = default(T);
			InitTypes();
			if (ConfigInstance == null)
				return false;

			Traverse property = Traverse.Create(ConfigInstance).Property(propertyName);
			if (!property.PropertyExists())
			{
				Debug.LogWarning("Mod Config State did not have a property with the name: " + propertyName);
				return false;

			}
			object propertyValue = property.GetValue();
			var foundType = propertyValue.GetType();
			var T_Type = typeof(T);
			if (foundType != T_Type)
			{
				Debug.LogWarning("Mod Config State had a property with the name: " + propertyName+", but it was typeOf "+foundType.Name+", instead of the expected "+ T_Type.Name);
				return false;
			}

			value = (T)propertyValue;
			return true;
		}

		/// <summary>
		/// Integration with CustomizeBuildings custom pipe & rail capacities
		/// </summary>

		static object ConfigInstance = null;

		public static bool TryGetModifiedConduitValues(out float solidCapacity, out float liquidCapacity, out float gasCapacity)
		{
			gasCapacity = ConduitFlow.MAX_GAS_MASS;
			liquidCapacity = ConduitFlow.MAX_LIQUID_MASS;
			solidCapacity = SolidConduitFlow.MAX_SOLID_MASS;
			InitTypes();
			if (ConfigInstance == null)
				return false;

			if (TryGetConfigValue<float>("PipeGasMaxPressure", out var gas))
				gasCapacity = gas;
			if (TryGetConfigValue<float>("PipeLiquidMaxPressure", out var liquid))
				liquidCapacity = liquid;
			if (TryGetConfigValue<float>("ConveyorRailPackageSize", out var solid))
				solidCapacity = solid;

			SgtLogger.l($"Successfully loaded conduit config values from CustomizeBuildings:\nsolid: {solidCapacity}\nliquid: {liquidCapacity}\ngas: {gasCapacity}");
			return true;
		}

		public static bool TryGetSteamTurbineWattageAndPumpRate(out float wattage, out float pumpRate)
		{
			wattage = SteamTurbineConfig2.MAX_WATTAGE;
			pumpRate = 2f;
			InitTypes();
			if (ConfigInstance == null)
				return false;
			if (TryGetConfigValue<float>("SteamTurbineWattage", out var wattage_prop))
				wattage = wattage_prop;
			if (TryGetConfigValue<float>("SteamTurbinePumpRateKG", out var pumpRate_prop))
				pumpRate = pumpRate_prop;
			return true;
		}

		public static bool TryGetOtherTurbineValues(out float pumpRate, out float heatTransferPercent, out float minActiveTemp, out float idealTemp, out float outputTemp, out float overheatTemp)
		{
			pumpRate = 2f;
			heatTransferPercent = 0.1f;
			minActiveTemp = 398.15f;
			idealTemp = 473.15f;
			outputTemp = 368.15f;
			overheatTemp = 373.15f;

			InitTypes();
			if (ConfigInstance == null)
				return false;

			if (TryGetConfigValue<float>("SteamTurbinePumpRateKG", out var pumpRate_prop))
				pumpRate = pumpRate_prop;
			if (TryGetConfigValue<float>("SteamTurbineHeatTransferPercent", out var heat_prop))
				heatTransferPercent = heat_prop;
			if (TryGetConfigValue<float>("SteamTurbineMinActiveTemperature", out var minActiveTemp_prop))
				minActiveTemp = minActiveTemp_prop;
			if (TryGetConfigValue<float>("SteamTurbineIdealTemperature", out var idealTemp_prop))
				idealTemp = idealTemp_prop;
			if (TryGetConfigValue<float>("SteamTurbineOutputTemperature", out var outputTemp_prop))
				outputTemp = outputTemp_prop;
			if (TryGetConfigValue<float>("SteamTurbineOverheatTemperature", out var overheatTemp_prop))
				overheatTemp = overheatTemp_prop;

			SgtLogger.l("CustomizeBuildings Steam Turbine Integration:");
			SgtLogger.l("SteamTurbinePumpRateKG: " + pumpRate);
			SgtLogger.l("SteamTurbineHeatTransferPercent: " + heatTransferPercent);
			SgtLogger.l("SteamTurbineMinActiveTemperature: " + minActiveTemp);
			SgtLogger.l("SteamTurbineIdealTemperature: " + idealTemp);
			SgtLogger.l("SteamTurbineOutputTemperature: " + outputTemp);
			SgtLogger.l("SteamTurbineOverheatTemperature: " + overheatTemp);

			return true;
		}

		public static float GetTurbineBaseValue()
		{
			float steamTurbineBaseValue = SteamTurbineConfig2.MAX_WATTAGE;

			InitTypes();
			if (ConfigInstance != null)
			{
				if (TryGetConfigValue<float>("SteamTurbineWattage", out var _steamTurbineWattage))
					steamTurbineBaseValue = _steamTurbineWattage;
				SgtLogger.l("Custom Steam Turbine wattage from customizebuildings: " + steamTurbineBaseValue);
			}
			return steamTurbineBaseValue;
		}

		static void InitTypes()
		{
			if (ConfigInstance != null)
				return;

			var CustomizeBuildings_CustomizeBuildingsState = Type.GetType("CustomizeBuildings.CustomizeBuildingsState, CustomizeBuildings");
			if (CustomizeBuildings_CustomizeBuildingsState == null)
			{
				SgtLogger.l("CustomizeBuildings types not found.");
				//UtilMethods.ListAllTypesWithAssemblies();
				return;
			}
			var m_StateManager_field = AccessTools.Field(CustomizeBuildings_CustomizeBuildingsState, "StateManager");
			if (m_StateManager_field == null)
			{
				SgtLogger.error("CustomizeBuildings_CustomizeBuildingsState.StateManager field not found.");
				return;
			}
			object m_StateManager = null;
			try
			{
				m_StateManager = m_StateManager_field.GetValue(null);
			}
			catch (Exception e)
			{
				SgtLogger.error("Failure to get state manager from CustomizeBuildings_CustomizeBuildingsState:\n" + e.Message);
				return;
			}
			if (m_StateManager == null)
			{
				SgtLogger.error("Failure to get state manager from CustomizeBuildings_CustomizeBuildingsState");
				return;
			}


			ConfigInstance = null;
			var m_GetConfigInstance = AccessTools.Property(m_StateManager.GetType(), "State");
			if (m_GetConfigInstance == null)
			{
				SgtLogger.error("CustomizeBuildings_CustomizeBuildingsState.Manager.State method not found.");
				return;
			}
			try
			{
				ConfigInstance = m_GetConfigInstance.GetValue(m_StateManager);
			}
			catch (Exception e)
			{
				SgtLogger.error("Failure to get Config Instance from CustomizeBuildings_CustomizeBuildingsState:\n" + e.Message);
			}
			SgtLogger.l("CustomizeBuildings integration: " + (ConfigInstance != null ? "Success" : "Failed"));
		}

		/// <summary>
		/// The oil well of Chemical Processing - Industrial Overhaul has altered outputs and is set to work autonomously
		/// CustomizeBuilding breaks the oil well if the NoDupeOilWell config is enabled, so its patch needs to be turned off to not break that
		/// </summary>
		/// <param name="harmony"></param>
		internal static void FixOilWell(Harmony harmony)
		{
			if (!Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				return;

			var m_OilWellCapConfig_ConfigureBuildingTemplate = AccessTools.Method(typeof(OilWellCapConfig), nameof(OilWellCapConfig.ConfigureBuildingTemplate));

			harmony.Unpatch(m_OilWellCapConfig_ConfigureBuildingTemplate, HarmonyPatchType.Postfix, "CustomizeBuildings");
		}
	}
}
