using Epic.OnlineServices.Platform;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SkillsInfoScreen
{
	internal class ModIntegration_CleanHud
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
				Debug.LogWarning("Mod Config State had a property with the name: " + propertyName + ", but it was typeOf " + foundType.Name + ", instead of the expected " + T_Type.Name);
				return false;
			}

			value = (T)propertyValue;
			return true;
		}

		/// <summary>
		/// Integration with CleanHud small buttons
		/// </summary>

		static object ConfigInstance = null;

		static void InitTypes()
		{
			if (ConfigInstance != null)
				return;

			var CleanHUD_Options = Type.GetType("CleanHUD.Options, CleanHUD");
			if (CleanHUD_Options == null)
			{
				SgtLogger.l("CleanHUD.Options type not found.");
				UtilMethods.ListAllTypesWithAssemblies();
				return;
			}


			ConfigInstance = null;
			var m_GetConfigInstance = CleanHUD_Options.GetProperty("Opts", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public );
			if (m_GetConfigInstance == null)
			{

				SgtLogger.error("CleanHUD_Options.Opts property not found.");
				return;
			}
			try
			{
				ConfigInstance = m_GetConfigInstance.GetValue(null);
			}
			catch (Exception e)
			{
				SgtLogger.error("Failure to get Config Instance from CleanHUD_Options:\n" + e.Message);
			}
			SgtLogger.l("CleanHUD_Options integration: " + (ConfigInstance != null ? "Success" : "Failed"));
		}
	}
}
