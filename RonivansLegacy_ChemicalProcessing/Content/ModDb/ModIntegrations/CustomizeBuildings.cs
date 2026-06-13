using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


		public static bool TryGetModifiedConduitValues(out float solidCapacity, out float liquidCapacity, out float gasCapacity)
		{
			gasCapacity = ConduitFlow.MAX_GAS_MASS;
			liquidCapacity = ConduitFlow.MAX_LIQUID_MASS;
			solidCapacity = SolidConduitFlow.MAX_SOLID_MASS;
			InitTypes(true);
			if (!Initialized)
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
			if (!Initialized)
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
			if (!Initialized)
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

			InitTypes(true);
			if (Initialized)
			{
				if (TryGetConfigValue<float>("SteamTurbineWattage", out var _steamTurbineWattage))
					steamTurbineBaseValue = _steamTurbineWattage;
				SgtLogger.l("Custom Steam Turbine wattage from customizebuildings: " + steamTurbineBaseValue);
			}
			return steamTurbineBaseValue;
		}

		static bool Initialized = false;
		static GetConfigValueDelegate Delegate_TryGetConfigValue;
		public delegate object GetConfigValueDelegate(string name);

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
			if (!Initialized)
				return false;

			object propertyValue = Delegate_TryGetConfigValue(propertyName);
			value = (T)propertyValue;
			return true;
		}
		static void InitTypes(bool force = false)
		{
			if (Initialized && !force)
				return;


			///fetch the config class type
			var CustomizeBuildings_CustomizeBuildingsState = Type.GetType("CustomizeBuildings.CustomizeBuildingsState, CustomizeBuildings");
			if (CustomizeBuildings_CustomizeBuildingsState == null)
			{
				Debug.Log("CustomizeBuildings types not found.");
				return;
			}
			if(CustomizeBuildings_CustomizeBuildingsState.BaseType == null)
			{
				Debug.Log("CustomizeBuildings_CustomizeBuildingsState BaseType was null");
			}
			///fetch the method info of the static getter method
			var m_TryGetVal = AccessTools.Method(CustomizeBuildings_CustomizeBuildingsState.BaseType, "TryGetValue");
			if (m_TryGetVal == null)
			{
				Debug.LogWarning("CustomizeBuildings.CustomizeBuildingsState.TryGetValue not found.");
				return;
			}
			///create delegate for static TryGet method
			try
			{
				Delegate_TryGetConfigValue = (GetConfigValueDelegate)Delegate.CreateDelegate(typeof(GetConfigValueDelegate), m_TryGetVal);
			}
			catch (Exception e)
			{
				Debug.LogWarning("Failure to create delegate for CustomizeBuildingsState.TryGetValue:\n" + e.Message);
			}
			Initialized = Delegate_TryGetConfigValue != null;
			Debug.Log("CustomizeBuildings integration: " + (Initialized ? "Success" : "Failed"));
		}

		internal static void IntegrationPatches(Harmony harmony)
		{
			FixOilWell(harmony);
			FixValveBaseTemperature(harmony);
		}
		/// <summary>
		/// The oil well of Chemical Processing - Industrial Overhaul has altered outputs and is set to work autonomously
		/// CustomizeBuilding mirrors that in a destructive way which breaks the chemproc oil well if the NoDupeOilWell config is enabled, so its patch needs to be turned off to not break that
		/// </summary>
		/// <param name="harmony"></param>
		internal static void FixOilWell(Harmony harmony)
		{
			if (!Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				return;

			var m_OilWellCapConfig_ConfigureBuildingTemplate = AccessTools.Method(typeof(OilWellCapConfig), nameof(OilWellCapConfig.ConfigureBuildingTemplate));
			harmony.Unpatch(m_OilWellCapConfig_ConfigureBuildingTemplate, HarmonyPatchType.Postfix, "CustomizeBuildings");

			//absolute garbage patch in a chinese cheat mod that breaks roughly everything about the the oil well
			harmony.Unpatch(m_OilWellCapConfig_ConfigureBuildingTemplate, HarmonyPatchType.Prefix, "gengjiaqingsong");
			
		}
		/// <summary>
		/// CustomizeBuildings temperature valve does not respect proper inheritance, skipping base.OnSpawn()
		/// </summary>	
		internal static void FixValveBaseTemperature(Harmony harmony)
		{
			if (!Config.Instance.HighPressureApplications_Enabled)
				return;

			Type valveBaseTempType = Type.GetType("CustomizeBuildings.ValveBaseTemperature, CustomizeBuildings");
			if(valveBaseTempType == null)
			{
				SgtLogger.warning("CustomizeBuildings.ValveBaseTemperature not found, skipping patch.");
				return;
			}
			var targetMethod = AccessTools.Method(valveBaseTempType, "OnSpawn");
			if (targetMethod == null)
			{
				SgtLogger.warning("CustomizeBuildings.ValveBaseTemperature.OnSpawn method not found, skipping patch.");
				return;
			}
			harmony.Patch(targetMethod, postfix: new HarmonyMethod(typeof(CustomizeBuildings), nameof(ValveBaseTemperature_OnSpawn_Postfix)));

		}
		public static void ValveBaseTemperature_OnSpawn_Postfix(ValveBase __instance)
		{
			SgtLogger.l("ValveBaseTemperature OnSpawn postfix running for " + __instance.name+", conduitType: "+__instance.conduitType);
			if (__instance.conduitType == ConduitType.Gas || __instance.conduitType == ConduitType.Liquid)
			{
				float conduitMax = HighPressureConduitRegistration.GetMaxConduitCapacity(__instance.conduitType, true);
				if (__instance.maxFlow < conduitMax)
					__instance.maxFlow *= HighPressureConduitRegistration.GetConduitMultiplier(__instance.conduitType);

				ModAssets.AdjustValveAnimFlowAnim(__instance, __instance.maxFlow);

			}
		}
	}
}
