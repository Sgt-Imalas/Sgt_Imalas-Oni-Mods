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


			var gasPipeField = Traverse.Create(ConfigInstance).Property("PipeGasMaxPressure");
			var liquidPipeField = Traverse.Create(ConfigInstance).Property("PipeLiquidMaxPressure");
			var solidPipeField = Traverse.Create(ConfigInstance).Property("ConveyorRailPackageSize");

			if (gasPipeField.PropertyExists())
			{
				gasCapacity = (float)gasPipeField.GetValue();
			}
			else
				SgtLogger.warning("could not find gas capacity property!");
			if (liquidPipeField.PropertyExists())
			{
				liquidCapacity = (float)liquidPipeField.GetValue();
			}
			else
				SgtLogger.warning("could not find liquid capacity property!");
			if (solidPipeField.PropertyExists())
			{
				solidCapacity = (float)solidPipeField.GetValue();
			}
			else
				SgtLogger.warning("could not find solid capacity property!");
			SgtLogger.l($"Successfully loaded conduit config values from CustomizeBuildings:\nsolid: {solidCapacity}\nliquid: {liquidCapacity}\ngas: {gasCapacity}");

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


			var pumpRate_prop = Traverse.Create(ConfigInstance).Property("SteamTurbinePumpRateKG");
			if(pumpRate_prop.PropertyExists()) 
				pumpRate = (float)pumpRate_prop.GetValue();

			var heatTransferPercent_prop = Traverse.Create(ConfigInstance).Property("SteamTurbineHeatTransferPercent");
			if (heatTransferPercent_prop.PropertyExists())
				heatTransferPercent = (float)heatTransferPercent_prop.GetValue();

			var minActiveTemp_prop = Traverse.Create(ConfigInstance).Property("SteamTurbineMinActiveTemperature");
			if (minActiveTemp_prop.PropertyExists())
				minActiveTemp = (float)minActiveTemp_prop.GetValue();

			var idealTemp_prop = Traverse.Create(ConfigInstance).Property("SteamTurbineIdealTemperature");
			if (idealTemp_prop.PropertyExists())
				idealTemp = (float)idealTemp_prop.GetValue();

			var outputTemp_prop = Traverse.Create(ConfigInstance).Property("SteamTurbineOutputTemperature");
			if (outputTemp_prop.PropertyExists())
				outputTemp = (float)outputTemp_prop.GetValue();

			var overheatTemp_prop = Traverse.Create(ConfigInstance).Property("SteamTurbineOverheatTemperature");
			if (overheatTemp_prop.PropertyExists())
				overheatTemp = (float)overheatTemp_prop.GetValue();

			SgtLogger.l("CustomizeBuildings Steam Turbine Integration:");
			SgtLogger.l("SteamTurbinePumpRateKG: "+pumpRate);
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
				var _steamTurbineWattage = Traverse.Create(ConfigInstance).Property("SteamTurbineWattage");
				if (_steamTurbineWattage.PropertyExists())
					steamTurbineBaseValue = (float)_steamTurbineWattage.GetValue();
				SgtLogger.l("Custom Steam Turbine wattage from customizebuildings: "+steamTurbineBaseValue);
			}
			return steamTurbineBaseValue;
		}

		static void InitTypes()
		{
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
	}
}
