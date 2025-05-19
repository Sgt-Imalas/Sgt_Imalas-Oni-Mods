using KMod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UtilLibs;
using UtilLibs.ModSyncing;
using static SaveGameModLoader.STRINGS.UI.FRONTEND;

namespace SaveGameModLoader
{
	public class ModlistManager
	{
		public Dictionary<string, SaveGameModList> Modlists = new();
		public Dictionary<string, SaveGameModList> ModPacks = new();
		private static readonly Lazy<ModlistManager> _instance = new Lazy<ModlistManager>(() => new ModlistManager());

		public static ModlistManager Instance { get { return _instance.Value; } }

		public GameObject ParentObjectRef;

		HashSet<string> ActiveModlistModIds = new HashSet<string>();
		private List<string> _activeModListIdOrder = new();

		public bool IsSyncing { get; set; }
		public string ActiveSave = string.Empty;

		int _differenceCount = 0;
		HashSet<string> _missingMods = new HashSet<string>();
		public static int ModListDifferencesPublic
		{
			get { return Instance._differenceCount; }
		}
		public static HashSet<string> MissingModsPublic
		{
			get { return Instance._missingMods; }
		}

		public bool ModIsNotInSync(KMod.Mod mod)
		{

			if (ModSyncUtils.IsModSyncMod(mod))
				return false;

			return (mod.IsEnabledForActiveDlc() && !ActiveModlistModIds.Contains(mod.label.defaultStaticID))
				|| (!mod.IsEnabledForActiveDlc() && ActiveModlistModIds.Contains(mod.label.defaultStaticID));
		}

		public SaveGameModList TryGetColonyModlist(string colonyName)
		{
			//GetAllStoredModlists();
			Modlists.TryGetValue(colonyName, out SaveGameModList result);
			//SgtLogger.log("ModList found for this savegame");
			return result;
		}

		public void InstantiateSyncViewWithoutRestart(List<KMod.Label> mods, GameObject parent)
		{
			InstantiateModView(mods, "", parent, false);
		}

		Tuple<SaveGameModList, string> PlibConfigSource = null;

		/// <summary>
		/// Create a modified Modview for syncing
		/// </summary>
		/// <param name="mods"></param>
		public void InstantiateModView(List<KMod.Label> mods, string activeSaveToLoad = "", GameObject parent = null, bool LoadOnCLose = true, System.Action AutoResumeOnSync = null, Tuple<SaveGameModList, string> _plibConfigSource = null)
		{
			ActiveSave = activeSaveToLoad;
			IsSyncing = true;
			AssignModDifferences(mods);

			//mods are synced:
			if (ModListDifferencesPublic == 0 && MissingModsPublic.Count == 0 && AutoResumeOnSync != null)
			{
				AutoResumeOnSync();
			}
			PlibConfigSource = _plibConfigSource;

			var assignAction = () => { AssignModDifferences(mods); };

			var ParentGO = parent == null ? ParentObjectRef : parent;

			var modScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, ParentGO);
			modScreen.gameObject.name = "SYNCSCREEN";
#if DEBUG
            // UIUtils.ListAllChildren(modScreen.transform);
#endif

			var screen = (SyncViewScreen)modScreen.AddComponent(typeof(SyncViewScreen));
			screen.LoadOnClose = LoadOnCLose;
			screen.RefreshAction = assignAction;
		}
		//public void ShowMissingMods()
		//{

		//    Manager.Dialog(Global.Instance.globalCanvas, 
		//        STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSTITLE, 
		//        string.Format(STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSDESC,
		//        ModListDifferences.Count,
		//        MissingMods.Count,
		//        ListMissingMods()));
		//}
		//public string ListMissingMods()
		//{
		//    StringBuilder stringBuilder = new StringBuilder();
		//    var SortedNames = MissingMods.Select(mod => mod.title).ToList();
		//    SortedNames.Sort();

		//    stringBuilder.AppendLine();
		//    Console.WriteLine("------Mod Sync------");
		//    Console.WriteLine("---[Missing Mods]---");

		//    for (int i = 0; i< SortedNames.Count; i++)
		//    {
		//        if (i < 35)
		//        {
		//            stringBuilder.AppendLine(" • " + SortedNames[i]);
		//        }
		//        Console.WriteLine(SortedNames[i]);
		//    }
		//    if (SortedNames.Count > 35)
		//    {
		//        stringBuilder.AppendLine(String.Format(STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSDESCEND, SortedNames.Count - 35));
		//    }

		//    Console.WriteLine("-----[List End]-----");
		//    Console.WriteLine("------Mod Sync------");
		//    return stringBuilder.ToString();
		//}

		public void AutoRestart(bool save = true)
		{
			if (_differenceCount > 0)
			{
				if (save)
					Global.Instance.modManager.Save();
				//ModListDifferences.Clear();
				//MissingMods.Clear();
				AutoLoadOnRestart();
			}
		}

		#region modsJsonOverriding
		public enum Status
		{
			NotInstalled,
			Installed,
			UninstallPending,
			ReinstallPending,
		}
		public class file_Mod
		{
			public Label label;
			public Status status;
			public bool enabled;
			public List<string> enabledForDlc;
			public int crash_count;
			public string reinstall_path;
		}
		public class modsJSON
		{
			public int version = 1;
			public List<KMod.Mod> mods;
		}
				
		public static modsJSON ReadGameMods()
		{
			var path = Path.Combine(IO_Utils.ModsFolder, "mods.json");
			SgtLogger.l(path);
			var fileInfo = new FileInfo(path);

			if (!fileInfo.Exists || fileInfo.Extension != ".json")
			{
				SgtLogger.logwarning("no valid file found.");
				return null;
			}
			else
			{
				FileStream filestream = fileInfo.OpenRead();
				using (var sr = new StreamReader(filestream))
				{
					string jsonString = sr.ReadToEnd();
					modsJSON modlist = JsonConvert.DeserializeObject<modsJSON>(jsonString);
					return modlist;
				}
			}
		}

		internal bool TryParseRML(string path, out List<Label> list)
		{
			SgtLogger.l(path, "trying to parse RML file");
			var fileInfo = new FileInfo(path);
			list = null;
			if (!fileInfo.Exists)
			{
				SgtLogger.logwarning("no valid file found.");
				return false;
			}
			else
			{
				FileStream filestream = fileInfo.OpenRead();
				try
				{
					using (var sr = new StreamReader(filestream))
					{
						string jsonString = sr.ReadToEnd();
						modsJSON modlist = JsonConvert.DeserializeObject<modsJSON>(jsonString);

						list = modlist.mods.FindAll(mod => mod.IsEnabledForActiveDlc()).Select(mod => mod.label).ToList();
						//enabledModLabels = 

						//ModlistManager.Instance.CreateOrAddToModPacks(fileInfo.Name, enabledModLabels);
						//ModlistManager.Instance.GetAllModPacks();
						//list = ModPacks[fileInfo.Name];
						return true;
					}
				}
				catch (Exception ex)
				{
					SgtLogger.warning("RML Parsing failed!");
					SgtLogger.warning(ex.Message);
					return false;
				}
			}
		}
		/// <summary>
		/// Activate all mods inside list if possible
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		internal bool TryParseLog(string path, out List<Label> list)
		{
			SgtLogger.l(path, "trying to parse log file for mods");
			var fileInfo = new FileInfo(path);
			list = null;
			if (!fileInfo.Exists)
			{
				SgtLogger.logwarning("no valid file found.");
				return false;
			}
			else
			{
				FileStream filestream = fileInfo.OpenRead();
				try
				{
					SgtLogger.l("aaa");
					using (var sr = new StreamReader(filestream))
					{
						var parsedLabels = new List<KMod.Label>();
						while (!sr.EndOfStream)
						{
							var currentLine = sr.ReadLine();

							if (currentLine.Contains("-- MAIN MENU --"))
							{
								sr.ReadToEnd();
								break;
							}


							Regex getSize = new Regex(@"Loading mod content DLL \[(.*)(:)(.*)\]");
							MatchCollection matches = getSize.Matches(currentLine);
							if (matches.Count == 1)
							{
								GroupCollection results = matches[0].Groups;
								SgtLogger.l(results.Count + "");

								if (results.Count >= 4)
								{
									bool workshopMod = ulong.TryParse(results[3].Value, out _);
									var newLabel = new KMod.Label()
									{
										id = results[3].Value,
										title = results[1].Value,
										version = 404,
										distribution_platform = workshopMod ? Label.DistributionPlatform.Steam : Label.DistributionPlatform.Local
									};
									//SgtLogger.l($"New Label: {newLabel.defaultStaticID}");
									parsedLabels.Add(newLabel);
								}
							}
						}
						if (parsedLabels.Count == 0)
							return false;
						list = parsedLabels;

						//ModlistManager.Instance.CreateOrAddToModPacks(fileInfo.Name, parsedLabels);
						//ModlistManager.Instance.GetAllModPacks();
						//list = ModPacks[fileInfo.Name];
						return true;
					}
				}
				catch (Exception ex)
				{
					SgtLogger.warning("Log Parsing failed!");
					SgtLogger.warning(ex.Message);
					return false;
				}
			}
		}

		public void OverwriteGameMods(modsJSON modlist)
		{
			try
			{
				var path = Path.Combine(IO_Utils.ModsFolder, "mods.json");
				SgtLogger.l("WRITING TO: " + path);
				var fileInfo = new FileInfo(path);

				FileStream fcreate = fileInfo.Open(FileMode.Create);//(path, FileMode.Create);

				var JsonString = JsonConvert.SerializeObject(modlist, Formatting.Indented);
				SgtLogger.l(JsonString);
				using (var streamWriter = new StreamWriter(fcreate))
				{
					SgtLogger.log("Overwriting mods.json");
					streamWriter.Write(JsonString);
				}
			}
			catch (Exception e)
			{
				SgtLogger.logError("Could not write file, Exception: " + e);
			}
		}

		#endregion


		public void SyncFromModListWithoutAutoLoad(List<string> modList, System.Action OnFinishAction = null, bool dontDisableActiveMods = false)
		{
			SgtLogger.l($"Syncing from list; {modList.Count} entries, addingOnly: {dontDisableActiveMods}");

			AssignModDifferences(modList);
			SyncAllMods(modList, null, false, OnFinishAction, dontDisableActiveMods);
		}

		public void SyncAllMods(List<string> modList, bool? enableAll, bool restartAfter = true, System.Action OnFinishAction = null, bool dontDisableActiveMods = false)
		{

			if (modList == null)
			{
				modList = ActiveModlistModIds.ToList();
			}

			if (restartAfter)
				//RestartSyncing(enableAll, restartAfter);
				NormalSyncing(enableAll, restartAfter, dontDisableActiveMods);
			else
			{
				NormalSyncing(enableAll, restartAfter, dontDisableActiveMods);
				KMod.Manager.Dialog(GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay),
			   SINGLEMODLIST.POPUPSYNCEDTITLE,
			   dontDisableActiveMods ? SINGLEMODLIST.POPUPSYNCEDTEXTENABLEONLY : SINGLEMODLIST.POPUPSYNCEDTEXT,
			   SINGLEMODLIST.RETURNTWO,
			   OnFinishAction
			   );
				//}
			}
		}
		public void RestartSyncing(bool? enableAll, bool restartAfter = true)
		{
			modsJSON ModFileDeserialized = ReadGameMods();
			if (ModFileDeserialized == null)
				return;


			//foreach (var mod in ModFileDeserialized.mods)
			//    SgtLogger.l(mod.enabledForDlc.FirstOrDefault());


			string dlcId = DlcManager.IsExpansion1Active() ? DlcManager.EXPANSION1_ID : DlcManager.VANILLA_ID;

			for (int i = 0; i < ModFileDeserialized.mods.Count; i++)
			{
				var mod_entry = ModFileDeserialized.mods[i];

				bool enabled = enableAll.HasValue ? enableAll.Value : ActiveModlistModIds.Contains(mod_entry.label.defaultStaticID);

				if (ModFileDeserialized.mods[i].enabledForDlc == null)
				{
					ModFileDeserialized.mods[i].enabledForDlc = new List<string>();
				}

				if (enabled)
				{
					SgtLogger.l("ENABLE: " + ModFileDeserialized.mods[i].label);
					ModFileDeserialized.mods[i].enabledForDlc.Add(dlcId);
				}  //new List<string> { DlcManager.EXPANSION1_ID }; VANILLA_ID
				else
				{
					SgtLogger.l("DISABLE: " + ModFileDeserialized.mods[i].label);
					ModFileDeserialized.mods[i].enabledForDlc.Remove(dlcId);
				}
			}


			//foreach (var mod in ModFileDeserialized.mods)
			//    SgtLogger.l(mod.enabledForDlc.FirstOrDefault());



			OverwriteGameMods(ModFileDeserialized);

			AutoLoadOnRestart();
		}


		public void NormalSyncing(bool? enableAll = null, bool restartAfter = true, bool dontDisableActives = false)
		{
			var mm = Global.Instance.modManager;

			SgtLogger.l($"initiating syncing. EnableAll: {enableAll}, restartAfter: {restartAfter}, dontDisableActives:{dontDisableActives}");

			HashSet<int> positionReplacementIndicies = new HashSet<int>();
			List<KMod.Mod> toSortMods = new List<KMod.Mod>();

			if (PlibConfigSource != null)
			{
				var file = PlibConfigSource.first;
				var path = PlibConfigSource.second;
				if (file.TryGetPlibOptionsEntry(path, out var entries))
				{
					SaveGameModList.WritePlibOptions(entries);
				}
			}


			for (int index = 0; index < mm.mods.Count; index++)
			{
				var modToEdit = mm.mods[index];
				var modID = modToEdit.label.defaultStaticID;
				bool shouldBeEnabled = enableAll.HasValue ? enableAll.Value : ActiveModlistModIds.Contains(modID);
				bool isEnabled = modToEdit.IsEnabledForActiveDlc();

				if (ModSyncUtils.IsModSyncMod(modToEdit))
					shouldBeEnabled = true;

				if (shouldBeEnabled == false && dontDisableActives && isEnabled)
					shouldBeEnabled = true;

				if (shouldBeEnabled != isEnabled)
				{
					if (modToEdit.available_content != 0)
					{
						modToEdit.SetEnabledForActiveDlc(shouldBeEnabled);
						SgtLogger.l(shouldBeEnabled ? "enabled Mod: " + modToEdit.title : "disabled Mod: " + modToEdit.title);
					}
					else
						SgtLogger.l("mod not compatible: " + modToEdit.title);
				}


				if (shouldBeEnabled && modToEdit.available_content != 0)
				{
					SgtLogger.l(index + "", modToEdit.label.title);
					positionReplacementIndicies.Add(index);
					toSortMods.Add(modToEdit);
				}
			}

			if (!dontDisableActives)
			{
				SgtLogger.l("applying mod order");
				// for (int s = 0; s < _activeModListIdOrder.Count; s ++)
				//     SgtLogger.l(_activeModListIdOrder[s], s.ToString());    

				var sortedModsArray = toSortMods.OrderBy(item => _activeModListIdOrder.IndexOf(item.label.defaultStaticID)).ToArray();
				var ModsArray = mm.mods.ToArray();

				int sortedIndex = 0;
				for (int i = 0; i < ModsArray.Length; i++)
				{
					if (positionReplacementIndicies.Contains(i))
					{
						//SgtLogger.l($"placing mod nr {sortedIndex}, {sortedModsArray[sortedIndex].label.title} at {i} in array");
						ModsArray[i] = sortedModsArray[sortedIndex];
						++sortedIndex;
					}
				}

				mm.mods = ModsArray.ToList();
			}

			if (!restartAfter)
			{
				Global.Instance.modManager.dirty = true;
				Global.Instance.modManager.Update(null);
			}
			else
				Global.Instance.modManager.Save();


			if (restartAfter)
				AutoRestart();

		}
		public void AssignModDifferences(List<KMod.Label> modList)
		{
			AssignModDifferences(new List<string>(modList.Select(mod => mod.defaultStaticID)));

		}
		public void AssignModDifferences(List<string> modList)
		{
			_activeModListIdOrder = new(modList);
			ActiveModlistModIds = new(modList);

			_differenceCount = 0;

			KMod.Manager modManager = Global.Instance.modManager;
			HashSet<string> allModsInProfile = new HashSet<string>(modList);
			foreach (var mod in modManager.mods)
			{

				bool isCurrentlyActive = mod.IsEnabledForActiveDlc();
				if (allModsInProfile.Contains(mod.label.defaultStaticID))
				{
					if (!isCurrentlyActive)
						++_differenceCount;
					allModsInProfile.Remove(mod.label.defaultStaticID);
				}
				else
				{
					if (isCurrentlyActive)
						++_differenceCount;
				}
			}
			_missingMods = new HashSet<string>(allModsInProfile);
			SgtLogger.l($"Asserted differences for modlist, difference count: {_differenceCount}, missing: {_missingMods.Count}");


			//var thisMod = modManager.mods.Find(mod => mod.label.id == ModAssets.ModID).label;


			//var allMods = modManager.mods.Select(mod => mod.label.id).ToList();
			//var enabledMods = modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label.id).ToList();



		}
		public class StringEqualityComparer : IEqualityComparer<string>
		{
			public bool Equals(string l1, string l2)
			{
				return l1 == l2;
			}

			public int GetHashCode(string obj)
			{
				//SgtLogger.log(obj.id + ": "+obj.title);
				return obj.GetHashCode();
			}

		}
		public class ModDifferencesByIdComparer : IEqualityComparer<Label>
		{
			public bool Equals(Label l1, Label l2)
			{
				return l1.defaultStaticID == l2.defaultStaticID || l1.title == l2.title;
			}

			public int GetHashCode(Label obj)
			{
				//SgtLogger.log(obj.id + ": "+obj.title);
				return obj.defaultStaticID.GetHashCode();
			}

		}

		void AutoLoadOnRestart()
		{

			if (ActiveSave != string.Empty)
				KPlayerPrefs.SetString("AutoResumeSaveFile", ActiveSave);
			ActiveSave = string.Empty;
			App.instance.Restart();
		}


		public void InstantiateModViewForPathOnly(string referencedPath, System.Action autoResumeAction = null)
		{
			var mods = TryGetColonyModlist(SaveGameModList.GetModListFileName(referencedPath));
			if (mods == null)
			{
				SgtLogger.logError("No Modlist found for " + SaveGameModList.GetModListFileName(referencedPath));
				return;
			}

			if (!mods.TryGetModListEntry(referencedPath, out var list))
			{
				SgtLogger.logError("No ModConfig found for " + referencedPath);
				return;
			}
			InstantiateModView(list, referencedPath, AutoResumeOnSync: autoResumeAction, _plibConfigSource: new(mods, referencedPath));
		}

		public void GetAllStoredModlists()
		{
			Modlists.Clear();
			//MissingMods.Clear();
			var files = new DirectoryInfo(ModAssets.ModPath).GetFiles();


			foreach (FileInfo modlist in files)
			{
				try
				{
					//SgtLogger.log("Trying to load: " + modlist);
					var list = SaveGameModList.ReadModlistListFromFile(modlist);

					list.CleanupDuplicates();
					ReadModTitles(list);

					if (list != null && list.SavePoints.Count > 0)
					{
						Modlists.Add(list.ReferencedColonySaveName, list);
					}
				}
				catch (Exception e)
				{
					SgtLogger.warning("Couln't load savegamemod list from: " + modlist.FullName + ", Error: " + e);
				}
			}
			SgtLogger.log("Found Mod Configs for " + files.Count() + " Colonies");
		}

		private void ReadModTitles(SaveGameModList list)
		{
			foreach (var modlist in list.GetSavePoints().Values)
			{
				foreach (var mod in modlist)
				{
					if (!StoredModTitles.ContainsKey(mod.defaultStaticID))
					{
						StoredModTitles.Add(mod.defaultStaticID, mod.title);
					}
				}
			}
		}

		public void GetAllModPacks()
		{
			ModPacks.Clear();
			//MissingMods.Clear();
			var files = new DirectoryInfo(ModAssets.ModPacksPath).GetFiles();

			foreach (FileInfo modlist in files)
			{
				try
				{
					//SgtLogger.log("Trying to load: " + modlist);
					var list = SaveGameModList.ReadModlistListFromFile(modlist);
					ReadModTitles(list);
					if (list != null && list.SavePoints.Count > 0)
					{
						ModPacks.Add(list.ReferencedColonySaveName, list);
					}
				}
				catch (Exception e)
				{
					SgtLogger.warning("Couln't load Mod list from: " + modlist.FullName + ", Error: " + e);
				}
			}
			SgtLogger.log("Found " + files.Count() + " custom profiles");
		}

		public bool CreateOrAddToModLists(string savePath, List<KMod.Label> list)
		{
			bool hasBeenInitialized = false;

			Modlists.TryGetValue(SaveGameModList.GetModListFileName(savePath), out SaveGameModList colonyModSave);

			if (colonyModSave == null)
			{
				hasBeenInitialized = true;
				colonyModSave = new SaveGameModList(savePath);
			}

			colonyModSave.Type = DlcManager.IsExpansion1Active() ? SaveGameModList.DLCType.spacedOut : SaveGameModList.DLCType.baseGame;

			bool subListInitialized = colonyModSave.AddOrUpdateEntryToModList(savePath, list);
			Modlists[SaveGameModList.GetModListFileName(savePath)] = colonyModSave;
			if (hasBeenInitialized)
				SgtLogger.log("New mod config file created for: " + SaveGameModList.GetModListFileName(savePath));
			if (subListInitialized)
				SgtLogger.log("New mod list added for: " + savePath);
			else
				SgtLogger.log("mod list overwritten for: " + savePath);

			return hasBeenInitialized | subListInitialized;

		}


		public bool CreateOrAddToModPacks(string savePath, List<KMod.Label> list)
		{
			bool hasBeenInitialized = false;

			ModPacks.TryGetValue(savePath, out SaveGameModList ModPackFile);

			if (ModPackFile == null)
			{
				hasBeenInitialized = true;
				ModPackFile = new SaveGameModList(savePath, true);
			}

			int versionNumber = ModPackFile.GetSavePoints().Count + 1;

			var VersionString = "Version " + versionNumber.ToString();

			bool subListInitialized = ModPackFile.AddOrUpdateEntryToModList(VersionString, list, true);
#if DEBUG
            SgtLogger.log(savePath + "<>" + VersionString);
#endif

			ModPacks[(savePath)] = ModPackFile;
			if (hasBeenInitialized)
				SgtLogger.log("New mod pack file created: " + savePath);
			if (subListInitialized)
				SgtLogger.log("New mod pack added for: " + savePath);

			return hasBeenInitialized | subListInitialized;

		}

		public Dictionary<string, string> StoredModTitles = new Dictionary<string, string>();

		public bool TryGetModTitleFromStorage(string id, out string title)
		{
			if (StoredModTitles.TryGetValue(id, out title))
			{
				SgtLogger.l($"Found title for id {id}: {title}");
				return true;
			}
			else
			{
				SgtLogger.warning($"couldnt find title for id {id}");
				title = id;
				return false;
			}

		}

		//internal void UpdateModDict()
		//{
		//    SgtLogger.l("Mods have changed, updating dictionary...");

		//    GameModDictionary.Clear();

		//    foreach (KMod.Mod mod in Global.Instance.modManager.mods)
		//    {
		//        GameModDictionary.Add(mod.label, mod);
		//    }

		//    SgtLogger.l("Dictionary updated.");
		//}
	}
}
