using ModProfileManager_Addon.API;
using ModProfileManager_Addon.ModProfileData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UtilLibs;

namespace ModProfileManager_Addon
{
	public class SaveGameModList
	{
		public enum DLCType
		{
			undefined = 0,
			baseGame = 1,
			spacedOut = 2,
			modPack = 3
		}
		public string ReferencedColonySaveName;
		public string ModlistPath;
		public bool IsModPack = false;
		public DLCType Type = 0;
		private bool isClone = false;
		public void SetClone(bool cl) => isClone = cl;
		public bool IsClone() => isClone;

		public Dictionary<string, Dictionary<string, MPM_POptionDataEntry>> PlibModConfigSettings = new();
		public Dictionary<string, List<KMod.Label>> SavePoints = new();

		public Dictionary<string, List<KMod.Label>> GetSavePoints() => SavePoints;
		public static SaveGameModList CloneSingleEntryFromExisting(ModPresetEntry entry, string newName) => CloneSingleEntryFromExisting(entry.ModList, entry.Path, newName);

		public static SaveGameModList CloneSingleEntryFromExisting(SaveGameModList source, string path, string newName)
		{
			SaveGameModList newList = new SaveGameModList();
			newList.IsModPack = source.IsModPack;
			newList.ModlistPath = newName;
			newList.ReferencedColonySaveName = newName;


			source.TryGetModListEntry(path, out var entry);
			if (source.TryGetPlibOptionsEntry(path, out var plibsettings))
			{
				newList.PlibModConfigSettings.Add(newName, new(plibsettings));
			}
			newList.AddOrUpdateEntryToModList(newName, entry, true, true);
			return newList;
		}

		public static SaveGameModList ReadModlistListFromFile(FileInfo filePath)
		{

			//SgtLogger.l("filepath "+ filePath);
			if (!filePath.Exists || filePath.Extension != ".json")
			{
				SgtLogger.logwarning("Not a valid ModList.");
				return null;
			}
			else
			{
				FileStream filestream = filePath.OpenRead();
				using (var sr = new StreamReader(filestream))
				{
					string jsonString = sr.ReadToEnd();
					SaveGameModList modlist = JsonConvert.DeserializeObject<SaveGameModList>(jsonString);
					return modlist;
				}
			}
		}

		public void CleanupDuplicates()
		{
			List<string> savePointsToCleanup = new List<string>(16);
			foreach (var entry in this.SavePoints)
			{
				string currentSavePoint = entry.Key;
				if (!File.Exists(currentSavePoint))
				{
					savePointsToCleanup.Add(currentSavePoint);
				}
			}

			if (savePointsToCleanup.Count > 0)
			{
				SgtLogger.l($"cleaning {savePointsToCleanup.Count} obsolete entries from mod profile {ReferencedColonySaveName}");
				foreach (var savePointKey in savePointsToCleanup)
				{
					SavePoints.Remove(savePointKey);
					PlibModConfigSettings.Remove(savePointKey);
				}
				WriteModlistToFile();
			}
		}
		public bool DeleteFileIfEmpty(bool forceDelete = false)
		{
			if (ModlistPath == null || ModlistPath == string.Empty)
				return false;

			var path = !IsModPack ? Path.Combine(ModAssets.ModPath, ModlistPath + ".json") : Path.Combine(ModAssets.ModPacksPath, ModlistPath + ".json");

			if (SavePoints.Count == 0 || forceDelete)
			{
				SgtLogger.l("deleting profile: " + ModlistPath);
				if (File.Exists(path))
					File.Delete(path);
				return true;
			}
			return false;
		}

		public string GetSerialized(bool indented = true)
		{
			return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
		}
		public void WriteModlistToFile()
		{
			if (ModlistPath == null || ModlistPath == string.Empty || ModlistPath == ModAssets.TMP_PRESET || isClone)
				return;

			try
			{
				var path = !IsModPack ? Path.Combine(ModAssets.ModPath, ModlistPath + ".json") : Path.Combine(ModAssets.ModPacksPath, ModlistPath + ".json");

				if (DeleteFileIfEmpty())
					return;

				var fileInfo = new FileInfo(path);

				FileStream fcreate = fileInfo.Open(FileMode.Create);//(path, FileMode.Create);

				var JsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
				using (var streamWriter = new StreamWriter(fcreate))
				{
					if (!IsModPack)
					{
						SgtLogger.log("Writing save game profile to " + ModlistPath);
					}
					else
					{
						SgtLogger.log("Writing custom profile to " + ModlistPath);
					}
					streamWriter.Write(JsonString);
				}
			}
			catch (Exception e)
			{
				SgtLogger.logError("Could not write file, Exception: " + e);
			}
		}

		/// <summary>
		/// When Modlist is created by Deserializing
		/// </summary>
		public SaveGameModList() { }

		/// <summary>
		/// When Modlist is created by the game
		/// </summary>
		/// <param name="referencedColonySave"></param>
		/// <param name="guid"></param>
		public SaveGameModList(string referencedColonySave, bool _isModPack = false)
		{
			if (!_isModPack)
			{
				ReferencedColonySaveName = GetModListFileName(referencedColonySave);
				ModlistPath = GetModListFileName(referencedColonySave);
				Type = DlcManager.IsExpansion1Active() ? DLCType.spacedOut : DLCType.baseGame;
			}
			else
			{
				ReferencedColonySaveName = SanitationUtils.GetSanitizedNamePath(referencedColonySave);
				ModlistPath = SanitationUtils.GetSanitizedNamePath(referencedColonySave);
				Type = DLCType.modPack;
			}
		}

		public void SetModEnabledForDlc(KMod.Label label, bool enabled, string entry, bool writeAfter = true)
		{
			if (!SavePoints.TryGetValue(entry, out var points))
			{
				SgtLogger.error(entry + " not found in preset file");
				return;
			}
			if (!enabled)
				points.RemoveAll(entry => entry.defaultStaticID == label.defaultStaticID);
			else
				points.Add(label);

			if (writeAfter)
				WriteModlistToFile();
		}

		public static string GetModListFileName(string pathOfReference)
		{
			if (string.IsNullOrEmpty(pathOfReference))
				return null;
			string FileNameInSpe = Directory.GetParent(pathOfReference).Name;
			if (FileNameInSpe.Contains("auto_save"))
			{
				FileNameInSpe = GetModListFileName(Directory.GetParent(pathOfReference).FullName);
			}
			return FileNameInSpe;
		}

		public bool TryGetModListEntry(string path, out List<KMod.Label> result) => this.SavePoints.TryGetValue(path, out result);

		public bool AddOrUpdateEntryToModList(string subSavePath, List<KMod.Label> mods, bool isModPack = false, bool skipPlib = false)
		{
			this.IsModPack = isModPack;
			bool hasBeenInitialized = false;
			if (!TryGetModListEntry(subSavePath, out _))
			{
				hasBeenInitialized = true;
			}
			SavePoints[subSavePath] = mods;

			if (!skipPlib)
			{
				var plibConfigs = ReadPlibOptions();
				SgtLogger.l(plibConfigs.Count() + "", "plib config");
				if (plibConfigs != null && plibConfigs.Count() > 0)
				{
					PlibModConfigSettings[subSavePath] = plibConfigs;
				}
			}
			this.WriteModlistToFile();

			return hasBeenInitialized;
		}

		internal void SetPlibSettings(string newModProfilePath, Dictionary<string, MPM_POptionDataEntry> data)
		{
			if (PlibModConfigSettings == null)
			{
				PlibModConfigSettings = new();
			}
			PlibModConfigSettings[newModProfilePath] = data;
		}
		public class MPM_POptionDataEntry
		{
			public string CustomFileName;
			public bool SharedLocation = false;
			public bool Intented = false;
			public JObject ModConfigData;
			public MPM_POptionDataEntry(string customFileName, bool shared, bool intented, JObject data)
			{
				CustomFileName = customFileName;
				ModConfigData = data;
				Intented = intented;
				SharedLocation = shared;
			}
		}

		public bool TryGetPlibOptionsEntry(string path, out Dictionary<string, MPM_POptionDataEntry> entry)
		{
			entry = null;
			return (PlibModConfigSettings != null && PlibModConfigSettings.TryGetValue(path, out entry));
		}

		public static string GetModAssemblyNameSafe(KMod.Mod mod)
		{
			ICollection<Assembly> collection = mod?.loaded_mod_data?.dlls;
			string nameSpacelessModID = string.Join("",
				  Regex.Split(mod.staticID,
							 @"([^\w\.])").Select(p =>
											 p.Substring(p.LastIndexOf('.') + 1)));
			if (collection != null)
			{
				if (collection.Count == 1)
					return collection.First().GetNameSafe();

				List<string> potentialAssemblies = new List<string>();
				foreach (Assembly item2 in collection)
				{
					if (item2.GetNameSafe().ToLowerInvariant().Contains("plib"))
						continue;

					SgtLogger.l(mod.title + ": " + item2.GetNameSafe());
					potentialAssemblies.Add(item2.GetNameSafe());

				}
				if (potentialAssemblies.Count == 1)
					return potentialAssemblies.First();

				var mostLikely = potentialAssemblies.FirstOrDefault(item => item.Contains(nameSpacelessModID));
				if (mostLikely != null)
				{
					return mostLikely;
				}

			}
			return nameSpacelessModID;
		}

		public static void WritePlibOptions(Dictionary<string, MPM_POptionDataEntry> modConfigEntries)
		{
			var manager = Global.Instance.modManager;
			if (modConfigEntries != null)
			{
				SgtLogger.l("applying mod options, " + modConfigEntries.Count() + " entries in profile");


				foreach (var modConfig in modConfigEntries)
				{
					string modID = modConfig.Key;
					var mod = manager.mods.FirstOrDefault(m => m.staticID == modID);
					if (mod == null)
					{
						//SgtLogger.warning(modID + " not found for writing config.");
						Mod_API.ApplyCustomData(modID, modConfig.Value.ModConfigData);
						continue;
					}

					var config = modConfig.Value;

					var fileName = Path.GetFileName(config.CustomFileName);
					string nameSafe = GetModAssemblyNameSafe(mod);
					string TargetConfigFilePath = Path.Combine((nameSafe != null && config.SharedLocation)
						? Path.Combine(KMod.Manager.GetDirectory(), "config", nameSafe)
						: mod.ContentPath, fileName);

					TargetConfigFilePath = Path.GetFullPath(TargetConfigFilePath);
					SgtLogger.l(TargetConfigFilePath, "writing config for " + modID);
					try
					{
						bool intented = config.Intented;

						if (config.ModConfigData == null)
						{
							continue;
						}
						try
						{
							System.IO.Directory.CreateDirectory(Path.GetDirectoryName(TargetConfigFilePath));
							using JsonTextWriter jsonWriter = new JsonTextWriter(File.CreateText(TargetConfigFilePath));
							config.ModConfigData.WriteTo(jsonWriter);
						}
						catch (UnauthorizedAccessException thrown)
						{
							PUtil.LogExcWarn(thrown);
						}
						catch (IOException thrown2)
						{
							PUtil.LogExcWarn(thrown2);
						}
						catch (JsonException thrown3)
						{
							PUtil.LogExcWarn(thrown3);
						}

					}
					catch (Exception ex)
					{
						SgtLogger.error(ex.Message);
					}
				}
			}
			Mod_API.PrepareDataForOnLoadApplication(modConfigEntries);
		}

		public Dictionary<string, MPM_POptionDataEntry> ReadPlibOptions()
		{
			IEnumerable<PForwardedComponent> allComponents = PRegistry.Instance.GetAllComponents("PeterHan.PLib.Options.POptions");
			SgtLogger.l("fetching plib config settings for mod profile");
			if (allComponents == null || !allComponents.Any())
			{
				SgtLogger.l("no plib options found");
				return new();
			}

			//static id + data object
			Dictionary<string, MPM_POptionDataEntry> ModConfigs = new();
			SgtLogger.l(allComponents.Count() + " components found");

			foreach (var component in allComponents)
			{
				IDictionary<string, Type> instanceData = component.GetInstanceData<IDictionary<string, Type>>();
				foreach (var op in instanceData)
				{
					string modID = op.Key;
					if (modID == "PeterHan.ModUpdateDate") //skip mod update file to not break stuff
						continue;

					var type = op.Value;

					bool UseSharedConfigLocation = false;
					bool intentedFormat = false;
					string fileName = "config.json";

					foreach (var attribute in type.CustomAttributes)
					{
						if (attribute.AttributeType.FullName.Contains("ConfigFileAttribute"))
						{
							var args = attribute.ConstructorArguments;
							if (attribute.ConstructorArguments.Count > 2)
							{
								if (args[0].ArgumentType == typeof(string))
								{
									var altName = args[0].Value as string;
									//SgtLogger.l("alt file: " + altName);
									fileName = altName;
								}
								if (args[1].ArgumentType == typeof(bool))
								{
									var intented = (bool)args[1].Value;
									//SgtLogger.l("intented " + intented);
									intentedFormat = intented;
								}
								if (args[2].ArgumentType == typeof(bool))
								{
									var useshared = (bool)args[2].Value;
									//SgtLogger.l("using shared " + useshared);
									UseSharedConfigLocation = useshared;
								}
								break;
							}
							else
							{
								SgtLogger.warning("less than 3 constructor args found");
							}
						}

					}
					var modAssembly = type.Assembly;
					string nameSafe = modAssembly.GetNameSafe();
					string configFilePath = Path.Combine((nameSafe != null && UseSharedConfigLocation)
						? Path.Combine(KMod.Manager.GetDirectory(), "config", nameSafe)
						: PUtil.GetModPath(modAssembly), fileName);



					if (!File.Exists(configFilePath))
					{
						//SgtLogger.l("no Config file found for " + modID + " on path: " + configFilePath);
						continue;
					}
					try
					{
						configFilePath = Path.GetFullPath(configFilePath);
						using (StreamReader file = File.OpenText(configFilePath))
						{
							using (JsonTextReader reader = new JsonTextReader(file))
							{
								//SgtLogger.l(modID + "\n" + configFilePath, "FilePathToRead");
								JObject data = (JObject)JToken.ReadFrom(reader);
								//SgtLogger.l(o2.ToString(), modID);
								ModConfigs.Add(modID, new MPM_POptionDataEntry(fileName, UseSharedConfigLocation, intentedFormat, data));
							}
						}
					}
					catch (Exception ex)
					{
						SgtLogger.error(ex.Message);
					}
				}
			}
			Mod_API.StoreData(ref ModConfigs);
			return ModConfigs;
		}

	}
}
