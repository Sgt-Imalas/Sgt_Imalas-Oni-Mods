using Klei.AI;
using KMod;
using ModProfileManager_Addon.ModProfileData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ModProfileManager_Addon.SaveGameModList;

namespace ModProfileManager_Addon
{
    internal class ModAssets
    {
        public static string TMP_PRESET = "MPM_TMP_PRESET";

        public static ModPresetEntry SelectedModPack;
       


        public static void ToggleModActive(KMod.Label label, bool active)
        {
            SelectedModPack.ModList.SetModEnabledForDlc(label, active,SelectedModPack.Path);

            var id = label.defaultStaticID;
        }


        public static string ModPath;
        public static string ModPacksPath;

        public static GameObject ModPresetScreen;


        public static Dictionary<string, SaveGameModList> ModPacks = new();


        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("mpm_ui", platformSpecific: true);
            ModPresetScreen = bundle.LoadAsset<GameObject>("Assets/UIs/PresetOverview.prefab");
            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(ModPresetScreen);
            UIUtils.ListAllChildren(ModPresetScreen.transform);
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
        }

        #region pathSanitisation
        public static string GetSanitizedNamePath(string source)
        {
            SgtLogger.l("Sanitizing...");
            //SgtLogger.l(source, "1");
            source = Path.GetFileName(source);
            //SgtLogger.l(source, "2");
            source = ReplaceInvalidChars(source);
            //SgtLogger.l(source, "3");

            if (forbiddenNames.Contains(source.ToUpperInvariant()))
            {
                SgtLogger.l("file name was one of the forbidden ones, replacing..");
                source = Path.GetRandomFileName();
                //SgtLogger.l(source, "4");
            }

            return source;
        }

        public static string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
        #endregion

        public static List<ModPresetEntry> GetAllModPresets()
        {
            GetAllModPacks();
            var result = new List<ModPresetEntry>();
            foreach(var modPackCollection in ModPacks.Values)
            {
                foreach(var preset in modPackCollection.GetSavePoints())
                {
                    result.Add(new ModPresetEntry(modPackCollection, preset.Key));
                }
            }
            return result;
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

            var VersionString = versionNumber == 1 ? savePath : savePath+"_"+versionNumber;

            bool subListInitialized = ModPackFile.AddOrUpdateEntryToModList(VersionString, list, true);

            ModPacks[savePath] = ModPackFile;
            if (hasBeenInitialized)
                SgtLogger.log("New mod pack file created: " + savePath);
            if (subListInitialized)
                SgtLogger.log("New mod pack added for: " + savePath);

            return ModPackFile;

        }

        public static void SyncMods()
        {
            if((SelectedModPack.ModList.TryGetModListEntry(SelectedModPack.Path, out var mods)))
            {            

                if (SelectedModPack.ModList.TryGetPlibOptionsEntry(SelectedModPack.Path, out var configs))
                    SaveGameModList.WritePlibOptions(configs);
                SyncMods(mods);
            }
        }
        internal static void SyncMods(List<Label> modsState,bool? enableAll = null, bool restartAfter = false, bool dontDisableActives = false)
        {
            var mm = Global.Instance.modManager;

            SgtLogger.l($"initiating syncing. EnableAll: {enableAll}, restartAfter: {restartAfter}, dontDisableActives:{dontDisableActives}");

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

                if (modID == "SaveGameModLoader"|| modID == "ModProfileManager_Addon")
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
                App.instance.Restart();

        }

        internal static void HandleRenaming(ModPresetEntry modProfileTuple, string newModProfilePath)
        {
            var modProfile = modProfileTuple.ModList;
            var modProfilePath = modProfileTuple.Path;

            if (modProfile != null)
            {
                if(modProfile.SavePoints.Count == 1)
                {
                    modProfile.DeleteFileIfEmpty(true);
                    modProfile.ModlistPath = newModProfilePath;
                    modProfile.ReferencedColonySaveName = modProfilePath;
                    modProfile.WriteModlistToFile();
                }
                else if(modProfile.SavePoints.Count > 1 && modProfile.TryGetModListEntry(modProfilePath, out var mods))
                {

                    var newPack = CreateOrAddToModPacks(newModProfilePath, mods);
                    if (modProfile.TryGetPlibOptionsEntry(modProfilePath,out var data))
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

        internal static void HandleDeletion(ModPresetEntry modProfileTuple)
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
        
    }
}
