using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilLibs;

namespace ModProfileManager_Addon.API
{
	internal class Mod_API
	{
		public delegate JObject GetCustomModOptionDataDelegate();
		public delegate void ApplyCustomModOptionData(JObject data);
		public class ModOptionDataStorage
		{
			//unique data storage ID
			public string Id;
			//returns (if applicable to the given Gameobject)
			//called when getting the mod option data, returns the data as a JObject
			public GetCustomModOptionDataDelegate GetDataToStore;
			//called when applying the data, takes in GameObject and the data value as a string
			public ApplyCustomModOptionData ApplyStoredData;
			//override priority of this data storage rule
			public int OverridePriority = 0;

			public ModOptionDataStorage(string iD, GetCustomModOptionDataDelegate onStore, ApplyCustomModOptionData onApply, int overridePriority = 0)
			{
				Id = iD;
				this.GetDataToStore = onStore;
				this.ApplyStoredData = onApply;
				OverridePriority = overridePriority;
			}
		}
		internal static void RegisterCustomModOptionHandlers()
		{
			var q = AppDomain.CurrentDomain.GetAssemblies()
				   .SelectMany(t => t.GetTypes());

			SgtLogger.l("Loading custom option handlers");

			foreach (var type in q)
			{
				///This method should return a JObject that contains all custom option data you want to store
				var DataGetter = AccessTools.Method(type, "ModOptions_GetData");

				///This method recieves the the JObject data that got stored with the method above.
				var DataApplier = AccessTools.Method(type, "ModOptions_SetData",
				new[]
				{
					typeof(JObject)
				});
				string typeName = type.Assembly.GetName().Name + "_" + type.Name;


				if (DataGetter != null && DataApplier != null)
				{
					SgtLogger.l("trying to register additional mod option data for type " + typeName);
					var getterDelegate = (GetCustomModOptionDataDelegate)Delegate.CreateDelegate(typeof(GetCustomModOptionDataDelegate), DataGetter);
					var setterDelegate = (ApplyCustomModOptionData)Delegate.CreateDelegate(typeof(ApplyCustomModOptionData), DataApplier);
					if (getterDelegate != null && setterDelegate != null)
					{
						RegisterCustomModDataHandler(typeName, getterDelegate, setterDelegate);
					}
					else
					{
						SgtLogger.warning("failed to create delegates for " + typeName);
					}
				}
			}
		}

		public static Dictionary<string, ModOptionDataStorage> CustomModOptionDataEntries = new();
		/// <summary>
		/// Register methods to store and apply custom mod data for your mod
		/// </summary>
		public static void RegisterCustomModDataHandler(string ID, GetCustomModOptionDataDelegate GetDataToStore, ApplyCustomModOptionData ApplyStoredData, int OverridePriority = 0)
		{
			if (CustomModOptionDataEntries.TryGetValue(ID, out var value))
			{
				if (OverridePriority <= value.OverridePriority)
				{
					SgtLogger.warning($"Registering custom mod option data storage with the ID {ID}, but there was already an entry with that ID!. Override priority {OverridePriority} was too low to override");
					return;
				}
				SgtLogger.l($"Registering  custom mod option data storage with the ID {ID}, but there was already an entry with that ID. Overriding with higher priority");
			}
			else
			{
				SgtLogger.l($"Registering custom mod option data storage with the ID {ID}.");
			}
			CustomModOptionDataEntries[ID] = new ModOptionDataStorage(ID, GetDataToStore, ApplyStoredData);
		}

		public static void ApplyCustomData(string key, JObject data)
		{
			if (CustomModOptionDataEntries.TryGetValue(key, out var customApplicationCarrier))
			{
				try
				{
					customApplicationCarrier.ApplyStoredData(data);
				}
				catch (Exception ex)
				{
					SgtLogger.error("error while trying to apply data with the id " + key + ":\n" + ex.Message);
				}
			}
		}
		public static void ApplyCustomDataOnLoad()
		{
			if (!File.Exists(ModAssets.PendingCustomDataPath))
			{
				return;
			}
			try
			{
				if (!IO_Utils.ReadFromFile<Dictionary<string, JObject>>(ModAssets.PendingCustomDataPath, out var storage))
					return;
				foreach (var data in storage)
				{
					if (CustomModOptionDataEntries.TryGetValue(data.Key, out var customApplicationCarrier))
					{
						ApplyCustomData(data.Key, data.Value);
					}
				}
				File.Delete(ModAssets.PendingCustomDataPath);
			}
			catch (Exception e)
			{
				SgtLogger.error(e.Message, "Error while trying to apply custom data");
			}
		}

		internal static void StoreData(ref Dictionary<string, SaveGameModList.MPM_POptionDataEntry> modConfigs)
		{
			if (CustomModOptionDataEntries.Count == 0)
				return;

			SgtLogger.l("fetching custom data, count: " + CustomModOptionDataEntries.Count);
			foreach (var entry in CustomModOptionDataEntries)
			{
				try
				{
					var data = entry.Value.GetDataToStore();
					if (data != null)
					{
						modConfigs.Add(entry.Key, new(entry.Key, false, false, data));
					}
					else
					{
						SgtLogger.warning("data object for " + entry.Key + " was null");
					}
				}
				catch (Exception ex)
				{
					SgtLogger.l("error while trying to fetch data for " + entry.Key + ":\n" + ex.Message);
				}
			}
		}

		internal static void PrepareDataForOnLoadApplication(Dictionary<string, SaveGameModList.MPM_POptionDataEntry> modConfigEntries)
		{
			Dictionary<string, JObject> customData = new();

			foreach (var entry in modConfigEntries)
			{
				customData.Add(entry.Key, entry.Value.ModConfigData);
			}
			IO_Utils.WriteToFile(customData, ModAssets.PendingCustomDataPath);
		}
	}
}
