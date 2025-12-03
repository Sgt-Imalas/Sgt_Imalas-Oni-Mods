using KMod;
using ModProfileManager_Addon.IO;
using ModProfileManager_Addon.ModProfileData;
using ModProfileManager_Addon.UnityUI;
using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UtilLibs;
using UtilLibs.ModSyncing;

namespace ModProfileManager_Addon
{
	internal class ModAssets
	{
		public static string TMP_PRESET = "MPM_TMP_PRESET";

		public static ModPresetEntry SelectedModPack;
		public static Sprite ImportSprite, ExportSprite;

		public static void ToggleModActive(KMod.Label label, bool active)
		{
			SelectedModPack.ModList.SetModEnabledForDlc(label, active, SelectedModPack.Path);

			var id = label.defaultStaticID;
		}


		public static string ModPath;
		public static string ModPacksPath;
		public static string PendingCustomDataPath;

		public static GameObject ModPresetScreen;


		public static Dictionary<string, SaveGameModList> ModPacks = new();
		public static Dictionary<string, SaveGameModList> ClonePresets = new();


		public static void LoadAssets()
		{
			var bundle = AssetUtils.LoadAssetBundle("mpm_ui", platformSpecific: true);
			ModPresetScreen = bundle.LoadAsset<GameObject>("Assets/UIs/PresetOverview.prefab");
			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(ModPresetScreen);
			//UIUtils.ListAllChildren(ModPresetScreen.transform);
		}
		static List<string> forbiddenNames = new List<string>()
		{
			"CON", "PRN", "AUX", "NUL","COM1"
			,"COM2" ,"COM3" , "COM4", "COM5" , "COM6","COM7" , "COM8", "COM9" , "LPT1"
			,"LPT2" ,"LPT3" ,"LPT4" ,"LPT5" ,"LPT6" ,"LPT7" ,"LPT8" , "LPT9"
		};

		public static class Colors
		{
			public static Color Red = UIUtils.rgb(134, 69, 101);
			public static Color Blue = UIUtils.HSVShift(Red, 70f);
			public static Color Yellow = UIUtils.HSVShift(Red, 20f);

			public static Color DarkRed = UIUtils.Darken(Red, 40);
			public static Color DarkBlue = UIUtils.Darken(Blue, 40);
			public static Color DarkYellow = UIUtils.Darken(Yellow, 40);
		}


		public static List<ModProfileData.ModPresetEntry> GetAllModPresets()
		{
			GetAllModPacks();
			var result = new List<ModProfileData.ModPresetEntry>();
			foreach (var modPackCollection in ModPacks.Values)
			{
				foreach (var preset in modPackCollection.GetSavePoints())
				{
					result.Add(new ModProfileData.ModPresetEntry(modPackCollection, preset.Key));
				}
			}
			foreach (var modPackCollection in ClonePresets.Values)
			{
				foreach (var preset in modPackCollection.GetSavePoints())
				{
					result.Add(new ModPresetEntry(modPackCollection, preset.Key));
				}
			}
			return result;
		}
		public static void ImportPresetFromImportString(string import)
		{
			try
			{
				SgtLogger.l(import, "ToImport");

				string decompressed = StringCompression.DecompressString(import);
				SaveGameModList modlist = JsonConvert.DeserializeObject<SaveGameModList>(decompressed);

				if (modlist.SavePoints.Count == 0 || modlist.SavePoints.Count == 1 && modlist.SavePoints.Last().Value.Count == 0)
				{
					DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.TITLE_ERROR, STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.EMPTY);

				}
				if (ModPacks.ContainsKey(modlist.ReferencedColonySaveName))
				{
					DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.TITLE_ERROR,
						string.Format(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.DUPLICATE, modlist.ModlistPath),
						STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.DUPLICATE_REPLACE,
						modlist.WriteModlistToFile, on_cancel: () => { }
						);
				}
				else
				{
					modlist.WriteModlistToFile();
					int missingSteamMods = ModAssets.GetMissingSteamModCount(modlist, out List<ulong> missings);
					if (missingSteamMods > 0)
					{
						System.Action onConfirm = () => ModAssets.SubToAllDelayed(missings);
						DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.TITLE,
							string.Format(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.MISSING.SUCCESS, modlist.ModlistPath, missingSteamMods),
							STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.MISSING.SUBTOALL, onConfirm,
							STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.MISSING.CONTINUE, () => { });

					}
					else
					{
						DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.TITLE, string.Format(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.SUCCESS, modlist.ModlistPath));
					}
				}
			}
			catch (Exception e)
			{
				SgtLogger.l(e.Message);
				DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.TITLE_ERROR, STRINGS.UI.PRESETOVERVIEW.IMPORT_POPUP.ERROR);
			}

		}

		private static void SubToAllDelayed(List<ulong> missings)
		{
			foreach (var entry in missings)
				ModAssets.SubToMissingMod(entry);
		}

		private static int GetMissingSteamModCount(SaveGameModList modlist, out List<ulong> missings)
		{
			missings = new List<ulong>();
			SgtLogger.l("getting steam missing count");
			if (modlist == null || modlist.SavePoints.Count == 0 || modlist.SavePoints.Last().Value == null || modlist.SavePoints.Last().Value.Count == 0)
				return 0;
			var allMods = Global.Instance.modManager.mods;

			var modsInPreset = modlist.SavePoints.Last().Value;

			HashSet<string> toFilter = new(modsInPreset.Select(e => e.defaultStaticID));

			foreach (var mod in allMods)
			{
				if (mod.status == KMod.Mod.Status.UninstallPending || mod.status == KMod.Mod.Status.NotInstalled)
					continue;

				string defaultStaticID = mod.label.defaultStaticID;
				if (toFilter.Contains(defaultStaticID))
				{
					toFilter.Remove(defaultStaticID);
				}
			}
			if (toFilter.Count == 0)
				return 0;
			var allMissingSteamMods = modsInPreset.Where(m => toFilter.Contains(m.defaultStaticID) && m.distribution_platform == Label.DistributionPlatform.Steam).ToList();
			if (allMissingSteamMods.Count == 0)
				return 0;

			foreach (var item in allMissingSteamMods)
			{
				if (ulong.TryParse(item.id, out ulong steamId))
					missings.Add(steamId);
			}
			SgtLogger.l("missing count: " + missings.Count());
			return missings.Count();
		}

		public static void ExportToClipboard(SaveGameModList ModList)
		{
			if (ModList != null)
			{
				string ToCopy = StringCompression.CompressString(ModList.GetSerialized(false));
				IO_Utils.PutToClipboard(ToCopy);
				DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.PRESETOVERVIEW.EXPORT_POPUP.TITLE, STRINGS.UI.PRESETOVERVIEW.EXPORT_POPUP.TEXT);
			}
		}
		public static void GetAllModPacks()
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
			CloneImport.ImportFromClone();
		}
		public static SaveGameModList CreateOrAddToModPacks(string savePath, List<KMod.Label> list)
		{
			bool hasBeenInitialized = false;

			ModPacks.TryGetValue(savePath, out SaveGameModList ModPackFile);

			if (ModPackFile == null)
			{
				hasBeenInitialized = true;
				ModPackFile = new SaveGameModList(savePath, true);
			}

			int versionNumber = ModPackFile.GetSavePoints().Count + 1;

			var VersionString = versionNumber == 1 ? savePath : savePath + "_" + versionNumber;

			bool subListInitialized = ModPackFile.AddOrUpdateEntryToModList(VersionString, list, true);

			ModPacks[savePath] = ModPackFile;
			if (hasBeenInitialized)
				SgtLogger.log("New mod pack file created: " + savePath);
			if (subListInitialized)
				SgtLogger.log("New mod pack added for: " + savePath);

			return ModPackFile;

		}

		public static void SyncMods(bool dontDisableActives = false)
		{
			if ((SelectedModPack.ModList.TryGetModListEntry(SelectedModPack.Path, out var mods)))
			{
				if (SelectedModPack.ModList.TryGetPlibOptionsEntry(SelectedModPack.Path, out var configs))
					SaveGameModList.WritePlibOptions(configs);
				SyncMods(mods, dontDisableActives: dontDisableActives);
			}
		}
		internal static void SyncMods(List<Label> modsState, bool? enableAll = null, bool restartAfter = false, bool dontDisableActives = false)
		{
			var mm = Global.Instance.modManager;

			SgtLogger.l($"initiating syncing.");

			HashSet<int> positionReplacementIndicies = new HashSet<int>();
			List<KMod.Mod> toSortMods = new List<KMod.Mod>();
			var staticModIDs = modsState.Select(label => label.defaultStaticID).ToList();

			List<string> _activeModListIdOrder = new List<string>(staticModIDs);
			HashSet<string> ActiveModlistModIds = new HashSet<string>(staticModIDs);

			for (int index = 0; index < mm.mods.Count; index++)
			{
				var modToEdit = mm.mods[index];
				var modID = modToEdit.label.defaultStaticID;
				bool shouldBeEnabled = enableAll.HasValue ? enableAll.Value : ActiveModlistModIds.Contains(modID);
				bool isEnabled = modToEdit.IsEnabledForActiveDlc();

				if (ModSyncUtils.IsModSyncMod(modID))
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
				mm.dirty = true;
				mm.Update(null);
			}
			else
				mm.Save();


			if (restartAfter)
				App.instance.Restart();

		}

		internal static void HandleRenaming(ModProfileData.ModPresetEntry modProfileTuple, string newModProfilePath)
		{
			var modProfile = modProfileTuple.ModList;
			var modProfilePath = modProfileTuple.Path;
			SgtLogger.l("renaming, old: " + modProfilePath + ", new " + newModProfilePath);
			if (modProfilePath == newModProfilePath)
				return;

			if (modProfile != null)
			{
				if (modProfile.SavePoints.Count == 1)
				{
					modProfile.DeleteFileIfEmpty(true);
					modProfile.ModlistPath = newModProfilePath;
					modProfile.ReferencedColonySaveName = newModProfilePath;
					modProfile.WriteModlistToFile();
					ModPacks.Remove(modProfilePath);
					ModPacks.Add(newModProfilePath, modProfile);
				}
				else if (modProfile.SavePoints.Count > 1 && modProfile.TryGetModListEntry(modProfilePath, out var mods))
				{

					var newPack = CreateOrAddToModPacks(newModProfilePath, mods);
					if (modProfile.TryGetPlibOptionsEntry(modProfilePath, out var data))
					{
						newPack.SetPlibSettings(newModProfilePath, data);
					}
					modProfile.GetSavePoints().Remove(modProfilePath);
					modProfile.PlibModConfigSettings.Remove(modProfilePath);
					modProfile.WriteModlistToFile();
					newPack.WriteModlistToFile();
				}
			}

		}

		internal static void HandleDeletion(ModProfileData.ModPresetEntry modProfileTuple)
		{
			var modProfile = modProfileTuple.ModList;
			var modProfilePath = modProfileTuple.Path;
			if (modProfile != null && modProfile.TryGetModListEntry(modProfilePath, out _))
			{
				modProfile.SavePoints.Remove(modProfilePath);
				modProfile.PlibModConfigSettings.Remove(modProfilePath);
			}
			modProfile.WriteModlistToFile();
		}

		static Dictionary<string, string> ModDefaultIDToPlibModID = new();
		internal static void RegisterModMapping(KMod.Mod mod)
		{
			ModDefaultIDToPlibModID[mod.label.defaultStaticID] = mod.staticID;
		}

		public static void SubToMissingMod(ulong modId)
		{
			try
			{
				SteamUGC.SubscribeItem(new PublishedFileId_t(modId));
			}
			catch
			{
				SgtLogger.warning("subscribing to " + modId + " failed!");
			}
		}

		internal static void ShowModIndexShiftDialogue(KMod.Mod targetMod, GameObject target)
		{
			if (SelectedModPack.ModList.TryGetModListEntry(SelectedModPack.Path, out var mods))
			{
				var currentIndex = mods.FindIndex(m => m.defaultStaticID == targetMod.label.defaultStaticID);
				if (currentIndex >= 0)
				{
					Dialog_ModOrder.ShowIndexDialog(currentIndex, targetMod.title, target);
				}
			}
		}

		internal static void SetAllModsInPresetEnabled(bool enable)
		{
			if ((SelectedModPack.ModList.TryGetModListEntry(SelectedModPack.Path, out var mods)))
			{
				var mm = Global.Instance.modManager;
				HashSet<string> currentStaticIds = new HashSet<string>(mods.Select(modLabel => modLabel.defaultStaticID));
				if (enable)
				{
					foreach (var mod in mm.mods)
					{
						var defaulStaticLabelID = mod.label.defaultStaticID;
						if (!currentStaticIds.Contains(defaulStaticLabelID) && mod.available_content != 0 && mod.contentCompatability == ModContentCompatability.OK)
						{
							mods.Add(mod.label);
						}

					}
				}
				else
				{
					mods.RemoveAll(labl => !ModSyncUtils.IsModSyncMod(labl.defaultStaticID));
				}
				SelectedModPack.ModList.WriteModlistToFile();
			}
		}
	}
}
